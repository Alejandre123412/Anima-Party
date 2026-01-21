using Godot;
using System.Collections.Generic;

public partial class PlayerSelect3DController : Node3D
{
    // Referencias a nodos de la escena
    [Export] public Node3D Players;                        
    [Export] public Control PanelCountPanel;               
    [Export] public Control CharacterSelectionPanel;       

    [Export] public Label PlayerCountLabel;   // Label dentro de PanelCountPanel que muestra la cantidad
    [Export] public int maxPlayers = 4;       // Número máximo de jugadores
    [Export] public int minPlayers = 1;       // Número mínimo de jugadores

    // Datos temporales de jugadores
    public List<PlayerData> PlayerList = new List<PlayerData>();
    private int selectedPlayerCount = 1;       // Valor inicial

    public override void _Ready()
    {
        // Inicialmente solo el panel de cantidad visible
        PanelCountPanel.Visible = true;
        CharacterSelectionPanel.Visible = false;

        // Mostrar valor inicial en la UI
        UpdatePlayerCountLabel();

        // Inicializamos al menos un jugador
        SetPlayerCount(selectedPlayerCount);
    }

    // Función llamada por ArrowButton
    public void IncreasePlayers()
    {
        if (selectedPlayerCount < maxPlayers)
        {
            SetPlayerCount(selectedPlayerCount + 1);
        }
    }

    public void DecreasePlayers()
    {
        if (selectedPlayerCount > minPlayers)
        {
            SetPlayerCount(selectedPlayerCount - 1);
        }
    }

    // Función central para actualizar el número de jugadores
    public void SetPlayerCount(int count)
    {
        selectedPlayerCount = count;

        // Actualizamos la UI
        UpdatePlayerCountLabel();

        // Inicializamos PlayerList
        PlayerList.Clear();
        for (int i = 0; i < selectedPlayerCount; i++)
        {
            PlayerList.Add(new PlayerData());
        }

        // Actualizamos modelos en escena
        SpawnPlayerModels();
    }

    private void UpdatePlayerCountLabel()
    {
        if (PlayerCountLabel != null)
        {
            PlayerCountLabel.Text = $"{selectedPlayerCount}";
        }
    }

    private void SpawnPlayerModels()
    {
        // Limpiar modelos existentes
        foreach (Node child in Players.GetChildren())
        {
            child.QueueFree();
        }

        float spacing = 2f; // separación entre jugadores
        Vector3 center = Vector3.Zero;

        for (int i = 0; i < selectedPlayerCount; i++)
        {
            PackedScene playerScene = GD.Load<PackedScene>("res://assets/scene/player/Player.tscn");
            Node3D playerInstance = playerScene.Instantiate<Node3D>();
            playerInstance.Name = $"Player{i + 1}";

            // Posicionamiento según el número de jugadores
            Vector3 pos = Vector3.Zero;

            switch (selectedPlayerCount)
            {
                case 1:
                    pos = center; // centro
                    break;
                case 2:
                    pos = (i == 0) ? new Vector3(-spacing, 0, 0) : new Vector3(spacing, 0, 0);
                    break;
                case 3:
                    pos = i switch
                    {
                        0 => new Vector3(-spacing, 0, 0), // izquierda
                        1 => center, // centro
                        2 => new Vector3(spacing, 0, 0), // derecha
                        _ => Vector3.Zero
                    };
                    break;
                case 4:
                    pos = i switch
                    {
                        0 => new Vector3(-1.5f * spacing, 0, 0),
                        1 => new Vector3(-0.5f * spacing, 0, 0),
                        2 => new Vector3(0.5f * spacing, 0, 0),
                        3 => new Vector3(1.5f * spacing, 0, 0),
                        _ => Vector3.Zero
                    };
                    break;
            }

            playerInstance.SetPosition(pos);
            Players.AddChild(playerInstance);

            // Guardamos referencia en PlayerData
            PlayerList[i].CharacterModel = playerInstance;
        }
    }

    // Clase para almacenar datos de cada jugador
    public class PlayerData
    {
        public Node3D CharacterModel = null;
        public string SelectedCharacter = ""; // ID o nombre del personaje
    }

    // Llamar al terminar selección de cantidad para pasar a selección de personajes
    public void GoToCharacterSelection()
    {
        PanelCountPanel.Visible = false;
        CharacterSelectionPanel.Visible = true;

        // Aquí puedes inicializar la UI de CharacterSelectionPanel
    }
}
