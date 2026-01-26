using Godot;
using System.Collections.Generic;
using AnimaParty.assets.script.types;
using AnimaParty.autoload;

namespace AnimaParty.assets.scenes.Jugadores;
public partial class PlayerCount : Node
{
	
	private const int MaxPlayers = 4;
    private const string PlayerUiPath = "res://assets/scenes/Jugadores/Player.tscn";

    [Export] private PackedScene _nextScene;
    private Player _player;
#nullable enable
    private PackedScene? _playerUiScene;
    private HBoxContainer? _playersContainer;
    private Label? _countLabel;
    

    private readonly List<VBoxContainer> _containers = new();

    public override void _Ready()
    {
        _playerUiScene = GD.Load<PackedScene>(PlayerUiPath);
        var view = GetNode("..").GetNode("View");
        if (view == null) { GD.PrintErr("No se encontró View"); return; }
        _playersContainer=view.GetNode<HBoxContainer>("Players");
        if (_playersContainer == null) { GD.PrintErr("No se encontró Players"); return; }
        _countLabel = view.GetNode<Label>("Selection/PlayerCountLabel");
        if (_countLabel == null) { GD.PrintErr("No se encontró PlayerCountLabel"); return; }
        if (PlayerData.Instance.GetTempPlayers().Count == 0) { GD.PrintErr("No hay jugadores en PlayerData"); return; }
        _player = PlayerData.Instance.GetTempPlayer(0);
        for (int i = 0; i < PlayerData.Instance.GetTempPlayers().Count; i++)
            AddPlayerUi();
        //GD.Print($"Player Count: {PlayerData.PlayerCount}");
        //GD.Print($"Player Device: {player.DeviceId}");
    }

    public override void _Input(InputEvent @event)
    {
        //GD.Print($"Input Device: {@event.Device}");
        //GD.Print($"Input Count: {player.DeviceId}");
        if (_player.LeftPressed(@event))
            RemovePlayerUi();
        else if (_player.RightPressed(@event))
            AddPlayerUi();
        else if (_player.ConfirmPressed(@event))
        {
            for (int i = 1; i < _containers.Count; i++)
                PlayerData.Instance.AddDefaultPlayer();
            GetTree().ChangeSceneToFile(_nextScene.GetPath());
        }
    }

    private void AddPlayerUi()
    {
        if (_playerUiScene == null || _playersContainer == null) return;

        if(_containers.Count >= MaxPlayers) return;
        var playerUi = _playerUiScene.Instantiate<VBoxContainer>();
        var label = playerUi.GetNode<Label>("Player");
        if (label != null)
            label.Text = $"Player {_containers.Count+1}";

        _playersContainer.AddChild(playerUi);
        _containers.Add(playerUi);
        
        UpdateCounter();
    }

    private void RemovePlayerUi()
    {
        if (_containers.Count <= 1) return;

        var last = _containers[^1];
        _containers.RemoveAt(_containers.Count - 1);
        last.QueueFree();

        UpdateCounter();
    }

    private void UpdateCounter()
    {
        if (_countLabel != null)
            _countLabel.Text = _containers.Count.ToString();
    }
}
