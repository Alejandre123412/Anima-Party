using Godot;
using System.Collections.Generic;

public partial class PlayerCountMenu : Control
{
	public void StartGame(int playerCount)
	{
		GameData.Instance.Players.Clear();

		for (int i = 0; i < playerCount; i++)
		{
			GameData.Instance.Players.Add(new Dictionary<string, PackedScene>
			{
				{ "character", null } // por defecto
			});
		}

		GetTree().ChangeSceneToFile("res://scenes/table/PlayerSelectionTable.tscn");
	}
}
