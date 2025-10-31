using System.Collections.Concurrent;
using System.Diagnostics;

namespace EKG_Project;

class Program
{
    static void Main(string[] args)
    {
      /*
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
        Console.WriteLine("Test afsluttet ✔️"); */
      
      // 1️⃣ Opret subject (AlarmCenter)
      var alarmCenter = new Alarmcenter();

      // 2️⃣ Opret observers (de forskellige alarmer)
      var alarmToPatient = new AlarmToPatient();
      var alarmToRelative = new AlarmToRelative();
      var alarmToAmbulance = new AlarmToAmbulance();

      // 3️⃣ Tilmeld observers til subject
      alarmCenter.Attach(alarmToPatient);
      alarmCenter.Attach(alarmToRelative);
      alarmCenter.Attach(alarmToAmbulance);

      // 4️⃣ Udløs en hændelse
      Console.WriteLine(">>> Simulerer hjertestop - Alarmcenter sender besked...");
      alarmCenter.Notify();

      // 5️⃣ Fjern én observer og test igen
      alarmCenter.Detach(alarmToRelative);
      Console.WriteLine("\n>>> Simulerer ny hændelse - pårørende fjernet...");
      alarmCenter.Notify();

      Console.WriteLine("\n>>> Test afsluttet ✅");
    }
}