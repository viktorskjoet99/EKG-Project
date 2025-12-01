namespace EKG_Project;

public class STEpisodes
{
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    public double Duration => (EndTime - StartTime).TotalSeconds;

    public STEpisodes(DateTime start, DateTime end)
    {
        StartTime = start;
        EndTime = end;
    }
}