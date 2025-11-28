namespace EKG_Project;

public class FakeECGSensor : IECGSensor
{
    private int _i = 0;
    private readonly Random _rand = new Random();

    public int ReadRawSample()
    {
        // Simulerer EKG-lignende bølgeform + tilfældig støj
        double v = 1000 * Math.Sin(_i * 0.02) + _rand.NextDouble() * 50 - 25;
        _i++;
        return (short)Math.Clamp(v, short.MinValue, short.MaxValue);
    }
}