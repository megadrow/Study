using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Controls;
using DiplomWork.Dialogs;
using DiplomWork.Objects;
using Calculation;

namespace DiplomWork
{
    /// <summary>
    /// Interaction logic for PointDistribution.xaml
    /// </summary>
    public partial class PointDistribution
    {
        private bool _con;

        private DataInit Step { get; set; }

        private int StationCount { get; set; }

        private int PointCount { get; set; }

        private int PointUse { get; set; }

        public Settings Settings { get; set; }

        public double MouseX { get; set; }

        public double MouseY { get; set; }

        public double KmRes { get; set; }

        public double KmRes2 { get; set; }

        public PointDistribution(DataInit step, Settings settings)
        {
            InitializeComponent();
            Settings = settings;
            grd.Width = 2000;
            Step = step;
            StationCount = 0;
            PointCount = 0;
            DataContext = this;
            foreach (var pt in Step.GetAllPoints())
            {
                PointCount += pt.Num;
            }

            foreach (var st in Step.Stations)
            {
                StationCount += st.Num;
                //foreach (var pt in st.GetAllPoints())
                //{
                //    pointCount += pt.Num*st.Num;
                //}
            }
        }

        private void AddObjectToGrid(double left, double top, string text = null, bool ellipse = true)
        {
            CommonObject circle;
            if (ellipse)
            {
                circle = new StationEll(left, top);
            }
            else
            {
                circle = new StationRect(left, top);
            }


            circle.Number = Convert.ToInt32(text);
            var context = new ContextMenu();
            var mi = new MenuItem {Header = "Закрепить"};
            mi.Click += mi_Click;
            context.Items.Add(mi);

            mi = new MenuItem {Header = "Свойства"};
            //if (circle.GetType() == typeof(StationEll))
            //{
                mi.Click += PointProp_Click;
            //}
            
            context.Items.Add(mi);

            circle.ContextMenu = context;

            grd.Children.Add(circle);
        }

        private void mi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = sender as MenuItem;
                if (item != null) item.IsChecked = !item.IsChecked;
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }
        private void PointProp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                var item = (sender as MenuItem);

                if (item != null)
                {
                    var item2 = LogicalTreeHelper.GetParent(item);
                    var item3 = LogicalTreeHelper.GetParent(item2) as Popup;
                    if (item3 != null)
                    {
                        var item4 = (item3.PlacementTarget as CommonObject);
                        if (item4 != null)
                        {
                            var dlg = new PointInfo {DataContext = item4.GetCenter(), PiName = {Text = item4.Text}};

                            dlg.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }

        private void Ima_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_con)
            {
                if (PointUse < PointCount)
                {
                    PointUse++;
                    AddObjectToGrid(e.GetPosition(ima).X, e.GetPosition(ima).Y, PointUse.ToString());
                }
                else
                {
                    ErrorViewer.ShowInfo("Все точки расположены на поле");
                }
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var st in grd.Children.OfType<StationEll>())
                {
                    st.ToMoveMode();
                    _con = false;
                }
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }

