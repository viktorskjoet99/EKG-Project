namespace EKG_Project;

public class Analyzer
{

    private readonly double _stThreshold = 0.1; // mV
    private readonly int _sampleRate = 1000;  // Hz
    private readonly int _stDelayMS = 70;  // ms
    
    private STStatus _lastStatus = STStatus.Normal;
    private readonly Alarmcenter _alarmCenter;

    public Analyzer(Alarmcenter alarmCenter)
    {
        _alarmCenter = alarmCenter;
    }

    public List<STEvent> Analyze(List<ECGSample> samples)
    {
        var events = new List<STEvent>();

        Console.WriteLine($"[Analyzer] Fik nyt chunk med {samples.Count} samples.");
        Console.WriteLine($"Første sample: {samples[0].TimeStamp}, Sidste sample: {samples[^1].TimeStamp}");

        var values = samples.Select(s => (double)s.Lead1).ToList();

        var rPeaks = DetectRPeaks(values, threshold: 0.5);

        if (rPeaks.Count == 0)
        {
            Console.WriteLine("Ingen R-takke fundet");
            return events;
        }

        double baseline = EstimateBaseline(values);

        foreach (int rIndex in rPeaks)
        {
            int stIndex = rIndex + (int)(_stDelayMS * _sampleRate / 1000);

            if (stIndex >= values.Count)
                continue;

            double stValue = values[stIndex];
            double delta = stValue - baseline;

            if (delta > _stThreshold)
            {
                events.Add(new STEvent
                {
                    TimeStamp = samples[stIndex].TimeStamp,
                    Index = stIndex,
                    Status = STStatus.Elevation
                });
            }
            else if (delta < -_stThreshold)
            {
                events.Add(new STEvent
                {
                    TimeStamp = samples[stIndex].TimeStamp,
                    Index = stIndex,
                    Status = STStatus.Depression
                });
            }
        }

        // Samlet resultat for chunken (til alarm)
        if (events.Count > 0 && _lastStatus == STStatus.Normal)
        {
            Console.WriteLine("[Analyzer] Alarm udløst!");
            _alarmCenter.Notify();
        }

        _lastStatus = events.Count > 0 ? events.First().Status : STStatus.Normal;

        return events;
    }

    private List<int> DetectRPeaks(List<double> values, double threshold)
    {
        List<int> rPeaks = new List<int>();
        int minDistance = (int)(_sampleRate * 0.25);

        for (int i = 1; i < values.Count - 1; i++)
        {
            if (values[i] > threshold && values[i] > values[i - 1] && values[i] > values[i + 1])
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