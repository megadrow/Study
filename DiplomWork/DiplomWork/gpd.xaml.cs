using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Controls;
using DiplomWork.Dialogs;

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

        //public List<string> StNames { get; set; }
         

        public gpd(Settings settings, List<string> stantionList = null)
        {
            InitializeComponent();
            //StNames = new List<string>();
            if ((stantionList != null) && (stantionList.Count > 0))
            {
                ComboStationType.IsEnabled = true;
                foreach (var stanName in stantionList)
                {
                    GpdData.StanNames.Add(stanName);
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
            var obj = grd.Children.OfType<CommonObject>().ToList();

            foreach (var commonObject in obj)
            {
                commonObject.ToMoveMode();
            }
            mode = Mode.Move;

            DrawModeToolbar.Visibility = Visibility.Visible;
            ThreadModeToolbar.Visibility = Visibility.Hidden;
            StationModeToolbar.Visibility = Visibility.Hidden;
            var dataList = grd.Children.OfType<CommonObject>().ToList();
            drawmode = 0;
            foreach (var gpdData in dataList)
            {
                if ((gpdData as GpdData) != null)
                {
                    gpdData.Background = new SolidColorBrush(Colors.White);
                }
                else if ((gpdData as GpdModule) != null)
                {
                    gpdData.Background = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void ThreadModeChacked(object sender, RoutedEventArgs e)
        {
            var obj = grd.Children.OfType<CommonObject>().ToList();

            foreach (var commonObject in obj)
            {
                commonObject.ToMoveMode();
            }
            mode = Mode.Move;

            DrawModeToolbar.Visibility = Visibility.Hidden;
            ThreadModeToolbar.Visibility = Visibility.Visible;
            StationModeToolbar.Visibility = Visibility.Hidden;
            var dataList = grd.Children.OfType<CommonObject>().ToList();
            drawmode = 1;
            foreach (var gpdData in dataList)
            {
                if ((gpdData as GpdData) != null)
                {
                    gpdData.Background = (gpdData as GpdData).Process.Ind == -1 ? 
                        new SolidColorBrush(Colors.White) :
                        new SolidColorBrush((gpdData as GpdData).Process.Clr);
                }
                else if ((gpdData as GpdModule) != null)
                {
                    gpdData.Background = (gpdData as GpdModule).Process.Ind == -1 ?
                        new SolidColorBrush(Colors.Black) :
                        new SolidColorBrush((gpdData as GpdModule).Process.Clr);
                }
            }
        }

        private void StationModeChacked(object sender, RoutedEventArgs e)
        {
            var obj = grd.Children.OfType<CommonObject>().ToList();

            foreach (var commonObject in obj)
            {
                commonObject.ToMoveMode();
            }
            mode = Mode.Move;
            DmRbAdd.IsChecked = true;

            DrawModeToolbar.Visibility = Visibility.Hidden;
            ThreadModeToolbar.Visibility = Visibility.Hidden;
            StationModeToolbar.Visibility = Visibility.Visible;
            var dataList = grd.Children.OfType<CommonObject>().ToList();
            drawmode = 2;
            foreach (var gpdData in dataList)
            {
                if ((gpdData as GpdData) != null)
                {
                    gpdData.Background = (gpdData as GpdData).Stan.Ind == -1 ?
                        new SolidColorBrush(Colors.White) :
                        new SolidColorBrush((gpdData as GpdData).Stan.Clr);
                }
                else if ((gpdData as GpdModule) != null)
                {
                    gpdData.Background = (gpdData as GpdModule).Stan.Ind == -1 ?
                        new SolidColorBrush(Colors.Black) :
                        new SolidColorBrush((gpdData as GpdModule).Stan.Clr);
                }
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
            var mi = new MenuItem {Header = "Удалить"};
            mi.Click += DeletePopupClick;
            context.Items.Add(mi);

            mi = new MenuItem {Header = "Удалить соединения"};
            mi.Click += DeleteConPopupClick;
            context.Items.Add(mi);

            mi = new MenuItem {Header = "Свойства"};
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
                //PointInfo dlg;
                var item = (sender as MenuItem);

                var item2 = LogicalTreeHelper.GetParent(item);
                var item3 = LogicalTreeHelper.GetParent(item2) as Popup;
                var item4 = (item3.PlacementTarget as CommonObject);
                if ((item4 as GpdData) != null)
                {
                    var dlg = new GpdDataInfo {DataContext = item4};

                    //dlg.PIName.Text = (item3.PlacementTarget as CommonObject).Text;

                    if (dlg.ShowDialog() == true)
                    {
                        if (drawmode == 1)
                        {
                            item4.Background = (item4 as GpdData).Process.Ind == -1 ? 
                                new SolidColorBrush(Colors.White) : 
                                new SolidColorBrush((item4 as GpdData).Process.Clr);
                        }
                        if (drawmode == 2)
                        {
                            item4.Background = (item4 as GpdData).Stan.Ind == -1 ? 
                                new SolidColorBrush(Colors.White) : 
                                new SolidColorBrush((item4 as GpdData).Stan.Clr);
                        }
                    }
                }
                if ((item4 as GpdModule) != null)
                {
                    var dlg = new GpdModuleInfo {DataContext = item4};

                    //dlg.PIName.Text = (item3.PlacementTarget as CommonObject).Text;

                    if (dlg.ShowDialog() == true)
                    {
                        if (drawmode == 1)
                        {
                            item4.Background = (item4 as GpdModule).Process.Ind == -1 ? 
                                new SolidColorBrush(Colors.Black) : 
                                new SolidColorBrush((item4 as GpdModule).Process.Clr);
                        }
                        if (drawmode == 2)
                        {
                            item4.Background = (item4 as GpdModule).Stan.Ind == -1 ? 
                                new SolidColorBrush(Colors.Black) : 
                                new SolidColorBrush((item4 as GpdModule).Stan.Clr);
                        }
                    }
                }
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
            var moduleObject = sender as GpdModule;
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
                    if (ComboProcessType.Items.Count == 0) return;
                    dataObject.Process.Str = ComboProcessType.SelectedItem.ToString();
                    dataObject.Process.Ind = ComboProcessType.SelectedIndex;
                    dataObject.Process.Clr = colors[ComboProcessType.SelectedIndex % 20];

                    dataObject.Background = new SolidColorBrush(dataObject.Process.Clr);
                }
            }
            else if (moduleObject != null)
            {
                if (drawmode == 2)
                {

                    moduleObject.Stan.Str = ComboStationType.SelectedItem.ToString();
                    moduleObject.Stan.Ind = ComboStationType.SelectedIndex;
                    moduleObject.Stan.Clr = colors[ComboStationType.SelectedIndex % 20];

                    moduleObject.Background = new SolidColorBrush(moduleObject.Stan.Clr);
                }

                if (drawmode == 1)
                {
                    if (ComboProcessType.Items.Count == 0) return;
                    moduleObject.Process.Str = ComboProcessType.SelectedItem.ToString();
                    moduleObject.Process.Ind = ComboProcessType.SelectedIndex;
                    moduleObject.Process.Clr = colors[ComboProcessType.SelectedIndex % 20];

                    moduleObject.Background = new SolidColorBrush(moduleObject.Process.Clr);
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

        private void ModuleSettingClick(object sender, RoutedEventArgs e)
        {
            var dlg = new ModuleSetting();
            // UpdateTimer.Stop();
            var moduleList = grd.Children.OfType<GpdModule>().ToList();
            dlg.DataContext = moduleList;
            dlg.ShowDialog();
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
            GpdData.ProcNames.Add("Process " + (ComboProcessType.Items.Count - 1).ToString());
            item.Content = GpdData.ProcNames.Last();
            //item.Background = new SolidColorBrush(colors[(ComboProcessType.Items.Count - 1) % 20]);
            //ComboProcessType.Background = new SolidColorBrush(colors[(ComboProcessType.Items.Count - 1) % 20]);

            ComboProcessType.IsEnabled = true;
            ComboProcessType.SelectedIndex = ComboProcessType.Items.Count - 1;
        }

        private void GpdAlignmentClick(object sender, RoutedEventArgs e)
        {
            var obj = grd.Children.OfType<CommonObject>().ToList();
            const int parallelHeight = 50;
            int freeCount;
            var listLvl = new List<List<CommonObject>>();

            foreach (var commonObject in obj)
            {
                var data = commonObject as GpdData;
                if (data != null)
                {
                    data.CalcOutClear();
                }
                var dataM = commonObject as GpdModule;
                if (dataM != null)
                {
                    dataM.CalcOutClear();
                }
            }
            //foreach (var commonObject2 in obj)
            do
            {
                foreach (var commonObject in obj)
                {
                    var data = commonObject as GpdData;
                    if (data != null)
                    {
                        data.CalcOutCount();
                    }
                    var dataM = commonObject as GpdModule;
                    if (dataM != null)
                    {
                        dataM.CalcOutCount();
                    }
                    //ddd++;
                    //commonObject.Left = ddd*100;
                    //commonObject.Margin = new Thickness(commonObject.Left, commonObject.Top, 0, 0);
                    //commonObject.SetPosition(ddd * 100, commonObject.Top);

                }
                listLvl.Add(new List<CommonObject>());
                foreach (var commonObject in obj)
                {
                    var data = commonObject as GpdData;
                    if (data != null)
                    {
                        if ((data.Connection.Count > 0) && (!data.IsUse) && (data.OutConCount == 0))
                        {
                            data.IsUse = true;
                            listLvl[listLvl.Count - 1].Add(data);
                        }
                    }
                    var dataM = commonObject as GpdModule;
                    if (dataM != null)
                    {
                        if ((dataM.Connection.Count > 0) && (!dataM.IsUse) && (dataM.OutConCount == 0))
                        {
                            dataM.IsUse = true;
                            listLvl[listLvl.Count - 1].Add(dataM);
                        }
                    }
                }
                freeCount = 0;
                foreach (var commonObject in obj)
                {
                    var data = commonObject as GpdData;
                    if (data != null)
                    {
                        if (!data.IsUse)
                        {
                            freeCount++;
                        }
                    }
                    var dataM = commonObject as GpdModule;
                    if (dataM != null)
                    {
                        if (!dataM.IsUse) freeCount++;
                    }
                }
            } while (freeCount > 0);
            listLvl.Reverse();
            for (int i = 0; i < listLvl.Count; i++)
            {
                foreach (var list in listLvl[i])
                {
                    list.SetPosition(list.Left, parallelHeight*i);
                }
            }
            Update();
        }
    }
}
