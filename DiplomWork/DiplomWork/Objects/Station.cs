using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DiplomWork.Objects
{
    [Serializable]
    public class Station : ISerializable
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
            else
            {
                Name = "TaskStation";
            }
        }

        public Station()
        {
            Points = new List<TherminalPointNum>();
            Numbers++;
            Name = "Station " + Numbers.ToString();
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

         #region Сериализация

        public Station(SerializationInfo info, StreamingContext context)
        {
            Numbers = info.GetInt32("static.Numbers");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("static.Numbers", Numbers, typeof(int));
        }

        #endregion

    }
}
