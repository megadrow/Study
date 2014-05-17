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

        Delete,

        Disconnect
    }

    public class CommonObject : Label
    {
        public double StrokeThickness { get; set; }

        public Brush Stroke { get; set; }

        public string Text { get; set; }

        public int Number { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

        public Mode mode = Mode.Move;

        public List<StanConnection> Connection { get; set; }

        /// <summary>
        /// Временные элементы
        /// </summary>

        protected bool _catc = false;

        protected double _stX, _stY;

        private bool _arrow = false;

        public CommonObject(bool arrow = false)
        {
            Default();
            _arrow = arrow;
        }

        private void Default()
        {
            DataContext = this;
            Connection = new List<StanConnection>();
            
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;

            Panel.SetZIndex(this, 1);
            MouseLeftButtonDown += CommonObjectOnMouseDown;
            MouseLeftButtonUp += CommonObjectOnMouseUp;
            //MouseDown += EllipseOnMouseDown;
            //MouseUp += EllipseOnMouseUp;
            MouseMove += CommonObjectOnMouseMove;
            MouseLeave += CommonObjectOnMouseLeave;
        }

        protected virtual void Move(CommonObject obj, double x, double y)
        {
            if (_catc)
            {
                var circle = obj;
                var parent = LogicalTreeHelper.GetParent(circle) as Panel;
                circle.Margin = new Thickness(x - _stX, y - _stY, 0, 0);
                Left = x - _stX;
                Top = y - _stY;

                if (Left > parent.ActualWidth - Width)
                {
                    Left = parent.ActualWidth - Width;
                    circle.Margin = new Thickness(Left,
                                              Top, 0, 0);
                    _catc = false;
                }

                if (Top > parent.ActualHeight - Height)
                {
                    Top = parent.ActualHeight - Height;
                    circle.Margin = new Thickness(Left,
                                              Top, 0, 0);
                    _catc = false;
                }

                if (Left < 0)
                {
                    Left = 0;
                    circle.Margin = new Thickness(Left,
                                              Top, 0, 0);
                    _catc = false;
                }

                if (Top < 0)
                {
                    Top = 0;
                    circle.Margin = new Thickness(Left,
                                              Top, 0, 0);
                    _catc = false;
                }

                if (Connection != null)
                {
                    for (int i = 0; i < Connection.Count; i++)
                    {
                        Connection[i].UpdateLine(this);
                    }
                }
            }
        }

        public void CommonObjectOnMouseLeave(object sender, MouseEventArgs e)
        {
            switch (mode)
            {
                case Mode.Move:
                    {
                        //_catc = false;
                        //ToFront(sender, false);
                        var circle = sender as CommonObject;
                        var parent = LogicalTreeHelper.GetParent(circle) as Panel;
                        Move(circle, e.GetPosition(parent).X, e.GetPosition(parent).Y);
                    } break;
            }
        }

        protected void ToFront(object obj, bool up)
        {
            var circle = obj as CommonObject;
            if (up)
            {
                if (circle != null) Panel.SetZIndex(circle, 2);
            }
            else
            {
                if (circle != null) Panel.SetZIndex(circle, 1);
            }
        }

        public void SetPosition(double x, double y)
        {
            _catc = true;
            Move(this, x, y);
            _catc = false;
        }

        public Point GetCenter()
        {
            return new Point(Left + Width / 2, Top + Height / 2);
        }

        public void CommonObjectOnMouseDown(object sender, MouseButtonEventArgs e)
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
                case Mode.Disconnect:
                    {
                        Disconnect();
                    } break;
            }
        }

        public void Disconnect()
        {
            for (int i = Connection.Count - 1; i > -1; i--)
            {
                Connection[i].Delete();
            }
        }

        public virtual void Delete()
        {
            Disconnect();

            var parent = LogicalTreeHelper.GetParent(this) as Panel;
            if (parent != null) parent.Children.Remove(this);
        }

        public void CommonObjectOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (mode)
            {
                case Mode.Move:
                    {
                        if (_catc)
                        {
                            _catc = false;
                            _stX = 0.0;
                            _stY = 0.0;
                            ToFront(sender, false);
                        }
                    }
                    break;
                case Mode.Connect:
                    {
                        //StanConnection.ConectTo(sender as StationEll);
                    } break;
            }
        }

        public void ToConnectMode()
        {
            mode = Mode.Connect;
            StanConnection.ToTempObj(null);
            _catc = false;
        }

        public void ToDisconnectMode()
        {
            mode = Mode.Disconnect;
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

        public void CommonObjectOnMouseMove(object sender, MouseEventArgs e)
        {
            switch (mode)
            {
                case Mode.Move:
                    {
                        if (_catc)
                        {
                            var circle = sender as CommonObject;
                            var parent = LogicalTreeHelper.GetParent(circle) as Panel;
                            Move(circle, e.GetPosition(parent).X, e.GetPosition(parent).Y);
                        }
                    } break;
            }
        }
    }
}
