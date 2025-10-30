namespace DefaultNamespace;

public class AlarmToRelative : IAlarm
{
    public void StartAlarm()
    {
        Console.WriteLine("Call the hospital on xxxxxxxx");
    }

    public void StopAlarm()
    {
        Console.WriteLine("Alarm is off");
    }
}