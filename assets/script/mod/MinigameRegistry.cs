using System.Collections.Generic;

namespace AnimaParty.assets.script.mod;

public sealed class MinigameRegistry
{
    private readonly Dictionary<string, string> _minigames = new();

    public void Register(string id, string scenePath)
    {
        _minigames[id] = scenePath;
    }

    public string GetScene(string id)
    {
        return _minigames[id];
    }
}