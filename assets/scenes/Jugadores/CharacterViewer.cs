using Godot;
using System.Collections.Generic;
using AnimaParty.assets.script.data;
using AnimaParty.assets.script.types;

namespace AnimaParty.assets.scenes.Jugadores;

/// <summary>
/// Manages split-screen character viewports.
/// Responsible for creating one SubViewport and Camera per player
/// and enabling them when a player connects.
/// </summary>
public partial class CharacterViewer : Node3D
{
    [Export] private PackedScene nextScene;
    /// <summary>
    /// GridContainer that holds all SubViewportContainers.
    /// Expected to be UI/ViewPorts.
    /// </summary>
    [Export] private Control uiRoot;

    /// <summary>
    /// Default camera field of view.
    /// </summary>
    [Export] private float cameraFov = 70f;

    /// <summary>
    /// List of SubViewports, one per player.
    /// </summary>
    private readonly List<SubViewport> viewports = new();

    /// <summary>
    /// List of cameras associated with each SubViewport.
    /// </summary>
    private readonly List<Camera3D> cameras = new();
    
    private readonly List<Node3D?> activeCharacters = new();

    // Diccionario: jugador → índice del personaje seleccionado
    private Dictionary<Player, int> playerSelections = new();

// Diccionario: jugador → confirmado (true si ya eligió)
    private Dictionary<Player, bool> lockedSelections = new();

// Lista de rutas a los glb de personajes
    private List<string> characterFiles = new();

    /// <summary>
    /// Called when the node enters the scene tree.
    /// Initializes the split-screen layout.
    /// </summary>
    public override void _Ready()
    {
        var tempPlayers = PlayerData.GetTempPlayers();
        characterFiles = ListCharactersFromFolder("res://assets/characters/");

        playerSelections.Clear();
        lockedSelections.Clear();

        foreach (var player in tempPlayers)
        {
            playerSelections.Add(player,0);    // todos empiezan en el primer personaje
            lockedSelections.Add(player,false);
    
            // Instanciamos el primer personaje visible
            AssignCharacterToPlayer(player, characterFiles[0]);
        }
        SetupSplitScreen();
        
    }

    #region Characters
    private List<string> ListCharactersFromFolder(string folderPath)
    {
        List<string> characterFiles = new();
        var dir = DirAccess.Open(folderPath);
        if (dir == null) return characterFiles;

        dir.ListDirBegin();
        while (true)
        {
            string file = dir.GetNext();
            if (file == "") break;
            if (dir.CurrentIsDir()) continue;
            if (!file.EndsWith(".glb") && !file.EndsWith(".blend")) continue;

            characterFiles.Add($"{folderPath}{file}");
        }
        dir.ListDirEnd();
        return characterFiles;
    }
    private Node3D LoadCharacterFromGlb(string path)
    {
        // Crear document y estado
        var gltfDoc = new Godot.GltfDocument();
        var gltfState = new Godot.GltfState();

        // Cargar el archivo GLB en el state
        var err = gltfDoc.AppendFromFile(path, gltfState);
        if (err != Error.Ok)
        {
            GD.PrintErr($"No se pudo cargar {path}: {err}");
            return null;
        }

        // Generar la escena a partir del state
        Node sceneNode = gltfDoc.GenerateScene(gltfState);
        if (sceneNode == null)
        {
            GD.PrintErr($"No se pudo generar escena de {path}");
            return null;
        }

        return sceneNode as Node3D;
    }
    /// <summary>
    /// Asigna un personaje a un jugador y lo instancia en su viewport.
    /// </summary>
    private void AssignCharacterToPlayer(Player player, string glbPath)
    {
        if (player == null) return;

        int playerIndex = PlayerData.GetTempPlayers().IndexOf(player);
        if (playerIndex < 0 || playerIndex >= viewports.Count) return;

        // Limpiar personaje anterior
        if (activeCharacters.Count > playerIndex && activeCharacters[playerIndex] != null)
        {
            activeCharacters[playerIndex]?.QueueFree();
            activeCharacters[playerIndex] = null;
        }

        // Instanciar personaje desde GLB
        var characterNode = LoadCharacterFromGlb(glbPath);
        if (characterNode == null) return;

        viewports[playerIndex].AddChild(characterNode);

        // Guardar referencia en activeCharacters
        if (activeCharacters.Count <= playerIndex)
            activeCharacters.Add(characterNode);
        else
            activeCharacters[playerIndex] = characterNode;

        // Posición inicial frente a la cámara
        characterNode.GlobalTransform = Transform3D.Identity;

        // Actualizar diccionario de selección
        playerSelections[player] = characterFiles.IndexOf(glbPath);
    }


