using System.IO;
using System.IO.Compression;
using Godot.Collections;

namespace AnimaParty.assets.script.mod;

public sealed class ModLoader
{
    private readonly CharacterRegistry _characters;
    private readonly MinigameRegistry _minigames;

    public ModLoader(
        CharacterRegistry characters,
        MinigameRegistry minigames)
    {
        _characters = characters;
        _minigames = minigames;
    }

    public void LoadMod(string zipPath)
    {
        var archive = ZipFile.OpenRead(zipPath);
        var fs = new ModFileSystem(archive);

        var jsonText = fs.ReadText("mod.json");
        var info = JsonLoader.Parse(jsonText);

        var id = (string)info["id"];
        var name = (string)info["name"];
        var author = (string)info["author"];
        var version = (string)info["version"];
        var description = (string)info["description"];
        var deps = (Array<string>)info["dependencies"];

        var modData = new ModData(
            id, name, author, version, description, deps, info, archive);

        ModRegistry.Register(modData);

        var dllStream = fs.OpenStream("code/mod.dll");
        using var ms = new MemoryStream();
        dllStream.CopyTo(ms);

        var entry = AssemblyLoader.LoadModEntry(ms.ToArray());

        var context = new ModContext(modData, _characters, _minigames);
        entry.OnLoad(context);
    }
}