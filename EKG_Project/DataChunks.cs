namespace EKG_Project;

public class DataChunks
{
    private readonly List<ECGSample> _chunks = new();
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);
    private DateTime _lastUpdate = DateTime.MinValue;
    
    private Analyzer _analyzer;

    public DataChunks(Analyzer analyzer)
    {
        _analyzer = analyzer;
    }

    public void AddChunk(ECGSample chunk)
    {
        if (_lastUpdate == DateTime.MinValue)
        {
            _lastUpdate = chunk.TimeStamp;
        }
        _chunks.Add(chunk);

        if (chunk.TimeStamp - _lastUpdate >= _interval)
        {
            FinalizeChunk();
            _lastUpdate = chunk.TimeStamp;
        }
    }

    private void FinalizeChunk()
    {
        if (_chunks.Count == 0) return;
        
        var finishedChunk = new List<ECGSample>(_chunks);
        _chunks.Clear();
        
        _analyzer.Analyze(finishedChunk);
    }
    
}