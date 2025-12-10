using System.Collections.Concurrent;

namespace EKG_Project;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starter EKG system end-to-end test");

        // Initialiserer DATABASE
        DatabaseHelper.InitializeDataBase();

        // Initialiserer ALARM
        var alarmcenter = new Alarmcenter();
        alarmcenter.Attach(new AlarmToDoctor());
        alarmcenter.Attach(new AlarmToEmergencyContact());
        alarmcenter.Attach(new EmergencyCallCenter());

        // Initialiserer ANALYZER
        var analyzer = new Analyzer(alarmcenter);

        // Initialiserer DATACHUNKS
        var dataChunks = new DataChunks(analyzer);

        // Initialiserer køen
        var queue = new ECGDataqueue(10000);

        // Initialiserer SENSOR 
        var sensor = new ECGSensor();

        // Initialiserer PRODUCER og starter den
        var producer = new ECGReadingProducer(sensor, queue);
        producer.Start();

        // Initialiserer CONSUMER THREAD og starter den
        var consumer = new ECGReadingConsumer(queue, dataChunks);
        consumer.Start();
        
        Thread.Sleep(13000);   // mål i 12,59 sekunder = 6297 samples = 1 chunks
        producer.Stop();
        
        // Stopper dataflow: producer stoppes, kø lukkes og consumer afslutter
        queue.CompleteAdding();
        consumer.Stop();

        // Færdiggør det sidste chunk
        dataChunks.FinalizeRemaining();

        // Analyser HELE datasættet // Skal slettes
        var allSamples = dataChunks.GetAllSamples();
        Console.WriteLine($"\n=== FINAL ANALYSIS: Analyzing {allSamples.Count} total samples ===");
        var allEvents = analyzer.Analyze(allSamples);
        
        // Udskriver vores ST events for, hvad der har været
        Console.WriteLine("\nST Summary:");
        Console.WriteLine($"Elevations:   {allEvents.Count(e => e.Status == STStatus.Elevation)}");
        Console.WriteLine($"Depressions:  {allEvents.Count(e => e.Status == STStatus.Depression)}");
        Console.WriteLine($"Total ST evaluations: {allEvents.Count}\n");

        Console.WriteLine("\nST Events (detailed):");

        // Udskriver hvis der er tilfælde af ST-afvigelser, så hvordan det skete 
        if (allEvents.Any())
        {
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