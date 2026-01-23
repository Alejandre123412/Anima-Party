using AnimaParty.assets.script.data;
using AnimaParty.assets.script.types;
using Godot;

namespace AnimaParty.assets.scenes.table;

using Godot;

public partial class PlayerSlot : VBoxContainer
{
    [Export] public int SlotIndex;

    public PlayerSlotState State { get; private set; }
        = PlayerSlotState.Pending;

    public int SelectedCharacter { get; private set; } = 0;

    public void ForceActivate()
    {
        State = PlayerSlotState.Selecting;
        UpdateUI();
    }

    public void Accept()
    {
        if (State == PlayerSlotState.Pending)
            State = PlayerSlotState.Selecting;
        else if (State == PlayerSlotState.Selecting)
            State = PlayerSlotState.Locked;

        UpdateUI();
    }

    public void Cancel()
    {
        if (State == PlayerSlotState.Locked)
            State = PlayerSlotState.Selecting;
        else if (State == PlayerSlotState.Selecting)
            State = PlayerSlotState.Pending;

        UpdateUI();
    }

    public void ChangeCharacter(int dir, int max)
    {
        if (State != PlayerSlotState.Selecting)
            return;

        SelectedCharacter =
            (SelectedCharacter + dir + max) % max;

        UpdateUI();
    }

    private void UpdateUI()
    {
        // Aquí solo UI:
        // colores, textos, iconos, etc.
        // Ejemplo:
        Modulate = State switch
        {
            PlayerSlotState.Pending => Colors.Gray,
            PlayerSlotState.Selecting => Colors.White,
            PlayerSlotState.Locked => Colors.Green,
            _ => Colors.White
        };
    }
}