using System;
using System.Windows;
using System.Windows.Media;

namespace Controls
{
    public class StationRect : CommonObject
    {
        public StationRect()
        {
            Default();
        }

        public StationRect(double left, double top)
        {
            Default(left, top);
        }

        private void Default(double left = 0, double top = 0)
        {
            Background = new SolidColorBrush(Colors.BlueViolet);

            Height = 26;
            Width = 26;
            Top = top - Height / 2;
            Left = left - Width / 2;
            Margin = new Thickness(Left, Top, 0, 0);

            StrokeThickness = 1;
            Stroke = new SolidColorBrush(Colors.Black);
            Foreground = new SolidColorBrush(Colors.White);
        }

        public double GetDistanse(CommonObject st)
        {
            var x = GetCenter().X - st.GetCenter().X;
            var y = GetCenter().Y - st.GetCenter().Y;
            return Math.Sqrt(x*x + y*y);
        }
    }
}
