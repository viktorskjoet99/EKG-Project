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

        // Slettes, når analyzer klassen virker 
        Console.WriteLine($"[Analyzer] Received new chunk with {samples.Count} samples.");

        var values = samples.Select(s => (double)s.Lead1).ToList();
        Console.WriteLine($"Signal min: {values.Min()}, max: {values.Max()}");
        var rPeaks = DetectRPeaks(values, threshold: 0.5);
        Console.WriteLine($"Found {rPeaks.Count} R-peaks.");

        if (rPeaks.Count == 0)
        {
            Console.WriteLine("No R-takke detected");
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
            Console.WriteLine("[Analyzer] Alarm triggered");
            _alarmCenter.Notify();
        }

        _lastStatus = events.Count > 0 ? events.First().Status : STStatus.Normal;

        var stEpisodes = GroupByContinuousST(samples, baseline);

        foreach (var ep in stEpisodes)
        {
            Console.WriteLine($"ST Episode start={ep.StartTime}, end={ep.EndTime}, duration={ep.Duration} sec");
        }
        
        return events;
    }

    private List<STEpisodes> GroupByContinuousST(List<ECGSample> samples, double baseline)
    {
        var episodes = new List<STEpisodes>();
        bool inEpisode = false;
        DateTime start = DateTime.MinValue;
        DateTime end = DateTime.MinValue;

        foreach (var s in samples)
        {
            double delta = s.Lead1 - baseline;

            if (delta > _stThreshold) // Elevation detekteret
            {
                if (!inEpisode)
                {
                    inEpisode = true;
                    start = s.TimeStamp;
                }
                end = s.TimeStamp;
            }
            else
            {
                // Hvis vi forlader en episode
                if (inEpisode)
                {
                    if ((end - start).TotalSeconds >= 10) // min varighed
                        episodes.Add(new STEpisodes(start, end));

                    inEpisode = false;
                }
            }
        }

        // Sidste episode hvis aktiv
        if (inEpisode)
            episodes.Add(new STEpisodes(start, end));

        return episodes;
    }
    
    private List<int> DetectRPeaks(List<double> values, double threshold)
    {
        List<int> rPeaks = new List<int>();
        int minDistance = (int)(_sampleRate * 0.25);

        for (int i = 1; i < values.Count - 1; i++)
        {
            bool positivePeak =
                values[i] > threshold &&
                values[i] > values[i - 1] &&
                values[i] > values[i + 1];

            bool negativePeak =
                -values[i] > threshold &&     
                values[i] < values[i - 1] &&
                values[i] < values[i + 1];

            if (positivePeak || negativePeak)
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