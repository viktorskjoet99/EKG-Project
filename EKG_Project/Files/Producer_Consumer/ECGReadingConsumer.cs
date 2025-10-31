using System.Collections.Concurrent;

namespace EKG_Project;

public class ECGReadingConsumer
{
    private readonly BlockingCollection<ECGSample> _dataqueue;
    private readonly ECGProcessor _processor;

    public ECGReadingConsumer(BlockingCollection<ECGSample> dataqueue, ECGProcessor processor)
    {
        _dataqueue = dataqueue;
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
