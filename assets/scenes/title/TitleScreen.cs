using Godot;
using System;
using AnimaParty.assets.scenes.player;

namespace AnimaParty.assets.scenes.title;
public partial class TitleScreen : Node
{
	private bool _isPress = false;
	private static Player _player;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouse || @event is InputEventKey) return;
		if (_player == null)
			_player = new Player();
		GD.Print(@event.Device);
		_player.SetDevice(@event.Device);
		GD.Print(_player.GetDevice());
		
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
		GetTree().ChangeSceneToFile("res://assets/scenes/table/PlayerSelect3D.tscn");
	}

	public static Player PassPlayer()
	{
		return _player;
	}
}
