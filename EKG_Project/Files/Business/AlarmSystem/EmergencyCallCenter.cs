namespace EKG_Project;

public class EmergencyCallCenter : IAlarm
{
    public void Update(STStatus status)
    {
        if (status == STStatus.Elevation)
        Console.WriteLine(" [EMERGENCY DISPATCH] Ambulance dispatched - Suspected heart attack");
    }
}