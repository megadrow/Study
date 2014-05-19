using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls
{
    public class GpdModule : CommonObject
    {
        public int InOutNo { get; set; }

        public int OutConCount { get; set; }
        public int Level { get; set; }
        public bool IsUse { get; set; }

        public GpdModule()
        {
            Default();
        }

         public GpdModule(double left, double top)
        {
            Default(left, top);
        }

        private void Default(double left = 0, double top = 0)
        {
            Background = new SolidColorBrush(Colors.Black);

            Height = 13;
            Width = 33;
            Top = top - Height / 2;
            Left = left - Width / 2;
            Margin = new Thickness(Left, Top, 0, 0);
            
            StrokeThickness = 1;
            Stroke = new SolidColorBrush(Colors.Black);
            Foreground = new SolidColorBrush(Colors.White);

            MouseLeftButtonDown -= CommonObjectOnMouseDown;
            MouseLeftButtonDown += OnMouseMove;
        }

        public double GetDistanse(CommonObject st)
        {
            var x = GetCenter().X - st.GetCenter().X;
            var y = GetCenter().Y - st.GetCenter().Y;
            return Math.Sqrt(x*x + y*y);
        }

        public void CalcOutClear()
        {
            OutConCount = 0;
            IsUse = false;
        }

        public void CalcOutCount()
        {
            OutConCount = 0;
            if ((Connection.Count == 0) || IsUse)
            {
                OutConCount = -1;
                IsUse = true;
                return;
            }
            foreach (var connection in Connection)
            {
                var gpdData = connection.GetStartObject() as GpdData;
                if (gpdData != null && (Equals(this, connection.GetEndObject()) && (gpdData.InOutNo != 0) &&
                                                                       (gpdData.IsUse == false)))
                {
                    OutConCount++;
                }
            }
        }

        public void CheckInOutNo()
        {
            InOutNo = 0;
            foreach (var connection in Connection)
            {
                if (InOutNo == 0)
                {
                    if (Equals(this, connection.GetStartObject()))
                    {
                        InOutNo = 1;
                        continue;
                    }
                    if (Equals(this, connection.GetEndObject()))
                    {
                        InOutNo = 2;
                        continue;
                    }
                }
                if (InOutNo == 1)
                {
                    if (Equals(this, connection.GetEndObject()))
                    {
                        InOutNo = 3;
                        break;
                    }
                }
                if (InOutNo == 2)
                {
                    if (Equals(this, connection.GetStartObject()))
                    {
                        InOutNo = 3;
                        break;
                    }
                }
            }
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
                            if ((StanConnection.GetTempObject() as GpdData) != null)
                            {
                                StanConnection.SetArrow();
                                StanConnection.Connect(sender as CommonObject);
                                CheckInOutNo();
                                var gpdData = Connection[Connection.Count - 1].GetEndObject() as GpdData;
                                if (gpdData != null)
                                    gpdData.CheckInOutNo();
                            }
                            else
                            {
                                MessageBox.Show("Модули могут быть соеденины только с данными", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            StanConnection.ToTempObj(null);
                        }

                    }
                    break;
                case Mode.Delete:
                    {
                        var list = new List<GpdData>();
                        foreach (var connection in Connection)
                        {
                            if (Equals(sender, connection.GetStartObject()))
                            {
                                list.Add(connection.GetEndObject() as GpdData);
                            }
                            else
                            {
                                list.Add(connection.GetStartObject() as GpdData);
                            }
                        }
                        Delete();
                        foreach (var gpdData in list)
                        {
                            gpdData.CheckInOutNo();
                        }
                    }
                    break;
                case Mode.Disconnect:
                    {
                        var list = new List<GpdData>();
                        foreach (var connection in Connection)
                        {
                            if (Equals(sender, connection.GetStartObject()))
                            {
                                list.Add(connection.GetEndObject() as GpdData);
                            }
                            else
                            {
                                list.Add(connection.GetStartObject() as GpdData);
                            }
                        }
                        Disconnect();
                        foreach (var gpdData in list)
                        {
                            gpdData.CheckInOutNo();
                        }
                        InOutNo = 0;
                    } break;
            }
        }
    }
}
