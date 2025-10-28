using System;
using System.Data.SQLite;
using System.IO;

namespace EKG_Project
{
    public static class DatabaseHelper
    {
        private static string connectionString = @"Data Source=..\..\Files\ECG_Database.db;Version=3;";

        public static void InitializeDataBase()
        {
            if (!File.Exists(@"..\..\Files\ECG_Database.db"))
            {
                // Opret databasefilen
                SQLiteConnection.CreateFile(@"..\..\Files\ECG_Database.db");

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Vi gemmer lige nu KUN Lead I
                    // Hvis hardwaregruppen senere kan levere Lead II og Lead III,
                    // kan tabellen udvides med ekstra kolonner (fx via ALTER TABLE eller ved at slette DB og genskabe).

                    string createECGTable = @"
                        CREATE TABLE IF NOT EXISTS ecg (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            time INTEGER NOT NULL,
                            lead_I REAL NOT NULL
                        );";

                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = createECGTable;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        // Indsæt en måling (kun Lead I)
        public static void InsertMeasurement(int time, double leadI)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string insertQuery = @"
                    INSERT INTO ecg (time, lead_I)
                    VALUES (@time, @leadI);";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@time", time);
                    command.Parameters.AddWithValue("@leadI", leadI);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Læs og print alle målinger (kun Lead I)
        public static void PrintAllMeasurements()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT * FROM ecg;";

                using (var command = new SQLiteCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(
                            $"ID: {reader["id"]}, Time: {reader["time"]}, Lead I: {reader["lead_I"]}");
                    }
                }
            }
        }
    }
}
