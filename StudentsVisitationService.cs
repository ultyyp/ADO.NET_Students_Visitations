using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Data.Sqlite;


namespace ADO.NET_Testing;

public class Visitation 
{
    public long STUDENTID { get; set; }
    public DateOnly DATE { get; set; }

    public override string ToString()
    {
        return $"STUDENT ID: {STUDENTID}, DATE: {DATE}";
    }
}


public class StudentsVisitationService
{
    internal string connectionString = "Data Source=mydatabase.db";

   

    public void CreateTable()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE Visitations"
+ "("
+ "ID INTEGER PRIMARY KEY AUTOINCREMENT,"
+ "STUDENTID INTEGER    NOT NULL ,"
+ "DATE     DATETIME    NOT NULL ,"
+ "FOREIGN KEY(STUDENTID) REFERENCES Students(ID)"
+ "); ";

        command.ExecuteNonQuery();
    }

    public void DropTable()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "DROP TABLE Visitations";
        command.ExecuteNonQuery();
    }

    public void AddVisitation(Visitation visit)
    {
        var ss = new StudentsService();
        var students = ss.GetStudents();

        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            $"INSERT INTO Visitations (STUDENTID, DATE)" +
            $"VALUES ({visit.STUDENTID}, '{visit.DATE}');";
        command.ExecuteNonQuery();
        Console.WriteLine($"{students[visit.STUDENTID-1].FIO}'s Visit Added!");
    }

    public Int64 GetCount()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Visitations;";
        Int64 count = (Int64)command.ExecuteScalar();
        return count;
    }

    public bool TableExists()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT count(*) FROM sqlite_master WHERE type = 'table' AND name = 'Visitations';";
        Int64 count = (Int64)command.ExecuteScalar();
        if (count >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Visitation[] GetVisitations()
    {
        using var connection = new SqliteConnection(connectionString);
        var sql = "SELECT * FROM Visitations";
        connection.Open();
        using var command = new SqliteCommand(sql, connection);
        using var reader = command.ExecuteReader();
        var result = new List<Visitation>();
        foreach(IDataRecord row in reader)
        {
            var visit = new Visitation
            {
                STUDENTID = (long) row.GetValue(row.GetOrdinal("STUDENTID")),
                DATE = DateOnly.Parse(row.GetString(row.GetOrdinal("DATE")))
            };
            result.Add(visit); 
        }

        return result.ToArray();
        
    }





}
