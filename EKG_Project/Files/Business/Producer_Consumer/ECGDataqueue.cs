

namespace EKG_Project;

public class ECGDataqueue
{
    private readonly ECGSensor _sensor;
    private DataChunks _dataChunks;
    
    public bool Running { get; private set; }

    public ECGDataqueue(ECGSensor sensor, DataChunks dataChunks)
    {
        _sensor = sensor;
        _dataChunks = dataChunks;
    }

    public void Run()
    {
        Running = true;
        while (Running)
        {
            var sample = _sensor.ReadRawSample();
            _dataChunks.AddChunk(new ECGSample
            {
                Lead1 = sample,
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