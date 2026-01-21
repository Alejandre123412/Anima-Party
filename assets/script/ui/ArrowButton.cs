using Godot;

public partial class ArrowButton : StaticBody3D
{
	[Export] public bool IsRightButton;
	[Export] public NodePath ControllerPath;

	private PlayerSelectScene _controller;

	public override void _Ready()
	{
		_controller = GetNode<PlayerSelectScene>(ControllerPath);
	}

	public void Press()
	{
		if (IsRightButton)
			_controller.IncreasePlayers();
		else
			_controller.DecreasePlayers();
	}
}
