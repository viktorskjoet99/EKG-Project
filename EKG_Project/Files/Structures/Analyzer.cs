namespace EKG_Project;

public class Analyzer
{

    public void Analyze(List<ECGSample> samples)
    {
        // Her kan vi lave analyse af det modtaget chunks fra datachunks   
        Console.WriteLine($"[Analyzer] Fik nyt chunk med {samples.Count} samples.");
        Console.WriteLine($"Første sample: {samples[0].TimeStamp}, Sidste sample: {samples[^1].TimeStamp}");
    }
}