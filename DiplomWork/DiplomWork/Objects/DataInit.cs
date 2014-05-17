using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DiplomWork.Objects
{
    [Serializable]
    public class DataInit : ISerializable 
    {
        public ObservableCollection<StationNum> Stations { get; set; }
        public ObservableCollection<StationNum> PointsTask { get; set; }

        [XmlAttribute("chk")]
        private static bool chk = false;

        public DataInit()
        {
            if (!chk)
            {
                Stations = new ObservableCollection<StationNum>();
                PointsTask = new ObservableCollection<StationNum> {new StationNum(true)};
                chk = true;
            }
        }

        public static bool IsAvailable()
        {
            return chk;
        }

        public static void Clear()
        {
            chk = false;
        }

        public int GetPointCount()
        {
            return PointsTask[0].GetPointCount();
        }

        public string GetPointName(int ptNum)
        {
            return PointsTask[0].GetPointName(ptNum);
        }

        public string GetStationName(int stNum)
        {
            return Stations[stNum].GetName();
        }

        public List<TherminalPointNum> GetAllPoints()
        {
            return PointsTask[0].GetAllPoints();
        }

        public void AddStation()
        {
            var station = new StationNum();
            foreach (var point in GetAllPoints())
            {
                station.AddPoint(point);
            }
            Stations.Add(station);
        }

        public void AddPoint()
        {
            PointsTask[0].AddPoint();
            foreach (var station in Stations)
            {
                station.AddPoint(PointsTask[0].GetPoint(GetPointCount() - 1));
            }
        }

        public int GetStationCount()
        {
            return Stations.Count;
        }

        public TherminalPointNum GetPoint(int ptNum)
        {
            return PointsTask[0].GetPoint(ptNum);
        }

        public int GetPointTask(int ptNum)
        {
            return PointsTask[0].GetPoint(ptNum).Num;
        }

        public int GetPointNumber(int stationId, int ptId)
        {
            return Stations[stationId].GetPoint(ptId).Num;
        }

        #region Сериализация

        public DataInit(SerializationInfo info, StreamingContext context)
        {
            chk = info.GetBoolean("static.chk");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("static.chk", chk, typeof(bool));
        }

        #endregion
    }
}
