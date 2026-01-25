namespace AnimaParty.assets.script.mod;

public interface IModEntry
{
    void OnLoad(ModContext context);
    void OnUnload();
}