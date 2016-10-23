namespace Assets.Scripts
{
    public interface IObserver
    {
        void Notify();
    }

    public interface IPlayerHealtObserver
    {
        void PlayerDeathNotify();
    }

    public interface EnemyDeathObserver : IObserver
    {
        void EnemyDestroyNotify(bool killed);
    }

    public interface IObserveSubject
    {
        void NotifyObservers();
        void RegisterObserver(IObserver observer);
        void UnregisterObserver(IObserver observer);
    }
}
