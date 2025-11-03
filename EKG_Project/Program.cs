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



        /*

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

      */
        /*
        // Sørg for at databasen findes
        DatabaseHelper.InitializeDataBase();

        // Opret en "falsk" EKG-sensor
        FakeECGSensor sensor = new FakeECGSensor();

        Console.WriteLine("Starter simulering af EKG-målinger...");

        // Simuler 20 målinger (eller lav en uendelig løkke, hvis du vil)
        for (int i = 0; i < 20; i++)
        {
            // Læs en værdi fra den falske sensor
            short sample = sensor.ReadRawSample();

            // Brug Unix timestamp som tidspunkt
            int timestamp = (int)DateTimeOffset.Now.ToUnixTimeSeconds();

            // Indsæt målingen i databasen
            DatabaseHelper.InsertMeasurement(timestamp, sample);

            Console.WriteLine($"Indsat måling: {sample}");

            // Vent lidt mellem målinger (fx 1 sekund)
            Thread.Sleep(1000);
        }

        Console.WriteLine("Simulering færdig. Her er alle målinger:");
        DatabaseHelper.PrintAllMeasurements();
        */
        
        var analyzer = new Analyzer();
        var dataChunks = new DataChunks(analyzer);

        Console.WriteLine("=== Start DataChunks test ===");

        DateTime start = DateTime.UtcNow;

        // Simuler 45 minutters målinger
        for (int i = 0; i < 45; i++)
        {
            var sample = new ECGSample
            {
                Lead1 = i,
                TimeStamp = start.AddMinutes(i)  // Én sample pr. minut
            };

            dataChunks.AddChunk(sample);
        }

        Console.WriteLine("=== Test afsluttet ===");

}
}