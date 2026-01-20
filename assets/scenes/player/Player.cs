using Godot;
using System;

public partial class Player : Node3D
{
	private MeshInstance3D model;

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
	
}
