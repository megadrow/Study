using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Controls;

namespace DiplomWork
{
    /// <summary>
    /// Interaction logic for gpd.xaml
    /// </summary>
    public partial class gpd : Page
    {
        private int dataCount = 0;
        private int moduleCount = 0;
        private int drawmode;
        private Mode mode = Mode.Move;

        private Color[] colors =
            {
                Colors.Blue, Colors.BlueViolet, Colors.Brown, Colors.BurlyWood, Colors.CadetBlue, Colors.Chartreuse,
                Colors.Chocolate,
                Colors.Coral, Colors.CornflowerBlue, Colors.Crimson, Colors.Cyan, Colors.DarkCyan, Colors.DarkGoldenrod,
                Colors.DarkGray,
                Colors.DarkGreen, Colors.DarkKhaki, Colors.DarkMagenta, Colors.DarkOrange, Colors.Yellow,
                Colors.DarkSalmon
            };
        public Settings Settings { get; set; }

        public List<string> StNames { get; set; } 

        public gpd(Settings settings, List<string> stantionList = null)
        {
            InitializeComponent();
            StNames = new List<string>();
            if ((stantionList != null) && (stantionList.Count > 0))
            {
                ComboStationType.IsEnabled = true;
                foreach (var stanName in stantionList)
                {
                    StNames.Add(stanName);
                    var cbItem = new ComboBoxItem();
                    cbItem.Content = stanName;
                    ComboStationType.Items.Add(cbItem);
                }
                ComboStationType.SelectedIndex = 0;
            }
            RbDrawMode.IsChecked = true;
            ComboDrawType.SelectedIndex = 0;
            drawmode = 0;
            DmRbAdd.IsChecked = true;
            Settings = settings;
            grd.Width = 2000;
            DataContext = this;
        }

        private void DrawModeChacked(object sender, RoutedEventArgs e)
        {
            DrawModeToolbar.Visibility = Visibility.Visible;
            ThreadModeToolbar.Visibility = Visibility.Hidden;
            StationModeToolbar.Visibility = Visibility.Hidden;
            var dataList = grd.Children.OfType<GpdData>().ToList();
            drawmode = 0;
            foreach (var gpdData in dataList)
            {
                gpdData.Background = new SolidColorBrush(Colors.White);
            }
        }

        private void ThreadModeChacked(object sender, RoutedEventArgs e)
        {
            DrawModeToolbar.Visibility = Visibility.Hidden;
            ThreadModeToolbar.Visibility = Visibility.Visible;
            StationModeToolbar.Visibility = Visibility.Hidden;
            var dataList = grd.Children.OfType<GpdData>().ToList();
            drawmode = 1;
            foreach (var gpdData in dataList)
            {
                gpdData.Background = gpdData.Process.Ind == -1 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(gpdData.Process.Clr);
            }
        }

        private void StationModeChacked(object sender, RoutedEventArgs e)
        {
            DrawModeToolbar.Visibility = Visibility.Hidden;
            ThreadModeToolbar.Visibility = Visibility.Hidden;
            StationModeToolbar.Visibility = Visibility.Visible;
            var dataList = grd.Children.OfType<GpdData>().ToList();
            drawmode = 2;
            foreach (var gpdData in dataList)
            {
                gpdData.Background = gpdData.Stan.Ind == -1 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(gpdData.Stan.Clr);
            }
        }

        private void AddObjectToGrid(double left, double top, bool ellipse = true)
        {
            CommonObject obj;
            switch (ComboDrawType.SelectedIndex)
            {
                case 0:
                    {
                        dataCount++;
                        obj = new GpdData(left, top) {Number = dataCount};
                    } break;
                case 1:
                    {
                        moduleCount++;
                        obj = new GpdModule(left, top) {Number = moduleCount};
                    } break;
                default:
                    {
                        return;
                    }
            }
            obj.MouseLeftButtonDown += ObjectOnLeftMouseDown;
            var context = new ContextMenu();
            var mi = new MenuItem();
            mi.Header = "Удалить";
            mi.Click += DeletePopupClick;
            context.Items.Add(mi);

            mi = new MenuItem();
            mi.Header = "Удалить соединения";
            mi.Click += DeleteConPopupClick;
            context.Items.Add(mi);

            mi = new MenuItem();
            mi.Header = "Свойства";
            //if (circle.GetType() == typeof(StationEll))
            //{
            mi.Click += PropertyPopupClick;
            //}

            context.Items.Add(mi);

            obj.ContextMenu = context;

            grd.Children.Add(obj);
            if((obj as GpdData) != null)
                (obj as GpdData).SubscribeToGrid();
        }

