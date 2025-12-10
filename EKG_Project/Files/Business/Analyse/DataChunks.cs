using UnitsNet;

namespace EKG_Project;

public class DataChunks
{
    private readonly List<ECGSample> _chunks = new();
    private readonly int _chunkSize = 6297;
    private readonly List<ECGSample> _allSamples = new();
    
    private Analyzer _analyzer;

    public DataChunks(Analyzer analyzer)
    {
        _analyzer = analyzer;
    }

    public void AddChunk(ECGSample chunk)
    {
        _chunks.Add(chunk);
        _allSamples.Add(chunk);           

        if (_chunks.Count >= _chunkSize)
        {
            FinalizeChunk();
        }
    }
    
    private void FinalizeChunk()
    {
        if (_chunks.Count == 0) return;

        if (_chunks.Count < _chunkSize)
        {
            Console.WriteLine($"Skipping incomplete chunk ({_chunks.Count} samples)");
            return;
        }
    
        var finishedChunk = new List<ECGSample>(_chunks);
        _chunks.Clear();
        
        foreach (var sample in finishedChunk)
        {
            int timestamp = (int)new DateTimeOffset(sample.TimeStamp).ToUnixTimeSeconds();
            DatabaseHelper.InsertMeasurement(timestamp, sample.Lead1);
        }
    
        Console.WriteLine($" {finishedChunk.Count} measurements saved to the database");
    }
    
    public List<ECGSample> GetAllSamples()
    {
        return new List<ECGSample>(_allSamples);
    }
    
    public void FinalizeRemaining()
    {
        if (_chunks.Count == 0) return;
    
        Console.WriteLine($"Finalizing remaining {_chunks.Count} samples");
        
        foreach (var sample in _chunks)
        {
            int timestamp = (int)new DateTimeOffset(sample.TimeStamp).ToUnixTimeSeconds();
            DatabaseHelper.InsertMeasurement(timestamp, sample.Lead1);
        }
    
        _chunks.Clear();
    }
    
}