using System;
using System.Linq;

using Microsoft.Data.SqlClient;

namespace IncreaseAgeStoredProcedure
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Integrated Security = true;Database=MinionsDB");
            connection.Open();

            int[] minionIds = Console.ReadLine()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            using (connection)
            {
                CreateProcedure(connection);

                for (int i = 0; i < minionIds.Length; i++)
                {
                    int currId = minionIds[i];

                    ExecuteProcedure(currId, connection);
                }
            }
        }

        private static void CreateProcedure(SqlConnection connection)
        {
            SqlCommand createProcedure = new SqlCommand(Queries.createProcedure, connection);

            createProcedure.ExecuteNonQuery();
        }

        private static void ExecuteProcedure(int id, SqlConnection connection)
        {
            SqlCommand executeProcedure = new SqlCommand(Queries.executeProcedure, connection);

            executeProcedure.Parameters.AddWithValue(@"Id", id);

            executeProcedure.ExecuteNonQuery();

            PrintMinion(id, connection);
        }

        private static void PrintMinion(int id, SqlConnection connection)
        {
            SqlCommand printMinion = new SqlCommand(Queries.selectMinion, connection);

            printMinion.Parameters.AddWithValue("@Id", id);

            var minionReader = printMinion.ExecuteReader();

            using (minionReader)
            {
                while (minionReader.Read())
                {
                    Console.WriteLine($"{id} {minionReader["Name"]} - {minionReader["Age"]} years old");
                }
            }
        }
    }
}
