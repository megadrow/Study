using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media;
using System.Windows.Shapes;
using Controls;
using DiplomWork.Dialogs;

namespace DiplomWork
{
    /// <summary>
    /// Interaction logic for NetChartResult.xaml
    /// </summary>
    public partial class NetChartResult
    {
        private readonly int _stCount;
        private readonly int _magCount;
        private int _stat;
        private readonly int _xxTmp;
        private readonly int[,] _matHlp;
        private readonly List<List<bool>> _mask;
        private readonly int[,] _stZam2;
        private readonly int[,] _zamen;
        private readonly int[,] _zamenSt;

        private readonly List<string> _conflictNames;
        private readonly int[,] _conflictMatrix;

        public NetChartResult(List<string> conNames, int[,] conMat, 
            IEnumerable<NetStation> stations, IEnumerable<NetBus> buses, IEnumerable<Line> lines, List<List<bool>> mask,
            int[,] stzam, int[,] zamen, int[,] zamenst, int[,] mathlp, int[,] nmatrix, IEnumerable<string> varList,
            int bCount, int sCount, int tmp)
        {
            InitializeComponent();
            DataContext = this;
            RbtnChart.IsChecked = true;
            foreach (var l in lines.Select(line => new Line
                {
                    X1 = line.X1,
                    X2 = line.X2,
                    Y1 = line.Y1,
                    Y2 = line.Y2,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 2
                }))
            {
                grd.Children.Add(l);
            }
            foreach (var station in stations.Select(st => 
                new NetStation(st.GetCenter().X, st.GetCenter().Y) {Number = st.Number}))
            {
                station.MouseLeftButtonDown -= station.NetStationOnMouseDown;
                grd.Children.Add(station);
            }
            foreach (var b in buses.Select(bus => 
                new NetBus(bus.GetCenter().X, bus.GetCenter().Y) {Number = bus.Number}))
            {
                b.MouseLeftButtonDown -= b.CommonObjectOnMouseDown;
                grd.Children.Add(b);
            }

            _conflictNames = conNames;
            _conflictMatrix = conMat;

            ResChart.ChartAreas.Add("def");
       
            ResChart.ChartAreas["def"].CursorX.IsUserEnabled = true;
            ResChart.ChartAreas["def"].CursorX.IsUserSelectionEnabled = true;
            ResChart.ChartAreas["def"].AxisX.ScaleView.Zoomable = true;
            ResChart.ChartAreas["def"].AxisX.ScrollBar.IsPositionedInside = true;
            
            ResChart.ChartAreas["def"].CursorY.IsUserEnabled = true;
            ResChart.ChartAreas["def"].CursorY.IsUserSelectionEnabled = true;
            ResChart.ChartAreas["def"].AxisY.ScaleView.Zoomable = true;
            ResChart.ChartAreas["def"].AxisY.ScrollBar.IsPositionedInside = true;
            ResChart.ChartAreas["def"].AxisY.Interval = 1;
            ResChart.ChartAreas["def"].AxisY.MajorGrid.Enabled = false;
            ResChart.ChartAreas["def"].AxisY.Minimum = 0;
            //string[] axisXData = new string[] { "a", "b", "c", "d", "e", "c", "a", "b", "c", "a", "b", "c", "a", "b", "c" };
            //double[] axisYData = new double[] { 0.1, 1.5, 1.9, 0.1, 1.5, 1.9, 0.1, 1.5, 1.9, 0.1, 1.5, 1.9, 0.1, 1.5, 1 };
            //ResChart.Series["name1"].Points.DataBindXY(axisXData, axisYData);
            //ResChart.Series["name1"].Points.AddXY()
            //ResChart.Series["name1"].MarkerStyle = MarkerStyle.Square;
            //ResChart.Series["name1"].Points[0].Label = "567";

            ////////ITS WORK

            _stat = 0;
            _xxTmp = tmp;
            _matHlp = mathlp;
            _magCount = bCount;
            var newMatrixes = nmatrix;
            var intSwap = new int[4];
            var prog = new int[bCount];
            _stCount = sCount;
            _stZam2 = stzam;
            _zamen = zamen;
            _zamenSt = zamenst;
            _mask = mask;
            for (int i = 0; i < _xxTmp - 1; i++)
            {
                for (int j = i + 1; j < _xxTmp; j++)
                {
                    if (newMatrixes[_matHlp[i, 1], 1] > newMatrixes[_matHlp[j, 1], 1])
                    {
                        intSwap[0] = _matHlp[i, 0];
                        intSwap[1] = _matHlp[i, 1];
                        intSwap[2] = _matHlp[i, 2];
                        intSwap[3] = _matHlp[i, 3];
                        _matHlp[i, 0] = _matHlp[j, 0];
                        _matHlp[i, 1] = _matHlp[j, 1];
                        _matHlp[i, 2] = _matHlp[j, 2];
                        _matHlp[i, 3] = _matHlp[j, 3];
                        _matHlp[j, 0] = intSwap[0];
                        _matHlp[j, 1] = intSwap[1];
                        _matHlp[j, 2] = intSwap[2];
                        _matHlp[j, 3] = intSwap[3];
                    }
                }
            }

            System.Drawing.Color co = System.Drawing.Color.Black;

            for (int i = 0; i < _xxTmp; i++)
            {
                if (_matHlp[i, 0] != 0)
                {
                    var max = 0;
                    for (int j = 0; j < _magCount; j++)
                    {
                        if (mask[_matHlp[i, 1]][j])
                        {
                            if (prog[j] > max) max = prog[j];
                        }
                    }

                    if (co == System.Drawing.Color.Black)
                        co = System.Drawing.Color.Red;
                    else
                        if (co == System.Drawing.Color.Red)
                            co = System.Drawing.Color.Blue;
                        else
                            if (co == System.Drawing.Color.Blue)
                                co = System.Drawing.Color.Green;
                            else
                                if (co == System.Drawing.Color.Green)
                                    co = System.Drawing.Color.Yellow;
                                else
                                    if (co == System.Drawing.Color.Yellow)
                                        co = System.Drawing.Color.Black;

                    for (int j = 0; j < _magCount; j++)
                    {
                        if (mask[_matHlp[i, 1]][j])
                        {
                            var serie = new Series("name" + j.ToString() + _stat.ToString());
                            ResChart.Series.Add(serie);
                            serie.ChartArea = "def";
                            serie.ChartType = SeriesChartType.Line;
                            serie.MarkerStyle = MarkerStyle.Square;
                            serie.MarkerSize = 10;
                            serie.BorderWidth = 5;
                            serie.Color = co;
                            serie.Points.AddXY(max, j + 1);
                            
                            int j1 = 0, j2 = 0;
                            for (int z = 0; z < _stCount; z++)
                            {
                                if (_matHlp[i, 2] == _stZam2[z, 1])
                                {
                                    j1 = z;
                                }
                                if (_matHlp[i, 3] == _stZam2[z, 1])
                                {
                                    j2 = z;
                                }
                            }
                            serie.Points.AddXY(max + _matHlp[i, 0], j + 1);
                            serie.Color = co;
                            serie.Points[serie.Points.Count - 1].Label = _stZam2[j1, 1].ToString() +
                                "-" + _stZam2[j2, 1].ToString();
                            _stat++;
                            //Seriesl[j]->AddXY(Max + matHlp[i, 0], j + 1,
                            //       (stZam2[j1, 0]).ToString() + "-" + (stZam2[j2, 0]).ToString(), co);

                            prog[j] = max + _matHlp[i, 0];
                        }
                    }
                }
            }

            //for (int i = 0; i < 7; i++)
            LbVariantsRes.ItemsSource = varList;
            LbVariantsRes.SelectedIndex = 0;

        }

