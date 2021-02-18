using System;
using System.Collections.Generic;
using System.Text;

namespace ChangeTownNamesCasing
{
    public class Queries
    {
        public const string updateTownNames = @"UPDATE Towns
                                            SET Name = UPPER(Name)
                                            WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

        public const string selectTownsForCountry = @"SELECT t.Name 
                                                        FROM Towns as t
                                                        JOIN Countries AS c ON c.Id = t.CountryCode
                                                        WHERE c.Name = @countryName";
    }
}
