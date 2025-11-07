using System.Collections.Concurrent;
using EKG_Project;
using System.Diagnostics;

namespace EKG_Project;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Starter EKG system end-to-end test ===");

        // 1) Sørg for at databasen findes
        DatabaseHelper.InitializeDataBase();

        // 2) Opret de nødvendige objekter
        var analyzer   = new Analyzer();            // analyserer hver chunk
        var dataChunks = new DataChunks(analyzer);  // samler + gemmer data (via din finalize)
        var queue      = new BlockingCollection<ECGSample>(boundedCapacity: 10000);

        // Producer (FakeECGSensor -> queue)
        var sensor   = new FakeECGSensor();
        var producer = new ECGReadingProducer(sensor, queue);

        // Processor (viser at vi også kan samle i RAM til evt. visning)
        var processor = new ECGProcessor(dataChunks);

        // 3) Start producer og consumer i separate tråde
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

        // 4) Kør systemet i X sekunder (justér frit)
        const int seconds = 30;
        for (int i = 0; i < seconds; i++)
        {
            Thread.Sleep(1000);
            Console.WriteLine($"[{DateTime.Now:T}] Systemet kører... sek: {i + 1}");
        }

        // 5) Stop tråde pænt
        producer.Stop();          // beder producer om at stoppe
        queue.CompleteAdding();   // signalér at der ikke kommer flere samples
        consumerThread.Join();    // vent på consumer

        // (Valgfrit) Processér snapshot fra RAM-processoren (printer til konsol)
        processor.ProcessSamples();

        // 6) Udskriv det fra databasen som bekræftelse
        Console.WriteLine("\nMålinger gemt i databasen\n");
        DatabaseHelper.PrintAllMeasurements();

        Console.WriteLine("\nTest afsluttet");
}
}