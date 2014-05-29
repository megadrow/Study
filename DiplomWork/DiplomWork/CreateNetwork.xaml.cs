using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Controls;
using DiplomWork.Dialogs;

namespace DiplomWork
{
    enum NetObjectType
    {
        TypeStation,
        TypeBus
    }
    /// <summary>
    /// Interaction logic for CreateNetwork.xaml
    /// </summary>
    public partial class CreateNetwork
    {
        public List<string> StNames { get; set; } 
        public int[,] Matrix { get; set; }

        private Mode _mode = Mode.Move;
        private NetObjectType _objectType = NetObjectType.TypeStation;
        private int _stationCount;
        private int _busCount;

        public CreateNetwork(List<string> names, int[,] mtx)
        {
            InitializeComponent();
            Matrix = mtx;
            StNames = names;
            BtnAddStation.IsChecked = true;
            DmRbAdd.IsChecked = true;
            DataContext = this;
        }

        private void Ima_OnMouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Ima_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((_mode == Mode.Move))
            {
                AddObjectToGrid(e.GetPosition(ima).X, e.GetPosition(ima).Y);
            }
        }

        private void ShowMatrix(object sender, RoutedEventArgs e)
        {
            var dlg = new MatrixShowDlg(StNames, Matrix);
            dlg.ShowDialog();
        }

        private void AddBusChecked(object sender, RoutedEventArgs e)
        {
            _objectType = NetObjectType.TypeBus;
        }

        private void AddStationChecked(object sender, RoutedEventArgs e)
        {
            _objectType = NetObjectType.TypeStation;
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
            _mode = Mode.Move;
        }

        private void DelModeChecked(object sender, RoutedEventArgs e)
        {
            var obj = grd.Children.OfType<CommonObject>().ToList();

            foreach (var commonObject in obj)
            {
                commonObject.ToDeleteMode();
            }

            _mode = Mode.Delete;
        }

        private void ConModeChecked(object sender, RoutedEventArgs e)
        {
            var obj = grd.Children.OfType<CommonObject>().ToList();

            foreach (var commonObject in obj)
            {
                commonObject.ToConnectMode();
            }

            _mode = Mode.Connect;
        }

        private void UnconModeChecked(object sender, RoutedEventArgs e)
        {
            var obj = grd.Children.OfType<CommonObject>().ToList();

            foreach (var commonObject in obj)
            {
                commonObject.ToDisconnectMode();
            }

            _mode = Mode.Disconnect;
        }

        private void AddObjectToGrid(double left, double top)
        {
            CommonObject obj;
            switch (_objectType)
            {
                case NetObjectType.TypeStation:
                    {
                        _stationCount++;
                        obj = new NetStation(left, top) { Number = _stationCount };
                    } break;
                case NetObjectType.TypeBus:
                    {
                        _busCount++;
                        obj = new NetBus(left, top) { Number = _busCount };
                    } break;
                default:
                    {
                        return;
                    }
            }
            obj.MouseLeftButtonDown += ObjectOnLeftMouseDown;
            var context = new ContextMenu();
            var mi = new MenuItem { Header = "Удалить" };
            mi.Click += DeletePopupClick;
            context.Items.Add(mi);

            mi = new MenuItem { Header = "Удалить соединения" };
            mi.Click += DeleteConPopupClick;
            context.Items.Add(mi);

            //mi = new MenuItem { Header = "Свойства" };
            ////if (circle.GetType() == typeof(StationEll))
            ////{
            //mi.Click += PropertyPopupClick;
            ////}

            //context.Items.Add(mi);

            obj.ContextMenu = context;

            grd.Children.Add(obj);
            //if ((obj as GpdData) != null)
            //    (obj as GpdData).SubscribeToGrid();
        }

        private void CheckValidationOfObject(CommonObject obj)
        {
            if ((obj as NetStation) != null)
            {
                foreach (var data in grd.Children.OfType<NetStation>().ToList().Where(data => data.Number > obj.Number))
                {
                    data.Number--;
                    data.Update();
                }
                _stationCount--;
            }

            if ((obj as NetBus) != null)
            {
                foreach (var data in grd.Children.OfType<NetBus>().ToList().Where(data => data.Number > obj.Number))
                {
                    data.Number--;
                    data.Update();
                }
                _busCount--;
            }

            Update();
        }

        private void ObjectOnLeftMouseDown(object sender, MouseEventArgs e)
        {
            var commonObject = sender as CommonObject;
            if (commonObject != null && commonObject.mode == Mode.Delete)
            {
                CheckValidationOfObject(commonObject);
            }
        }

