using Godot;
using System.Collections.Generic;
using AnimaParty.assets.script.data;

namespace AnimaParty.assets.scenes.main;
public partial class GameManager : Node3D
{
    [Export] public PackedScene PlayerScene;
    [Export] public Node3D Players;

    public int PlayerCount = 1;
    public List<Mesh> SelectedMeshes = new();

    public override void _Ready()
    {
        PlayerCount = PlayerData.PlayerCount;
        SelectedMeshes = PlayerData.SelectedMeshes;
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        PlayerJugable leader = null;

        for (int i = 0; i < PlayerCount; i++)
        {
            PlayerJugable player = PlayerScene.Instantiate<PlayerJugable>();
            Players.AddChild(player);

            player.Name = $"Player_{i + 1}";
            player.GlobalPosition = new Vector3(0, 0, i * 1.2f);

            ApplyMesh(player, i);

            if (i == 0)
            {
                player.IsLeader = true;
                leader = player;
            }
            else
            {
                player.IsLeader = false;
                player.LeaderPath = player.GetPathTo(leader);
            }
        }
    }

    private void ApplyMesh(PlayerJugable player, int index)
    {
        if (index >= SelectedMeshes.Count)
            return;

        var meshInstance = player.GetNode<MeshInstance3D>("MeshInstance3D");
        meshInstance.Mesh = SelectedMeshes[index];
    }
}