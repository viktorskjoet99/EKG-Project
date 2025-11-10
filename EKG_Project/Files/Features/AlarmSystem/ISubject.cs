namespace EKG_Project;

public interface ISubject
{
    public void Attach(IAlarm observer);
    public void Detach(IAlarm observer);
    public void Notify();
}