namespace EKG_Project;

public class AlarmToEmergencyContact : IAlarm
{
    public void Update(STStatus status)
    {
        if (status == STStatus.Elevation)
        {
            Console.WriteLine("[EMERGENCY CONTACT] Critical heart event - Hospital has been notified");
        }
    }
}