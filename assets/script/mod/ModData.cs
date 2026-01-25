
using System.IO.Compression;
using Godot.Collections;
namespace AnimaParty.assets.script.mod;

public sealed class ModData
{
    public string Id { get; }
    public string Name { get; }
    public string Author { get; }
    public string Version { get; }
    public string Description { get; }
    public Array<string> Dependencies { get; }

    public Dictionary RawInfo { get; }
    public ZipArchive Archive { get; }

    public ModData(
        string id,
        string name,
        string author,
        string version,
        string description,
        Array<string> dependencies,
        Dictionary rawInfo,
        ZipArchive archive)
    {
        Id = id;
        Name = name;
        Author = author;
        Version = version;
        Description = description;
        Dependencies = dependencies;
        RawInfo = rawInfo;
        Archive = archive;
    }
}