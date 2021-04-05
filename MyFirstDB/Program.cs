using System;
using Microsoft.Data.SqlClient;

namespace DBConection
{
    class Program
    {
        public static void ShowPersonsList(string source, string command)
        {
            using (SqlConnection connection = new())
            {
                connection.ConnectionString = source;
                SqlCommand sqlCommand = new(command, connection);
                connection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("{0} - {1} {2}", reader[0], reader[1], reader[2]));
                    }
                }
            }
            Console.WriteLine();
        }
        public static void CreatePerson(string source)
        {
            string[] personInfo = RequestPersonInfo();
            CheckPersonException(personInfo);
            string command = "INSERT INTO dbo.BigBangTheory " +
                              "(customer_id, first_name, second_name)" +
                             $"VALUES ('{Convert.ToInt32(personInfo[0])}', '{personInfo[1]}', '{personInfo[2]}')";
            using (SqlConnection connection = new())
            {
                connection.ConnectionString = source;
                SqlCommand sqlCommand = new(command, connection);
                connection.Open();
                Console.WriteLine("\nAffected rows: " + sqlCommand.ExecuteNonQuery());
            }
            string command1 = "SELECT customer_id, first_name, second_name" +
                            $" FROM dbo.BigBangTheory WHERE customer_id={personInfo[0]}";
            Console.WriteLine("Inserted row:");
            ShowPersonsList(source, command1);
        }
        public static void DeletePerson(string source)
        {
            Console.WriteLine("Enter person ID for delete:");
            int personId = Convert.ToInt32(Console.ReadLine());
            string findCommand = "SELECT customer_id, first_name, second_name " +
                                 $"FROM dbo.BigBangTheory WHERE customer_id={ personId}";
            Console.WriteLine("Selected person information: ");
            ShowPersonsList(source, findCommand);
            string deleteCommand = "DELETE FROM dbo.BigBangTheory " +
                                   $"WHERE customer_id={ personId}";
            bool permission = RequestPermission();
            if (permission)
            {
                using (SqlConnection connection = new())
                {

                    connection.ConnectionString = source;
                    SqlCommand sqlCommand = new(deleteCommand, connection);
                    connection.Open();
                    Console.WriteLine("\nAffected rows: " + sqlCommand.ExecuteNonQuery());
                }
            }
            else
            {
                Console.WriteLine("Delete operation was canceled...");
            }
        }
        public static bool RequestPermission()
        {
            bool permition = false;
            Console.WriteLine("Delete this person information? yes OR no?");
            string input = Console.ReadLine();
            if (input == "yes")
            {
                return true;
            }
            return permition;
        }
        public static string[] RequestPersonInfo()
        {
            string[] person = new string[3];
            Console.WriteLine("\nEnter new person ID: ");
            person[0] = Console.ReadLine();
            Console.WriteLine("Enter first name: ");
            person[1] = Console.ReadLine();
            Console.WriteLine("Enter second name: ");
            person[2] = Console.ReadLine();
            return person;
        }
        public static void CheckPersonException(string[] person)
        {
            foreach (var item in person)
            {
                if (string.Empty == item)
                {
                    throw new FormatException("Empty collum. Invalid input!");
                }
            }
            if (!int.TryParse(person[0], out int _))
            {
                throw new FormatException("\nWrong id-number!");
            }
            for (int i = 1; i < person.Length; i++)
            {
                foreach (var item in person[i])
                {
                    if (!char.IsLetter(item))
                    {
                        throw new FormatException($"\nInvalid symbol: {item}!");
                    }
                }
            }
        }
        static void Main()
        {
            string sourceDB = @"server=(LocalDB)\MSSQLLocalDB;
                                integrated security=SSPI;database=Customers;";
            string command = "SELECT customer_id, first_name, second_name " +
                "             FROM dbo.BigBangTheory ORDER BY customer_id";
            Console.WriteLine("Persons list: ");
            ShowPersonsList(sourceDB, command);
            try
            {
                CreatePerson(sourceDB);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            DeletePerson(sourceDB);
            Console.WriteLine("\nPersons list: ");
            ShowPersonsList(sourceDB, command);
        }
    }
}
