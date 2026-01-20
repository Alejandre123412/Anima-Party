using Godot;
using System.Collections.Generic;

public partial class PlayerSelectionTable : Node3D
{
	[Export] public PackedScene PodiumScene;

	public override void _Ready()
	{
		SpawnPodiums();
	}

	private void SpawnPodiums()
	{
		for (int i = 0; i < GameData.Instance.Players.Count; i++)
		{
			var podium = PodiumScene.Instantiate<PlayerPodium>();
			podium.PlayerIndex = i;
			AddChild(podium);
			podium.ShowCharacter(GameData.Instance.Players[i]["character"]);
		}
	}
}
