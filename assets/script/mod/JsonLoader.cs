using Godot;
using Godot.Collections;

namespace AnimaParty.assets.script.mod;

public static class JsonLoader
{
    public static Dictionary Parse(string jsonText)
    {
        var json = new Json();
        var err = json.Parse(jsonText);

        if (err != Error.Ok)
            return null;

        if (json.Data.VariantType != Variant.Type.Dictionary)
            return null;

        return (Dictionary)json.Data.Obj;
    }
}