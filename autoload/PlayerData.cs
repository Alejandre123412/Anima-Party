using System.Collections.Generic;
using AnimaParty.assets.script.types;
using Godot;

namespace AnimaParty.autoload;

public partial class PlayerData : Node
{
	public static PlayerData Instance;
	private readonly List<Player> readOnlyTempPlayer = new();
	public readonly List<Player> Players =new(); // DeviceID de cada jugador
	public int PlayerCount => Players.Count;

	public override void _Ready()
	{
		Instance = this;
	}

	public void AddDevice(int deviceId)
	{
		readOnlyTempPlayer.Add(Player.Intanciate(deviceId));
	}

	public List<Player> GetTempPlayers()
	{
		return readOnlyTempPlayer;
	}

	public void AddDefaultPlayer()
	{
		AddDevice(-1);
	}

	public bool HasNullPlayerList()
	{
		return Players == null || Players.Count == 0;
	}

	public Player GetTempPlayer(int index)
	{
		return readOnlyTempPlayer[index];
	}

	public void AddPlayer(Player player)
	{
		Players.Add(player);
	}
}