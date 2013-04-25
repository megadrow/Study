using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls
{
    public enum Mode
    {
        Move,

        Connect,

        Delete
    }

    public class CommonObject : Label
    {
        public double StrokeThickness { get; set; }

        public Brush Stroke { get; set; }

        public string Text { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

        public Mode mode = Mode.Move;

        public List<StanConnection> Connection { get; set; }

        /// <summary>
        /// Временные элементы
        /// </summary>

        private bool _catc = false;

        private double _stX, _stY;

        public CommonObject()
        {
            Default();
        }

        private void Default()
        {
            DataContext = this;
            Connection = new List<StanConnection>();
            
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;

            Panel.SetZIndex(this, 1);
            MouseLeftButtonDown += EllipseOnMouseDown;
            MouseLeftButtonUp += EllipseOnMouseUp;
            //MouseDown += EllipseOnMouseDown;
            //MouseUp += EllipseOnMouseUp;
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
                    } break;
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
            return new Point(Left + Width / 2, Top + Height / 2);
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
                            var circle = sender as CommonObject;
                            _stX = e.GetPosition(circle).X;
                            _stY = e.GetPosition(circle).Y;
                            ToFront(sender, true);
                        }
                    } break;
                case Mode.Connect:
                    {
                        StanConnection.ConectTo(sender as CommonObject, true);
                    } break;
                case Mode.Delete:
                    {
                        Delete();
                    } break;
            }
        }

        public void Delete()
        {
            for (int i = Connection.Count - 1; i > -1; i--)
            {
                Connection[i].Delete();
            }

            var parent = LogicalTreeHelper.GetParent(this) as Panel;
            if (parent != null) parent.Children.Remove(this);
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
                    } break;
            }
        }

        public void ToConnectMode()
        {
            mode = Mode.Connect;
            StanConnection.ToTempObj(null);
            _catc = false;
        }

        public void ToDeleteMode()
        {
            mode = Mode.Delete;
            StanConnection.ToTempObj(null);
            _catc = false;
        }

        public void Update()
        {
            DataContext = null;
            DataContext = this;
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
                                if (Connection != null)
                                {
                                    for (int i = 0; i < Connection.Count; i++)
                                    {
                                        Connection[i].UpdateLine(this);
                                    }
                                }

                            }
                        }
                    } break;
            }
        }
    }
}
