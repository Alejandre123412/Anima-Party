namespace AnimaParty.assets.script.mod;

public sealed class CoreMod : IModEntry
{
    public void OnLoad(ModContext context)
    {
        context.Characters.Register("core:player", "res://core/player.tscn");
    }

    public void OnUnload() { }
}
