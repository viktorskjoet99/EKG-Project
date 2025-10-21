using System.Collections.Concurrent;

namespace DefaultNamespace;

public class StartStop
{
    private ECGSensor _sensor;
    private BlockingCollection<DataChunks> _dataqueue;

    public void StartSystem()
    {
        _dataqueue = new BlockingCollection<DataChunks>(); // Måske der skal sættes en boundedCapacity
        
        _sensor = new ECGSensor(_dataqueue, samplerateHz: 1000, chunksecond: 2);

        _sensor.Start();
        Console.WriteLine("System started");
    }

    public void StopSystem()
    {
        if (_sensor != null)
        {
            _sensor.Stop();
            Console.WriteLine("System stopped");
        }
    }
}   