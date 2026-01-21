using AnimaParty.assets.script.data;
using Godot;

namespace AnimaParty.assets.script.ui;
public partial class PlayerCountMenu : Control
{
    // Nodo GameData
    private GameData gameData;

    public override void _Ready()
    {
        // Suponiendo que GameData es un nodo hijo de este menú
        gameData = GetNode<GameData>("GameData");
    }

    public void StartGame(int playerCount)
    {
        // Inicializar jugadores
        gameData.InitPlayers(playerCount);

        // Cambiar a escena de selección de jugador
        GetTree().ChangeSceneToFile("res://assets/scenes/table/PlayerSelect3D.tscn");
    }
}