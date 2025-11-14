namespace EKG_Project;

public class Analyzer
{

    private readonly double _stThreshold = 0.1; // mV
    private readonly int _sampleRate = 1000;  // Hz
    private readonly int _stDelayMS = 100;  // ms
    
    public void Analyze(List<ECGSample> samples)
    {
        // Her kan vi lave analyse af det modtaget chunks fra datachunks   
        Console.WriteLine($"[Analyzer] Fik nyt chunk med {samples.Count} samples.");
        Console.WriteLine($"Første sample: {samples[0].TimeStamp}, Sidste sample: {samples[^1].TimeStamp}");
    }
}