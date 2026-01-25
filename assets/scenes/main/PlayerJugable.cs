using Godot;
using System;

namespace AnimaParty.assets.scenes.main;
public partial class PlayerJugable : CharacterBody3D
{
	[Export] public bool isLeader = false;
	[Export] public NodePath leaderPath;

	private CharacterBody3D leader;

	[Export] public float speed = 6f;
	[Export] public float followDistance = 1.5f;

	public override void _Ready()
	{
		if (!isLeader && leaderPath != null)
		{
			leader = GetNode<CharacterBody3D>(leaderPath);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (isLeader)
		{
			handleInput((float)delta);
		}
		else
		{
			followLeader((float)delta);
		}
	}

	private void handleInput(float delta)
	{
		Vector2 input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		Vector3 dir = new Vector3(input.X, 0, input.Y);
		if (dir.Length() > 0)
			dir = dir.Normalized();

		Velocity = dir * speed;
		MoveAndSlide();
	}

	private void followLeader(float delta)
	{
		if (leader == null)
			return;

		Vector3 targetPos = leader.GlobalTransform.Origin -
		                    leader.GlobalTransform.Basis.Z * followDistance;

		Vector3 dir = targetPos - GlobalTransform.Origin;

		if (dir.Length() > 0.1f)
		{
			Velocity = dir.Normalized() * speed;
			MoveAndSlide();
		}
		else
		{
			Velocity = Vector3.Zero;
		}
	}
}

