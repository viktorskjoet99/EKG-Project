namespace EKG_Project;

public class ECGSample
{
    public int MeasurementId { get; set; }
    public double Lead1 { get; set; }
    //public double Lead2 { get; set; }
    //public int lead3 { get; set; }
    public DateTime TimeStamp { get; set; }
}