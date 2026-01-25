using Godot;
using System.Collections.Generic;
using AnimaParty.assets.script.data;
using AnimaParty.assets.script.types;

namespace AnimaParty.assets.scenes.main;

public partial class GameManager : Node3D
{
    [Export] public PackedScene PlayerScene; // La escena base del jugador
    [Export] public Node3D PlayersRoot;      // Nodo donde se añadirán los jugadores
    [Export] public Node3D CharactersRoot;   // Nodo con los modelos cargados (CharacterViewer)

    public override void _Ready()
    {
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        if (PlayerData.PlayerCount == 0)
        {
            GD.PrintErr("No hay jugadores registrados en PlayerData.");
            return;
        }

        PlayerJugable leader = null;

        if (PlayerData.HasNullPlayerList())
            return;

        for (int i = 0; i < PlayerData.PlayerCount; i++)
        {
            var pdata = PlayerData.Players[i];

            // Instanciamos el jugador
            var playerInstance = PlayerScene.Instantiate<PlayerJugable>();
            PlayersRoot.AddChild(playerInstance);

            playerInstance.Name = $"Player_{i + 1}";
            playerInstance.GlobalPosition = new Vector3(i * 2.0f, 0, 0); // separarlos un poco

            // Guardamos la referencia de la instancia en Player.player
            pdata.player = playerInstance;

            // Aplicamos el modelo seleccionado
            ApplySelectedCharacter(pdata);

            // Configuramos líder y seguidores
            if (i == 0)
            {
                playerInstance.isLeader = true;
                leader = playerInstance;
            }
            else
            {
                playerInstance.isLeader = false;
                playerInstance.leaderPath = playerInstance.GetPathTo(leader);
            }
        }
    }

    private void ApplySelectedCharacter(Player pdata)
    {
        if (pdata.player == null)
        {
            GD.PrintErr($"Player {pdata} no tiene instancia asignada.");
            return;
        }

        // Obtenemos el nodo del personaje seleccionado
        Node3D selectedCharacter = pdata.player;
        if (selectedCharacter == null)
        {
            GD.PrintErr($"Player {pdata} no tiene personaje seleccionado.");
            return;
        }

        // Limpiar hijos anteriores del player
        foreach (Node child in pdata.player.GetChildren())
            child.QueueFree();

        // Hacer que el modelo seleccionado sea hijo del player
        pdata.player.AddChild(selectedCharacter);

        // Ajustar transform para que quede en el lugar correcto
        selectedCharacter.Transform = Transform3D.Identity;
    }


}
