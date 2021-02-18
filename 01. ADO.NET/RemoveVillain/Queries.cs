using System;
using System.Collections.Generic;
using System.Text;

namespace RemoveVillain
{
    public class Queries
    {
        public const string selectVillainName = @"SELECT Name FROM Villains WHERE Id = @villainId";

        public const string freeMinions = @"DELETE FROM MinionsVillains 
                                            WHERE VillainId = @villainId";

        public const string deleteVillain = @"DELETE FROM Villains
                                                WHERE Id = @villainId";
    }
}
