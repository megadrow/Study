using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DiplomWork.Objects
{
    public class FirstStep
    {
        public ObservableCollection<StationNum> Stations { get; set; }
        public ObservableCollection<StationNum> PointsTask { get; set; }

        private static bool chk = false;

        public FirstStep()
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
    }
}
