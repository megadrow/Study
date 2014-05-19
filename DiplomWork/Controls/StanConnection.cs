using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Controls
{
    public class StanConnection
    {
        private Line line = new Line();
        private Line arrow1 = new Line();
        private Line arrow2 = new Line();
        private CommonObject obj1 { get; set; }
        private CommonObject obj2 { get; set; }
        private bool _arrow;
        private static bool stArrow;

        private static CommonObject tmpObject;

        public CommonObject GetStartObject()
        {
            return obj2;
        }

        public CommonObject GetEndObject()
        {
            return obj1;
        }

        public static CommonObject GetTempObject()
        {
            return tmpObject;
        }

        public static void ToTempObj(CommonObject obj)
        {
            tmpObject = obj;
        }

        public static bool IsEquelWithTemp(CommonObject obj)
        {
            return (Equals(tmpObject, obj));
        }

        public static bool IsCatch()
        {
            return (tmpObject != null);
        }

        public StanConnection(bool arrow = false)
        {
            line.Stroke = new SolidColorBrush(Colors.Black);
            line.StrokeThickness = 1;
            line.MouseDown += line_MouseDown;
            _arrow = stArrow;
        }

        public static void SetArrow()
        {
            stArrow = true;
        }

        private static void SetArrowCoords(Point start, Point end, out Point oneRes, out Point twoRes)
        {
            oneRes = new Point();
            twoRes = new Point();
            var x = end.X - start.X;
            var y = end.Y - start.Y;

            var length = Math.Sqrt(x * x + y * y);

            if (x * y > 0)
            {
                oneRes.X = end.X - Math.Cos(Math.Acos(x / length) + Math.PI / 8) * 25.0;
                oneRes.Y = end.Y - Math.Sin(Math.Asin(y / length) + Math.PI / 8) * 25.0;
                twoRes.X = end.X - Math.Cos(Math.Acos(x / length) - Math.PI / 8) * 25.0;
                twoRes.Y = end.Y - Math.Sin(Math.Asin(y / length) - Math.PI / 8) * 25.0;
            }
            else
            {
                oneRes.X = end.X - Math.Cos(Math.Acos(x / length) + Math.PI / 8) * 25.0;
                oneRes.Y = end.Y + Math.Sin(-Math.Asin(y / length) + Math.PI / 8) * 25.0;
                twoRes.X = end.X - Math.Cos(Math.Acos(x / length) - Math.PI / 8) * 25.0;
                twoRes.Y = end.Y + Math.Sin(-Math.Asin(y / length) - Math.PI / 8) * 25.0;
            }
        }

        void line_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (obj1.mode == Mode.Delete)
            {
                Delete();
            }
        }

        public void UpdateLine(CommonObject st)
        {
            Point arP1, arP2;
            if (Equals(obj1, st))
            {
                line.X1 = st.GetCenter().X;
                line.Y1 = st.GetCenter().Y;
                if (_arrow)
                {
                    var stP = new Point(line.X1, line.Y1);
                    var endP = new Point(line.X2, line.Y2);
                    SetArrowCoords(stP, endP, out arP1, out arP2);
                    arrow1.X2 = arP1.X;
                    arrow1.Y2 = arP1.Y;
                    arrow2.X2 = arP2.X;
                    arrow2.Y2 = arP2.Y;
                }
            }
            else
                if (Equals(obj2, st))
                {
                    line.X2 =  st.GetCenter().X;
                    line.Y2 =  st.GetCenter().Y;
                    if (_arrow)
                    {
                        arrow1.X1 = arrow2.X1 = line.X2;
                        arrow1.Y1 = arrow2.Y1 = line.Y2;
                        var stP = new Point(line.X1, line.Y1);
                        var endP = new Point(line.X2, line.Y2);
                        SetArrowCoords(stP, endP, out arP1, out arP2);
                        arrow1.X2 = arP1.X;
                        arrow1.Y2 = arP1.Y;
                        arrow2.X2 = arP2.X;
                        arrow2.Y2 = arP2.Y;
                    }
                    
                }
        }

        public void Delete()
        {
            obj1.Connection.Remove(this);
            obj2.Connection.Remove(this);

            var parent = LogicalTreeHelper.GetParent(line) as Panel;
            if (parent != null)
            {
                parent.Children.Remove(line);
                if (_arrow)
                {
                    parent.Children.Remove(arrow1);
                    parent.Children.Remove(arrow2);
                }
            }
        }

        public static void ConectTo(CommonObject st, bool start = false)
        {
            if (!IsCatch())
            {
                if (start)
                {
                    ToTempObj(st);
                }
            }
            else
            {
                if (!IsEquelWithTemp(st))
                {
                    Connect(st);
                    ToTempObj(null);
                }
            }
        }

        public static void Connect(CommonObject st)
        {
            Connect(tmpObject, st);
        }

        public static void Connect(CommonObject st1, CommonObject st2, bool direction = false)
        {
            if (Equals(st1, st2))
            {
                MessageBox.Show("Нельзя соединить станцию с собой", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ToTempObj(null);
                return;
            }

            if ((st1.Connection != null) && (st2.Connection != null))
            {
                if (st1.Connection.Any(t1 => st2.Connection.Any(t => t1 == t)))
                {
                    MessageBox.Show("Такое соединение уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var parent = LogicalTreeHelper.GetParent(st1) as Panel;

            st1.Connection.Add(new StanConnection(stArrow));
            var index1 = st1.Connection.Count - 1;
            st2.Connection.Add(st1.Connection[index1]);
            //var index2 = st2.Connection.Count - 1;

            st1.Connection[index1].obj1 = st1;
            st1.Connection[index1].obj2 = st2;

            st1.Connection[index1].line.X1 = st1.GetCenter().X;
            st1.Connection[index1].line.Y1 = st1.GetCenter().Y;
            st1.Connection[index1].line.X2 = st2.GetCenter().X;
            st1.Connection[index1].line.Y2 = st2.GetCenter().Y;
            st1.Connection[index1].line.StrokeThickness = 2;
            
            st1.Connection[index1].line.Stroke = new SolidColorBrush(Colors.Black);

            if (stArrow)
            {
                st1.Connection[index1].arrow1.X1 = st1.Connection[index1].arrow2.X1 = st1.Connection[index1].line.X2;
                st1.Connection[index1].arrow1.Y1 = st1.Connection[index1].arrow2.Y1 = st1.Connection[index1].line.Y2;
                var stP = new Point(st1.Connection[index1].line.X1, st1.Connection[index1].line.Y1);
                var endP = new Point(st1.Connection[index1].line.X2, st1.Connection[index1].line.Y2);
                Point arP1, arP2;
                SetArrowCoords(stP, endP, out arP1, out arP2);
                st1.Connection[index1].arrow1.X2 = arP1.X;
                st1.Connection[index1].arrow1.Y2 = arP1.Y;
                st1.Connection[index1].arrow2.X2 = arP2.X;
                st1.Connection[index1].arrow2.Y2 = arP2.Y;

                st1.Connection[index1].arrow1.StrokeThickness = 2;
                st1.Connection[index1].arrow2.StrokeThickness = 2;

                st1.Connection[index1].arrow1.Stroke = st1.Connection[index1].arrow2.Stroke = new SolidColorBrush(Colors.Black);
            }
            
            if (parent != null)
            {
                parent.Children.Add(st1.Connection[index1].line);
                if (stArrow)
                {
                    parent.Children.Add(st1.Connection[index1].arrow1);
                    parent.Children.Add(st1.Connection[index1].arrow2);
                }
            }
            stArrow = false;
        }
    }
}