        private void ConOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var st in grd.Children.OfType<StationEll>())
                {
                    st.ToConnectMode();
                    _con = true;
                }
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }

        private void AutoOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearGrid();
                ClearGridRect();
                var rand = new Random();
                
                for (int i = PointUse; i < PointCount; i++)
                {
                    PointUse++;
                    AddObjectToGrid(rand.NextDouble() * ima.ActualWidth, rand.NextDouble() * ima.ActualHeight, PointUse.ToString());
                }
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }

        private void ClearGridRect()
        {
            var gridChildrenCount = grd.Children.OfType<StationRect>().Count();
            for (int i = 0; i < gridChildrenCount; i++)
            {
                foreach (var child in grd.Children.OfType<StationRect>())
                {
                    child.Delete();
                    break;
                }
            }
        }

        private void ClearGrid()
        {
            var gridChildrenCount = grd.Children.OfType<StationEll>().Count();
            for (int i = 0; i < gridChildrenCount; i++)
            {
                foreach (var child in grd.Children.OfType<StationEll>())
                {
                    var menuItem = child.ContextMenu.Items[0] as MenuItem;
                    if (menuItem != null && !menuItem.IsChecked)
                    {
                        child.Delete();
                        PointUse--;
                        break;
                    }
                }
            }

            var ptUse = 1;

            foreach (var child in grd.Children.OfType<StationEll>())
            {
                child.Text = ptUse.ToString(CultureInfo.InvariantCulture);
                ptUse++;
                child.Update();
            }
        }

        private void ClearOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearGrid();
                ClearGridRect();
                //grd.Children.RemoveRange(1, grd.Children.Count - 1);
                //pointUse = 0;
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }

        private void SolveOnClick(object sender, RoutedEventArgs e)
        {
            if (PointCount > PointUse)
            {
                ErrorViewer.ShowInfo("На поле добавлены не все терминальные точки.\nДобавьте еще " + (PointCount-PointUse).ToString() + " точек.");
                return;
            }

            ClearGridRect();
            var xy = new double[PointCount,2];
            var cond = new int[StationCount];
            var stName = new List<string>();
            //for (int i = 0; i < Step.GetStationCount(); i++)
            //{
            //    foreach (var pt in Step.Stations[i].GetAllPoints())
            //    {
            //        cond[i] += pt.Num * Step.Stations[i].Num;
            //    }
            //}

            int j = 0;
            foreach (var st in Step.Stations)
            {
                for (int i = 0; i < st.Num; i++)
                {
                    stName.Add(st.GetName());
                    foreach (var pt in st.GetAllPoints())
                    {
                        cond[j] += pt.Num;
                    }
                    j++;
                }
            }

            j = 0;
            foreach (var child in grd.Children.OfType<StationEll>())
            {
                xy[j, 1] = child.GetCenter().X;
                xy[j, 0] = child.GetCenter().Y;
                j++;
            }
            int info;
            double[,] c;
            int[] xyc;
            double ebest;
            double ebest2;
            KMeans2.K_MeansGenerate(xy, PointCount, 2, StationCount, cond, 100, out info, out c, out xyc, out ebest, out ebest2);

            if (info != 1)
            {
                ErrorViewer.ShowInfo("Ошибка при вычислении k-средних");
                return;
            }

            KmRes = ebest;
            KmRes2 = ebest2;
            j = 0;
            var rand = new Random();
            var colors = new List<byte[]>();
            for (int i = 0; i < StationCount; i++)
            {
                colors.Add(new byte[3]);
                rand.NextBytes(colors[i]);
            }

            for (int i = 0; i < StationCount; i++)
            {
                AddObjectToGrid(c[1, i], c[0, i], (i + 1).ToString(), false);
            }

            var rect = grd.Children.OfType<StationRect>().ToList();

            foreach (var stationRect in rect)
            {
                stationRect.Background = new SolidColorBrush(Color.FromRgb(colors[j][0], colors[j][1], colors[j][2]));
                stationRect.Text = stName[stationRect.Number - 1];
                j++;
            }

            j = 0;
            var list = grd.Children.OfType<StationEll>().ToList();
            foreach (var child in list)
            {
                child.Background = new SolidColorBrush(Color.FromRgb(colors[xyc[j]][0], colors[xyc[j]][1], colors[xyc[j]][2]));
                StanConnection.Connect(child, rect[xyc[j]]);
                j++;
            }

            
        }

        private void SettingClick(object sender, RoutedEventArgs e)
        {
            var dlg = new SettingDlg(Settings, 0);
            // UpdateTimer.Stop();
            if (dlg.ShowDialog() == true)
            {
                Settings = dlg.ReturnSetting();
                Update();
            }

            //UpdateTimer.Start(Next, grMain);
        }

        private void Update()
        {
            DataContext = null;
            DataContext = this;
        }

        private void Ima_OnMouseMove(object sender, MouseEventArgs e)
        {
            MouseX = e.GetPosition(null).X;
            MouseY = e.GetPosition(null).Y;
            Update();
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            //if (Step.GetAllPoints().Sum(pts => pts.Num) == 0)
            //{
            //    ErrorViewer.ShowInfo("Не задано количество требуемых терминальных точек");
            //    return;
            //}


            //for (int i = 0; i < Step.GetPointCount(); i++)
            //{
            //    var ptsNum = 0;
            //    if (Step.GetPointTask(i) != 0)
            //    {
            //        ptsNum += Step.Stations.Sum(st => st.GetPoint(i).Num);

            //        if (ptsNum == 0)
            //        {
            //            ErrorViewer.ShowInfo(
            //                "Решение задачи невозможно\nОтсутствуют станции, к которым можно подключить точку: " +
            //                Step.GetPoint(i).GetName());
            //            return;
            //        }
            //    }
            //}

            var rect = grd.Children.OfType<StationRect>().ToList();
            var stNames = rect.Select(stationRect => stationRect.Text + "--" + stationRect.Number.ToString()).ToList();
            var result = new gpd(Settings, stNames);

            //result.StNames.Add(); 
            if (NavigationService != null) NavigationService.Navigate(result);
        }
    }
}
