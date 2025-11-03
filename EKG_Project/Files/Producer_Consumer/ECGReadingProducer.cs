using System.Collections.Concurrent;


namespace EKG_Project;

public class ECGReadingProducer
{
    private readonly BlockingCollection<ECGSample> _dataqueue;
    private readonly IECGSensor _sensor;
    
    private CancellationTokenSource _cts;
    private Thread _thread;

    public ECGReadingProducer(IECGSensor sensor, BlockingCollection<ECGSample> dataqueue)
    {
        _sensor = sensor;
        _dataqueue = dataqueue;
    }
    
    public void Start()
    {
        if (_thread != null) return;
        
        _cts = new CancellationTokenSource();
        
        _thread = new Thread(() => Run(_cts.Token))
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
    
    public void Run(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            short v = _sensor.ReadRawSample();

            var sample = new ECGSample()
            {
                Lead1 = v,
                TimeStamp = DateTime.UtcNow
            };
            _dataqueue.Add(sample);
            
            Thread.Sleep(1000);
        }
        _dataqueue.CompleteAdding();
    }
}