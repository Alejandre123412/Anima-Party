using System.Collections.Generic;
using Godot;

namespace CoreMod.Registry
{
    public static class CoreModRegistry
    {
        public static readonly Dictionary<string, PackedScene> Minigames = new();
        public static readonly Dictionary<string, PackedScene> GameModes = new();
        public static readonly Dictionary<string, PackedScene> Characters = new();

        /// <summary>
        /// Registers a minigame in the core registry.
        /// </summary>
        public static void RegisterMinigame(string id, PackedScene scene)
        {
            if (!Minigames.ContainsKey(id))
                Minigames.Add(id, scene);
        }

        /// <summary>
        /// Registers a game mode in the core registry.
        /// </summary>
        public static void RegisterGameMode(string id, PackedScene scene)
        {
            if (!GameModes.ContainsKey(id))
                GameModes.Add(id, scene);
        }

        /// <summary>
        /// Registers a character in the core registry.
        /// </summary>
        public static void RegisterCharacter(string id, PackedScene scene)
        {
            if (!Characters.ContainsKey(id))
                Characters.Add(id, scene);
        }

        /// <summary>
        /// Retrieve a minigame by ID.
        /// </summary>
        public static PackedScene GetMinigame(string id) => Minigames.TryGetValue(id, out var scene) ? scene : null;
    }
}