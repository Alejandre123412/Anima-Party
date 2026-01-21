using Godot;
using System;
using System.Collections.Generic;
using AnimaParty.assets.scenes.player;

namespace AnimaParty.assets.scenes.table;
public partial class PlayerSelectScene : Node3D
{
    [Export] private PackedScene _playerScene;

    private Node3D _playersRoot;
    private List<Player> _players = new();

    private int _playerCount = 1;
    private const int MaxPlayers = 4;

    public override void _Ready()
    {
        _playersRoot = GetNode<Node3D>("Players");

        CreatePlayers();
        UpdatePlayers();
    }
    
    private void CreatePlayers()
    {
        for (int i = 0; i < MaxPlayers; i++)
        {
            var player = _playerScene.Instantiate<Player>();
            _playersRoot.AddChild(player);

            // Posiciones fijas sobre la mesa
            player.Position = GetPlayerPosition(i);

            // Modelo base (puedes ajustar valores)
            player.SetDefaultModel();

            _players.Add(player);
        }
    }

    private Vector3 GetPlayerPosition(int index)
    {
        float spacing = 1.2f;
        float startX = -spacing * 1.5f;

        return new Vector3(
            startX + spacing * index,
            0.9f,   // altura sobre la mesa (AJUSTA ESTO)
            0f
        );
    }
    private void UpdatePlayers()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            bool active = i < _playerCount;

            _players[i].Visible = active;

            // Opcional: feedback visual
            _players[i].Scale = active
                ? Vector3.One
                : Vector3.One * 0.85f;
        }
    }

    public void IncreasePlayers()
    {
        _playerCount = Mathf.Min(_playerCount + 1, MaxPlayers);
        UpdatePlayers();
    }

    public void DecreasePlayers()
    {
        _playerCount = Mathf.Max(_playerCount - 1, 1);
        UpdatePlayers();
    }

    public int GetPlayerCount()
    {
        return _playerCount;
    }

}