using System;
using System.Runtime.Serialization;

namespace DiplomWork.Objects
{
    [Serializable]
    public class TherminalPointInStation : ISerializable
    {
        public string Name { get; set; }

        public static int Numbers { get; set; }

        public int Number { get; set; }

        public TherminalPointInStation()
        {
            Numbers++;
            Number = Numbers;
            Name = "Point " + Number;
        }

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

        #region Сериализация

        public TherminalPointInStation(SerializationInfo info, StreamingContext context)
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
