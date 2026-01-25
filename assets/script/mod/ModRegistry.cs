using System.Collections.Generic;
namespace AnimaParty.assets.script.mod;

public static class ModRegistry
{
    private static readonly Dictionary<string, ModData> _mods = new();

    public static IReadOnlyDictionary<string, ModData> Mods => _mods;

    public static void Register(ModData mod)
    {
        _mods.Add(mod.Id, mod);
    }

    public static ModData Get(string id)
    {
        return _mods[id];
    }

    public static bool Exists(string id)
    {
        return _mods.ContainsKey(id);
    }
}