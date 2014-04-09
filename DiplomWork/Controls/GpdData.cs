using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls
{
    public class ClrIdxStr
    {
        public ClrIdxStr()
        {
            Ind = -1;
        }
        public Color Clr { get; set; }
        public int Ind { get; set; }
        public string Str { get; set; }
    }

    public class GpdData : CommonObject
    {
        
        private int _cicle = 0;
        public bool ShowSub
        {
            get { return _showSub; }
            set
            {
                _showSub = value;
                if (subscribe != null)
                {
                    if (_showSub)
                    {
                        subscribe.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        subscribe.Visibility = Visibility.Hidden;
                    }   
                }
            }
        }

        public int Cicle 
        {
            get { return _cicle; }
            set
            {
                _cicle = value;
                if (subscribe != null && _cicle > 0)
                {
                    subscribe.Text = _cicle.ToString();
                }
                else
                {
                    subscribe.Text = "";
                }
            } 
        }

        public ClrIdxStr Process { get; set; }
        public ClrIdxStr Stan { get; set; }
        private TextBlock subscribe;
        private bool _showSub;

        public GpdData()
        {
            Default();
        }

        public GpdData(double left, double top)
        {
            Default(left, top);
        }

        private void Default(double left = 0, double top = 0)
        {
            Background = new SolidColorBrush(Colors.White);

            Height = 26;
            Width = 26;
            Top = top - Height / 2;
            Left = left - Width / 2;
            Margin = new Thickness(Left, Top, 0, 0);

            SubscribeInit();
            
            Stan = new ClrIdxStr();
            Process = new ClrIdxStr();

            StrokeThickness = 1;
            Stroke = new SolidColorBrush(Colors.Black);
            Foreground = new SolidColorBrush(Colors.Black);
            MouseLeftButtonDown -= CommonObjectOnMouseDown;
            MouseLeftButtonDown += OnMouseDown;
            MouseMove += OnMouseMove;
        }

        private void SubscribeInit()
        {
            ShowSub = false;
            subscribe = new TextBlock();
            subscribe.DataContext = this;
            subscribe.HorizontalAlignment = HorizontalAlignment.Left;
            subscribe.VerticalAlignment = VerticalAlignment.Top;
            subscribe.FontSize = 10;
            subscribe.Margin = new Thickness(Left + Width, Top, 0, 0);

            var parent = this.Parent as Grid;
            if (parent != null)
            {
                parent.Children.Add(subscribe);
            }
        }

        public void SubscribeToGrid()
        {
            var parent = this.Parent as Grid;
            if (parent != null)
            {
                parent.Children.Add(subscribe);
            }
        }
        
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            switch (mode)
            {
                case Mode.Move:
                    {
                        if (_catc)
                        {
                            subscribe.Margin = new Thickness(Left + Width, Top, 0, 0);
                        }
                    } break;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
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
                    }
                    break;
                case Mode.Connect:
                    {
                        if (!StanConnection.IsCatch())
                        {
                            StanConnection.ToTempObj(sender as CommonObject);
                        }
                        else
                        {
                            if ((StanConnection.GetTempObject() as GpdModule) != null)
                            {
                                StanConnection.Connect(sender as CommonObject);
                            }
                            else
                            {
                                MessageBox.Show("Данные могут быть соеденины только с модулями", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            StanConnection.ToTempObj(null);
                        }
                        
                    }
                    break;
                case Mode.Delete:
                    {
                        Delete();
                    }
                    break;
                case Mode.Disconnect:
                    {
                        Disconnect();
                    } break;
            }
        }

        public override void Delete()
        {
            var parent = LogicalTreeHelper.GetParent(this) as Panel;
            if (parent != null) parent.Children.Remove(subscribe);
            base.Delete();
        }

        public double GetDistanse(CommonObject st)
        {
            var x = GetCenter().X - st.GetCenter().X;
            var y = GetCenter().Y - st.GetCenter().Y;
            return Math.Sqrt(x * x + y * y);
        }
    }
}
