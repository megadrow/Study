using System.Collections.Generic;

namespace DiplomWork
{
    public class PointCoverW
    {
        public List<int> PointCover { get; set; }

        public List<int> StationCount { get; set; }

        public List<int> PointCount { get; set; }

        public List<string> PointView { get; private set; }

        public PointCoverW(int stCount, int ptCount)
        {
            PointCount = new List<int>();
            StationCount = new List<int>();
            PointCover = new List<int>();
            PointView = new List<string>();

            for (int i = 0; i < ptCount; i++)
            {
                PointView.Add(string.Empty);
                PointCount.Add(0);
                PointCover.Add(0);
            }

            for (int i = 0; i < stCount; i++)
            {
                StationCount.Add(0);
            }
        }

        public void SetPtView()
        {
            for (int i = 0; i < PointCount.Count; i++)
            {
                PointView[i] = PointCover[i].ToString() + "/" + PointCount[i].ToString();
            }
        }
    }
}