    /// <summary>
    /// Cambia el personaje que el jugador está visualizando, evitando duplicados.
    /// </summary>
    private void ChangeCharacter(Player player, int delta)
    {
        if (player == null) return;
        
        if (lockedSelections.ContainsKey(player) && lockedSelections[player]) return;
        if (characterFiles.Count == 0) return;

        int currentIndex = playerSelections.ContainsKey(player) ? playerSelections[player] : 0;
        int nextIndex = currentIndex + delta;
        int count = characterFiles.Count;

        // Wrap-around
        if (nextIndex < 0) nextIndex = count - 1;
        if (nextIndex >= count) nextIndex = 0;

        // Evitar personajes ya confirmados por otros jugadores
        bool safe = false;
        while (!safe)
        {
            safe = true;
            foreach (var kvp in lockedSelections)
            {
                Player otherPlayer = kvp.Key;
                bool locked = kvp.Value;
                if (otherPlayer == player) continue;
                if (locked && playerSelections.ContainsKey(otherPlayer) && playerSelections[otherPlayer] == nextIndex)
                {
                    safe = false;
                    nextIndex += delta > 0 ? 1 : -1;
                    if (nextIndex < 0) nextIndex = count - 1;
                    if (nextIndex >= count) nextIndex = 0;
                    break;
                }
            }
        }

        // Actualizar selección y viewport
        playerSelections[player] = nextIndex;
        AssignCharacterToPlayer(player, characterFiles[nextIndex]);
    }

    /// <summary>
    /// Confirma la selección del jugador, bloqueando el personaje para otros.
    /// </summary>
    private void ConfirmCharacter(Player player)
    {
        if (player == null) return;

        lockedSelections[player] = true;

        if (!playerSelections.ContainsKey(player)) return;

        int charIndex = playerSelections[player];
        GD.Print($"Player {player} confirmó {characterFiles[charIndex]}");
    }

    /// <summary>
    /// Permite que el jugador retracte su selección y vuelva a elegir.
    /// </summary>
    private void RetractSelection(Player player)
    {
        if (player == null) return;

        // Solo se puede retractar si estaba confirmado
        if (lockedSelections.ContainsKey(player) && lockedSelections[player])
        {
            lockedSelections[player] = false;
            GD.Print($"Player {player} retractó su selección de {characterFiles[playerSelections[player]]}");
        }
    }
    
    #endregion
    #region SetupSplitScreen
    /// <summary>
    /// Creates the split-screen layout based on the player count.
    /// Uses a GridContainer to automatically arrange viewports.
    /// </summary>
    private void SetupSplitScreen()
    {
        viewports.Clear();
        cameras.Clear();

        // Remove any existing UI elements
        foreach (Node child in uiRoot.GetChildren())
            child.QueueFree();

        int playerCount = PlayerData.GetTempPlayers().Count;

        // Configure grid columns (1 column for single player, 2 otherwise)
        if (uiRoot is GridContainer grid)
            grid.Columns = playerCount <= 1 ? 1 : 2;

        // Create one viewport per player
        for (int i = 0; i < playerCount; i++)
            CreatePlayerViewport(i);
    }

