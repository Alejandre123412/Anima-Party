using Godot;

namespace AnimaParty.assets.scenes.main;

public partial class SceneTrigger : Area3D
{
    [Export] public PackedScene TargetScenePath; // Escena a la que se va
    [Export] public string InteractAction = "ui_accept"; // Botón para interactuar (A)

    private bool playerInZone = false;
    private Node3D player;

    public override void _Ready()
    {
        this.BodyEntered += OnBodyEntered;
        this.BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is PlayerJugable)
        {
            playerInZone = true;
            player = body;
            GD.Print("Jugador en zona: pulsa A para entrar");
        }
    }

    private void OnBodyExited(Node3D body)
    {
        if (body == player)
        {
            playerInZone = false;
            player = null;
        }
    }

    public override void _Process(double delta)
    {
        if (playerInZone && Input.IsActionJustPressed(InteractAction))
        {
            GD.Print($"Cambiando a escena {TargetScenePath}");
            GetTree().ChangeSceneToPacked(TargetScenePath);
        }
    }
}