        private void VarChanged(object sender, SelectionChangedEventArgs e)
        {
            var prog = new int[_magCount];

            for (int i = 0; i < _magCount; i++)
            {
                prog[i] = 0;
            }

            for (int i = 0; i < _xxTmp; i++)
            {
                _matHlp[i, 0] = _zamen[LbVariantsRes.SelectedIndex, i];
            }
            ResChart.Series.Clear();
            _stat = 0;

            System.Drawing.Color co = System.Drawing.Color.Black;

            for (int i = 0; i < _xxTmp; i++)
            {
                if (_matHlp[i, 0] != 0)
                {
                    var max = 0;
                    for (int j = 0; j < _magCount; j++)
                    {
                        if (_mask[_matHlp[i, 1]][j])
                        {
                            if (prog[j] > max) max = prog[j];
                        }
                    }

                    if (co == System.Drawing.Color.Black)
                        co = System.Drawing.Color.Red;
                    else if (co == System.Drawing.Color.Red)
                        co = System.Drawing.Color.Blue;
                    else if (co == System.Drawing.Color.Blue)
                        co = System.Drawing.Color.Green;
                    else if (co == System.Drawing.Color.Green)
                        co = System.Drawing.Color.Yellow;
                    else if (co == System.Drawing.Color.Yellow)
                        co = System.Drawing.Color.Black;

                    for (int j = 0; j < _magCount; j++)
                    {
                        if (_mask[_matHlp[i, 1]][j])
                        {
                            var serie = new Series("name" + j.ToString() + _stat.ToString());
                            ResChart.Series.Add(serie);
                            serie.ChartArea = "def";
                            serie.ChartType = SeriesChartType.Line;
                            serie.MarkerStyle = MarkerStyle.Square;
                            serie.MarkerSize = 10;
                            serie.BorderWidth = 5;
                            serie.Color = co;
                            serie.Points.AddXY(max, j + 1);

                            //Seriesl[j]->AddNullXY(Max, j + 1, "");
                            //Seriesl[j]->AddXY(Max, j + 1, "", co);
                            int j1 = 0, j2 = 0;
                            for (int z = 0; z < _stCount; z++)
                            {
                                if (_matHlp[i, 2] == _zamenSt[LbVariantsRes.SelectedIndex, z])
                                {
                                    j1 = z;
                                }
                                if (_matHlp[i, 3] == _zamenSt[LbVariantsRes.SelectedIndex, z])
                                {
                                    j2 = z;
                                }
                            }
                            serie.Points.AddXY(max + _matHlp[i, 0], j + 1);
                            serie.Color = co;
                            serie.Points[serie.Points.Count - 1].Label = (j1+1).ToString() +
                                                                         "-" + (j2+1).ToString();
                            _stat++;
                            //Seriesl[j]->AddXY(Max + matHlp[i, 0], j + 1,
                            //       (stZam2[j1, 0]).ToString() + "-" + (stZam2[j2, 0]).ToString(), co);

                            prog[j] = max + _matHlp[i, 0];
                        }
                    }
                }
            }

            var list =  grd.Children.OfType<NetStation>().ToList();
            for (int i = 0; i < _stCount; i++)
            {
                if (LbVariantsRes.SelectedIndex == 0)
                {
                    list[i].Number = i + 1;
                    list[i].Update();
                }
                else
                {
                    for (int j = 0; j < _stCount; j++)
                    {
                        if (_zamenSt[LbVariantsRes.SelectedIndex, j] == list[i].Number)
                        {
                            list[i].Number = (j + 1);
                            list[i].Update();
                            break;
                        }
                    }
                }
            }
            DataContext = null;
            DataContext = this;
        }

        private void ChartChecked(object sender, RoutedEventArgs e)
        {
            N1.Visibility = Visibility.Hidden;
            WinHost.Visibility = Visibility.Visible;
        }

        private void GraphChecked(object sender, RoutedEventArgs e)
        {
            N1.Visibility = Visibility.Visible;
            WinHost.Visibility = Visibility.Hidden;
        }

        private void MatrixShowClicked(object sender, RoutedEventArgs e)
        {
            var res = new MatrixShowDlg(_conflictNames, _conflictMatrix);
            res.ShowDialog();
        }
    }
}
