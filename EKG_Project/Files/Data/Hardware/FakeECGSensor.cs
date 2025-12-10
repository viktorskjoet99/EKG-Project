using System.Globalization;

namespace EKG_Project;

public class FakeECGSensor : IECGSensor
{
    private readonly List<double> _samples = new();
    private int _index = 0;

    public bool IsFinished => _index >= _samples.Count;

    public FakeECGSensor(string filePath)
    {
        foreach (var line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            if (parts.Length < 12) continue;

            // Brug LEAD 1 (index 1)
            if (double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                _samples.Add(value);
            }
        }

        Console.WriteLine($"[FakeECGSensor] Loaded {_samples.Count} samples from file.");
    }

    public double ReadRawSample()
    {
        if (IsFinished)
            return 0;

        return _samples[_index++];
    }
}
