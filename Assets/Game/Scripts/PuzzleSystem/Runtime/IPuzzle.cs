namespace PuzzleSystem
{
    public interface IPuzzle
    {
        string ID { get; }
        bool isCompleted { get;}

        void Enter();
        void Exit();
        void Refresh();
    }
}