using System.Collections.Concurrent;


namespace EKG_Project;

public class ECGReadingProducer
{
    private readonly IECGSensor _sensor;
    private readonly ECGDataqueue _dataqueue;
    private readonly int _sampleRate = 500;
    private readonly DateTime _startTime = DateTime.UtcNow;
    private const int MaxSamples = 6297;  

    private Thread? _thread;

    public bool IsFinished => _sensor.IsFinished;

    public ECGReadingProducer(IECGSensor sensor, ECGDataqueue dataqueue)
    {
        _sensor = sensor;
        _dataqueue = dataqueue;
    }

    public void Start()
    {
        _thread = new Thread(Run);
        _thread.Start();
    }

    private void Run()
    {
        int index = 0;

        while (!_sensor.IsFinished && index < MaxSamples)
        {
            double v = _sensor.ReadRawSample();

            var sample = new ECGSample
            {
                Lead1 = v,
                TimeStamp = _startTime.AddSeconds(index / (double)_sampleRate)
            };

            _dataqueue.Add(sample);
            index++;
            Thread.Sleep(1000 / _sampleRate); 
        }

        _dataqueue.CompleteAdding();
    }

    public void Stop()
    {
        // __cts.Cancel();
        _thread?.Join();
    }
}