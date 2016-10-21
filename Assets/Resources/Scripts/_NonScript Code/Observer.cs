namespace Assets.Scripts
{
    public interface IObserver
    {
        void Notify();
    }

    public interface IObserveSubject
    {
        void NotifyObservers();
        void RegisterObserver(IObserver observer);
        void UnregisterObserver(IObserver observer);
    }
}