    /// <summary>
    /// Creates a SubViewportContainer with a SubViewport and a Camera3D
    /// for the specified player index.
    /// </summary>
    /// <param name="index">Player index</param>
    private void CreatePlayerViewport(int index)
    {
        var container = new SubViewportContainer
        {
            Stretch = true,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        uiRoot.AddChild(container);

        // Crear SubViewport
        var viewport = new SubViewport
        {
            World3D = GetViewport().World3D,
            RenderTargetUpdateMode = SubViewport.UpdateMode.Disabled
        };
        container.AddChild(viewport);
        viewports.Add(viewport);

        // Cámara para este viewport
        var camera = new Camera3D
        {
            Fov = cameraFov,
            Current = true
        };
        viewport.AddChild(camera);
        cameras.Add(camera);

        // Si el jugador NO está conectado, añadimos un panel con mensaje
        if (index >= PlayerData.GetTempPlayers().Count || !PlayerData.GetTempPlayers()[index].IsConnected())
        {
            var panel = new Panel
            {
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill
            };
            container.AddChild(panel);

            var label = new Label
            {
                Text = "Press A / ui_accept to join",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            panel.AddChild(label);
        }
        else
        {
            // Si ya está conectado, activamos viewport
            viewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Always;
        }
    }


    /// <summary>
    /// Enables or disables a player's SubViewport depending on whether
    /// the player is connected.
    /// </summary>
    /// <param name="index">Player index</param>
    private void UpdateViewportState(int index)
    {
        var player = PlayerData.GetTempPlayer(index);
        bool connected = player.IsConnected();

        viewports[index].RenderTargetUpdateMode =
            connected
                ? SubViewport.UpdateMode.Always
                : SubViewport.UpdateMode.Disabled;
    }
    #endregion

    #region Input
    /// <summary>
    /// Handles player connection input.
    /// When the accept action is pressed, assigns the input device
    /// to the first available player slot.
    /// </summary>
    /// <param name="event">Input event</param>
    public override void _Input(InputEvent @event)
    {
        if (!@event.IsActionPressed("ui_accept"))
            return;

        int deviceId = @event.Device;

        // 1️⃣ Evitar que se conecte el mismo device dos veces
        if (IsDeviceAlreadyAssigned(deviceId))
            return;

        var tempPlayers = PlayerData.GetTempPlayers();

        // 2️⃣ Buscar primer Player null sin DeviceId
        for (int i = 0; i < tempPlayers.Count; i++)
        {
            var player = tempPlayers[i];

            if (player.IsConnected())
                continue;

            // 3️⃣ Conectar el jugador
            player.SetDeviceId(deviceId);
            GD.Print($"Player {i} connected with device {deviceId}");

            // 4️⃣ Ocultar overlay
            //overlays[i].Visible = false;

            // 5️⃣ Activar viewport
            viewports[i].RenderTargetUpdateMode = SubViewport.UpdateMode.Always;

            // 6️⃣ Asignar personaje y cámara
            //AssignCharacterToPlayer(i);
            //AdjustCameraToCharacter(i);

            break;
        }

        foreach (var player in tempPlayers)
        {
            if (!player.IsConnected()) continue;
            int index = PlayerData.GetTempPlayers().IndexOf(player);

            if (!lockedSelections[player])
            {
                if (player.LeftPressed(@event))
                    ChangeCharacter(player, -1);
                else if (player.RightPressed(@event))
                    ChangeCharacter(player, 1);
                else if (player.ConfirmPressed(@event))
                    ConfirmCharacter(player);
            }
            
            if(player.CancelPressed(@event))
                RetractSelection(player);
        }
    }

    /// <summary>
    /// Checks whether a device is already assigned to a player.
    /// </summary>
    /// <param name="deviceId">Input device id</param>
    /// <returns>True if already assigned</returns>
    private bool IsDeviceAlreadyAssigned(int deviceId)
    {
        foreach (var player in PlayerData.GetTempPlayers())
        {
            if (player.DeviceId == deviceId)
                return true;
        }
        return false;
    }
    #endregion
    private bool AllPlayersConfirmed()
    {
        foreach (var player in PlayerData.GetTempPlayers())
        {
            if (!lockedSelections.ContainsKey(player) || !lockedSelections[player])
                return false;
        }
        return true;
    }
    private void FinalizeSelections()
    {
        if (!AllPlayersConfirmed()) return;


        for (int i = 0; i < PlayerData.GetTempPlayers().Count; i++)
        {
            var player = PlayerData.GetTempPlayers()[i];
            var model = activeCharacters[i]; // nodo 3D del personaje seleccionado

            if (model == null) continue;

            
            
            // Crear la estructura de jugador para el juego
            PlayerData.AddPlayer(new Player(player.DeviceId,model));
        }

        // Cambiar de pestaña / escena al juego
        // Por ejemplo, usando un CanvasLayer de UI o SceneTree
        GetTree().ChangeSceneToPacked(nextScene);
    }


}
