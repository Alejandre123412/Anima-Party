using System.Collections.Generic;
using AnimaParty.assets.script.types;
using Godot;

namespace AnimaParty.autoload;

public partial class GlobalNodes : Node
{
    public static GlobalNodes Instance;

    // Lista de todos los personajes
    public List<Node3D> Characters { get; } = new();
    public PlayeableCharacter3D?[] CharacterSlots = new PlayeableCharacter3D?[4];

    public override void _Ready()
    {
        Instance = this;
        // Opcional: este nodo nunca se destruye aunque cambies escenas
        //GetTree().Root.AddChild(this);
    }
}
