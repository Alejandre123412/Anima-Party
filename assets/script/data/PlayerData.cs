using System.Collections.Generic;
using Godot;

namespace AnimaParty.assets.script.data;

public static class PlayerData
{
	public static int PlayerCount = 1;
	public static List<Mesh> SelectedMeshes = new();
	public static List<int> SelectedCharacters => Devices;
	public static List<int> Devices = new();

	public static void Reset()
	{
		PlayerCount = 0;
		SelectedMeshes.Clear();
		Devices.Clear();
	}
}