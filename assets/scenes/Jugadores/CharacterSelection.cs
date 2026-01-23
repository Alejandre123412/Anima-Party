using Godot;
using System;
using System.Collections.Generic;

namespace AnimaParty.assets.scenes.Jugadores;
public partial class CharacterSelection : Node3D
{
	private List<MeshInstance3D> _characters = new();
	private int _index;

	[Export] private float spacing = 2.5f;

	public override void _Ready()
	{
		var root = GetNode<Node3D>("CharactersRoot");

		int i = 0;
		foreach (var child in root.GetChildren())
		{
			if (child is MeshInstance3D mesh)
			{
				mesh.Position = new Vector3(i * spacing, 0, 0);
				_characters.Add(mesh);
				i++;
			}
		}

		CenterCharacters();
		UpdateVisuals();
	}

	private void CenterCharacters()
	{
		float offset = (_characters.Count - 1) * spacing * 0.5f;
		foreach (var c in _characters)
			c.Position -= new Vector3(offset, 0, 0);
	}

	public void Move(int dir)
	{
		_index = (_index + dir + _characters.Count) % _characters.Count;
		UpdateVisuals();
	}

	public int SelectedIndex => _index;

	private void UpdateVisuals()
	{
		for (int i = 0; i < _characters.Count; i++)
		{
			_characters[i].Scale =
				i == _index ? Vector3.One * 1.25f : Vector3.One;

			_characters[i].Modulate =
				i == _index ? Colors.White : Colors.Gray;
		}
	}
}

