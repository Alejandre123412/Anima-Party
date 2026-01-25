using Godot;
using System.Collections.Generic;
using AnimaParty.assets.script.data;
using AnimaParty.assets.script.types;

namespace AnimaParty.assets.scenes.Jugadores;

public partial class CharacterViewer : Node3D
{
    [Export] private PackedScene nextScene;
    [Export] private Control uiRoot;
    [Export] private float cameraFov = 70f;

    private readonly List<SubViewport> viewports = new();
    private readonly List<Camera3D> cameras = new();
    private readonly List<Node3D?> activeCharacters = new();

    private readonly Dictionary<Player, int> playerSelections = new();
    private readonly Dictionary<Player, bool> lockedSelections = new();

    private List<PackedScene> characterScenes = new();

    public override void _Ready()
    {
        characterScenes = LoadCharacterScenes("res://assets/models/characters/");
        SetupSplitScreen();

        var players = PlayerData.GetTempPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            playerSelections[players[i]] = 0;
            lockedSelections[players[i]] = false;
            AssignCharacterToPlayer(players[i], 0);
        }
    }

    public override void _Process(double delta)
    {
        FinalizeSelections();
    }

    #region Characters

    private List<PackedScene> LoadCharacterScenes(string folderPath)
    {
        var list = new List<PackedScene>();
        var dir = DirAccess.Open(folderPath);
        if (dir == null) return list;

        dir.ListDirBegin();
        while (true)
        {
            var file = dir.GetNext();
            if (file == "") break;
            if (dir.CurrentIsDir()) continue;
            if (!file.EndsWith(".glb") && !file.EndsWith(".blend")) continue;

            var scene = GD.Load<PackedScene>($"{folderPath}{file}");
            if (scene != null)
                list.Add(scene);
        }
        dir.ListDirEnd();

        return list;
    }

    private void AssignCharacterToPlayer(Player player, int characterIndex)
    {
        int playerIndex = PlayerData.GetTempPlayers().IndexOf(player);
        if (playerIndex < 0 || playerIndex >= viewports.Count) return;

        if (activeCharacters.Count > playerIndex && activeCharacters[playerIndex] != null)
        {
            activeCharacters[playerIndex]!.QueueFree();
            activeCharacters[playerIndex] = null;
        }

        var instance = characterScenes[characterIndex].Instantiate<Node3D>();
        viewports[playerIndex].AddChild(instance);

        if (activeCharacters.Count <= playerIndex)
            activeCharacters.Add(instance);
        else
            activeCharacters[playerIndex] = instance;

        NormalizeCharacter(instance);
        PositionCamera(cameras[playerIndex], instance);

        playerSelections[player] = characterIndex;
    }

    private void ChangeCharacter(Player player, int delta)
    {
        if (lockedSelections[player]) return;

        int current = playerSelections[player];
        int count = characterScenes.Count;
        int next = (current + delta + count) % count;

        foreach (var kv in lockedSelections)
        {
            if (kv.Key != player && kv.Value && playerSelections[kv.Key] == next)
                return;
        }

        AssignCharacterToPlayer(player, next);
    }

    private void ConfirmCharacter(Player player)
    {
        lockedSelections[player] = true;
    }

    private void RetractSelection(Player player)
    {
        if (lockedSelections[player])
            lockedSelections[player] = false;
    }

    #endregion

    #region Camera & Scale
    private Aabb GetCharacterBounds(Node3D root)
    {
        bool hasBounds = false;
        Aabb result = new Aabb(Vector3.Zero, Vector3.Zero);

        foreach (Node child in root.GetChildren())
        {
            if (child is VisualInstance3D vis)
            {
                // Obtener AABB local y transformarlo al espacio del personaje
                Aabb localAabb = vis.GetAabb();
                Aabb worldAabb = root.GlobalTransform * localAabb;

                if (!hasBounds)
                {
                    result = worldAabb;
                    hasBounds = true;
                }
                else
                {
                    result = result.Merge(worldAabb);
                }
            }
            else if (child is Node3D node3D)
            {
                var childAabb = GetCharacterBounds(node3D);
                if (childAabb.Size != Vector3.Zero)
                {
                    if (!hasBounds)
                    {
                        result = childAabb;
                        hasBounds = true;
                    }
                    else
                    {
                        result = result.Merge(childAabb);
                    }
                }
            }
        }

        return result;
    }
    private void NormalizeCharacter(Node3D character, float targetHeight = 2f)
    {
        var aabb = GetCharacterBounds(character);
        if (aabb.Size.Y <= 0) return;

        float scale = targetHeight / aabb.Size.Y;
        character.Scale = Vector3.One * scale;

        // subir o bajar para que los pies queden en y = 0
        character.Position -= new Vector3(0, aabb.Position.Y * scale, 0);
    }
    private void PositionCamera(Camera3D cam, Node3D character)
    {
        var aabb = GetCharacterBounds(character);
        float radius = Mathf.Max(aabb.Size.X, aabb.Size.Z);

        cam.Position = new Vector3(0, radius * 1.5f, radius * 2f);
        cam.LookAt(character.GlobalPosition + Vector3.Up * (aabb.Size.Y * 0.5f), Vector3.Up);
    }

    #endregion

    #region SplitScreen

    private void SetupSplitScreen()
    {
        viewports.Clear();
        cameras.Clear();

        foreach (Node child in uiRoot.GetChildren())
            child.QueueFree();

        int playerCount = PlayerData.GetTempPlayers().Count;

        if (uiRoot is GridContainer grid)
            grid.Columns = playerCount <= 1 ? 1 : 2;

        for (int i = 0; i < playerCount; i++)
            CreatePlayerViewport(i);
    }

    private void CreatePlayerViewport(int index)
    {
        var container = new SubViewportContainer
        {
            Stretch = true,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        uiRoot.AddChild(container);

        var viewport = new SubViewport
        {
            World3D = GetViewport().World3D,
            RenderTargetUpdateMode = SubViewport.UpdateMode.Disabled
        };
        container.AddChild(viewport);
        viewports.Add(viewport);

        var camera = new Camera3D
        {
            Fov = cameraFov,
            Current = true
        };
        viewport.AddChild(camera);
        cameras.Add(camera);

        if (PlayerData.GetTempPlayers()[index].IsConnected())
            viewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Always;
    }

    #endregion

    #region Input

    public override void _Input(InputEvent @event)
    {
        var tempPlayers = PlayerData.GetTempPlayers();
        if (@event.Device >= 0 && !IsDeviceAlreadyAssigned(@event.Device))
        {
            GD.Print($"Detected device {@event.Device}");
        }
        SelectCharacter(@event, tempPlayers);
        ConectDevice(@event, tempPlayers);
    }

    private void ConectDevice(InputEvent @event, List<Player> tempPlayers)
    {
        bool hasDisconectedPlayer = false;
        foreach (Player player in tempPlayers)
        {
            if (!player.IsConnected())
            {
                hasDisconectedPlayer = true;
                break;
            }
        }
        if (@event.IsActionPressed("ui_accept")&& hasDisconectedPlayer)
        {
            int deviceId = @event.Device;
            if (IsDeviceAlreadyAssigned(deviceId)) return;

            for (int i = 0; i < tempPlayers.Count; i++)
            {
                if (!tempPlayers[i].IsConnected())
                {
                    tempPlayers[i].SetDeviceId(deviceId);
                    viewports[i].RenderTargetUpdateMode = SubViewport.UpdateMode.Always;
                    break;
                }
            }
        }
    }

    private void SelectCharacter(InputEvent @event, List<Player> tempPlayers)
    {
        foreach (var player in tempPlayers)
        {
            if (!player.IsConnected()) continue;

            if (!lockedSelections[player])
            {
                if (player.LeftPressed(@event)) ChangeCharacter(player, -1);
                else if (player.RightPressed(@event)) ChangeCharacter(player, 1);
                else if (player.ConfirmPressed(@event)) ConfirmCharacter(player);
            }

            if (player.CancelPressed(@event))
                RetractSelection(player);
        }
    }
    
    private bool IsDeviceAlreadyAssigned(int deviceId)
    {
        foreach (var player in PlayerData.GetTempPlayers())
            if (player.DeviceId == deviceId)
                return true;
        return false;
    }

    #endregion

    private bool AllPlayersConfirmed()
    {
        foreach (var player in PlayerData.GetTempPlayers())
            if (!lockedSelections[player])
                return false;
        return true;
    }

    private void FinalizeSelections()
    {
        if (!AllPlayersConfirmed()) return;

        for (int i = 0; i < PlayerData.GetTempPlayers().Count; i++)
        {
            var player = PlayerData.GetTempPlayers()[i];
            var model = activeCharacters[i];
            if (model != null)
                PlayerData.AddPlayer(new Player(player.DeviceId, model));
        }

        GetTree().ChangeSceneToPacked(nextScene);
    }
}
