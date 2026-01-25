using System.Collections.Generic;

namespace AnimaParty.assets.script.mod;

public sealed class CharacterRegistry
{
    private readonly Dictionary<string, string> _characters = new();

    public void Register(string id, string scenePath)
    {
        _characters[id] = scenePath;
    }

    public string GetScene(string id)
    {
        return _characters[id];
    }
}
