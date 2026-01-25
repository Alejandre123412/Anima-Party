namespace AnimaParty.assets.script.mod;

public sealed class ModContext
{
    public ModData Mod { get; }

    public CharacterRegistry Characters { get; }
    public MinigameRegistry Minigames { get; }

    public ModContext(
        ModData mod,
        CharacterRegistry characters,
        MinigameRegistry minigames)
    {
        Mod = mod;
        Characters = characters;
        Minigames = minigames;
    }
}
