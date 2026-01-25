using System;
using System.IO;
using System.Reflection;

namespace AnimaParty.assets.script.mod;

public static class AssemblyLoader
{
    public static IModEntry LoadModEntry(byte[] dllBytes)
    {
        var asm = Assembly.Load(dllBytes);

        foreach (var type in asm.GetTypes())
        {
            if (typeof(IModEntry).IsAssignableFrom(type) && !type.IsInterface)
            {
                return (IModEntry)Activator.CreateInstance(type);
            }
        }

        return null;
    }
}