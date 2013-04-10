using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls
{
    public enum Mode
    {
        Move,

        Connect
    }

    public class StationEll : Label
    {
        public double StrokeThickness { get; set; }

        public Brush Stroke { get; set; }

        public string Text { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }
        

        private Mode mode = Mode.Move;

        public StanConnection Connection { get; set; }

        /// <summary>
        /// Временные элементы
        /// </summary>

        private bool _catc = false;

        private double _stX, _stY;
        
        public StationEll()
        {
            Default();
        }

        public StationEll(double left, double top)
        {
            Default(left, top);
        }

        private void Default(double left = 0, double top = 0)
        {
            DataContext = this;
            Background = new SolidColorBrush(Colors.BlueViolet);
            Height = 26;
            Width = 26;
            Top = top - Height / 2;
            Left = left - Width / 2;
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new Thickness(Left, Top, 0, 0);
            StrokeThickness = 1;
            Stroke = new SolidColorBrush(Colors.Black);
            Foreground = new SolidColorBrush(Colors.White);

            Panel.SetZIndex(this, 1);

            MouseDown += EllipseOnMouseDown;
            MouseUp += EllipseOnMouseUp;
            MouseMove += EllipseOnMouseMove;
            MouseLeave += EllipseOnMouseLeave;
        }

        private void EllipseOnMouseLeave(object sender, MouseEventArgs e)
        {
            switch (mode)
            {
                case Mode.Move:
                    {
                        _catc = false;
                        ToFront(sender, false);
                    }break;
            }
        }

        private void ToFront(object obj, bool up)
        {
            var circle = obj as StationEll;
            if (up)
            {
                if (circle != null) Panel.SetZIndex(circle, 2);
            }
            else
            {
                if (circle != null) Panel.SetZIndex(circle, 1);
            }
        }

        public Point GetCenter()
        {
            return new Point(Left + Width/2, Top + Height/2);
        }

        private void EllipseOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (mode)
            {
                case Mode.Move:
                    {
                        if (!_catc)
                        {
                            _catc = true;
                            var circle = sender as StationEll;
                            _stX = e.GetPosition(circle).X;
                            _stY = e.GetPosition(circle).Y;
                            ToFront(sender, true);
                        }
                    }break;
                case Mode.Connect:
                    {
                        StanConnection.ConectTo(sender as StationEll, true);
                    }break;
            }
        }

        private void EllipseOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (mode)
            {
                case Mode.Move:
                    {
                        if (_catc)
                        {
                            _catc = false;
                            ToFront(sender, false);
                        }
                    }
                    break;
                case Mode.Connect:
                    {
                        StanConnection.ConectTo(sender as StationEll);
                    }break;
            }
        }

        public void ToConnectMode()
        {
            mode = Mode.Connect;
            StanConnection.ToTempObj(null);
            _catc = false;
        }

        public void ToMoveMode()
        {
            mode = Mode.Move;
            _catc = false;
        }

        private void EllipseOnMouseMove(object sender, MouseEventArgs e)
        {
            switch (mode)
            {
                case Mode.Move:
                    {
                        if (_catc)
                        {
                            var circle = sender as StationEll;
                            if (circle != null)
                            {
                                var parent = LogicalTreeHelper.GetParent(circle) as Panel;
                                circle.Margin = new Thickness(e.GetPosition(parent).X - _stX,
                                                              e.GetPosition(parent).Y - _stY, 0, 0);
                                Left = e.GetPosition(parent).X - _stX;
                                Top = e.GetPosition(parent).Y - _stY;
                                if (Connection != null) Connection.UpdateLine(this);
                            }
                        }
                    }break;
            }
        }
    }
}
