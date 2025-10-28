using Microsoft.Data.Sqlite;
namespace DefaultNamespace;

public class Database
{
    private const string DatabaseFile = "ECGDatabase.db";
    private const string ConnectionString = "Data Source=ECGDatabase.db";

    public void Initialize()
    {
        if (!File.Exists(DatabaseFile))
        {
            Console.WriteLine($"Creating database {DatabaseFile}");
            using var fs  = new FileStream(DatabaseFile, FileMode.CreateNew);
        }
        
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"";
    }
}