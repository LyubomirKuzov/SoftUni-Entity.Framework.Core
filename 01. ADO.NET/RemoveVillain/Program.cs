using System;

using Microsoft.Data.SqlClient;

namespace RemoveVillain
{
    class Program
    {
        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());

            SqlConnection connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Database=MinionsDB");
            connection.Open();

            using (connection)
            {
                string villainName = SelectVillainName(villainId, connection);

                if (villainName == null)
                {
                    Console.WriteLine("No such villain was found.");
                }

                else
                {
                    int minionsFreed = FreeMinions(villainId, connection);

                    DeleteVillain(villainId, connection);

                    Console.WriteLine($"{villainName} was deleted.");
                    Console.WriteLine($"{minionsFreed} minions were released.");
                }
            }
        }

        private static string SelectVillainName(int villainId, SqlConnection connection)
        {
            SqlCommand selectVillainName = new SqlCommand(Queries.selectVillainName, connection);

            selectVillainName.Parameters.AddWithValue("@villainId", villainId);

            return selectVillainName.ExecuteScalar().ToString();
        }

        private static int FreeMinions(int villainId, SqlConnection connection)
        {
            SqlCommand freeMinions = new SqlCommand(Queries.freeMinions, connection);

            freeMinions.Parameters.AddWithValue("@villainId", villainId);

            return freeMinions.ExecuteNonQuery();
        }

        private static void DeleteVillain(int villainId, SqlConnection connection)
        {
            SqlCommand deleteVillain = new SqlCommand(Queries.deleteVillain, connection);

            deleteVillain.Parameters.AddWithValue("@villainId", villainId);

            deleteVillain.ExecuteNonQuery();
        }
    }
}
