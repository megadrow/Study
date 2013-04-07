using System;

namespace DiplomWork.Objects
{
    public class TherminalPointInStation
    {
        public string Name { get; set; }

        public static int Numbers { get; set; }

        public int Number { get; set; }

        public TherminalPointInStation( string name = null)
        {
            Numbers++;
            Number = Numbers;
            Name = name ?? "Point " + Number;
        }

        public TherminalPointInStation(TherminalPointInStation point)
        {
            Name = point.Name;
            Number = point.Number;
        }
    }
}
