namespace AnimaParty.assets.script.types;

public class PlayerInfo
{
    public int Id = -1;                  // Device id (-1 = no conectado)
    public int SelectedCharacter = 0;    // Índice del personaje
    public bool IsSelectedCharacter = false; // Bloquea selección
}