using Godot;
using System;

namespace AnimaParty.assets.scenes.main;
public partial class PlayerJugable : CharacterBody3D
{
	[Export] public bool IsLeader = false;
	[Export] public NodePath LeaderPath;

	private CharacterBody3D _leader;

	[Export] public float Speed = 6f;
	[Export] public float FollowDistance = 1.5f;

	public override void _Ready()
	{
		if (!IsLeader && LeaderPath != null)
		{
			_leader = GetNode<CharacterBody3D>(LeaderPath);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsLeader)
		{
			HandleInput((float)delta);
		}
		else
		{
			FollowLeader((float)delta);
		}
	}

	private void HandleInput(float delta)
	{
		Vector2 input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		Vector3 dir = new Vector3(input.X, 0, input.Y);
		if (dir.Length() > 0)
			dir = dir.Normalized();

		Velocity = dir * Speed;
		MoveAndSlide();
	}

	private void FollowLeader(float delta)
	{
		if (_leader == null)
			return;

		Vector3 targetPos = _leader.GlobalTransform.Origin -
		                    _leader.GlobalTransform.Basis.Z * FollowDistance;

		Vector3 dir = targetPos - GlobalTransform.Origin;

		if (dir.Length() > 0.1f)
		{
			Velocity = dir.Normalized() * Speed;
			MoveAndSlide();
		}
		else
		{
			Velocity = Vector3.Zero;
		}
	}
}

