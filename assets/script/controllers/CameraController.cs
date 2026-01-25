using Godot;

namespace AnimaParty.assets.script.controllers;

public partial class CameraController : Node3D
{
    [Export] public Vector3 offset = new Vector3(0, 1.5f, -3);
    [Export] public float lerpSpeed = 5f;

    private Camera3D camera;
    private Node3D target;

    public override void _Ready()
    {
        camera = GetNode<Camera3D>(".");
    }

    public void setTarget(Node3D target)
    {
        this.target = target;
    }

    public override void _Process(double delta)
    {
        if (target == null) return;

        var desiredPos = target.GlobalPosition + offset;
        //_camera.GlobalPosition = Vector3.Lerp(desiredPos, (float)(LerpSpeed * delta));
        camera.LookAt(target.GlobalPosition, Vector3.Up);
    }
}