using System;

using Microsoft.Data.SqlClient;

namespace VillainNames
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Database=MinionsDB");
            connection.Open();

            using (connection)
            {
                SqlCommand commandSelectVillains = new SqlCommand(@"SELECT x1.VillainName,
	                                                            x1.MinionsCount
	                                                            FROM (SELECT v.Name AS VillainName,
	                                                            COUNT(mv.MinionId) AS MinionsCount
	                                                            FROM MinionsVillains AS mv
	                                                        	INNER JOIN Villains AS v ON v.Id = mv.VillainId
	                                                        	GROUP BY v.Name) AS x1
	                                                        	WHERE x1.MinionsCount > 3
	                                                        	ORDER BY x1.MinionsCount DESC", connection);

                SqlDataReader villains = commandSelectVillains.ExecuteReader();

                while (villains.Read())
                {
                    var villainName = villains["VillainName"];
                    var minionsCount = villains["MinionsCount"];

                    Console.WriteLine($"{villainName} - {minionsCount}");
                }
            }
        }
    }
}
