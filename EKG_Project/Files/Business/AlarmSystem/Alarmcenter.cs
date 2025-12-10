namespace EKG_Project;

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

    public void Notify(STStatus status)
    {
        foreach (var observer in _observers)
        {
            observer.Update(status);
        }
    }
}