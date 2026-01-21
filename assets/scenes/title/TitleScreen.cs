using Godot;
using System;
namespace AnimaParty.assets.scenes.title;
public partial class TitleScreen : Node
{

	private bool cambiado = false;

	public override void _Process(double delta)
	{
		if (cambiado) return;
		// Left Click
		bool mouseClick = Input.IsMouseButtonPressed(MouseButton.Left);

		// L + R (LB + RB / L1 + R1)
		bool l = Input.IsJoyButtonPressed(0, JoyButton.LeftShoulder);
		bool r = Input.IsJoyButtonPressed(0, JoyButton.RightShoulder);

		if (mouseClick || (l && r))
		{
			cambiado = true;
			Cambiar();
		}
	}

	private void Cambiar()
	{
		GetTree().ChangeSceneToFile("res://assets/scenes/table/PlayerSelect3D.tscn");
	}
}
