using System.Collections.Concurrent;

namespace EKG_Project;

public class ECGReadingConsumer
{
    private readonly BlockingCollection<ECGSample> _dataqueue;
    private readonly ECGProcessor _processor;
    private readonly ECGSample _sensor;

    public ECGReadingConsumer(BlockingCollection<ECGSample> dataqueue, ECGSample sensor, ECGProcessor processor)
    {
        _dataqueue = dataqueue;
        _sensor = sensor;
        _processor = processor;
    }

    public void Run()
    {
        foreach (var sample in _dataqueue.GetConsumingEnumerable())
        {
            _processor.AddSample(sample);
        }
    }
    
}
