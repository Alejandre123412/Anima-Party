using Godot;
using System.Collections.Generic;
using AnimaParty.assets.script.types;
using AnimaParty.autoload;
using GlobalNodes = AnimaParty.autoload.GlobalNodes;

namespace AnimaParty.assets.scenes.Jugadores;

public partial class CharacterViewer : Node3D
{
    [Export] private PackedScene _nextScene;
    [Export] private Control _uiRoot;
    [Export] private float _cameraFov = 70f;

    private readonly List<SubViewport> _viewports = new();
    private readonly List<Camera3D> _cameras = new();
    #nullable enable
    private readonly List<Node3D?> _activeCharacters = new();

    private readonly Dictionary<Player, int> _playerSelections = new();
    private readonly Dictionary<Player, bool> _lockedSelections = new();

    private readonly List<PackedScene> _characterScenes = new();

    public override void _Ready()
    {
        LoadCharacterScenes("res://assets/models/characters/");
        SetupSplitScreen();

        var players = PlayerData.Instance.GetTempPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            _playerSelections[players[i]] = 0;
            _lockedSelections[players[i]] = false;
            AssignCharacterToPlayer(players[i], 0);
        }
    }

    public override void _Process(double delta)
    {
        FinalizeSelections();
    }

    #region Characters

    private void LoadCharacterScenes(string folderPath)
    {
        var dir = DirAccess.Open(folderPath);
        if (dir == null) return;

        dir.ListDirBegin();
        while (true)
        {
            var file = dir.GetNext();
            if (file == "") break;
            if (dir.CurrentIsDir()) continue;
            if (!file.EndsWith(".glb") && !file.EndsWith(".blend")) continue;

            var scene = GD.Load<PackedScene>($"{folderPath}{file}");
            if (scene != null)
                _characterScenes.Add(scene);
        }
        dir.ListDirEnd();
    }

    private void AssignCharacterToPlayer(Player player, int characterIndex)
    {
        int playerIndex = PlayerData.Instance.GetTempPlayers().IndexOf(player);
        if (playerIndex < 0 || playerIndex >= _viewports.Count) return;

        if (_activeCharacters.Count > playerIndex && _activeCharacters[playerIndex] != null)
        {
            _activeCharacters[playerIndex]!.QueueFree();
            _activeCharacters[playerIndex] = null;
        }

        var instance = _characterScenes[characterIndex].Instantiate<Node3D>();
        _viewports[playerIndex].AddChild(instance);

        if (_activeCharacters.Count <= playerIndex)
            _activeCharacters.Add(instance);
        else
            _activeCharacters[playerIndex] = instance;

        NormalizeCharacter(instance);
        PositionCamera(_cameras[playerIndex], instance);

        _playerSelections[player] = characterIndex;
    }

    private void ChangeCharacter(Player player, int delta)
    {
        if (_lockedSelections[player]) return;

        int current = _playerSelections[player];
        int count = _characterScenes.Count;
        int next = (current + delta + count) % count;

        foreach (var kv in _lockedSelections)
        {
            if (kv.Key != player && kv.Value && _playerSelections[kv.Key] == next)
                return;
        }

        AssignCharacterToPlayer(player, next);
    }

    private void ConfirmCharacter(Player player)
    {
        _lockedSelections[player] = true;
    }

    private void RetractSelection(Player player)
    {
        if (_lockedSelections[player])
            _lockedSelections[player] = false;
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
        _viewports.Clear();
        _cameras.Clear();

        foreach (Node child in _uiRoot.GetChildren())
            child.QueueFree();

        int playerCount = PlayerData.Instance.GetTempPlayers().Count;

        if (_uiRoot is GridContainer grid)
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
        _uiRoot.AddChild(container);

        var viewport = new SubViewport
        {
            World3D = GetViewport().World3D,
            RenderTargetUpdateMode = SubViewport.UpdateMode.Disabled
        };
        container.AddChild(viewport);
        _viewports.Add(viewport);

        var camera = new Camera3D
        {
            Fov = _cameraFov,
            Current = true
        };
        viewport.AddChild(camera);
        _cameras.Add(camera);

        if (PlayerData.Instance.GetTempPlayers()[index].IsConnected())
            viewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Always;
    }

    #endregion

    #region Input

    public override void _Input(InputEvent @event)
    {
        var tempPlayers = PlayerData.Instance.GetTempPlayers();
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
                    _viewports[i].RenderTargetUpdateMode = SubViewport.UpdateMode.Always;
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

            if (!_lockedSelections[player])
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
        foreach (var player in PlayerData.Instance.GetTempPlayers())
            if (player.DeviceId == deviceId)
                return true;
        return false;
    }

    #endregion

    private bool AllPlayersConfirmed()
    {
        foreach (var player in PlayerData.Instance.GetTempPlayers())
            if (!_lockedSelections[player])
                return false;
        return true;
    }

    private void FinalizeSelections()
    {
        if (!AllPlayersConfirmed()) return;

        for (int i = 0; i < PlayerData.Instance.GetTempPlayers().Count; i++)
        {
            var player = PlayerData.Instance.GetTempPlayers()[i];
            var model = _activeCharacters[i];

            if (model != null)
            {
                // 1️⃣ Reparentar a nodo global para que no se destruya
                model.GetParent()?.RemoveChild(model);
                GlobalNodes.Instance.AddChild(model);

                // 2️⃣ Guardar en PlayerData
                PlayerData.Instance.AddPlayer(new Player(player.DeviceId, model));

                // 3️⃣ Guardar en lista global de personajes
                if (!GlobalNodes.Instance.Characters.Contains(model))
                    GlobalNodes.Instance.Characters.Add(model);
            }
        }

        // 4️⃣ Cambiar de escena
        GetTree().ChangeSceneToPacked(_nextScene);
    }

}
