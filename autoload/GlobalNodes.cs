using System.Collections.Generic;
using Godot;

namespace AnimaParty.autoload;

public partial class GlobalNodes : Node
{
    public static GlobalNodes Instance;

    // Lista de todos los personajes
    public List<Node3D> Characters { get; } = new();

    public override void _Ready()
    {
        Instance = this;
        // Opcional: este nodo nunca se destruye aunque cambies escenas
        //GetTree().Root.AddChild(this);
    }
}
