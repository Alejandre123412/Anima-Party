using Godot;
using System;
using System.Collections.Generic;
using AnimaParty.assets.scenes.player;
using AnimaParty.assets.scenes.title;

namespace AnimaParty.assets.scenes.table;
public partial class PlayerSelect3d : Node3D
{
	private PlayerCountPanel _playerCountPanel;
	public List<Player> Players = new();
	private Player _player;

	public bool IsPlayerCountSelection;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//_player = TitleScreen.PassPlayer();
		GD.Print(_player);
		Players.Add(_player);
		_playerCountPanel = GetNode<PlayerCountPanel>("PlayerCountPanel");
		IsPlayerCountSelection = true;
	}

	public override void _Input(InputEvent @event)
	{
		if(IsPlayerCountSelection)
		{
			GD.Print(@event.Device);
			GD.Print(_player.GetDevice());
			if (@event.Device != _player.GetDevice())
				return;

			if (@event.IsActionPressed("ui_right"))
			{
				Players.Add(new Player());
				_playerCountPanel.GetBtnR().Play("OnPress");
			}
			else if (@event.IsActionPressed("ui_left"))
			{
				Players.Remove(Players[Players.Count-1]);
				_playerCountPanel.GetBtnL().Play("OnPress");
			}
		}
		else
		{
			
		}
	}

}
