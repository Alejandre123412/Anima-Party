using Godot;
using AnimaParty.assets.script.types;
using AnimaParty.autoload;

namespace AnimaParty.assets.scenes.title;
public partial class TitleScreen : Node
{
	private bool _isPress;
	[Export] private PackedScene _nextScene;
	public override void _Ready()
	{
		//var mods=ModLoader.LoadAllMods().GetLoadedMods();
		//GD.Print($"Mods loaded: {mods.Count}");
	}

	public override void _Input(InputEvent @event)
	{
		if (!Player.IsAcceptableDevice(@event)) return;
		bool hasPress = @event.IsActionPressed("ui_l1")||@event.IsActionPressed("ui_r1");
		if (!_isPress && hasPress)
		{
			_isPress = true;
		}

		if (_isPress && hasPress)
		{
			if(_nextScene == null) return;
			GD.Print("Pressed");
			GD.Print($"Diviced encharge: {@event.Device}");
			PlayerData.Instance.AddDevice(@event.Device);
			Cambiar();
		}
	}

	private void Cambiar()
	{
		if (_nextScene == null)
		{
			GD.PrintErr("Error no existe la siguente escena");
			return;
		}
		GD.Print($"NextScene path: {_nextScene?.ResourcePath}");
		GetTree().ChangeSceneToPacked(_nextScene);
	}
}
