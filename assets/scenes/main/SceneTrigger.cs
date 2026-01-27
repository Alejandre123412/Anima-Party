using Godot;
using AnimaParty.assets.script.types;

public partial class SceneTrigger : Area3D
{
    [Export] public NodePath LabelPath;
    [Export] public PackedScene SceneToLoad;

    private Label3D _label;
    private PlayeableCharacter3D _player;

    public override void _Ready()
    {
        if (!LabelPath.IsEmpty)
            _label = GetNode<Label3D>(LabelPath);

        if (_label != null)
            _label.Visible = false;

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is PlayeableCharacter3D player)
        {
            _player = player;

            if (_label != null)
                _label.Visible = true;

            // Conectamos la señal correctamente
            _player.Connect("ActionPressedEventHandler", new Callable(this, nameof(OnPlayerActionPressed)));
        }
    }

    private void OnBodyExited(Node body)
    {
        if (body is PlayeableCharacter3D player)
        {
            if (_label != null)
                _label.Visible = false;

            // Desconectamos la señal
            player.Disconnect("ActionPressedEventHandler", new Callable(this, nameof(OnPlayerActionPressed)));
            _player = null;
        }
    }

    private void OnPlayerActionPressed()
    {
        if (SceneToLoad != null)
            GetTree().ChangeSceneToPacked(SceneToLoad);
    }
}