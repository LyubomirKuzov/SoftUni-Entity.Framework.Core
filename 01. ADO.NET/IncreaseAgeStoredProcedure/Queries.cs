using System;
using System.Collections.Generic;
using System.Text;

namespace IncreaseAgeStoredProcedure
{
    public class Queries
    {
        public const string createProcedure = @"CREATE OR ALTER PROC usp_GetOlder @id INT
                                                        AS
                                                        UPDATE Minions
                                                           SET Age += 1
                                                         WHERE Id = @id";

        public const string executeProcedure = @"EXEC usp_GetOlder @id";

        public const string selectMinion = @"SELECT Name, Age FROM Minions WHERE Id = @Id";
    }
}
