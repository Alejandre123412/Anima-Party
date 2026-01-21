using Godot;
using System.Collections.Generic;

namespace AnimaParty.assets.script.data;
public partial class GameData : Node
{
    // Lista de jugadores
    public List<Dictionary<string, PackedScene>> PlayerList = new List<Dictionary<string, PackedScene>>();

    // Método para inicializar jugadores
    public void InitPlayers(int playerCount)
    {
        PlayerList.Clear();

        for (int i = 0; i < playerCount; i++)
        {
            PlayerList.Add(new Dictionary<string, PackedScene>
            {
                { "character", null } // por defecto
            });
        }
    }
}