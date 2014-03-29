using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls
{
    public class GpdData : CommonObject
    {
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

            StrokeThickness = 1;
            Stroke = new SolidColorBrush(Colors.Black);
            Foreground = new SolidColorBrush(Colors.Black);
            MouseLeftButtonDown -= CommonObjectOnMouseDown;
            MouseLeftButtonDown += OnMouseMove;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
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

        public double GetDistanse(CommonObject st)
        {
            var x = GetCenter().X - st.GetCenter().X;
            var y = GetCenter().Y - st.GetCenter().Y;
            return Math.Sqrt(x * x + y * y);
        }
    }
}
