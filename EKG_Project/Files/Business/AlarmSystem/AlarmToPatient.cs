namespace EKG_Project;

public class AlarmToPatient : IAlarm
{
    public void Update()
    {
        Console.WriteLine("You are showing signs of heart arrhythmia, please find a sit or a place to lie down");
    }
}