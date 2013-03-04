namespace DiplomWork.Objects
{
    class TherminalPointInStation
    {
        public string Name { get; set; }

        public int Max { get; set; }

        public int CurrentUse { get; set; }

        public TherminalPointInStation(string name, int max = 0, int current = 0)
        {
            Name = name;
            Max = max;
            CurrentUse = current;
        }

        public TherminalPointInStation(TherminalPointInStation point)
        {
            Name = point.Name;
            Max = point.Max;
            CurrentUse = point.CurrentUse;
        }
    }
}
