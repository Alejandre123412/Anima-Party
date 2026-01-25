using Godot;

namespace AnimaParty.assets.script.mod.boostrap;

public static class GameBootstrap
{
    public static CharacterRegistry Characters { get; private set; }
    public static MinigameRegistry Minigames { get; private set; }

    public static void Initialize()
    {
        Characters = new CharacterRegistry();
        Minigames = new MinigameRegistry();

        LoadCoreMod();
        LoadExternalMods();
    }

    private static void LoadCoreMod()
    {
        var coreModData = new ModData(
            id: "core",
            name: "Core Game",
            author: "AnimaParty",
            version: "1.0.0",
            description: "Base game content",
            dependencies: new Godot.Collections.Array<string>(),
            rawInfo: new Godot.Collections.Dictionary(),
            archive: null
        );

        ModRegistry.Register(coreModData);

        var coreContext = new ModContext(
            coreModData,
            Characters,
            Minigames
        );

        var coreMod = new CoreMod();
        coreMod.OnLoad(coreContext);
    }

    private static void LoadExternalMods()
    {
        var loader = new ModLoader(Characters, Minigames);

        // ejemplo
        foreach (var path in DirAccess.GetFilesAt("user://mods"))
        {
            if (path.EndsWith(".zip"))
                loader.LoadMod($"user://mods/{path}");
        }
    }
}
