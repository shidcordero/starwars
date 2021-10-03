using StarWarsApiCSharp;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace StarWars.Core.Models
{
    public class Starship : StarWarsApiCSharp.Starship
    {
        public List<Person> PilotList { get; set; } = new List<Person>();

        public int ParsedPassengers
        {
            get
            {
                if (Passengers != null)
                {
                    if (Passengers != "unknown" && Passengers != "n/a")
                    {
                        if (Passengers.Contains(',', StringComparison.OrdinalIgnoreCase))
                        {
                            Passengers = Passengers.Replace(",", "", StringComparison.OrdinalIgnoreCase);
                        }

                        return int.Parse(Passengers, CultureInfo.InvariantCulture);
                    }
                }

                return 0;
            }
        }
    }
}