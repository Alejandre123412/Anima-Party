using Godot;
using AnimaParty.assets.script.types;
using AnimaParty.autoload;

namespace AnimaParty.assets.scenes.main;

public partial class GameManager : Node
{
	#region Exports
	[Export] public Node3D PlayersRoot;
	[Export] public Camera3D MainCamera;
	[Export] public float PlayerSpacing = 2.0f;
	[Export] public float MoveSpeed = 5.0f;
	[Export] public float FollowDistance = 1.5f;
	[Export] public float CameraHeight = 5.0f;
	[Export] public float CameraDistance = 6.0f;
	[Export] public float CameraLerpSpeed = 5.0f;
	#endregion

	private Player _leader;

	public override void _Ready()
	{
		SetupPlayerLine();
		SetupCamera();
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;

		if (_leader != null)
			MoveLeader(_leader, dt);

		MoveFollowers(dt);
		UpdateCamera(dt);
	}

	#region Setup
	private void SetupPlayerLine()
	{
		var players = PlayerData.Instance.Players;
		for (int i = 0; i < players.Count; i++)
		{
			var node = players[i].player;
			if (node == null) continue;

			// ❌ Problema estaba aquí
			// PlayersRoot.AddChild(node);

			// ✅ Solución: quitar del padre actual si lo tiene
			if (node.GetParent() != PlayersRoot)
			{
				node.GetParent()?.RemoveChild(node);
				PlayersRoot.AddChild(node);
			}

			node.GlobalPosition = new Vector3(i * PlayerSpacing, 0, 0);

			if (i == 0)
				_leader = players[i];
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
		for (int i = 1; i < PlayerData.Instance.PlayerCount; i++)
		{
			Node3D follower = PlayerData.Instance.Players[i].player;
			Node3D target = PlayerData.Instance.Players[i - 1].player;

			if (follower == null || target == null) continue;

			Vector3 dir = target.GlobalPosition - follower.GlobalPosition;
			float dist = dir.Length();

			if (dist > FollowDistance)
			{
				dir = dir.Normalized();
				follower.GlobalPosition += dir * MoveSpeed * dt;
				follower.LookAt(target.GlobalPosition, Vector3.Up);
			}
		}
	}
	#endregion

	#region Camera
	private void SetupCamera()
	{
		if (MainCamera == null || PlayersRoot == null) return;

		// Encuentra el centro de la fila de jugadores
		Vector3 min = new Vector3(float.MaxValue, 0, float.MaxValue);
		Vector3 max = new Vector3(float.MinValue, 0, float.MinValue);

		foreach (Node3D child in PlayersRoot.GetChildren())
		{
			min = new Vector3(
				Mathf.Min(min.X, child.GlobalPosition.X),
				Mathf.Min(min.Y, child.GlobalPosition.Y),
				Mathf.Min(min.Z, child.GlobalPosition.Z)
			);
			max = new Vector3(
				Mathf.Max(max.X, child.GlobalPosition.X),
				Mathf.Max(max.Y, child.GlobalPosition.Y),
				Mathf.Max(max.Z, child.GlobalPosition.Z)
			);
		}

		Vector3 center = (min + max) * 0.5f;

		// Posición de la cámara atrás y arriba de la fila
		MainCamera.GlobalPosition = center + new Vector3(0, CameraHeight, CameraDistance);
		MainCamera.LookAt(center, Vector3.Up);
	}

	private void UpdateCamera(float dt)
	{
		if (MainCamera == null || _leader == null || _leader.player == null) return;

		Vector3 leaderPos = _leader.player.GlobalPosition;
		Vector3 targetPos = leaderPos - _leader.player.GlobalTransform.Basis.Z * CameraDistance;
		targetPos.Y += CameraHeight;

		// Si la distancia es muy pequeña, ponla directamente
		if ((MainCamera.GlobalPosition - targetPos).Length() < 0.01f)
			MainCamera.GlobalPosition = targetPos;
		else
			MainCamera.GlobalPosition = MainCamera.GlobalPosition.Lerp(targetPos, CameraLerpSpeed * dt);

		MainCamera.LookAt(leaderPos, Vector3.Up);
	}
	#endregion
}