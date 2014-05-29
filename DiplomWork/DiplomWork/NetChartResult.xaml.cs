using System.Drawing;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media;

namespace DiplomWork
{
    /// <summary>
    /// Interaction logic for NetChartResult.xaml
    /// </summary>
    public partial class NetChartResult : Page
    {
        public NetChartResult()
        {
            InitializeComponent();
            ResChart.ChartAreas.Add("def");
            ResChart.Series.Add(new Series("name1"));
            ResChart.Series["name1"].ChartArea = "def";
            ResChart.Series["name1"].ChartType = SeriesChartType.Line;
            ResChart.Series["name1"].Color = System.Drawing.Color.Yellow;
            //ResChart.ChartAreas["def"].AlignWithChartArea
            //ResChart.ChartAreas["def"].Position.Auto = false;
            //ResChart.ChartAreas["def"].Position.           

            //ResChart.ChartAreas["def"].AxisX.ScrollBar.IsVisible = true;
            //ResChart.ChartAreas["def"].AxisX.ScrollBar.Enabled = true;
            //ResChart.ChartAreas["def"].AxisX.ScrollBar.IsPositionedInside = true;

            ResChart.ChartAreas["def"].CursorX.IsUserEnabled = true;
            ResChart.ChartAreas["def"].CursorX.IsUserSelectionEnabled = true;
            ResChart.ChartAreas["def"].AxisX.ScaleView.Zoomable = true;
            ResChart.ChartAreas["def"].AxisX.ScrollBar.IsPositionedInside = true;
            ResChart.ChartAreas["def"].CursorY.IsUserEnabled = true;
            ResChart.ChartAreas["def"].CursorY.IsUserSelectionEnabled = true;
            ResChart.ChartAreas["def"].AxisY.ScaleView.Zoomable = true;
            ResChart.ChartAreas["def"].AxisY.ScrollBar.IsPositionedInside = true;
            string[] axisXData = new string[] { "a", "b", "c", "d", "e", "c", "a", "b", "c", "a", "b", "c", "a", "b", "c" };
            double[] axisYData = new double[] { 0.1, 1.5, 1.9, 0.1, 1.5, 1.9, 0.1, 1.5, 1.9, 0.1, 1.5, 1.9, 0.1, 1.5, 1 };
            ResChart.Series["name1"].Points.DataBindXY(axisXData, axisYData);
            ResChart.Series["name1"].Points[0].MarkerStyle = MarkerStyle.Square;
            ResChart.Series["name1"].Points[0].Label = "567";
        }
    }
}
