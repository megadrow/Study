using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Controls
{
    public class NetBus : CommonObject
    {
        public NetBus()
        {
            Default();
        }

        public NetBus(double left, double top)
        {
            Default(left, top);
        }

        private void Default(double left = 0, double top = 0)
        {
            Background = new SolidColorBrush(Colors.Black);

            Height = 13;
            Width = 70;
            Top = top - Height / 2;
            Left = left - Width / 2;
            Margin = new Thickness(Left, Top, 0, 0);

            StrokeThickness = 1;
            Stroke = new SolidColorBrush(Colors.Black);
            Foreground = new SolidColorBrush(Colors.White);

            //MouseLeftButtonDown -= CommonObjectOnMouseDown;
            //MouseLeftButtonDown += OnMouseMove;
        }
    }
}
