using Godot;
using System.Collections.Generic;
using AnimaParty.assets.scenes.title;
using AnimaParty.assets.script.data;

namespace AnimaParty.assets.scenes.Jugadores;

public partial class PlayerCount : Node
{
    private const int MaxPlayers = 4;
    private const string PlayerScenePath =
        "res://assets/scenes/Jugadores/Player.tscn";

    private PackedScene _playerScene;
    private HBoxContainer _players;
    private Label _countLabel;

    private readonly List<VBoxContainer> _containers = new();
    private static Device _playerDevice;

    public override void _Ready()
    {
        _playerScene = GD.Load<PackedScene>(PlayerScenePath);
        if (_playerScene == null)
        {
            GD.PrintErr($"No se pudo cargar {PlayerScenePath}");
            return;
        }

        _playerDevice = TitleScreen.GetPlayerDevice();

        PlayerData.PlayerCount = 0;
        PlayerData.Devices.Add(_playerDevice.DeviceId);

        var view = GetParent().GetNode("View");
        _players = view.GetNode<HBoxContainer>("Players");
        _countLabel = view.GetNode("Selection")
                          .GetNode<Label>("PlayerCounter");

        AddPlayer();
    }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsPressed())
            return;

        if (_playerDevice.CompareTo(new Device(@event.Device)) != 0)
            return;

        if (@event.IsActionPressed("ui_left"))
            RemovePlayer();
        else if (@event.IsActionPressed("ui_right"))
            AddPlayer();
        else if (@event.IsActionPressed("ui_accept"))
            GetTree().ChangeSceneToFile(
                "res://assets/scenes/Jugadores/PlayerSelection.tscn");
    }

    private void AddPlayer()
    {
        if (PlayerData.PlayerCount >= MaxPlayers)
            return;

        var player = _playerScene.Instantiate<VBoxContainer>();
        if (player == null)
            return;

        PlayerData.PlayerCount++;

        player.SizeFlagsHorizontal =
            Control.SizeFlags.Expand | Control.SizeFlags.ShrinkCenter;
        //player.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
        //player.StretchRatio = 1;

        player.GetNode<Label>("Player").Text =
            $"Player {PlayerData.PlayerCount}";

        _players.AddChild(player);
        _containers.Add(player);

        _countLabel.Text = PlayerData.PlayerCount.ToString();
    }

    private void RemovePlayer()
    {
        if (PlayerData.PlayerCount <= 1)
            return;

        PlayerData.PlayerCount--;

        var player = _containers[^1];
        _containers.RemoveAt(_containers.Count - 1);

        player.QueueFree();
        _countLabel.Text = PlayerData.PlayerCount.ToString();
    }
}
