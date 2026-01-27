using Godot;
using AnimaParty.assets.script.types;
using AnimaParty.autoload;

namespace AnimaParty.assets.scenes.main;

public partial class GameManager : Node
{
    #region Exports
    [Export] public Node3D PlayersRoot;
    [Export] public Camera3D MainCamera;

    [Export] public float PlayerSpacing = 2f;
    [Export] public float MoveSpeed = 5f;

    [Export] public float CameraHeight = 10f;     // Altura fija
    [Export] public float CameraDistance = 10f;   // Distancia del jugador
    [Export] public float CameraRotateSpeed = 2f; // Velocidad de giro de cámara
    #endregion

    private Player _leader;

    public override void _Ready()
    {
        SetupPlayerLine();
        SetupCamera();
    }

    public override void _Process(double delta)
    {
        UpdateCamera((float)delta);
    }

    #region Setup Players
    private void SetupPlayerLine()
    {
        var players = PlayerData.Instance.Players;
        if (PlayersRoot == null)
        {
            GD.PrintErr("PlayersRoot no asignado.");
            return;
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].player is not Node3D body) 
                continue;

            if (body.GetParent() != PlayersRoot)
            {
                body.GetParent()?.RemoveChild(body);

                // Creamos CharacterBody3D
                var player = new script.types.PlayeableCharacter3D();

                // Collider
                var collision = new CollisionShape3D
                {
                    Shape = new CapsuleShape3D { Radius = 0.5f, Height = 1.8f },
                    Position = new Vector3(0, 0.9f, 0)
                };
                player.AddChild(collision);

                // Modelo
                player.AddChild(body);
                player.Camera=MainCamera;
                PlayersRoot.AddChild(player);
            }

            // Posición inicial en línea
            Vector3 offset = new Vector3(i * PlayerSpacing, 0, 0);
            body.GlobalPosition = PlayersRoot.GlobalPosition + offset;
            body.LookAt(Vector3.Zero, Vector3.Up);

            if (i == 0)
                _leader = players[i];
        }
    }
    #endregion

    #region Camera
    private void SetupCamera()
    {
        if (MainCamera == null) return;

        // Fija la cámara en el centro de la plaza
        MainCamera.GlobalPosition = new Vector3(0, CameraHeight, 0);

        // Mirando inicialmente al líder
        if (_leader != null)
        {
            Vector3 leaderPos = (_leader.player as Node3D).GlobalPosition;
            MainCamera.LookAt(leaderPos, Vector3.Up);
        }
    }

    private void UpdateCamera(float delta)
    {
        if (MainCamera == null || _leader == null) return;

        Vector3 leaderPos = (_leader.player as Node3D).GlobalPosition;

        // Solo giramos la cámara alrededor de Y para mirar al líder
        Vector3 dirToLeader = (leaderPos - MainCamera.GlobalPosition).Normalized();
        float targetYaw = Mathf.Atan2(-dirToLeader.X, -dirToLeader.Z);

        // Obtenemos la rotación actual
        Vector3 currentRot = MainCamera.Rotation;
        currentRot.Y = Mathf.LerpAngle(currentRot.Y, targetYaw, delta * CameraRotateSpeed);
        MainCamera.Rotation = currentRot;
    }
    #endregion
}
