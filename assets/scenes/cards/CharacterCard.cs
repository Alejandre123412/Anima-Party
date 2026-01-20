using Godot;

public partial class CharacterCard : Area3D
{
	[Export] public PackedScene CharacterScene;
	[Export] public int PlayerIndex;

	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	private void OnMouseEntered()
	{
		var podium = GetPodium();
		podium?.ShowCharacter(CharacterScene);
	}

	private void OnMouseExited()
	{
		var podium = GetPodium();
		var defaultScene = GameData.Instance.Players[PlayerIndex]["character"];
		podium?.ShowCharacter(defaultScene);
	}

	public void ConfirmSelection()
	{
		GameData.Instance.Players[PlayerIndex]["character"] = CharacterScene;
	}

	private PlayerPodium GetPodium()
	{
		var group = "podiums_" + PlayerIndex;
		var pods = GetTree().GetNodesInGroup(group);
		if (pods.Count > 0)
			return pods[0] as PlayerPodium;
		return null;
	}
}
