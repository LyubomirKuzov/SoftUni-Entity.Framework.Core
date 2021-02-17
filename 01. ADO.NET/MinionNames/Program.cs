using System;

using Microsoft.Data.SqlClient;

namespace MinionNames
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Integrated Security = true;Database=MinionsDB");
            connection.Open();

            int desiredVillainId = int.Parse(Console.ReadLine());

            using (connection)
            {
                SqlCommand commandSelectVillainId = new SqlCommand(@"SELECT Name
	                                                                     FROM Villains
                                                                         WHERE Id = @id", connection);

                commandSelectVillainId.Parameters.AddWithValue("@id", desiredVillainId);

                using (commandSelectVillainId)
                {
                    var villainName = commandSelectVillainId.ExecuteScalar();

                    if (villainName == null)
                    {
                        Console.WriteLine($"No villain with ID {desiredVillainId} exists in the database.");
                        return;
                    }

                    Console.WriteLine($"Villain: {villainName}");
                }

                SqlCommand commandSelectVillainMinions = new SqlCommand(@"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                                                          m.Name,
	                                                                      m.Age
	                                                                      FROM MinionsVillains AS mv
	                                                                      INNER JOIN Minions AS m ON m.Id = mv.MinionId
	                                                                      WHERE mv.VillainId = @villainId
	                                                                      ORDER BY m.Name", connection);

                commandSelectVillainMinions.Parameters.AddWithValue("@villainId", desiredVillainId);

                using (commandSelectVillainMinions)
                {
                    SqlDataReader allMinions = commandSelectVillainMinions.ExecuteReader();

                    if (!allMinions.HasRows)
                    {
                        Console.WriteLine("(no minions)");
                        return;
                    }

                    while (allMinions.Read())
                    {
                        var rowNumber = allMinions["RowNum"];
                        var minionName = allMinions["Name"];
                        var minionAge = allMinions["Age"];

                        Console.WriteLine($"{rowNumber}. {minionName} {minionAge}");
                    }
                }
            }
        }
    }
}
