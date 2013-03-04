using System.Collections.Generic;

namespace DiplomWork.Objects
{
    class Station
    {
        private List<TherminalPointInStation> _points = new List<TherminalPointInStation>();

        public string Name { get; set; }

        public List<TherminalPointInStation> Points
        {
            set { _points = value; }
            get { return _points; }
        }
    }
}
