using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiplomWork.Objects
{
    public class StationNum
    {
        public  Station Station { get; set; }
        public int Num { get; set; }

        public StationNum(string name, bool isUnReg = false, int num = 0)
        {
            Station = new Station(name, isUnReg);
            Num = num;
        }

        public StationNum(bool isUnReg = false, int num = 0)
        {
            Station = new Station(isUnReg);
            Num = num;
        }

        public StationNum()
        {
            Station = new Station(false);
            Num = 0;
        }

        public void AddPoint(string name = null)
        {
            Station.AddPoint(name);
        }

        public void AddPoint(TherminalPointNum thetm)
        {
            Station.AddPoint(thetm);
        }

        public string GetPointName(int ptNum)
        {
            return Station.GetPointName(ptNum);
        }

        public string GetName()
        {
            return Station.Name;
        }

        public int GetPointCount()
        {
            return Station.Points.Count;
        }

        public List<TherminalPointNum> GetAllPoints()
        {
            return Station.Points;
        }

        public TherminalPointNum GetPoint(int ptNum)
        {
            return Station.Points[ptNum];
        }
        
    }
}
