using Godot;
using System;
using AnimaParty.assets.script.data;
using Microsoft.VisualBasic.CompilerServices;

namespace AnimaParty.assets.scenes.player;

public partial class Player : Node3D
{
	private MeshInstance3D model;
	private int _device;

	public Player()
	{
		_device=-1;
	}

	public override void _Ready()
	{
		// Buscamos el MeshInstance3D hijo
		model = GetNode<MeshInstance3D>("Model");
	}

	/// <summary>
	/// Cambia el mesh del jugador cargándolo desde una ruta
	/// </summary>
	public void SetModelFromPath(string path)
	{
		var loadedMesh = GD.Load<Mesh>(path);

		if (loadedMesh == null)
		{
			GD.PushError($"No se pudo cargar el mesh: {path}");
			return;
		}

		// Duplicamos para evitar modificar instancias compartidas
		model.Mesh = loadedMesh.Duplicate() as Mesh;
	}
	
	public void SetDefaultModel()
	{
		string path = "res://assets/model/default.obj";
		var loadedMesh = GD.Load<Mesh>(path);

		if (loadedMesh == null)
		{
			GD.PushError($"No se pudo cargar el mesh: {path}");
			return;
		}

		// Duplicamos para evitar modificar instancias compartidas
		model.Mesh = loadedMesh.Duplicate() as Mesh;
	}

	public int GetDevice()
	{
		return _device;
	}
	
	public bool SetDevice(int device)
	{
		if(_device<=0)
		{
			_device = device;
			return true;
		}
		return false;
	}
}
