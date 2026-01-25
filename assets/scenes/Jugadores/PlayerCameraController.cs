using Godot;

namespace AnimaParty.assets.scenes.Jugadores;

/// <summary>
/// Controls a player's camera in a SubViewport.
/// Can follow a target Node3D, adjust height and distance, and interpolate smoothly.
/// </summary>
public partial class PlayerCameraController : Camera3D
{
    [Export] public float HeightOffset = 1.5f; // Vertical offset above the character
    [Export] public float LerpSpeed = 5f;      // Speed of interpolation
    [Export] public float DistanceMultiplier = 1.0f; // Multiplier for distance from character
    
    private Vector3 targetCenter;   // Center position of the character
    private float targetDistance;   // Desired distance from the target
    private bool hasTarget; // Whether a target has been set
    #nullable  enable
    private Node3D? target;        // The character node this camera will follow

    /// <summary>
    /// Assigns a target to follow and calculates camera distance based on character size.
    /// </summary>
    /// <param name="character">Target Node3D</param>
    /// <param name="center">Center position of the character</param>
    /// <param name="size">Bounding size of the character</param>
    public void SetTarget(Node3D character, Vector3 center, Vector3 size)
    {
        target = character;
        targetCenter = center;

        // Compute distance based on bounding size and FOV
        float radius = size.Length() * 0.6f;
        float fovRad = Mathf.DegToRad(Fov);
        targetDistance = (radius / Mathf.Tan(fovRad * 0.5f)) * DistanceMultiplier;

        hasTarget = true;
    }

    /// <summary>
    /// Moves the camera smoothly to follow the target every frame.
    /// </summary>
    /// <param name="delta">Frame delta time</param>
    public override void _Process(double delta)
    {
        if (!hasTarget || target == null)
            return;

        // Desired position relative to target
        Vector3 desiredPos = targetCenter + new Vector3(0, HeightOffset, targetDistance);

        // Interpolate position
        GlobalPosition = GlobalPosition.Lerp(desiredPos, (float)delta * LerpSpeed);

        // Always look at the target center
        LookAt(targetCenter, Vector3.Up);
    }
}