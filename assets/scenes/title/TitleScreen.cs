using Godot;
using AnimaParty.assets.script.data;
using AnimaParty.assets.script.types;
//using AnimaParty.assets.script.utils;

namespace AnimaParty.assets.scenes.title;
public partial class TitleScreen : Node
{
	private bool isPress;
	[Export] private PackedScene nextScene;
	public override void _Ready()
	{
		//var mods=ModLoader.LoadAllMods().GetLoadedMods();
		//GD.Print($"Mods loaded: {mods.Count}");
	}

	public override void _Input(InputEvent @event)
	{
		if (!Player.IsAcceptableDevice(@event)) return;
		bool hasPress = @event.IsActionPressed("ui_l1")||@event.IsActionPressed("ui_r1");
		if (!isPress && hasPress)
		{
			isPress = true;
		}

		if (isPress && hasPress)
		{
			GD.Print("Pressed");
			GD.Print($"Diviced encharge: {@event.Device}");
			PlayerData.AddDevice(@event.Device);
			Cambiar();
		}
	}

	private void Cambiar()
	{
		GetTree().ChangeSceneToFile(nextScene.GetPath());
	}
}
