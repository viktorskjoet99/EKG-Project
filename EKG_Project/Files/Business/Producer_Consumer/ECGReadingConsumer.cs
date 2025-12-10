using System.Collections.Concurrent;

namespace EKG_Project;

public class ECGReadingConsumer
{
    private readonly BlockingCollection<ECGSample> _dataqueue;
    private readonly DataChunks _dataChunks;
    private Thread _thread;

    public ECGReadingConsumer(BlockingCollection<ECGSample> dataqueue, DataChunks dataChunks)
    {
        _dataqueue = dataqueue;
        _dataChunks = dataChunks;
    }

    public void Start()
    {
        _thread = new Thread(Run);
        _thread.Start();
    }

    
    public void Run()
    {
        foreach (var sample in _dataqueue.GetConsumingEnumerable())
        {
            _dataChunks.AddChunk(sample);
        }
    }

    public void Stop()
    {
        _thread.Join();
    }
    
}
