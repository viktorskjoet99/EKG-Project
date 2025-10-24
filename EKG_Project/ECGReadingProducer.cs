using System.Collections.Concurrent;
using DefaultNamespace;

namespace EKG_Project;

public class ECGReadingProducer
{
    private readonly BlockingCollection<ECGSample> _dataqueue;
    private readonly ECGSensor _sensor;
    private int _samplerateHz;
    
    private CancellationTokenSource _cts;
    private Thread _thread;

    public ECGReadingProducer(ECGSensor sensor, BlockingCollection<ECGSample> dataqueue, int samplerateHz = 1000)
    {
        _sensor = sensor;
        _dataqueue = dataqueue;
        _samplerateHz = samplerateHz;
    }
    
    public void Start()
    {
        if (_thread != null) return;
        _thread = new Thread(() => Run())
        {
            IsBackground = true,
            Name = "ECGSensorProducer"
        };
        _thread.Start();
    }

    public void Stop()
    {
        if (_thread == null){ return;}
        _cts.Cancel();
        _thread.Join();
        _thread = null;
        _cts.Dispose();
        _cts = null;
        
        _dataqueue.CompleteAdding();
    }
    
    public static void Run()
    {
        var token = _cts.Token == CancellationToken.None;
    }
    
   
}