using Godot;
using System;
using System.Collections.Generic;
using AnimaParty.assets.scenes.player;

namespace AnimaParty.assets.scenes.table;

public partial class Players : Node3D
{
	private PlayerSelect3d _global;
	private List<Player> _players;
	private int _count;

	private Node3D _player;

	private Label3D _labelCount;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_global = this.GetParent<PlayerSelect3d>();
		_count = _global.Players.Count;
		_players=_global.Players;
		_player=GetNode<Node3D>("res://assets/scenes/player.tscn");
		_labelCount = PlayerCountPanel.GetLabel();
		_labelCount.Text=_count.ToString();
		GD.Print(_labelCount.Text);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_labelCount.Text=_count.ToString();
		if(_global.IsPlayerCountSelection && _count != _global.Players.Count)
		{
			foreach (var player in _players)
			{
				RemoveChild(player);
			}
			foreach (var player in _global.Players)
			{
				AddChild(player);
			}
			_count = _global.Players.Count;
			_players=_global.Players;
		}
	}
}
