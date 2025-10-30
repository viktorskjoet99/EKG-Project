namespace DefaultNamespace;

public class AlarmToPatient : IAlarm
{
    public void StartAlarm()
    {
        Console.WriteLine("You are showing signs of heart arrhythmia, please find a sit or a place to lie down");
    }

    public void StopAlarm()
    {
        Console.WriteLine("Alarm is off");
    }
}