namespace EKG_Project;

public class Analyzer
{

    private readonly double _stThreshold = 0.1; // mV
    private readonly int _sampleRate = 1000;  // Hz
    private readonly int _stDelayMS = 100;  // ms
    
    private readonly Alarmcenter _alarmCenter;

    public Analyzer(Alarmcenter alarmCenter)
    {
        _alarmCenter = alarmCenter;
    }
    
    public void Analyze(List<ECGSample> samples)
    {
        // Her kan vi lave analyse af det modtaget chunks fra datachunks   
        Console.WriteLine($"[Analyzer] Fik nyt chunk med {samples.Count} samples.");
        Console.WriteLine($"Første sample: {samples[0].TimeStamp}, Sidste sample: {samples[^1].TimeStamp}");
        
        var values = samples.Select(s => (double)s.Lead1).ToList();

        var rPeaks = DetectRPeaks(values, threshold: 0.7);

        if (rPeaks.Count == 0)
        {
            Console.WriteLine("Ingen R-takke fundet");
            return;
        }

        double baseline = EstimateBaseline(values);
        
        List<double> stValues = new List<double>();
        foreach (int rIndex in rPeaks)
        {
            int stIndex = rIndex + (int)(_stDelayMS * _sampleRate / 1000);
            if (stIndex < values.Count) stValues.Add(values[stIndex]);
        }

        if (stValues.Count == 0)
        {
            Console.WriteLine("Ingen R-takke fundet");
            return;
        }
        
        double avgST = stValues.Average();
        double delta = avgST - baseline;

        STStatus result;
        
        if (delta > _stThreshold)
            result = STStatus.Elevation;
        else if (delta < -_stThreshold)
            result = STStatus.Depression;
        else
            result = STStatus.Normal;
        
        Console.WriteLine($"[Analyzer] Baseline={baseline:F3}, ST={avgST:F3}, Delta={delta:F3}");
        Console.WriteLine($"[Analyzer] Resultat: {result}");
        
        if (result == STStatus.Elevation)
            _alarmCenter.Notify();

        if (result == STStatus.Depression)
            _alarmCenter.Notify();
    }

    private List<int> DetectRPeaks(List<double> values, double threshold)
    {
        List<int> rPeaks = new List<int>();
        int minDistance = (int)(_sampleRate * 0.25);

        for (int i = 1; i < values.Count - 1; i++)
        {
            if (values[i] > threshold && values[i] > values[i - 1] && values[i] < values[i + 1])
            {
                if (rPeaks.Count == 0 || (i - rPeaks.Last()) > minDistance)
                    rPeaks.Add(i);
            }
        }
        return rPeaks;
    }

    private double EstimateBaseline(List<double> values)
    {
        var sorted = values.OrderBy(x => x).ToList();
        int n = sorted.Count;
        int start = (int)(n * 0.2);
        int end = (int)(n * 0.8);
        return sorted.Skip(start).Take(end - start).Average();
    }
}