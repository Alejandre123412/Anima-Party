using AnimaParty.assets.scenes.main;
using Godot;
using AnimaParty.assets.script.data;
using AnimaParty.autoload;

namespace AnimaParty.assets.script.types;

public partial class PlayeableCharacter3D : CharacterBody3D
{
    [Export] public float Speed = 5f;
    [Export] public float JumpVelocity = 4.5f;
    [Export] public float GravityMultiplier = 9.8f;

    private MinigamePermissions _minigamePermissions = MinigamePermissions.None;
    private Vector3 _customVelocity = Vector3.Zero;

    // Dirección que le pasa GameManager (opcional)
    private Vector3 _moveIntent = Vector3.Zero;
    private float _moveSpeed = 5f;

    // Referencia a la cámara
    [Export] public Camera3D Camera;
    [Export] private SceneTrigger? sceneTrigger;

    public override void _Ready()
    {
        sceneTrigger=GetNode("..").GetNode("..").GetNodeOrNull<SceneTrigger>("Area3D");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;
        
        if (!IsOnFloor())
            velocity += GetGravity() * (float)delta * GravityMultiplier;
        //
        // Salto
        if (Input.IsActionJustPressed("ui_accept"))
            sceneTrigger?.AcceptInput();
        
        // Movimientos independientes de GameManager
        Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Vector3 moveDir = Vector3.Zero;
        
        if (inputDir != Vector2.Zero && Camera != null)
        {
            // Transformar el input según la orientación de la cámara
            Vector3 camForward = Camera.GlobalTransform.Basis.Z;
            Vector3 camRight   = Camera.GlobalTransform.Basis.X;
        
            // Invertimos forward para que "arriba" sea hacia adelante
            camForward.Y = 0;
            camForward = camForward.Normalized();
        
            camRight.Y = 0;
            camRight = camRight.Normalized();
        
            // Combinamos los inputs
            moveDir = (camForward * inputDir.Y + camRight * inputDir.X);
        
            // Normalizamos solo si no es cero
            if (moveDir.Length() > 0)
                moveDir = moveDir.Normalized();
        }
        
        if (moveDir != Vector3.Zero)
        {
            velocity.X = moveDir.X * Speed;
            velocity.Z = moveDir.Z * Speed;
        
            // Rotación suave hacia la dirección de movimiento
            float targetYaw = Mathf.Atan2(moveDir.X, moveDir.Z);
            Vector3 rot = Rotation;
            rot.Y = Mathf.LerpAngle(rot.Y, targetYaw, 10f * (float)delta);
            Rotation = rot;
        }
        else
        {
            // Deceleración
            velocity.X = Mathf.MoveToward(velocity.X, 0, Speed * (float)delta * 5);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0, Speed * (float)delta * 5);
        }
        
        Velocity = velocity;
        MoveAndSlide();
    }
    
    // ---------------- API opcional para GameManager ----------------
    public void SetMoveIntent(Vector3 intent) => _moveIntent = intent;
    public void SetMoveSpeed(float speed) => _moveSpeed = speed;
    public void SetMinigamePermissions(MinigamePermissions permissions) => _minigamePermissions = permissions;
    public void SetCustomMovement(Vector3 velocity) => _customVelocity = velocity;
    public void ClearCustomMovement() => _customVelocity = Vector3.Zero;
    public void ResetMinigamePermissions()
    {
        _minigamePermissions = MinigamePermissions.None;
        _customVelocity = Vector3.Zero;
    }

    public bool CanMove()
    {
        if (GameState.CurrentPhase == GamePhase.ModeSelect) return false;
        if (GameState.CurrentPhase == GamePhase.MenuPrincipal) return true;
        if (GameState.CurrentPhase == GamePhase.Minigame)
            return _minigamePermissions.HasFlag(MinigamePermissions.Move) ||
                   _minigamePermissions.HasFlag(MinigamePermissions.CustomMove);
        return false;
    }
}
