namespace AnimaParty.assets.script.data;

[System.Flags]
public enum MinigamePermissions
{
    None        = 0,
    Move        = 1 << 0, // movimiento normal
    Jump        = 1 << 1,
    Dash        = 1 << 2,
    Attack     = 1 << 3,

    CustomMove = 1 << 8  // ⚠ movimiento controlado por el minijuego
}
