using System.IO;
using System.IO.Compression;
using System.Text;
namespace AnimaParty.assets.script.mod;

public sealed class ModFileSystem
{
    private readonly ZipArchive _archive;

    public ModFileSystem(ZipArchive archive)
    {
        _archive = archive;
    }

    public string ReadText(string path)
    {
        var entry = _archive.GetEntry(path);
        if (entry == null) return null;

        using var stream = entry.Open();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    public Stream OpenStream(string path)
    {
        return _archive.GetEntry(path)?.Open();
    }
}
