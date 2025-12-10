using System.Collections.Concurrent;

namespace EKG_Project;

public class ECGDataqueue
{
    private readonly BlockingCollection<ECGSample> _queue;
    
    public ECGDataqueue(int boundedCapacity = 10000)
    {
        _queue = new BlockingCollection<ECGSample>(boundedCapacity);
    }
    
    public void Add(ECGSample sample)
    {
        _queue.Add(sample);
    }
    
    public void CompleteAdding()
    {
        _queue.CompleteAdding();
    }
    
    public IEnumerable<ECGSample> GetConsumingEnumerable()
    {
        return _queue.GetConsumingEnumerable();
    }
}