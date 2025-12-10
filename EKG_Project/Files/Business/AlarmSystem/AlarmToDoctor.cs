namespace EKG_Project;

public class AlarmToDoctor : IAlarm
{
    public void Update(STStatus status)
    {
        if (status == STStatus.Elevation)
            Console.WriteLine("[DOCTOR ALERT] ST-ELEVATION detected - Immediate intervention required!");
        else if (status == STStatus.Depression)
            Console.WriteLine("[DOCTOR ALERT] ST-Depression detected - Monitor patient closely");
    }
}