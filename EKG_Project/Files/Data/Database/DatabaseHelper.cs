using System;
using System.Data.SQLite;
using System.IO;

namespace EKG_Project
{
    public static class DatabaseHelper
    {
        // Den her skal køres i Visual Studios
        //private static string FileName = @"..\..\..\Files\Database\ECG_Database.db";
        // Den her skal køres i Rider 
        private static readonly string FileName = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "Files", "Data", "Database", "ECG_Database.db"
        );
        private static string _connectionString = $"Data Source={FileName};Version=3;";

        public static void InitializeDataBase()
        {
            if (!File.Exists(FileName))
            {
                // Opret databasefilen
                SQLiteConnection.CreateFile(FileName);

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    // Vi gemmer lige nu KUN Lead I
                    // Hvis hardwaregruppen senere kan levere Lead II og Lead III,
                    // kan tabellen udvides med ekstra kolonner (fx via ALTER TABLE eller ved at slette DB og genskabe).
                    

                    string createEcgTable = @"
                        CREATE TABLE IF NOT EXISTS ecg (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            dateTime INTEGER NOT NULL,
                            leadI INTEGER NOT NULL
                        );";

                    //(ovenfor) bruger INTEGER istedet for REAL. Det er en hel værdi fra ADC'en mellem 0-4047.

                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = createEcgTable;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        // Indsæt en måling (kun Lead I)
        public static void InsertMeasurement(int dateTime, double leadI)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string insertQuery = @"
                    INSERT INTO ecg (dateTime, leadI)
                    VALUES (@dateTime, @leadI);";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@dateTime", dateTime);
                    command.Parameters.AddWithValue("@leadI", leadI);

                    command.ExecuteNonQuery();
                }
                
            }
        }

        // Læs og print alle målinger (kun Lead_I)
        public static void PrintAllMeasurements()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT * FROM ecg;";

                using (var command = new SQLiteCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(
                            $"ID: {reader["id"]}, DateTime: {reader["dateTime"]}, Lead_I: {reader["leadI"]}");
                    }
                }
            }
        }
    }
}
