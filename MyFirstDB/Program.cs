using System;
using Microsoft.Data.SqlClient;

namespace DBConection
{
    class Program
    {
        public static void ShowPersonsList(string source, string command, string message)
        {
            Console.WriteLine(message);
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
                             $"VALUES ('{Convert.ToInt32(personInfo[0])}'," +
                             $" '{personInfo[1]}', '{personInfo[2]}')";
            UseConnection(source, command);
            string command1 = "SELECT customer_id, first_name, second_name" +
                            $" FROM dbo.BigBangTheory WHERE customer_id={personInfo[0]}";
            ShowPersonsList(source, command1, "Inserted row:");
        }

        //  Doesn't work properly
        public static void UpdatePerson(string source)
        {
            Console.WriteLine("Enter person ID for UPDATE:");
            string personID = Console.ReadLine();
            string findCommand = "SELECT customer_id, first_name, second_name " +
                                 $"FROM dbo.BigBangTheory WHERE customer_id={Convert.ToInt32(personID)}";
            ShowPersonsList(source, findCommand, "Selected person information:");
            string[] person = RequestUpdateInfo(personID);
            CheckPersonException(person);
            string updateCommand = "UPDATE [dbo.BigBangTheory] Customers " +
                                   $"SET first_name={person[1]},second_name={person[2]}" +
                                   $"WHERE customer_id={Convert.ToInt32(person[0])}";
            //UseConnection(source, updateCommand);
            using (SqlConnection connection = new())
            {
                connection.ConnectionString = source;
                SqlCommand sqlCommand = new(updateCommand, connection);
                connection.Open();
                Console.WriteLine("\nAffected rows: " + sqlCommand.ExecuteNonQuery());
            }

        }

        public static void DeletePerson(string source)
        {
            Console.WriteLine("Enter person ID for delete:");
            int personID = Convert.ToInt32(Console.ReadLine());
            string findCommand = "SELECT customer_id, first_name, second_name " +
                                 $"FROM dbo.BigBangTheory WHERE customer_id={personID}";
            ShowPersonsList(source, findCommand, "Selected person information:");
            string deleteCommand = "DELETE FROM dbo.BigBangTheory " +
                                   $"WHERE customer_id={personID}";
            bool permission = RequestPermission("Delete");
            if (permission)
            {
                UseConnection(source, deleteCommand);
            }
            else
            {
                Console.WriteLine("Delete operation was canceled...");
            }
        }
        public static void UseConnection(string source, string command)
        {
            using (SqlConnection connection = new())
            {
                connection.ConnectionString = source;
                SqlCommand sqlCommand = new(command, connection);
                connection.Open();
                Console.WriteLine("\nAffected rows: " + sqlCommand.ExecuteNonQuery());
            }
        }

        public static bool RequestPermission(string operation)
        {
            bool permission = false;
            Console.WriteLine($"{operation} this person information? yes OR no?");
            string input = Console.ReadLine();
            if (input == "yes")
            {
                return true;
            }
            return permission;
        }
        public static string[] RequestUpdateInfo(string personID)
        {
            string[] person = new string[3];
            person[0] = personID;
            Console.WriteLine("Enter new first name: ");
            person[1] = Console.ReadLine();
            Console.WriteLine("Enter new second name: ");
            person[2] = Console.ReadLine();
            return person;
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
            ShowPersonsList(sourceDB, command, "Persons list: ");
            try
            {
                CreatePerson(sourceDB);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                UpdatePerson(sourceDB);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            DeletePerson(sourceDB);
            ShowPersonsList(sourceDB, command, "\nPersons list: ");
        }
    }
}
