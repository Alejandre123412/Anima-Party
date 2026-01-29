using System.Collections.Generic;
using AnimaParty.autoload;
using Godot;

namespace AnimaParty.assets.scenes.main.modos;

public partial class ModeSelectManager : Node
{
    [Export] public Camera3D Camera;
    [Export] public NodePath EntrancesPath;

    private Node3D _entrances;
    private readonly List<PlayerJugable> _players = new();
    private int _currentIndex = 0;

    public override void _Ready()
    {
        _entrances = GetNode<Node3D>(EntrancesPath);

        // Crear hasta 4 jugadores
        for (int i = 0; i < 4; i++)
        {
            PlayerJugable player = new PlayerJugable();
            player.Name = "Player" + (i + 1);
            player.IsLeader = (i == 0); // primer jugador humano es líder
            player.IsAi = i != 0;       // los demás son IA
            AddChild(player);
            _players.Add(player);
            GlobalNodes.Instance.Characters.Add(player);

            // Posición inicial en entradas
            if (i < _entrances.GetChildCount())
                player.GlobalTransform = _entrances.GetChild<Node3D>(i).GlobalTransform;
        }

        // Guardar en slots globales
        for (int i = 0; i < _players.Count; i++)
            GlobalNodes.Instance.CharacterSlots[i] = _players[i];

        UpdateCamera();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_left"))
        {
            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = _players.Count - 1;
            UpdateCamera();
        }
        else if (Input.IsActionJustPressed("ui_right"))
        {
            _currentIndex++;
            if (_currentIndex >= _players.Count) _currentIndex = 0;
            UpdateCamera();
        }

        if (Input.IsActionJustPressed("ui_accept"))
        {
            // Confirmar selección y cambiar escena
            GetTree().ChangeSceneToFile("res://assets/scenes/Minigame.tscn");
        }
    }

    private void UpdateCamera()
    {
        PlayerJugable currentPlayer = _players[_currentIndex];
        Node3D entrance = _entrances.GetChild<Node3D>(_currentIndex);

        // Posición de cámara: detrás y arriba del jugador
        Vector3 camPos = currentPlayer.GlobalTransform.Origin + new Vector3(0, 3, 5);
        Camera.GlobalTransform = new Transform3D(Camera.GlobalTransform.Basis, camPos);

        // Mirar a la entrada correspondiente
        Camera.LookAt(entrance.GlobalTransform.Origin, Vector3.Up);
    }
}