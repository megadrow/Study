using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Controls
{
    public class NetStation : CommonObject
    {
        public NetStation()
        {
            Default();
        }

        public NetStation(double left, double top)
        {
            Default(left, top);
        }

        private void Default(double left = 0, double top = 0)
        {
            Background = new SolidColorBrush(Colors.White);

            //ProcNames = new List<string>();
            //StanNames = new List<string>();

            Height = 26;
            Width = 26;
            Top = top - Height / 2;
            Left = left - Width / 2;
            Margin = new Thickness(Left, Top, 0, 0);
            
            StrokeThickness = 1;
            Stroke = new SolidColorBrush(Colors.Black);
            Foreground = new SolidColorBrush(Colors.Black);
            //MouseLeftButtonDown -= CommonObjectOnMouseDown;
            //MouseLeftButtonDown += OnMouseDown;
            //MouseMove += OnMouseMove;
        }
    }
}
