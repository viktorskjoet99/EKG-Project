namespace DefaultNamespace;

public class AlarmToAmbulance : IAlarm
{
    public void Update()
    {
        Console.WriteLine("HEART ATTACK, drive to this adresse: xxxxx");
    }
}