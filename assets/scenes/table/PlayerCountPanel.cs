using Godot;
using System;

namespace AnimaParty.assets.scenes.table;

public partial class PlayerCountPanel : Node3D
{
	private AnimationPlayer _btnL;
	private AnimationPlayer _btnR;
	private static PlayerCountPanel _panel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_btnL=GetNode<Node3D>("ButtonL").GetNode<AnimationPlayer>("AnimationPlayer");
		_btnR=GetNode<Node3D>("ButtonR").GetNode<AnimationPlayer>("AnimationPlayer");
		_panel=this;
	}
	
	public AnimationPlayer GetBtnR()
	{
		return _btnR;
	}

	public AnimationPlayer GetBtnL()
	{
		return _btnL;
	}
	
	public static Label3D GetLabel()
	{
		return _panel.GetNode<Label3D>("Monitor/PlayerCount");
	}
}
