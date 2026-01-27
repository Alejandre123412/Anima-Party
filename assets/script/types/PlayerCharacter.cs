using AnimaParty.autoload;
using Godot;

namespace AnimaParty.assets.script.types;

public partial class PlayerCharacter : CharacterBody3D
{
    public Player PlayerData;
    public bool IsLeader;

    public override void _PhysicsProcess(double delta)
    {
        switch (GameState.CurrentPhase)
        {
            case GamePhase.MenuPrincipal:
                //ProcessMenuPrincipal((float)delta);
                break;

            case GamePhase.ModeSelect:
                //ProcessModeSelect((float)delta);
                break;

            case GamePhase.Minigame:
                //ProcessMinigame((float)delta);
                break;
        }
    }
}
