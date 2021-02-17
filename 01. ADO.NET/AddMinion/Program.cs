using System;
using System.Linq;

using Microsoft.Data.SqlClient;

namespace AddMinion
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] minionArgs = Console.ReadLine()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            string[] villainArgs = Console.ReadLine()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            string minionName = minionArgs[1];
            int minionAge = int.Parse(minionArgs[2]);
            string minionTownName = minionArgs[3];

            string villainName = villainArgs[1];

            SqlConnection connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Database=MinionsDB");
            connection.Open();

            using (connection)
            {
                //SqlTransaction transaction = connection.BeginTransaction();

                Towns(minionTownName, connection);

                Villains(villainName, connection);

                Minions(minionName, minionAge, minionTownName, villainName, connection);

                //transaction.Commit();
            }
        }

        public static void Towns(string minionTownName, SqlConnection connection)
        {
            SqlCommand selectTownId = new SqlCommand(@"SELECT Id 
                                                        FROM Towns
                                                        WHERE Name = @townName", connection);

            selectTownId.Parameters.AddWithValue("@townName", minionTownName);

            var townId = selectTownId.ExecuteScalar();

            if (townId == null)
            {
                SqlCommand insertTown = new SqlCommand(@"INSERT INTO Towns(Name)
                                                          VALUES (@townName)", connection);

                insertTown.Parameters.AddWithValue("@townName", minionTownName);

                insertTown.ExecuteNonQuery();

                Console.WriteLine($"Town {minionTownName} was added to the database.");
            }
        }

        public static void Villains(string villainName, SqlConnection connection)
        {
            SqlCommand selectVillainId = new SqlCommand(@"SELECT Id
                                                          FROM Villains
                                                            WHERE Name = @villainName", connection);

            selectVillainId.Parameters.AddWithValue("@villainName", villainName);

            var villainId = selectVillainId.ExecuteScalar();

            if (villainId == null)
            {
                SqlCommand insertVillain = new SqlCommand(@"INSERT INTO Villains(Name, EvilnessFactorId)
                                                            VALUES(@villainName, 4)", connection);

                insertVillain.Parameters.AddWithValue("@villainName", villainName);

                insertVillain.ExecuteNonQuery();

                Console.WriteLine($"Villain {villainName} was added to the database.");
            }
        }

        public static void Minions(string minionName, int minionAge, string minionTownName, string villainName, SqlConnection connection)
        {
            SqlCommand selectMinionId = new SqlCommand(@"SELECT Id
                                                            FROM Minions
                                                            WHERE Name = @name", connection);

            selectMinionId.Parameters.AddWithValue("@name", minionName);

            var minionId = selectMinionId.ExecuteScalar();

            if (minionId == null)
            {
                SqlCommand selectTownId = new SqlCommand(@"SELECT Id 
                                                        FROM Towns
                                                        WHERE Name = @townName", connection);

                selectTownId.Parameters.AddWithValue("@townName", minionTownName);

                var townId = selectTownId.ExecuteScalar();

                SqlCommand insertMinion = new SqlCommand(@"INSERT INTO Minions(Name, Age, TownId)
                                                        VALUES (@name, @age, @townId)", connection);

                insertMinion.Parameters.AddWithValue("@name", minionName);
                insertMinion.Parameters.AddWithValue("@age", minionAge);
                insertMinion.Parameters.AddWithValue("@townId", townId);

                insertMinion.ExecuteNonQuery();

                SqlCommand selectVillainId = new SqlCommand(@"SELECT Id
                                                            FROM Villains
                                                            WHERE Name = @name", connection);

                selectVillainId.Parameters.AddWithValue("@name", villainName);

                int villainId = (int)selectVillainId.ExecuteScalar();
                minionId = (int)selectMinionId.ExecuteScalar();

                SqlCommand insertIntoMappingTable = new SqlCommand(@"INSERT INTO MinionsVillains(MinionId, VillainId)
                                                                     VALUES (@minionId, @villainId)", connection);

                insertIntoMappingTable.Parameters.AddWithValue("@minionId", minionId);
                insertIntoMappingTable.Parameters.AddWithValue("@villainId", villainId);

                insertIntoMappingTable.ExecuteNonQuery();

                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
            }
        }
    }
}
