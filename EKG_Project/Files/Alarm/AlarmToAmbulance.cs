namespace EKG_Project;

public class AlarmToAmbulance : IAlarm
{
    public void Update()
    {
        Console.WriteLine("HEART ATTACK, drive to this adresse: xxxxx");
    }
}