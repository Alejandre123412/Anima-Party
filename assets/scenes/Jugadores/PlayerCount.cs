using Godot;
using System;
using System.Collections.Generic;
using AnimaParty.assets.scenes.title;
using AnimaParty.assets.script.data;

namespace AnimaParty.assets.scenes.Jugadores;
public partial class PlayerCount : Node
{
	private HBoxContainer _players;
	private PackedScene _playerScene;
	private const string PlayerScenePath = "res://assets/scenes/Jugadores/Player.tscn";
	private static int _count = 0;
	private List<VBoxContainer> _containers=new List<VBoxContainer>();
	private const int MaxPlayers = 4;
	private Label _countLabel;
	private Label _left;
	private Label _right;
	private bool _isInserting=false;
	private static Device _playerDevice;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_playerScene = GD.Load<PackedScene>(PlayerScenePath);
		if (_playerScene == null)
		{
			GD.PrintErr($"Error: No se pudo cargar la escena en {PlayerScenePath}");
			return;
		}

		var parent = GetParent().GetNode("View");
		_countLabel = parent.GetNode("Selection").GetNode<Label>("PlayerCounter");
		_left = parent.GetNode("Selection").GetNode<Label>("<");
		_right = parent.GetNode("Selection").GetNode<Label>(">");
		_players = parent.GetNode<HBoxContainer>("Players");
		_playerDevice = TitleScreen.GetPlayerDevice();
		
		InstantiatePlayer();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (!@event.IsPressed())
			return;

		if (_playerDevice.CompareTo(new Device(@event.Device)) != 0)
			return;

		if (_isInserting)
			return;

		if (@event.IsActionPressed("ui_left"))
		{
			DeinstantiatePlayer();
		}
		else if (@event.IsActionPressed("ui_right"))
		{
			InstantiatePlayer();
		}
		else if (@event.IsActionPressed("ui_accept"))
		{
			GetTree().ChangeSceneToFile("res://assets/scenes/Jugadores/PlayerSelection.tscn");
		}
	}


	private void InstantiatePlayer()
	{
		_count++;
		if(_count>MaxPlayers)
		{
			GD.PrintErr("Error: Maximo de la escena en {0}",_count);
			_count--;
			return;
		}
		_isInserting = true;
		VBoxContainer player=_playerScene.Instantiate() as VBoxContainer;
		if (player == null)
		{
			GD.PrintErr("Error al instanciar el enemigo.");
			return;
		}
		// 3. Configurar propiedades (posición, etc.)
		player.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter | Control.SizeFlags.Expand;
		player.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
		player.SetStretchRatio(1);

		player.GetNode<Label>("Player").Text = $"Player {_count}";

		// 4. Añadirlo al árbol de escenas (hijo de este GameManager)
		_players.AddChild(player);
		_containers.Add(player);
		//GD.Print($"Enemigo instanciado en");
		
		_countLabel.Text = $"{_count}";
		_isInserting = false;
	}
	
	private void DeinstantiatePlayer()
	{
		_count--;
		if (_count <= 0)
		{
			GD.PrintErr("Error: Maximo de la escena en {0}",_count);
			_count++;
			return;
		}

		_isInserting = true;

		
		var player = _containers[_count];

		_players.RemoveChild(player);
		_containers.RemoveAt(_count);
		player.QueueFree();

		_countLabel.Text = _count.ToString();

		_isInserting = false;
		
	}

	public static int GetPlayerCount()
	{
		return _count;
	}

	public static Device GetPlayerDevice()
	{
		return _playerDevice;
	}
}
