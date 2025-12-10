namespace EKG_Project;

public class Analyzer
{
    private readonly int _sampleRate = 500; // Hz
    private readonly int _stDelayMS = 150; // ms

    private STStatus _lastStatus = STStatus.Normal;
    private readonly Alarmcenter _alarmCenter;

    public Analyzer(Alarmcenter alarmCenter)
    {
        _alarmCenter = alarmCenter;
    }

    public List<STEvent> Analyze(List<ECGSample> samples)
    {
        var events = new List<STEvent>();

        Console.WriteLine("Running Analyzer: ");
        
        var values = samples.Select(s => s.Lead1).ToList();
        
        double baseline = EstimateBaseline(values);
        
        var centered = values.Select(v => v - baseline).ToList();
        
        var rPeaks = DetectRPeaks(centered);
        Console.WriteLine($"Found {rPeaks.Count} R-peaks.");

        if (rPeaks.Count == 0)
            return events;
        
        double peakToPeak = centered.Max() - centered.Min();
        double dynamicST = peakToPeak * 0.08; 
        
        foreach (var rIndex in rPeaks)
        {
            int stIndex = rIndex + (int)(_stDelayMS * _sampleRate / 1000);

            if (stIndex >= values.Count)
                continue;

            double stValue = values[stIndex];
            double delta = stValue - baseline;

            if (delta > dynamicST)
            {
                events.Add(new STEvent
                {
                    Index = stIndex,
                    TimeStamp = samples[stIndex].TimeStamp,
                    Status = STStatus.Elevation
                });
            }
            else if (delta < -dynamicST)
            {
                events.Add(new STEvent
                {
                    Index = stIndex,
                    TimeStamp = samples[stIndex].TimeStamp,
                    Status = STStatus.Depression
                });
            }
        }

        if (events.Any())
        {
            int elevations = events.Count(e => e.Status == STStatus.Elevation);
            int depressions = events.Count(e => e.Status == STStatus.Depression);

            if (elevations > 0 && _lastStatus != STStatus.Elevation) 
            {
                Console.WriteLine($"ST ELEVATION detected: {elevations} events");
                _alarmCenter.Notify(STStatus.Elevation);
                _lastStatus = STStatus.Elevation;
            }
            else if (depressions > 0 && _lastStatus != STStatus.Depression) 
            {
                Console.WriteLine($"ST DEPRESSION detected: {depressions} events");
                _alarmCenter.Notify(STStatus.Depression);
                _lastStatus = STStatus.Depression;
            }
        }
        else if (_lastStatus != STStatus.Normal)
        {
            Console.WriteLine($"ST normalized");
            _lastStatus = STStatus.Normal;
        }
        
        return events;
    }

    private List<int> DetectRPeaks(List<double> values)
    {
        var rPeaks = new List<int>();
        int minDistance = (int)(_sampleRate * 0.25); 
        
        var positiveValues = values.Where(v => v > 0).ToList();
        double maxPositive = positiveValues.Any() ? positiveValues.Max() : values.Max();
        double dynThreshold = maxPositive * 0.70;  // 70% af højeste peak

        for (int i = 1; i < values.Count - 1; i++)
        {
            double v = values[i];

            bool isPeak =
                v > dynThreshold &&
                v > values[i - 1] &&
                v > values[i + 1];

            if (isPeak)
            {
                if (rPeaks.Count == 0 || (i - rPeaks.Last()) > minDistance)
                    rPeaks.Add(i);
            }
        }
        Console.WriteLine($"Found {rPeaks.Count} R-peaks in {values.Count} samples");
        return rPeaks;
    }
    
    private double EstimateBaseline(List<double> values)
    {
        var sorted = values.OrderBy(x => x).ToList();
        return sorted[values.Count / 2];
    }
    
}