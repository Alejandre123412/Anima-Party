namespace CoreMod.Interfaces
{
    public interface IMinigame
    {
        void Setup();
        void StartGame();
        void EndGame();
    }

    public interface IGameMode
    {
        void Configure();
        void StartMode();
        void EndMode();
    }

    public interface IBaseCharacter
    {
        int PlayerId { get; }
        void InitializeCharacter();
    }
}