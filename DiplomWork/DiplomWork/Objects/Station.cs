using System.Collections.Generic;

namespace DiplomWork.Objects
{
    public class Station
    {
        private static int Numbers { get; set; } 

        public Station(string name, bool isUnReg = false)
        {
            if (!isUnReg)
            {
                Name = name;
                Numbers++;
            }
        }

        public Station( bool isUnReg = false)
        {
            if (!isUnReg)
            {
                Numbers++;
                Name = "Station " + Numbers.ToString();
            }
        }

        public void AddPoint(string name = null)
        {
            Points.Add(new TherminalPointInStation(name));
        }

        public void AddPoint(TherminalPointInStation thetm)
        {
            Points.Add(new TherminalPointInStation(thetm));
        }

        private List<TherminalPointInStation> _points = new List<TherminalPointInStation>();

        public string Name { get; set; }

        public List<TherminalPointInStation> Points
        {
            set { _points = value; }
            get { return _points; }
        }
    }
}
