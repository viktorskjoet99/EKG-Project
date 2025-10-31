

namespace EKG_Project;

public class ECGDataqueue
{
    private readonly ECGSensor _sensor;
    private ECGProcessor _processor;
    
    public bool Running { get; private set; }

    public ECGDataqueue(ECGSensor sensor, ECGProcessor processor)
    {
        _sensor = sensor;
        _processor = processor;
    }

    public void Run()
    {
        Running = true;
        while (Running)
        {
            var sample = _sensor.ReadRawSample();
            _processor.AddSample(new ECGSample
            {
                Value = sample,
                TimeStamp = DateTime.UtcNow,
            });
            Thread.Sleep(1);
        }
    }

    public void Stop()
    {
        Running = false;
    }
}