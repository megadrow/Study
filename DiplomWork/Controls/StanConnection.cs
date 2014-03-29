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
        private CommonObject obj1 { get; set; }
        private CommonObject obj2 { get; set; }

        private static CommonObject tmpObject;

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

        public StanConnection()
        {
            line.Stroke = new SolidColorBrush(Colors.Black);
            line.StrokeThickness = 1;
            line.MouseDown += line_MouseDown;
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
            if (Equals(obj1, st))
            {
                line.X1 = st.GetCenter().X;
                line.Y1 = st.GetCenter().Y;
            }
            else
                if (Equals(obj2, st))
                {
                    line.X2 = st.GetCenter().X;
                    line.Y2 = st.GetCenter().Y;
                }
        }

        public void Delete()
        {
            obj1.Connection.Remove(this);
            obj2.Connection.Remove(this);

            var parent = LogicalTreeHelper.GetParent(line) as Panel;
            if (parent != null) parent.Children.Remove(line);
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

            st1.Connection.Add(new StanConnection());
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
            if (parent != null)
            {
                parent.Children.Add(st1.Connection[index1].line);
            }
        }
    }
}
