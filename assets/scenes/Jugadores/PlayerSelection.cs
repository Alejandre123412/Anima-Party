using Godot;
using System.Collections.Generic;
using AnimaParty.assets.scenes.table;
using AnimaParty.assets.script.data;
using AnimaParty.assets.script.types;

namespace AnimaParty.assets.scenes.Jugadores;
public partial class PlayerSelection : Control
{
    private List<PlayerSlot> _slots = new();
    private int _activePlayers = 1; // Player 1 siempre activo

    public override void _Ready()
    {
        var container = GetNode("Main").GetNode<HBoxContainer>("PlayerContainer");

        foreach (var child in container.GetChildren())
            _slots.Add(child as PlayerSlot);

        // Activar Player 1 automáticamente
        _slots[0].ForceActivate();
    }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsPressed())
            return;

        var slot = _slots[_activePlayers - 1];

        if (@event.IsActionPressed("ui_left"))
            slot.ChangeCharacter(-1, 6);

        else if (@event.IsActionPressed("ui_right"))
            slot.ChangeCharacter(1, 6);

        else if (@event.IsActionPressed("ui_accept"))
        {
            slot.Accept();

            // Si pasa a Selecting por primera vez, activar siguiente slot
            if (slot.State == PlayerSlotState.Selecting &&
                _activePlayers < _slots.Count)
            {
                _activePlayers++;
                _slots[_activePlayers - 1].ForceActivate();
            }

            if (AllLocked())
                GoNext();
        }
        else if (@event.IsActionPressed("ui_cancel"))
        {
            slot.Cancel();
        }
    }

    private bool AllLocked()
    {
        for (int i = 0; i < _activePlayers; i++)
        {
            if (_slots[i].State != PlayerSlotState.Locked)
                return false;
        }
        return true;
    }

    private void GoNext()
    {
        // Guardar datos finales
        PlayerData.PlayerCount = _activePlayers;
        PlayerData.SelectedCharacters.Clear();

        for (int i = 0; i < _activePlayers; i++)
            PlayerData.SelectedCharacters.Add(
                _slots[i].SelectedCharacter);

        GetTree().ChangeSceneToFile(
            "res://assets/scenes/MenuPrincipal.tscn");
    }
}
