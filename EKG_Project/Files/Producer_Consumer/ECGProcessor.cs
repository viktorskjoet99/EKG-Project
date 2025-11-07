namespace EKG_Project;

public class ECGProcessor
{
    private readonly object _lock = new object();
    private readonly List<ECGSample> _samples = new();
    private readonly DataChunks _dataChunks;

    public ECGProcessor(DataChunks dataChunks)
    {
        _dataChunks = dataChunks;
    }

    public int Count()
    {
        lock (_lock)
        {
            return _samples.Count;
        }
    }

    public void AddSample(ECGSample sample)
    {
        lock (_lock)
        {
            _samples.Add(sample);
        }
        
        _dataChunks.AddChunk(sample);
    }

    public void ProcessSamples()
    {
        List<ECGSample> snapshot;
        lock (_lock)
        {
            snapshot = new List<ECGSample>(_samples);
        }

        foreach (var sample in snapshot)
        {
            Console.WriteLine($"Value: {sample.Lead1}, TimeStamp: {sample.TimeStamp}");
        }
    }
}