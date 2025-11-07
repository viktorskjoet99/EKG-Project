using UnitsNet;

namespace EKG_Project;

public class DataChunks
{
    private readonly List<ECGSample> _chunks = new();
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10); // Skal ændres til 15 min, når vi får ECG måler 
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

        // Gem hver måling i databasen
        foreach (var sample in finishedChunk)
        {
            int timestamp = (int)new DateTimeOffset(sample.TimeStamp).ToUnixTimeSeconds();
            DatabaseHelper.InsertMeasurement(timestamp, sample.Lead1);
        }

        // Og analyser dataen som før
        _analyzer.Analyze(finishedChunk);

        Console.WriteLine($"🩺 {finishedChunk.Count} målinger gemt i databasen.");
    }
    
}