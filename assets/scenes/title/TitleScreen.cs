using Godot;
using System;
using AnimaParty.assets.scenes.player;
using AnimaParty.assets.script.data;

namespace AnimaParty.assets.scenes.title;
public partial class TitleScreen : Node
{
	private bool _isPress = false;
	private static Device Player { get; set; }
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouse || @event is InputEventKey) return;
		Player=new Device(@event.Device);
		
		bool hasPress = @event.IsActionPressed("ui_l1")||@event.IsActionPressed("ui_r1");
		if (!_isPress && hasPress)
		{
			_isPress = true;
		}

		if (_isPress && hasPress)
		{
			GD.Print("Pressed");
			Cambiar();
		}
	}

	private void Cambiar()
	{
		GetTree().ChangeSceneToFile("res://assets/scenes/Jugadores/PlayerCount.tscn");
	}

	public static Device GetPlayerDevice()
	{
		return Player;
	}
}
