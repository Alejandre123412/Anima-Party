using System.Collections.Generic;
using AnimaParty.assets.script.types;
using Godot;

namespace AnimaParty.assets.script.data;

public static class PlayerData
{
	private static readonly List<Player> readOnlyTempPlayer = new();
	public static readonly List<Player> Players =new(); // DeviceID de cada jugador
	public static int PlayerCount => Players.Count;

	public static void AddDevice(int deviceId)
	{
		readOnlyTempPlayer.Add(Player.Intanciate(deviceId));
	}

	public static List<Player> GetTempPlayers()
	{
		return readOnlyTempPlayer;
	}

	public static void AddDefaultPlayer()
	{
		AddDevice(-1);
	}

	public static bool HasNullPlayerList()
	{
		return Players == null || Players.Count == 0;
	}

	public static Player GetTempPlayer(int index)
	{
		return readOnlyTempPlayer[index];
	}

	public static void AddPlayer(Player player)
	{
		Players.Add(player);
	}
}