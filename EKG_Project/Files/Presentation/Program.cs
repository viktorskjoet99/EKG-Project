using System.Collections.Concurrent;

namespace EKG_Project;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starter EKG system end-to-end test");

        // 1) DATABASE
        DatabaseHelper.InitializeDataBase();

        // 2) ALARM
        var alarmcenter = new Alarmcenter();
        alarmcenter.Attach(new AlarmToDoctor());
        alarmcenter.Attach(new AlarmToEmergencyContact());
        alarmcenter.Attach(new EmergencyCallCenter());

        // 3) ANALYZER
        var analyzer = new Analyzer(alarmcenter);

        // 4) DATACHUNKS
        var dataChunks = new DataChunks(analyzer);

        // 5) QUEUE
        var queue = new BlockingCollection<ECGSample>(boundedCapacity: 10000);

        // 6) SENSOR (hardware)
        var sensor = new ECGSensor();

        // 7) PRODUCER
        var producer = new ECGReadingProducer(sensor, queue);
        producer.Start();

        // 8) CONSUMER THREAD
        var consumer = new ECGReadingConsumer(queue, dataChunks);
        consumer.Start();

        // 9) Vent til producer stopper
        Thread.Sleep(13000);   // mål i 10 sekunder = 5000 samples = 1 chunks
        producer.Stop();

        // 10) Stop queue
        queue.CompleteAdding();
        consumer.Stop();

        // 11) Færdiggør det sidste chunk
        dataChunks.FinalizeRemaining();

        // 12) Analyser HELE datasættet
        var allSamples = dataChunks.GetAllSamples();
        var allEvents = analyzer.Analyze(allSamples);

        Console.WriteLine("\nST Summary:");
        Console.WriteLine($"Elevations:   {allEvents.Count(e => e.Status == STStatus.Elevation)}");
        Console.WriteLine($"Depressions:  {allEvents.Count(e => e.Status == STStatus.Depression)}");
        Console.WriteLine($"Total ST evaluations: {allEvents.Count}\n");

        Console.WriteLine("\nST Events (detailed):");

        if (allEvents.Any())
        {
            Console.WriteLine("No ST events detected.");
            foreach (var ev in allEvents)
                Console.WriteLine($"{ev.Status} at {ev.TimeStamp:HH:mm:ss.fff}, id: {ev.Index}");
        }
        else
        {
            Console.WriteLine("No ST events detected.");
        }
        
        Console.WriteLine("\nSystem afsluttet\nAlle chunks er analyseret og alle målinger er gemt i databasen.");
    }
}