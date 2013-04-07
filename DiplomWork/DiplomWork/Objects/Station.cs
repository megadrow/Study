using System;
using System.Collections.Generic;

namespace DiplomWork.Objects
{
    public class Station
    {
        private static int Numbers { get; set; }

        public string Name { get; set; }

        public List<TherminalPointNum> Points { get; set; }

        public Station(string name, bool isUnReg = false)
        {
            Points = new List<TherminalPointNum>();
            if (!isUnReg)
            {
                Name = name;
                Numbers++;
            }
        }

        public Station( bool isUnReg = false)
        {
            Points = new List<TherminalPointNum>();
            if (!isUnReg)
            {
                Numbers++;
                Name = "Station " + Numbers.ToString();
            }
        }

        public void AddPoint(string name = null)
        {
            Points.Add(new TherminalPointNum(name));
        }

        public void AddPoint(TherminalPointNum thetm)
        {
            Points.Add(new TherminalPointNum(thetm));
        }

        public string GetPointName(int ptNum)
        {
            try
            {
                return Points[ptNum].GetName();
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
                return null;
            }
        }

    }
}
