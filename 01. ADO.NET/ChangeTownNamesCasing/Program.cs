using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace ChangeTownNamesCasing
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Database=MinionsDB");
            connection.Open();

            string countryName = Console.ReadLine();

            using (connection)
            {
                SqlCommand updateTownNames = new SqlCommand(Queries.updateTownNames, connection);

                updateTownNames.Parameters.AddWithValue("@countryName", countryName);

                int namesUpdatedCount = updateTownNames.ExecuteNonQuery();

                if (namesUpdatedCount == 0)
                {
                    Console.WriteLine("No town names were affected.");
                }

                else
                {
                    Console.WriteLine($"{namesUpdatedCount} town names were affected.");

                    PrintTownNames(countryName, connection);
                }
            }
        }

        private static void PrintTownNames(string countryName, SqlConnection connection)
        {
            SqlCommand selectTownsForCountry = new SqlCommand(Queries.selectTownsForCountry, connection);

            selectTownsForCountry.Parameters.AddWithValue("@countryName", countryName);

            var reader = selectTownsForCountry.ExecuteReader();

            List<string> cityNames = new List<string>();

            while (reader.Read())
            {
                cityNames.Add(reader["Name"] as string);
            }

            Console.WriteLine($"[{string.Join(", ", cityNames)}]");
        }
    }
}
