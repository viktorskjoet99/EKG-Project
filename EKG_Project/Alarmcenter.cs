namespace DefaultNamespace;

public class Alarmcenter : ISubject
{
    private List<IAlarm> _observers = new();

    public void Attach(IAlarm observer)
    {
        _observers.Add(observer);
    }

    public void Detach(IAlarm observer)
    {
        _observers.Remove(observer);
    }

    public void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.Update();
        }
    }
}