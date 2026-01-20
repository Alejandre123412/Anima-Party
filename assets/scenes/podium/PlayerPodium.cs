using Godot;

public partial class PlayerPodium : Node3D
{
	[Export] public PackedScene DefaultCharacterScene;

	private Node3D _currentCharacter;
	public int PlayerIndex;

	public override void _Ready()
	{
		AddToGroup("podiums_" + PlayerIndex);
	}

	public void ShowCharacter(PackedScene characterScene)
	{
		if (_currentCharacter != null)
			_currentCharacter.QueueFree();

		var scene = characterScene ?? DefaultCharacterScene;
		_currentCharacter = scene.Instantiate<Node3D>();
		GetNode<Node3D>("CharacterSpawn").AddChild(_currentCharacter);
	}
}
