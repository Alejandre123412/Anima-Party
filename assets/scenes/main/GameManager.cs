using Godot;
using System.Collections.Generic;
using AnimaParty.assets.script.data;
using AnimaParty.assets.script.types;

namespace AnimaParty.assets.scenes.main;

public partial class GameManager : Node
{
	#region Exports
	[Export] public Node3D PlayersRoot;
	[Export] public Camera3D MainCamera;  // Cámara principal
	[Export] public float PlayerSpacing = 2.0f;
	[Export] public float MoveSpeed = 5.0f;
	[Export] public float FollowDistance = 1.5f;
	[Export] public float CameraHeight = 5.0f;      // Altura de la cámara sobre la fila
	[Export] public float CameraDistance = 6.0f;    // Distancia detrás del líder
	[Export] public float CameraLerpSpeed = 5.0f;   // Suavizado
	#endregion

	private Player leader;

	public override void _Ready()
	{
		SetupPlayerLine();
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;

		if (leader != null)
			MoveLeader(leader, dt);

		MoveFollowers(dt);
		UpdateCamera(dt);
	}

	#region Setup
	private void SetupPlayerLine()
	{
		if (PlayerData.PlayerCount == 0) return;

		// Posicionar todos los jugadores en fila inicial
		for (int i = 0; i < PlayerData.PlayerCount; i++)
		{
			Player pdata = PlayerData.Players[i];
			Node3D node = pdata.player;

			if (node == null) continue;

			PlayersRoot.AddChild(node);
			node.GlobalPosition = new Vector3(i * PlayerSpacing, 0, 0);

			// Primer jugador = líder
			if (i == 0)
				leader = pdata;
		}
	}
	#endregion

	#region Movement
	private void MoveLeader(Player leaderPlayer, float dt)
	{
		Node3D node = leaderPlayer.player;
		if (node == null) return;

		Vector3 inputDir = Vector3.Zero;

		if (Input.IsActionPressed("ui_up")) inputDir.Z -= 1;
		if (Input.IsActionPressed("ui_down")) inputDir.Z += 1;
		if (Input.IsActionPressed("ui_left")) inputDir.X -= 1;
		if (Input.IsActionPressed("ui_right")) inputDir.X += 1;

		if (inputDir.Length() > 0)
		{
			inputDir = inputDir.Normalized();
			node.GlobalPosition += inputDir * MoveSpeed * dt;
			node.LookAt(node.GlobalPosition + inputDir, Vector3.Up);
		}
	}

	private void MoveFollowers(float dt)
	{
		for (int i = 1; i < PlayerData.PlayerCount; i++)
		{
			Player follower = PlayerData.Players[i];
			Node3D followerNode = follower.player;
			if (followerNode == null) continue;

			Node3D targetNode = PlayerData.Players[i - 1].player;
			if (targetNode == null) continue;

			Vector3 dir = targetNode.GlobalPosition - followerNode.GlobalPosition;
			float distance = dir.Length();

			if (distance > FollowDistance)
			{
				dir = dir.Normalized();
				followerNode.GlobalPosition += dir * MoveSpeed * dt;
				followerNode.LookAt(targetNode.GlobalPosition, Vector3.Up);
			}
		}
	}
	#endregion

	#region Camera
	private void UpdateCamera(float dt)
	{
		if (MainCamera == null || leader == null || leader.player == null) return;

		Vector3 leaderPos = leader.player.GlobalPosition;
		Vector3 targetPos = leaderPos - leader.player.GlobalTransform.Basis.Z * CameraDistance;
		targetPos.Y += CameraHeight;

		// Suavizado con lerp
		MainCamera.GlobalPosition = MainCamera.GlobalPosition.Lerp(targetPos, CameraLerpSpeed * dt);
		MainCamera.LookAt(leaderPos, Vector3.Up);
	}
	#endregion
}
