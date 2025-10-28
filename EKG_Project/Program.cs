using System.Collections.Concurrent;
using System.Diagnostics;

namespace EKG_Project;

class Program
{
    static void Main(string[] args)
    {
      
        // 1) Fælles kø og processor
        var queue = new BlockingCollection<ECGSample>(boundedCapacity: 5000);
        var processor = new ECGProcessor();

        // 2) Fake sensor (erstatter MCP3208 midlertidigt)
        var sensor = new FakeECGSensor();

        // 3) Start jeres rigtige producer
        var producer = new ECGReadingProducer(sensor, queue);
        producer.Start();

        // 4) Start consumer i egen tråd
        var consumer = new ECGReadingConsumer(queue, processor);
        var consumerThread = new Thread(consumer.Run)
        {
            IsBackground = true,
            Name = "ECGConsumer"
        };
        consumerThread.Start();

        // 5) Kør i 5 sekunder og udskriv løbende statistik
        for (int i = 0; i < 5; i++)
        {
            Thread.Sleep(1000);
            Console.WriteLine($"Sekund {i + 1}: QueueCount = {queue.Count}, Samples = {processor.Count()}");
        }

        // 6) Stop systemet
        producer.Stop();
        consumerThread.Join();

        // 7) Udskriv endeligt snapshot
        processor.ProcessSamples();
        Console.WriteLine("Test afsluttet ✔️");
    }
}