using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiplomWork.Objects
{
    public class TherminalPointNum
    {
        public TherminalPointInStation Point { get; set; }
        public int Num { get; set; }

        public TherminalPointNum(string name = null, int num = 0)
        {
            Point = new TherminalPointInStation(name);
            Num = num;
        }

        public TherminalPointNum(TherminalPointNum point)
        {
            Point = new TherminalPointInStation(point.Point);
            Num = point.Num;
        }

        public string GetName()
        {
            return Point.Name;
        }
    }
}
