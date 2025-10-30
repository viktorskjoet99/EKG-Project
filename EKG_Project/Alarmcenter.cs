namespace DefaultNamespace;

public class Alarmcenter
{
    private IAlarm _alarm;

    public Alarmcenter(IAlarm alarm)
    {
        _alarm = alarm;
    }

    public IAlarm Alarm
    {
        private get => _alarm; 
        set => _alarm = value;
    }
}