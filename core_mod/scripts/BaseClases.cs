using Godot;
using CoreMod.Interfaces;

namespace CoreMod.Base
{
    public abstract partial class BaseMinigame : Node, IMinigame
    {
        public virtual void Setup() {}
        public virtual void StartGame() {}
        public virtual void EndGame() {}
    }

    public abstract partial class BaseGameMode : Node, IGameMode
    {
        public virtual void Configure() {}
        public virtual void StartMode() {}
        public virtual void EndMode() {}
    }

    public abstract partial class BaseCharacter : Node3D, IBaseCharacter
    {
        public int PlayerId { get; private set; } = -1;

        public virtual void InitializeCharacter() {}
        public void SetPlayerId(int id) => PlayerId = id;
    }
}