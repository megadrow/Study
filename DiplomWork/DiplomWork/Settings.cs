﻿namespace DiplomWork
{
    public class Settings
    {
        public int AreaWidth { get; set; }

        public int AreaHeight { get; set; }

        public int AreaWidthGpd { get; set; }

        public int AreaHeightGpd { get; set; }

        public Settings Copy()
        {
            var sett = new Settings { AreaHeight = this.AreaHeight, AreaWidth = this.AreaWidth, AreaHeightGpd = this.AreaHeightGpd, AreaWidthGpd = this.AreaWidthGpd};

            return sett;
        }
    }
}
