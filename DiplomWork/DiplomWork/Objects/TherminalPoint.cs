namespace DiplomWork.Objects
{
    public class TherminalPointInStation
    {
        public string Name { get; set; }

        public static int Numbers { get; set; }

        public int Number { get; set; }

        public int Max { get; set; }

        public int CurrentUse { get; set; }

        public TherminalPointInStation( string name = null, int max = 0, int current = 0)
        {
            Numbers++;
            Number = Numbers;
            Name = name ?? "Point " + Number;
            Max = max;
            CurrentUse = current;
        }

        public TherminalPointInStation(TherminalPointInStation point)
        {
            Name = point.Name;
            Max = point.Max;
            CurrentUse = point.CurrentUse;
            Number = point.Number;
        }
    }
}
