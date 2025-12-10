namespace EKG_Project;

public interface IECGSensor
{
    double ReadRawSample();
    bool IsFinished { get; }
}