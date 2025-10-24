using System.Collections.Concurrent;
using EKG_Project;

namespace DefaultNamespace;

public class StartStop
{
    private ECGReadingProducer _producer;
    private BlockingCollection<ECGSample> _dataqueue;

    public void StartSystem()
    {
        _dataqueue = new BlockingCollection<ECGSample>(); // Måske der skal sættes en boundedCapacity
        
        var ecgSensor = new ECGSensor();
        _producer = new ECGReadingProducer(ecgSensor, _dataqueue, 1000);

        _producer.Start();
        Console.WriteLine("System started");
    }

    public void StopSystem()
    {
        if (_producer == null)  return;
        {
            _producer.Stop();
            Console.WriteLine("System stopped");
        }
    }
}   