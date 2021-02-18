using System;
using System.Collections.Generic;
using System.Text;

namespace IncreaseMinionAge
{
    public class Queries
    {
        public const string updateMinions = @"UPDATE Minions
                                                SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                                WHERE Id = @Id";

        public const string selectMinions = @"SELECT Name, Age FROM Minions";
    }
}
