using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls
{
    public class ClrIdxStr
    {
        private Color[] colors =
            {
                Colors.Blue, Colors.BlueViolet, Colors.Brown, Colors.BurlyWood, Colors.CadetBlue, Colors.Chartreuse,
                Colors.Chocolate,
                Colors.Coral, Colors.CornflowerBlue, Colors.Crimson, Colors.Cyan, Colors.DarkCyan, Colors.DarkGoldenrod,
                Colors.DarkGray,
                Colors.DarkGreen, Colors.DarkKhaki, Colors.DarkMagenta, Colors.DarkOrange, Colors.Yellow,
                Colors.DarkSalmon
            };
        public ClrIdxStr()
        {
            Ind = -1;
        }

        private int _ind;
        public Color Clr { get; set; }
        public int Ind { get { return _ind; } set { _ind = value;
            Clr = (_ind!=-1) ? colors[_ind%20] : Colors.White;
        } }
        public string Str { get; set; }
    }

    public class GpdData : CommonObject
    {
        
        private int _cicle = 0;
        public static List<string> ProcNames = new List<string>();
        public static List<string> StanNames = new List<string>();
        public bool ShowSub
        {
            get { return _showSub; }
            set
            {
                _showSub = value;
                if (_subscribe != null)
                {
                    _subscribe.Visibility = _showSub ? Visibility.Visible : Visibility.Hidden;   
                }
            }
        }

        public int Cicle 
        {
            get { return _cicle; }
            set
            {
                _cicle = value;
                _subscribe.Text = _subscribe != null && _cicle > 0 ? _cicle.ToString() : "";
            }
        }

        public int InOutNo { get; set; }    //0 - no 1 - in 2 - out 3 - in and out

        public int OutConCount { get; set; }
        public int Level { get; set; }
        public bool IsUse { get; set; }

        public ClrIdxStr Process { get; set; }
        public ClrIdxStr Stan { get; set; }
        private TextBlock _subscribe;
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

            //ProcNames = new List<string>();
            //StanNames = new List<string>();

            Height = 26;
            Width = 26;
            Top = top - Height / 2;
            Left = left - Width / 2;
            Margin = new Thickness(Left, Top, 0, 0);

            SubscribeInit();
            
            Stan = new ClrIdxStr();
            Process = new ClrIdxStr();

            InOutNo = 0;
            OutConCount = 0;
            Level = 0;
            IsUse = false;

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
            _subscribe = new TextBlock();
            _subscribe.DataContext = this;
            _subscribe.HorizontalAlignment = HorizontalAlignment.Left;
            _subscribe.VerticalAlignment = VerticalAlignment.Top;
            _subscribe.FontSize = 10;
            _subscribe.Margin = new Thickness(Left + Width, Top, 0, 0);

            var parent = this.Parent as Grid;
            if (parent != null)
            {
                parent.Children.Add(_subscribe);
            }
        }

        public void SubscribeToGrid()
        {
            var parent = this.Parent as Grid;
            if (parent != null)
            {
                parent.Children.Add(_subscribe);
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
                            _subscribe.Margin = new Thickness(Left + Width, Top, 0, 0);
                        }
                    } break;
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
                                StanConnection.SetArrow();
                                StanConnection.Connect(sender as CommonObject);
                                CheckInOutNo();
                                var gpdModule    = Connection[Connection.Count - 1].GetEndObject() as GpdModule;
                                if (gpdModule != null)
                                    gpdModule.CheckInOutNo();
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
                        var list = new List<GpdModule>();
                        foreach (var connection in Connection)
                        {
                            if (Equals(sender, connection.GetStartObject()))
                            {
                                list.Add(connection.GetEndObject() as GpdModule);
                            }
                            else
                            {
                                list.Add(connection.GetStartObject() as GpdModule);
                            }
                        }
                        Delete();
                        foreach (var gpdModule in list)
                        {
                            gpdModule.CheckInOutNo();
                        }
                    }
                    break;
                case Mode.Disconnect:
                    {
                        var list = new List<GpdModule>();
                        foreach (var connection in Connection)
                        {
                            if (Equals(sender, connection.GetStartObject()))
                            {
                                list.Add(connection.GetEndObject() as GpdModule);
                            }
                            else
                            {
                                list.Add(connection.GetStartObject() as GpdModule);
                            }
                        }
                        Disconnect();
                        foreach (var gpdModule in list)
                        {
                            gpdModule.CheckInOutNo();
                        }
                        InOutNo = 0;
                    } break;
            }
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
                var gpdModule = connection.GetStartObject() as GpdModule;
                if (gpdModule != null && (Equals(this, connection.GetEndObject()) && (gpdModule.InOutNo != 0) &&
                                                                       (gpdModule.IsUse == false)))
                {
                    OutConCount++;
                }
            }
        }

        public override void Delete()
        {
            var parent = LogicalTreeHelper.GetParent(this) as Panel;
            if (parent != null) parent.Children.Remove(_subscribe);
            base.Delete();
        }

        protected override void Move(CommonObject obj, double x, double y)
        {
            base.Move(obj, x, y);
            var data = obj as GpdData;
            data._subscribe.Margin = new Thickness(Left + Width, Top, 0, 0);
        }

        public double GetDistanse(CommonObject st)
        {
            var x = GetCenter().X - st.GetCenter().X;
            var y = GetCenter().Y - st.GetCenter().Y;
            return Math.Sqrt(x * x + y * y);
        }
    }
}
