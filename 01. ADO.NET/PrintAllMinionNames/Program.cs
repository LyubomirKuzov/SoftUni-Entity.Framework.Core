using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace PrintAllMinionNames
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Database=MinionsDB");
            connection.Open();

            using (connection)
            {
                SqlCommand selectMinionNames = new SqlCommand(Queries.selectMinionNames, connection);

                List<string> minionNames = new List<string>();

                SqlDataReader minionNamesReader = selectMinionNames.ExecuteReader();

                while (minionNamesReader.Read())
                {
                    minionNames.Add(minionNamesReader["Name"].ToString());
                }

                PrintNames(minionNames);
            }
        }

        private static void PrintNames(List<string> minionNames)
        {
            for (int i = 0; i < minionNames.Count / 2; i++)
            {
                Console.WriteLine(minionNames[0 + i]);
                Console.WriteLine(minionNames[minionNames.Count - 1 - i]);
            }

            if (minionNames.Count % 2 != 0)
            {
                Console.WriteLine(minionNames[minionNames.Count / 2]);
            }
        }
    }
}
