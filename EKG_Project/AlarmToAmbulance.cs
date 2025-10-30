namespace DefaultNamespace;

public class AlarmToAmbulance : IAlarm
{
    public void StartAlarm()
    {
        Console.WriteLine("HEART ATTACK, drive to this adresse: xxxxx");
    }

    public void StopAlarm()
    {
        Console.WriteLine("Alarm is off");
    }
}