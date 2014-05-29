using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
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
            MouseLeftButtonDown -= CommonObjectOnMouseDown;
            MouseLeftButtonDown += NetStationOnMouseDown;
            //MouseMove += OnMouseMove;
        }

        public void NetStationOnMouseDown(object sender, MouseButtonEventArgs e)
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
                        if (Connection.Count == 2)
                        {
                            MessageBox.Show("Данная станция имеет максимальное количество соединений", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            StanConnection.ToTempObj(null);
                            break;
                        }
                        if (!StanConnection.IsCatch())
                        {
                            StanConnection.ToTempObj(sender as CommonObject);
                        }
                        else
                        {
                            if ((StanConnection.GetTempObject() as NetBus) != null)
                            {
                                StanConnection.Connect(sender as CommonObject);
                            }
                            else
                            {
                                MessageBox.Show("Станции могут быть соеденины только с магистралями", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            StanConnection.ToTempObj(null);
                        }
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
    }
}
