using Bogus;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET_Testing
{
    public class Student
    {
        public string FIO { get; set; }
        public DateOnly DOB { get; set; }

        public string EMAIL { get; set; }

        public override string ToString()
        {
            return $"FIO: {FIO}, DOB: {DOB}, EMAIL: {EMAIL}";
        }
    }


    public class StudentsService
    {
        internal string connectionString = "Data Source=mydatabase.db";


        public void CreateTable()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE Students"
    + "("
    + "ID INTEGER PRIMARY KEY AUTOINCREMENT,"
    + "FIO      TEXT        NOT NULL ,"
    + "DOB     DATETIME    NOT NULL," 
    + "EMAIL    TEXT        NOT NULL"   
    + "); ";

            command.ExecuteNonQuery();
        }

        public void DropTable()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DROP TABLE Students";
            command.ExecuteNonQuery();
        }

        public Int64 GetCount()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Students;";
            Int64 count = (Int64)command.ExecuteScalar();
            return count;
        }

        public bool StudentExists(int id)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(*) FROM Students WHERE ID = {id};";
            Int64 count = (Int64)command.ExecuteScalar();
            if(count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddStudent(Student student)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText =
                $"INSERT INTO Students (FIO, DOB, EMAIL)" +
                $"VALUES ('{student.FIO}', '{student.DOB}', '{student.EMAIL}');";
            command.ExecuteNonQuery();
            Console.WriteLine($"{student.FIO} Added To Students!");
        }

        public bool TableExists()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT count(*) FROM sqlite_master WHERE type = 'table' AND name = 'Students';";
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

        public Student[] GetStudents()
        {
            using var connection = new SqliteConnection(connectionString);
            var sql = "SELECT * FROM Students";
            connection.Open();
            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();
            var result = new List<Student>();
            foreach (IDataRecord row in reader)
            {
                var student = new Student
                {
                    FIO = row.GetString(row.GetOrdinal("FIO")),
                    DOB = DateOnly.Parse(row.GetString(row.GetOrdinal("DOB"))),
                    EMAIL = row.GetString(row.GetOrdinal("EMAIL"))
                };
                result.Add(student);
            }

            return result.ToArray();

        }

        



    }
}
