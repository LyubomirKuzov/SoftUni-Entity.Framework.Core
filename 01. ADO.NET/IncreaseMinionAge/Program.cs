using System;
using System.Linq;

using Microsoft.Data.SqlClient;

namespace IncreaseMinionAge
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Database=MinionsDB");
            connection.Open();

            int[] minionIds = Console.ReadLine()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            using (connection)
            {
                for (int i = 0; i < minionIds.Length; i++)
                {
                    int currId = minionIds[0];

                    UpdateMinion(currId, connection);
                }

                Print(connection);
            }
        }

        private static void UpdateMinion(int id, SqlConnection connection)
        {
            SqlCommand updateMinion = new SqlCommand(Queries.updateMinions, connection);

            updateMinion.Parameters.AddWithValue("@Id", id);

            updateMinion.ExecuteNonQuery();
        }

        private static void Print(SqlConnection connection)
        {
            SqlCommand selectMinions = new SqlCommand(Queries.selectMinions, connection);

            SqlDataReader reader = selectMinions.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]} {reader["Age"]}");
            }
        }
    }
}
