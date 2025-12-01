using System.Collections.Concurrent;
using EKG_Project;
using System.Diagnostics;
using System.Globalization;

namespace EKG_Project;

class Program
{
    static void Main(string[] args)
    {
       
        /*
        Console.WriteLine("=== Starter EKG system end-to-end test ===");

        // Sørg for at databasen findes
        DatabaseHelper.InitializeDataBase();
        
        // Instans af alarm
        var alarmcenter = new Alarmcenter();
        alarmcenter.Attach(new AlarmToPatient());
        alarmcenter.Attach(new AlarmToRelative());
        alarmcenter.Attach(new AlarmToAmbulance());

        // Opret de nødvendige objekter
        var analyzer   = new Analyzer(alarmcenter);            // analyserer hver chunk
        var dataChunks = new DataChunks(analyzer);  // samler + gemmer data (via din finalize)
        var queue      = new BlockingCollection<ECGSample>(boundedCapacity: 10000);

        // Producer (FakeECGSensor -> queue)
        var sensor   = new FakeECGSensor();
        var producer = new ECGReadingProducer(sensor, queue);

        // Processor (viser at vi også kan samle i RAM til evt. visning)
        var processor = new ECGProcessor(dataChunks);

        // Start producer og consumer i separate tråde
        // Producer kører sin egen tråd via Start()/Stop()
        producer.Start();

        // Consumer tømmer queue og sender til både processor og DataChunks
        var consumerThread = new Thread(() =>
        {
            try
            {
                while (!queue.IsCompleted)
                {
                    foreach (var sample in queue.GetConsumingEnumerable())
                    {
                        // a) til "RAM"-processor (kan bruges til visning/logik)
                        processor.AddSample(sample);

                        // b) til DataChunks (som ved 15 min kalder finalize og gemmer i DB)
                        dataChunks.AddChunk(sample);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Ignorer når vi lukker ned
            }
        })
        {
            IsBackground = true,
            Name = "ECGConsumer"
        };
        consumerThread.Start();

        // Kør systemet i X sekunder (justér frit)
        const int seconds = 30;
        for (int i = 0; i < seconds; i++)
        {
            Thread.Sleep(1000);
            Console.WriteLine($"[{DateTime.Now:T}] Systemet kører... sek: {i + 1}");
        }

        // Stop tråde pænt
        producer.Stop();          // beder producer om at stoppe
        queue.CompleteAdding();   // signalér at der ikke kommer flere samples
        consumerThread.Join();    // vent på consumer

        // Udskriv det fra databasen som bekræftelse
        Console.WriteLine("\nMålinger gemt i databasen\n");
        DatabaseHelper.PrintAllMeasurements();

        Console.WriteLine("\nTest afsluttet");
        */
        
        var alarmcenter = new Alarmcenter();
        var analyzer = new Analyzer(alarmcenter);

        var samples = LoadCsv("/Users/viktorskjot/Desktop/Physionet filer/ST/I03_12lead.csv");
        var events = analyzer.Analyze(samples);

        foreach (var ev in events)
        {
            Console.WriteLine($"{ev.Status} at {ev.TimeStamp:HH:mm:ss.fff}, index {ev.Index}");
        }

        Console.WriteLine("CSV test complete.");
}
    
    public static List<ECGSample> LoadCsv(string path)
    {
        var samples = new List<ECGSample>();

        var lines = File.ReadAllLines(path);

        // Skip header
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');

            // CSV columns:
            // 0 = time_s
            // 1 = I (Lead 1)
        
            double timeSeconds = double.Parse(parts[0], CultureInfo.InvariantCulture);
            double lead1Value  = double.Parse(parts[1], CultureInfo.InvariantCulture);

            samples.Add(new ECGSample
            {
                TimeStamp = DateTime.Now.AddSeconds(timeSeconds),
                Lead1 = (int)lead1Value  // Convert double → int because your ECGSample uses int
            });
        }

        return samples;
    }
}