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

		UpdateCameraLook();
	}


	#region Setup
	private void SetupPlayerLine()
	{
		var players = PlayerData.Instance.Players;

		if (PlayersRoot == null)
		{
			GD.PrintErr("PlayersRoot no está asignado.");
			return;
		}

		for (int i = 0; i < players.Count; i++)
		{
			if (players[i].player is not Node3D body)
				continue;

			// Asegurar jerarquía
			if (body.GetParent() != PlayersRoot)
			{
				body.GetParent()?.RemoveChild(body);
				PlayersRoot.AddChild(body);
			}

			// Posición relativa al PlayersRoot (spawn)
			Vector3 localOffset = new Vector3(i * PlayerSpacing, 0f, 0f);
			body.GlobalPosition = PlayersRoot.GlobalPosition + localOffset;

			// Asegurar que miran al centro de la isla (0,0,0)
			body.LookAt(Vector3.Zero, Vector3.Up);

			// Primer jugador = líder
			if (i == 0)
				_leader = players[i];
		}
	}

	#endregion

	#region Movement
	private void MoveLeader(Player leaderPlayer, float dt)
	{
		if (leaderPlayer.player is not CharacterBody3D body) return;

		Vector3 inputDir = Vector3.Zero;

		if (Input.IsActionPressed("ui_up")) inputDir.Z -= 1;
		if (Input.IsActionPressed("ui_down")) inputDir.Z += 1;
		if (Input.IsActionPressed("ui_left")) inputDir.X -= 1;
		if (Input.IsActionPressed("ui_right")) inputDir.X += 1;

		if (inputDir == Vector3.Zero)
		{
			body.Velocity = Vector3.Zero;
			return;
		}

		inputDir = inputDir.Normalized();
		body.Velocity = inputDir * MoveSpeed;
		body.MoveAndSlide();

		body.LookAt(body.GlobalPosition + inputDir, Vector3.Up);
	}

	private void MoveFollowers(float dt)
	{
		for (int i = 1; i < PlayerData.Instance.PlayerCount; i++)
		{
			if (PlayerData.Instance.Players[i].player is not CharacterBody3D follower)
				continue;

			if (PlayerData.Instance.Players[i - 1].player is not CharacterBody3D target)
				continue;

			Vector3 toTarget = target.GlobalPosition - follower.GlobalPosition;
			float dist = toTarget.Length();

			if (dist < FollowDistance)
			{
				follower.Velocity = Vector3.Zero;
				continue;
			}

			Vector3 dir = toTarget.Normalized();
			follower.Velocity = dir * MoveSpeed;
			follower.MoveAndSlide();

			follower.LookAt(target.GlobalPosition, Vector3.Up);
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

	private void UpdateCameraLook()
	{
		if (MainCamera == null) return;

		Vector3 center = GetPlayersCenter();
		MainCamera.LookAt(center, Vector3.Up);
	}

	private Vector3 GetPlayersCenter()
	{
		Vector3 sum = Vector3.Zero;
		int count = 0;

		foreach (var p in PlayerData.Instance.Players)
		{
			if (p.player == null) continue;
			sum += p.player.GlobalPosition;
			count++;
		}

		return count > 0 ? sum / count : Vector3.Zero;
	}

	#endregion
}