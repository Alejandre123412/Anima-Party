using AnimaParty.assets.script.types;
using AnimaParty.autoload;
using Godot;

namespace AnimaParty.assets.scenes.main;

public partial class SceneTrigger : Area3D
{
    [Export] public NodePath LabelPath;
    [Export] public PackedScene SceneToLoad;
    [Export] private Node3D PlayerRoot; 

    private Label3D _label;
    private bool _playerInside = false;

    public override void _Ready()
    {
        if (!LabelPath.IsEmpty)
            _label = GetNode<Label3D>(LabelPath);

        if (_label != null)
            _label.Visible = false;

        Monitoring = true;

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public void AcceptInput()
    {
        GD.Print("Jugador accepted");
        if (!_playerInside)
            return;
        PutEveryThingInGlobal(PlayerRoot);
        LoadScene();
        
    }

    private void PutEveryThingInGlobal(Node3D root)
    {
        var children = root.GetChildren();
        var i = 0;
        foreach (var child in children)
        {
            if (i>=4)
                break;
            if (child is not PlayeableCharacter3D player)
                return;
            GlobalNodes.Instance.CharacterSlots[i]=player;
            i++;
        }
    }
    
    private void OnBodyEntered(Node body)
    {
        GD.Print("Jugador entered");
        if (body is PlayeableCharacter3D)
        {
            _playerInside = true;

            if (_label != null)
                _label.Visible = true;

            GD.Print("Jugador dentro del Area");
        }
    }

    private void OnBodyExited(Node body)
    {
        GD.Print("Jugador exited");
        if (body is PlayeableCharacter3D)
        {
            _playerInside = false;

            if (_label != null)
                _label.Visible = false;

            GD.Print("Jugador fuera del Area");
        }
    }

    private void LoadScene()
    {
        if (SceneToLoad != null)
        {
            GD.Print("Cambiando escena");
            GetTree().ChangeSceneToPacked(SceneToLoad);
        }
        else
        {
            GD.PrintErr("SceneToLoad no asignada");
        }
    }
}