        private void PropertyPopupClick(object sender, RoutedEventArgs e)
        {
            try
            {
                PointInfo dlg;
                var item = (sender as MenuItem);

                var item2 = LogicalTreeHelper.GetParent(item);
                var item3 = LogicalTreeHelper.GetParent(item2) as Popup;
                var item4 = (item3.PlacementTarget as CommonObject);
                if ((item4 as GpdData) != null)
                {
                    dlg = new PointInfo(1);
                }
                else
                {
                    dlg = new PointInfo(2);
                }

                dlg.DataContext = (item3.PlacementTarget as CommonObject);
                //dlg.PIName.Text = (item3.PlacementTarget as CommonObject).Text;

                dlg.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }

        private void DeletePopupClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = (sender as MenuItem);
                var item2 = LogicalTreeHelper.GetParent(item);
                var item3 = LogicalTreeHelper.GetParent(item2) as Popup;
                var item4 = (item3.PlacementTarget as CommonObject);
                item4.Delete();
                CheckValidationOfObject(item4);
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }

        private void DeleteConPopupClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = (sender as MenuItem);
                var item2 = LogicalTreeHelper.GetParent(item);
                var item3 = LogicalTreeHelper.GetParent(item2) as Popup;
                (item3.PlacementTarget as CommonObject).Disconnect();
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }

        private void CheckValidationOfObject(CommonObject obj)
        {
            if ((obj as GpdData) != null)
            {
                foreach (var data in grd.Children.OfType<GpdData>().ToList())
                {
                    if (data.Number > obj.Number)
                    {
                        data.Number--;
                        data.Update();
                    }
                }
                dataCount--;
            }

            if ((obj as GpdModule) != null)
            {
                foreach (var data in grd.Children.OfType<GpdModule>().ToList())
                {
                    if (data.Number > obj.Number)
                    {
                        data.Number--;
                        data.Update();
                    }
                }
                moduleCount--;
            }

            Update();
        }

        private void ObjectOnLeftMouseDown(object sender, MouseEventArgs e)
        {
            var commonObject = sender as CommonObject;
            if (commonObject != null && commonObject.mode == Mode.Delete && drawmode == 0)
            {
                CheckValidationOfObject(commonObject);
            }
            var dataObject = sender as GpdData;
            if (dataObject != null)
            {
                if (drawmode == 2)
                {
                    dataObject.Stan.Str = ComboStationType.SelectedItem.ToString();
                    dataObject.Stan.Ind = ComboStationType.SelectedIndex;
                    dataObject.Stan.Clr = colors[ComboStationType.SelectedIndex % 20];

                    dataObject.Background = new SolidColorBrush(dataObject.Stan.Clr);
                }

                if (drawmode == 1)
                {
                    dataObject.Process.Str = ComboProcessType.SelectedItem.ToString();
                    dataObject.Process.Ind = ComboProcessType.SelectedIndex;
                    dataObject.Process.Clr = colors[ComboProcessType.SelectedIndex % 20];

                    dataObject.Background = new SolidColorBrush(dataObject.Process.Clr);
                }
            }


        }

        private void Ima_OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
        }

        private void Ima_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((mode == Mode.Move) && (drawmode == 0))
            {
                AddObjectToGrid(e.GetPosition(ima).X, e.GetPosition(ima).Y);
            }
        }

        private void SettingClick(object sender, RoutedEventArgs e)
        {
            var dlg = new SettingDlg(Settings, 1);
            // UpdateTimer.Stop();
            if (dlg.ShowDialog() == true)
            {
                Settings = dlg.ReturnSetting();
                Update();
            }
        }

        private void Update()
        {
            DataContext = null;
            DataContext = this;
        }

        private void AddModeChecked(object sender, RoutedEventArgs e)
        {
            var obj = grd.Children.OfType<CommonObject>().ToList();

            foreach (var commonObject in obj)
            {
                commonObject.ToMoveMode();
            }
            mode = Mode.Move;
        }

        private void DelModeChecked(object sender, RoutedEventArgs e)
        {
            var obj = grd.Children.OfType<CommonObject>().ToList();

            foreach (var commonObject in obj)
            {
                commonObject.ToDeleteMode();
            }

            mode = Mode.Delete;
        }

        private void ConModeChecked(object sender, RoutedEventArgs e)
        {
            var obj = grd.Children.OfType<CommonObject>().ToList();

            foreach (var commonObject in obj)
            {
                commonObject.ToConnectMode();
            }

            mode = Mode.Connect;
        }

        private void UnconModeChecked(object sender, RoutedEventArgs e)
        {
            var obj = grd.Children.OfType<CommonObject>().ToList();

            foreach (var commonObject in obj)
            {
                commonObject.ToDisconnectMode();
            }

            mode = Mode.Disconnect;
        }

        private void ProcessEdit(object sender, RoutedEventArgs e)
        {
            var item = new ComboBoxItem();
            ComboProcessType.Items.Add(item);
            item.Content = "Process " + (ComboProcessType.Items.Count - 1).ToString();
            ComboProcessType.IsEnabled = true;
            ComboProcessType.SelectedIndex = ComboProcessType.Items.Count - 1;
        }
    }
}
