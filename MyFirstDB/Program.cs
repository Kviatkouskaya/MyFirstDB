using System;
using Microsoft.Data.SqlClient;

namespace DBConection
{
    class Program
    {
        public static void PersonsList(string source, string command)
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
                        Console.WriteLine(string.Format("{0}  {1}", reader[0], reader[1]));
                    }
                }
                connection.Close();
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
            string command1 = "SELECT first_name, second_name" +
                            $" FROM dbo.BigBangTheory WHERE customer_id={personInfo[0]}";
            using (SqlConnection connection = new())
            {
                connection.ConnectionString = source;
                SqlCommand sqlCommand = new(command, connection);
                connection.Open();
                Console.WriteLine("\nAffected rows: " + sqlCommand.ExecuteNonQuery());
            }
            Console.WriteLine("Insert row:"); 
            PersonsList(source, command1);
        }
        public static string[] RequestPersonInfo()
        {
            string[] person = new string[3];
            Console.WriteLine("\nEnter customer_id: ");
            person[0] =Console.ReadLine();
            Console.WriteLine("Enter first name: ");
            person[1] = Console.ReadLine();
            Console.WriteLine("Enter second name: ");
            person[2] = Console.ReadLine();
            return person;
        }
        public static void CheckPersonException(string [] person)
        {
            if (!int.TryParse(person[0], out int _))
            {
                throw new FormatException("\nWrong id-number!");
            }
            foreach (var item in person[1])
            {
                if (!char.IsLetter(item))
                {
                    throw new FormatException("\nInvalid symbol in NAME!");
                }
            }
            foreach (var item in person[2])
            {
                if (!char.IsLetter(item))
                {
                    throw new FormatException("\nInvalid symbol in SURNAME!");
                }
            }
        }
        static void Main()
        {
            string sourceDB = @"server=(LocalDB)\MSSQLLocalDB;integrated security=SSPI;database=Customers;";
            string command = "SELECT first_name, second_name FROM dbo.BigBangTheory ORDER BY first_name";
            Console.WriteLine("Persons list: ");
            PersonsList(sourceDB, command);
            try
            {
                CreatePerson(sourceDB);
            }
            catch(FormatException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
