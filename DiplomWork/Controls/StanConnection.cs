using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Controls
{
    public class StanConnection
    {
        public Line line = new Line();
        public StationEll obj1 { get; set; }
        public StationEll obj2 { get; set; }

        public StanConnection()
        {
            line.Stroke = new SolidColorBrush(Colors.Black);
            line.StrokeThickness = 1;
        }

        public void UpdateLine(StationEll st)
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

        public static void Connect(StationEll st1, StationEll st2)
        {
            if (st1 == st2)
            {
                throw new Exception("Нельзя соединить станцию с собой");
            }

            if ((st1.Connection != null) && (st2.Connection != null) && (st2.Connection == st1.Connection))
            {
                throw new Exception("Такое соединение уже существует"); 
            }

            var parent = LogicalTreeHelper.GetParent(st1) as Panel;

            st1.Connection = st2.Connection = new StanConnection();
            
            st1.Connection.obj1 = st1;
            st1.Connection.obj2 = st2;
            st2.Connection = st1.Connection;

            st1.Connection.line.X1 = st1.GetCenter().X;
            st1.Connection.line.Y1 = st1.GetCenter().Y;
            st1.Connection.line.X2 = st2.GetCenter().X;
            st1.Connection.line.Y2 = st2.GetCenter().Y;
            st1.Connection.line.StrokeThickness = 2;
            st1.Connection.line.Stroke = new SolidColorBrush(Colors.Black);
            if (parent != null) parent.Children.Add(st1.Connection.line);
        }
    }
}