        private void DeletePopupClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = (sender as MenuItem);
                var item2 = LogicalTreeHelper.GetParent(item);
                var item3 = LogicalTreeHelper.GetParent(item2) as Popup;
                var commonObject = item3.PlacementTarget as CommonObject;
                if (commonObject == null) return;
                //var list = commonObject.Connection.Select(connection =>
                //    Equals((item3.PlacementTarget as CommonObject),
                //    connection.GetStartObject()) ?
                //    connection.GetEndObject() :
                //    connection.GetStartObject()).ToList();
                commonObject.Delete();
                CheckValidationOfObject(commonObject);
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
                var commonObject = item3.PlacementTarget as CommonObject;
                if (commonObject == null) return;
                commonObject.Disconnect();
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            var busList = grd.Children.OfType<NetBus>().ToList();
            var list = grd.Children.OfType<CommonObject>().ToList();
            var busCount = grd.Children.OfType<NetBus>().Count();
            if (busCount == 0)
            {
                ErrorViewer.ShowError("На поле не добавлена ни одна магистраль");
                return;
            }

            //Проверка связности графа
            foreach (var commonObject in list)
            {
                commonObject.TmpField = 1;
            }
            list[0].TmpField++;
            while (list.Count(commonObject => commonObject.TmpField == 2) > 0)
            {
                foreach (var commonObject in list.Where(commonObject => commonObject.TmpField == 2))
                {
                    foreach (var stanConnection in commonObject.Connection)
                    {
                        if (Equals(stanConnection.GetStartObject(), commonObject) &&
                            (stanConnection.GetEndObject().TmpField == 1))
                        {
                            stanConnection.GetEndObject().TmpField++;
                        }
                        else if (Equals(stanConnection.GetEndObject(), commonObject) &&
                                 (stanConnection.GetStartObject().TmpField == 1))
                        {
                            stanConnection.GetStartObject().TmpField++;
                        }
                    }
                    commonObject.TmpField++;
                }
            }

            if (list.Count(commonObject => commonObject.TmpField == 1) > 0)
            {
                ErrorViewer.ShowError("Граф не связный!\nПроверьте, чтобы все элементы были объеденены в единый граф");
                return;
            }
            //конец проверки на связность графа

            //Заполнение матрицы смежности магистралей
            var busAdj = new int[busCount, busCount];
            foreach (var bus in busList)
            {
                var busIndex = busList.IndexOf(bus);
                foreach (var connection in bus.Connection)
                {
                    if (Equals(connection.GetStartObject(), bus))
                    {
                        if ((connection.GetEndObject() as NetBus) != null)
                        {
                            busAdj[busIndex, busList.IndexOf(connection.GetEndObject() as NetBus)] =
                                busAdj[busList.IndexOf(connection.GetEndObject() as NetBus), busIndex] = 1;
                        }
                        else if ((connection.GetEndObject() as NetStation) != null)
                        {
                            foreach (var stCon in connection.GetEndObject().Connection)
                            {
                                if (Equals(stCon.GetStartObject(), connection.GetEndObject()))
                                {
                                    if (((stCon.GetEndObject() as NetBus) != null) && !Equals(stCon.GetEndObject(), bus))
                                    {
                                        busAdj[busIndex, busList.IndexOf(stCon.GetEndObject() as NetBus)] =
                                            busAdj[busList.IndexOf(stCon.GetEndObject() as NetBus), busIndex] = 1;
                                    }
                                }
                                else if (Equals(stCon.GetEndObject(), connection.GetEndObject()))
                                {
                                    if (((stCon.GetStartObject() as NetBus) != null) && !Equals(stCon.GetStartObject(), bus))
                                    {
                                        busAdj[busIndex, busList.IndexOf(stCon.GetStartObject() as NetBus)] =
                                            busAdj[busList.IndexOf(stCon.GetStartObject() as NetBus), busIndex] = 1;
                                    }
                                }
                            }
                        }
                    }
                    else if (Equals(connection.GetEndObject(), bus))
                    {
                        if ((connection.GetStartObject() as NetBus) != null)
                        {
                            busAdj[busIndex, busList.IndexOf(connection.GetStartObject() as NetBus)] =
                                busAdj[busList.IndexOf(connection.GetStartObject() as NetBus), busIndex] = 1;
                        }
                        else if ((connection.GetStartObject() as NetStation) != null)
                        {
                            foreach (var stCon in connection.GetStartObject().Connection)
                            {
                                if (Equals(stCon.GetStartObject(), connection.GetStartObject()))
                                {
                                    if (((stCon.GetEndObject() as NetBus) != null) && !Equals(stCon.GetEndObject(), bus))
                                    {
                                        busAdj[busIndex, busList.IndexOf(stCon.GetEndObject() as NetBus)] =
                                            busAdj[busList.IndexOf(stCon.GetEndObject() as NetBus), busIndex] = 1;
                                    }
                                }
                                else if (Equals(stCon.GetEndObject(), connection.GetStartObject()))
                                {
                                    if (((stCon.GetStartObject() as NetBus) != null) && !Equals(stCon.GetStartObject(), bus))
                                    {
                                        busAdj[busIndex, busList.IndexOf(stCon.GetStartObject() as NetBus)] =
                                            busAdj[busList.IndexOf(stCon.GetStartObject() as NetBus), busIndex] = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //конец заполнения матрицы смежности магистралей
            var g = 1;
        }
    }
}
