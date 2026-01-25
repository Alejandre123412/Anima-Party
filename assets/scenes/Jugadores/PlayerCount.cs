using Godot;
using System.Collections.Generic;
using AnimaParty.assets.script.data;
using AnimaParty.assets.script.types;

namespace AnimaParty.assets.scenes.Jugadores;
public partial class PlayerCount : Node
{
	
	private const int MAX_PLAYERS = 4;
    private const string PLAYER_UI_PATH = "res://assets/scenes/Jugadores/Player.tscn";

    [Export] private PackedScene nextScene;
    private Player player;
#nullable enable
    private PackedScene? playerUiScene;
    private HBoxContainer? playersContainer;
    private Label? countLabel;
    

    private readonly List<VBoxContainer> containers = new();

    public override void _Ready()
    {
        playerUiScene = GD.Load<PackedScene>(PLAYER_UI_PATH);
        var view = GetNode("..").GetNode("View");
        if (view == null) { GD.PrintErr("No se encontró View"); return; }
        playersContainer=view.GetNode<HBoxContainer>("Players");
        if (playersContainer == null) { GD.PrintErr("No se encontró Players"); return; }
        countLabel = view.GetNode<Label>("Selection/PlayerCountLabel");
        if (countLabel == null) { GD.PrintErr("No se encontró PlayerCountLabel"); return; }
        if (PlayerData.PlayerCount == 0) { GD.PrintErr("No hay jugadores en PlayerData"); return; }
        player = PlayerData.GetTempPlayer(0);
        for (int i = 0; i < PlayerData.PlayerCount; i++)
            AddPlayerUi();
        GD.Print($"Player Count: {PlayerData.PlayerCount}");
        GD.Print($"Player Device: {player.DeviceId}");
    }

    public override void _Input(InputEvent @event)
    {
        GD.Print($"Input Device: {@event.Device}");
        GD.Print($"Input Count: {player.DeviceId}");
        if (player.LeftPressed(@event))
            RemovePlayerUi();
        else if (player.RightPressed(@event))
            AddPlayerUi();
        else if (player.ConfirmPressed(@event))
        {
            for (int i = 1; i < containers.Count; i++)
                PlayerData.AddDefaultPlayer();
            GetTree().ChangeSceneToFile(nextScene.GetPath());
        }
    }

    private void AddPlayerUi()
    {
        if (playerUiScene == null || playersContainer == null) return;

        if(containers.Count >= MAX_PLAYERS) return;
        var playerUi = playerUiScene.Instantiate<VBoxContainer>();
        var label = playerUi.GetNode<Label>("Player");
        if (label != null)
            label.Text = $"Player {containers.Count+1}";

        playersContainer.AddChild(playerUi);
        containers.Add(playerUi);
        
        UpdateCounter();
    }

    private void RemovePlayerUi()
    {
        if (containers.Count <= 1) return;

        var last = containers[^1];
        containers.RemoveAt(containers.Count - 1);
        last.QueueFree();

        UpdateCounter();
    }

    private void UpdateCounter()
    {
        if (countLabel != null)
            countLabel.Text = containers.Count.ToString();
    }
}
