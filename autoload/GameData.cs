using Godot;
using System.Collections.Generic;

public partial class GameData : Node
{
	// Lista de jugadores
	public List<Dictionary<string, PackedScene>> Players { get; set; } = new List<Dictionary<string, PackedScene>>();
}
