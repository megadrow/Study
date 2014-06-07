using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using Controls;
using DiplomWork.Dialogs;

namespace DiplomWork
{
    enum NetObjectType
    {
        TypeStation,
        TypeBus
    }

    class NetPath
    {
        public List<bool> Mask = new List<bool>();
        public List<int> Path = new List<int>();
        public string Name = "";
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
                if (grd.Children.OfType<NetStation>().Count() < StNames.Count)
                {
                    AddObjectToGrid(e.GetPosition(ima).X, e.GetPosition(ima).Y);
                }
                else
                {
                    ErrorViewer.ShowError("Количество станций не может превышать " + StNames.Count.ToString());
                }
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
            var stList = grd.Children.OfType<NetStation>().ToList();
            var list = grd.Children.OfType<CommonObject>().ToList();
            var busCount = grd.Children.OfType<NetBus>().Count();
            var stCount = list.OfType<NetStation>().Count();
            if (busCount == 0)
            {
                ErrorViewer.ShowError("На поле не добавлена ни одна магистраль");
                return;
            }

            if (stCount == 0)
            {
                ErrorViewer.ShowError("На поле не добавлена ни одна станция");
                return;
            }

            if (stCount != StNames.Count)
            {
                ErrorViewer.ShowError("Количество станций должно быть " + StNames.Count.ToString());
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
                                    if (((stCon.GetEndObject() as NetBus) != null) && 
                                        !Equals(stCon.GetEndObject(), bus))
                                    {
                                        busAdj[busIndex, busList.IndexOf(stCon.GetEndObject() as NetBus)] =
                                            busAdj[busList.IndexOf(stCon.GetEndObject() as NetBus), busIndex] = 1;
                                    }
                                }
                                else if (Equals(stCon.GetEndObject(), connection.GetStartObject()))
                                {
                                    if (((stCon.GetStartObject() as NetBus) != null) && 
                                        !Equals(stCon.GetStartObject(), bus))
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

            var tmp = new bool[busCount];
            var lasttmp = new bool[busCount];
            var tmpP = new int[busCount];
            var lasttmpP = new int[busCount];
            var mask = new List<List<bool>>();
            var mPaths = new List<List<int>>();
            var lis = new List<int>();
            var c = new List<NetPath>();
         

            for (int i = 0; i < busCount; i++)
            {
                tmp[i] = false;
                lasttmp[i] = false;
                tmpP[i] = 0;
                lasttmpP[i] = 0;
            }

            for (var i = 0; i < busCount; i++)
            {
                for (var j = 0; j < busCount; j++)
                {
                    tmp[j] = false;
                    tmpP[j] = 0;
                }
                tmp[i] = true;
                tmpP[i] = 1;
                mask.Add(new List<bool>());
                mPaths.Add(new List<int>());
                var k = mask.Count;

                for (int j = 0; j < busCount; j++)
                {
                    mask[k - 1].Add(tmp[j]);
                    mPaths[k - 1].Add(tmpP[j]);
                }
            }

            var mmm = false;
            int a;
            for (var i = 0; i < busCount; i++)
            {
                var noUse = i;
                for (int j = 0; j < busCount; j++)
                {
                    tmp[j] = false;
                    lasttmp[j] = false;
                    tmpP[j] = 0;
                    lasttmpP[j] = 0;
                }
                tmp[i] = true;
                lasttmp[i] = true;
                tmpP[i] = 1;
                lasttmpP[i] = 1;
                var q = 0;
                var numPa = 1;
                while (true)
                {
                    if (busAdj[i, q] == 1)
                    {
                        for (int j = 0; j < busCount; j++)
                        {
                            lasttmp[j] = tmp[j];
                            lasttmpP[j] = tmpP[j];
                        }
                        tmp[q] = true;
                        ++numPa;
                        tmpP[q] = numPa;
                        foreach (var t in mask)
                        {
                            mmm = t.Where((t1, r) => tmp[r] != t1).Any();
                            if (mmm == false) { break; }
                        }
                        if (mmm)
                        {
                            mask.Add(new List<bool>());
                            mPaths.Add(new List<int>());
                            int k = mask.Count;
                            for (int j = 0; j < busCount; j++)
                            {
                                mask[k - 1].Add(tmp[j]);
                                mPaths[k - 1].Add(tmpP[j]);
                            }
                            mmm = false;
                            lis.Add(i);
                            lis.Add(q);
                            i = q;
                            q = -1;
                        }
                        else
                        {
                            for (int j = 0; j < busCount; j++)
                            {
                                tmp[j] = lasttmp[j];
                                tmpP[j] = lasttmpP[j];
                            }
                            numPa--;
                            if (numPa < 1) numPa = 1;

                        }
                    }

                    if (q == busCount - 1)
                    {
                        a = lis.Count;
                        if (a != 0)
                        {
                            i = lis[a - 2];
                            q = lis[a - 1] - 1;
                            tmp[q + 1] = false;
                            tmpP[q + 1] = 0;
                            numPa--;
                            if (numPa < 1) { numPa = 1; }
                            //lis.RemoveRange(lis.Count - 2, lis.Count - 1);
                            lis.RemoveAt(lis.Count - 1);
                            lis.RemoveAt(lis.Count - 1);
                            //lis.erase(lis.end() - 2, lis.end());
                        }
                        else
                        {
                            if (a == 0)
                            {
                                break;
                            }
                        }

                    }
                    q++;
                }
                i = noUse;
            }

            a = mask.Count;

            for (int i = 0; i < a; i++)
            {
                var zong = new NetPath();
                
                for (int j = 0; j < busCount; j++)
                {
                    zong.Mask.Add(mask[i][j]);
                    if (mask[i][j])
                    {
                        zong.Name = zong.Name + (j + 1).ToString(CultureInfo.InvariantCulture);
                    }
                }
                c.Add(zong);
            }

            var newMatrixes = new int[a, 4];
            for (int i = 0; i < a; i++) {
                newMatrixes[i, 0] = i;
                newMatrixes[i, 1] = 0;
                var gg = new int[2];
                gg[0] = 0;
                gg[1] = 0;
                for (int j = 0; j < mask[i].Count; j++) {
                    if (mPaths[i][j] == 1) {
                        newMatrixes[i, 2] = j+1;
                        if (gg[0] < 1) {gg[0] = 1; gg[1] = j+1;}
                        newMatrixes[i, 1]++;
                        }
                    else {
                        if (mPaths[i][j] != 0)  {
                                newMatrixes[i, 1]++;
                                if (gg[0] < mPaths[i][j]) {
                                        gg[0] = mPaths[i][j]; gg[1] = j+1;
                                        }
                                }
                        }
                }
                if (gg[0] != 1)
                        newMatrixes[i, 3] = gg[1];
                else
                        newMatrixes[i, 3] = 0;

            }

            var conflictMatrix = new int[a,a];
            for (int i = 0; i < a; i++)
            {
                for (int j = 0; j < a; j++)
                {
                    if (i == j) { conflictMatrix[i, j] = 0; }
                    else
                    {
                        conflictMatrix[i, j] = 0;
                        for (int z = 0; z < busCount; z++)
                        {
                            if (mask[i][z] && mask[j][z])
                            {
                                conflictMatrix[i, j] = 1;
                                break;
                            }
                        }
                    }
                }
            }


            int xxTmp = 0;
            for (int i = 1; i < stCount; i++)
            {
                xxTmp += i;
            }
            var matHlp = new int[xxTmp,4];
            var matHlp2 = new int[xxTmp, 4];
            var matHlp3 = new int[xxTmp, 4];

            xxTmp = 0;
            for (int i = 1; i < stCount; i++)
            {
                for (int j = i + 1; j <= stCount; j++)
                {
                    int ind = 0;
                    int weigh = a + 1; ;
                    matHlp[xxTmp, 0] = Matrix[i - 1, j - 1];//StrToInt(grd->Cells[i, j]);
                    for (int k = 0; k < a; k++)
                    {
                        if(newMatrixes[k, 1] >= weigh) continue;
                        if (newMatrixes[k, 1] == 1)
                        {
                            var finded = 0;
                            var bus = busList[newMatrixes[k, 2] - 1];
                            foreach (var connection in bus.Connection)
                            {
                                if (Equals(connection.GetStartObject(), bus))
                                {
                                    if ((connection.GetEndObject() as NetStation) != null)
                                    {
                                        if (Equals(connection.GetEndObject(), stList[i - 1]) ||
                                            Equals(connection.GetEndObject(), stList[j - 1]))
                                        {
                                            finded++;
                                            if(finded == 2) break;
                                        }
                                    }
                                }
                                else if (Equals(connection.GetEndObject(), bus))
                                {
                                    if ((connection.GetStartObject() as NetStation) != null)
                                    {
                                        if (Equals(connection.GetStartObject(), stList[i - 1]) ||
                                            Equals(connection.GetStartObject(), stList[j - 1]))
                                        {
                                            finded++;
                                            if (finded == 2) break;
                                        }
                                    }
                                }
                            }
                            if (finded == 2)
                            {
                                ind = k;
                                break;
                            }
                        }
                        else if (newMatrixes[k, 1] > 1)
                        {
                            int finded = 0;
                            var bus = busList[newMatrixes[k, 2] - 1];
                            foreach (var connection in bus.Connection)
                            {
                                if (Equals(connection.GetStartObject(), bus))
                                {
                                    if ((connection.GetEndObject() as NetStation) != null)
                                    {
                                        if (Equals(connection.GetEndObject(), stList[i - 1]) ||
                                            Equals(connection.GetEndObject(), stList[j - 1]))
                                        {
                                            finded++;
                                            break;
                                        }
                                    }
                                }
                                else if (Equals(connection.GetEndObject(), bus))
                                {

                                    if ((connection.GetStartObject() as NetStation) != null)
                                    {
                                        if (Equals(connection.GetStartObject(), stList[i - 1]) ||
                                            Equals(connection.GetStartObject(), stList[j - 1]))
                                        {
                                            finded++;
                                            break;
                                        }
                                    }
                                }
                            }
                            bus = busList[newMatrixes[k, 3] - 1];
                            foreach (var connection in bus.Connection)
                            {
                                if (Equals(connection.GetStartObject(), bus))
                                {
                                    if ((connection.GetEndObject() as NetStation) != null)
                                    {
                                        if (Equals(connection.GetEndObject(), stList[i - 1]) ||
                                            Equals(connection.GetEndObject(), stList[j - 1]))
                                        {
                                            finded++;
                                            break;
                                        }
                                    }
                                }
                                else if (Equals(connection.GetEndObject(), bus))
                                {

                                    if ((connection.GetStartObject() as NetStation) != null)
                                    {
                                        if (Equals(connection.GetStartObject(), stList[i - 1]) ||
                                            Equals(connection.GetStartObject(), stList[j - 1]))
                                        {
                                            finded++;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (finded == 2)
                            {
                                if (weigh > newMatrixes[k, 1])
                                {
                                    ind = k;
                                    weigh = newMatrixes[k, 1];
                                }
                            }
                        }
                        
                    }
                    matHlp[xxTmp, 1] = ind;
                    matHlp[xxTmp, 2] = i;
                    matHlp[xxTmp, 3] = j;
                    xxTmp++;
                }
            }

            var intSwap = new int[4];

            for (var i = 0; i < xxTmp - 1; i++) {
                for (var j = i + 1; j < xxTmp; j++) {
                    if (matHlp[i, 0] < matHlp[j, 0])
                    {
                        intSwap[0] = matHlp[i, 0];
                        intSwap[1] = matHlp[i, 1];
                        intSwap[2] = matHlp[i, 2];
                        intSwap[3] = matHlp[i, 3];
                        matHlp[i, 0] = matHlp[j, 0];
                        matHlp[i, 1] = matHlp[j, 1];
                        matHlp[i, 2] = matHlp[j, 2];
                        matHlp[i, 3] = matHlp[j, 3];
                        matHlp[j, 0] = intSwap[0];
                        matHlp[j, 1] = intSwap[1];
                        matHlp[j, 2] = intSwap[2];
                        matHlp[j, 3] = intSwap[3];
                    }
                }
            }

            for (int i = 0; i < xxTmp; i++)
            {
                matHlp2[i, 0] = matHlp[i, 0];
                matHlp2[i, 1] = matHlp[i, 1];
                matHlp2[i, 2] = matHlp[i, 2];
                matHlp2[i, 3] = matHlp[i, 3];
            }

            for (int i = 0; i < xxTmp - 1; i++)
            {
                for (int j = i + 1; j < xxTmp; j++)
                {
                    if (newMatrixes[matHlp2[i, 1], 1] > newMatrixes[matHlp2[j, 1], 1])
                    {
                        intSwap[0] = matHlp2[i, 0];
                        intSwap[1] = matHlp2[i, 1];
                        intSwap[2] = matHlp2[i, 2];
                        intSwap[3] = matHlp2[i, 3];
                        matHlp2[i, 0] = matHlp2[j, 0];
                        matHlp2[i, 1] = matHlp2[j, 1];
                        matHlp2[i, 2] = matHlp2[j, 2];
                        matHlp2[i, 3] = matHlp2[j, 3];
                        matHlp2[j, 0] = intSwap[0];
                        matHlp2[j, 1] = intSwap[1];
                        matHlp2[j, 2] = intSwap[2];
                        matHlp2[j, 3] = intSwap[3];
                    }

                }
            }

            int J1 = 0;

            var prog = new int[busCount];
            var prog2 = new int[busCount];
            var lastprog = new int[busCount];

            for (int i = 0; i < busCount; i++)
            {
                prog[i] = 0;
                prog2[i] = 0;
                lastprog[i] = 0;
            }

            var stZam = new int[stCount, 2];           //матрица замены станций
            var stZam2 = new int[stCount, 2];
            var stUse = new int[stCount];
            var stUse2 = new int[stCount];             //массив использованных станций

            var aStr = new string[7];
            for (int i = 0; i < 7; i++) {
                    aStr[i] = "";
                    }

            var zamen = new int [7, xxTmp];
            var zamenSt = new int [7, stCount];

            for (int i = 0; i < xxTmp; i++) {
                    zamen[0, i] = matHlp2[i, 0];
                    }
            for (int i = 0; i < stCount; i++) {
                    zamenSt[0, i] = i+1;
                    aStr[0] += (i+1).ToString();
                    }

            for (int st = 0; st < 6; st++)
            {

                for (int i = 0; i < xxTmp; i++)
                {
                    matHlp2[i, 0] = 0;
                }

                for (int i = 0; i < busCount; i++)
                {
                    prog[i] = 0;
                    prog2[i] = 0;
                    lastprog[i] = 0;
                }

                for (int i = 0; i < stCount; i++)
                {
                    stUse[i] = 0;
                    stUse2[i] = 0;
                    stZam[i, 0] = i + 1;
                    stZam[i, 1] = 0;
                }
                int cntr = 0;

                int m1;
                int m2;
                for (int i = 0; i < xxTmp; i++)
                {
                    if (matHlp[i, 0] == 0)
                    {
                        break;
                    }
                    if (matHlp[i, 0] != 0)
                    {
                        bool fst;
                        if ((stUse[matHlp[i, 2] - 1] == 0) &&
                            (stUse[matHlp[i, 3] - 1] == 0))
                        {
                            fst = true;
                            for (int j = 0; j < xxTmp; j++)
                            {
                                if ((stUse2[matHlp2[j, 2] - 1] == 0) &&
                                    (stUse2[matHlp2[j, 3] - 1] == 0) &&
                                    (matHlp2[j, 0] == 0))
                                {
                                    if ((st == 2) && (cntr == 0))
                                    {
                                        cntr++;
                                    }
                                    else
                                        if ((st == 3) && (cntr == 0))
                                        {
                                            cntr++;
                                        }
                                        else
                                            if ((st == 4) && (cntr != 2))
                                            {
                                                cntr++;
                                            }
                                            else
                                                if ((st == 5) && (cntr != 2))
                                                {
                                                    cntr++;
                                                }
                                                else
                                                {
                                                    if (fst)
                                                    {
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            lastprog[z] = prog[z];
                                                        }
                                                        m1 = 0;
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            if (mask[matHlp2[j, 1]][z])
                                                            {
                                                                if (prog[z] > m1) m1 = prog[z];
                                                            }
                                                        }
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            if (mask[matHlp2[j, 1]][z])
                                                            {
                                                                prog[z] = m1 + matHlp[i, 0];
                                                            }
                                                        }
                                                        J1 = j;
                                                        fst = false;
                                                    }
                                                    else
                                                    {
                                                        m2 = 0;
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            prog2[z] = lastprog[z];
                                                        }
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            if (mask[matHlp2[j, 1]][z])
                                                            {
                                                                if (prog2[z] > m2) m2 = prog2[z];
                                                            }
                                                        }
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            if (mask[matHlp2[j, 1]][z])
                                                            {
                                                                prog2[z] = m2 + matHlp[i, 0];
                                                            }
                                                        }
                                                        m2 = 0;
                                                        m1 = 0;
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            if (prog[z] > m1) m1 = prog[z];
                                                            if (prog2[z] > m2) m2 = prog2[z];
                                                        }
                                                        if (m2 < m1)
                                                        {
                                                            for (int z = 0; z < busCount; z++)
                                                            {
                                                                prog[z] = prog2[z];
                                                            }
                                                            J1 = j;
                                                        }
                                                    }
                                                }

                                }
                            }
                            stUse[matHlp[i, 2] - 1] = 1;
                            stUse[matHlp[i, 3] - 1] = 1;
                            stUse2[matHlp2[J1, 2] - 1] = 1;
                            stUse2[matHlp2[J1, 3] - 1] = 1;
                            matHlp2[J1, 0] = matHlp[i, 0];

                            if ((st == 0) || (st == 2) || (st == 4))
                            {
                                stZam[matHlp[i, 2] - 1, 1] = matHlp2[J1, 2];
                                stZam[matHlp[i, 3] - 1, 1] = matHlp2[J1, 3];
                            }
                            else
                                if ((st == 1) || (st == 3) || (st == 5))
                                {

                                    stZam[matHlp[i, 2] - 1, 1] = matHlp2[J1, 3];
                                    stZam[matHlp[i, 3] - 1, 1] = matHlp2[J1, 2];

                                }
                        }
                        else
                            if ((stUse[matHlp[i, 2] - 1] == 1) &&
                                (stUse[matHlp[i, 3] - 1] == 1))
                            {
                                int j1 = 0, j2 = 0;
                                for (int z = 0; z < stCount; z++)
                                {
                                    if (matHlp[i, 2] == stZam[z, 0])
                                    {
                                        j1 = z;
                                    }
                                    if (matHlp[i, 3] == stZam[z, 0])
                                    {
                                        j2 = z;
                                    }
                                }
                                for (int j = 0; j < xxTmp; j++)
                                {
                                    if (((matHlp2[j, 2] == stZam[j1, 1]) &&
                                         (matHlp2[j, 3] == stZam[j2, 1]) &&
                                         (matHlp2[j, 0] == 0)) ||
                                        ((matHlp2[j, 2] == stZam[j2, 1]) &&
                                         (matHlp2[j, 3] == stZam[j1, 1]) &&
                                         (matHlp2[j, 0] == 0)))
                                    {
                                        matHlp2[j, 0] = matHlp[i, 0];
                                        m1 = 0;
                                        for (int z = 0; z < busCount; z++)
                                        {
                                            if (mask[matHlp2[j, 1]][z])
                                            {
                                                if (prog[z] > m1) m1 = prog[z];
                                            }
                                        }
                                        for (int z = 0; z < busCount; z++)
                                        {
                                            if (mask[matHlp2[j, 1]][z])
                                            {
                                                prog[z] = m1 + matHlp[i, 0];
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                                if ((stUse[matHlp[i, 2] - 1] == 1) &&
                                    (stUse[matHlp[i, 3] - 1] == 0))
                                {
                                    int j1 = 0, bz = 2;

                                    fst = true;
                                    for (int z = 0; z < stCount; z++)
                                    {
                                        if (matHlp[i, 2] == stZam[z, 0])
                                        {
                                            j1 = z;
                                        }
                                    }
                                    for (int j = 0; j < xxTmp; j++)
                                    {
                                        if ((matHlp2[j, 2] == stZam[j1, 1]) &&
                                            (stUse2[matHlp2[j, 3] - 1] == 0) &&
                                            (matHlp2[j, 0] == 0))
                                        {
                                            if (fst)
                                            {
                                                for (int z = 0; z < busCount; z++)
                                                {
                                                    lastprog[z] = prog[z];
                                                }
                                                m1 = 0;
                                                for (int z = 0; z < busCount; z++)
                                                {
                                                    if (mask[matHlp2[j, 1]][z])
                                                    {
                                                        if (prog[z] > m1) m1 = prog[z];
                                                    }
                                                }
                                                for (int z = 0; z < busCount; z++)
                                                {
                                                    if (mask[matHlp2[j, 1]][z])
                                                    {
                                                        prog[z] = m1 + matHlp[i, 0];
                                                    }
                                                }
                                                J1 = j;
                                                bz = 3;
                                                fst = false;
                                            }
                                            else
                                            {
                                                m2 = 0;
                                                for (int z = 0; z < busCount; z++)
                                                {
                                                    prog2[z] = lastprog[z];
                                                }
                                                for (int z = 0; z < busCount; z++)
                                                {
                                                    if (mask[matHlp2[j, 1]][z])
                                                    {
                                                        if (prog2[z] > m2) m2 = prog2[z];
                                                    }
                                                }
                                                for (int z = 0; z < busCount; z++)
                                                {
                                                    if (mask[matHlp2[j, 1]][z])
                                                    {
                                                        prog2[z] = m2 + matHlp[i, 0];
                                                    }
                                                }
                                                m2 = 0;
                                                m1 = 0;
                                                for (int z = 0; z < busCount; z++)
                                                {
                                                    if (prog[z] > m1) m1 = prog[z];
                                                    if (prog2[z] > m2) m2 = prog2[z];
                                                }
                                                if (m2 < m1)
                                                {
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        prog[z] = prog2[z];
                                                    }
                                                    J1 = j;
                                                    bz = 3;
                                                }
                                            }
                                        }
                                        else
                                            if ((stUse2[matHlp2[j, 2] - 1] == 0) &&
                                                (matHlp2[j, 3] == stZam[j1, 1]) &&
                                                (matHlp2[j, 0] == 0))
                                            {
                                                if (fst)
                                                {
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        lastprog[z] = prog[z];
                                                    }
                                                    m1 = 0;
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        if (mask[matHlp2[j, 1]][z])
                                                        {
                                                            if (prog[z] > m1) m1 = prog[z];
                                                        }
                                                    }
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        if (mask[matHlp2[j, 1]][z])
                                                        {
                                                            prog[z] = m1 + matHlp[i, 0];
                                                        }
                                                    }
                                                    J1 = j;
                                                    bz = 2;
                                                    fst = false;
                                                }
                                                else
                                                {
                                                    m2 = 0;
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        prog2[z] = lastprog[z];
                                                    }
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        if (mask[matHlp2[j, 1]][z])
                                                        {
                                                            if (prog2[z] > m2) m2 = prog2[z];
                                                        }
                                                    }
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        if (mask[matHlp2[j, 1]][z])
                                                        {
                                                            prog2[z] = m2 + matHlp[i, 0];
                                                        }
                                                    }
                                                    m2 = 0;
                                                    m1 = 0;
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        if (prog[z] > m1) m1 = prog[z];
                                                        if (prog2[z] > m2) m2 = prog2[z];
                                                    }
                                                    if (m2 < m1)
                                                    {
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            prog[z] = prog2[z];
                                                        }
                                                        J1 = j;
                                                        bz = 2;
                                                    }
                                                }
                                            }
                                    }
                                    matHlp2[J1, 0] = matHlp[i, 0];
                                    stUse[matHlp[i, 3] - 1] = 1;
                                    stUse2[matHlp2[J1, bz] - 1] = 1;

                                    stZam[matHlp[i, 3] - 1, 1] = matHlp2[J1, bz];



                                }
                                else
                                    if ((stUse[matHlp[i, 2] - 1] == 0) &&
                                        (stUse[matHlp[i, 3] - 1] == 1))
                                    {
                                        int j1 = 0, bz = 0;
                                        fst = true;
                                        for (int z = 0; z < stCount; z++)
                                        {
                                            if (matHlp[i, 3] == stZam[z, 0])
                                            {
                                                j1 = z;
                                            }
                                        }
                                        for (int j = 0; j < xxTmp; j++)
                                        {
                                            if ((matHlp2[j, 2] == stZam[j1, 1]) &&
                                                (stUse2[matHlp2[j, 3] - 1] == 0) &&
                                                (matHlp2[j, 0] == 0))
                                            {
                                                if (fst)
                                                {
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        lastprog[z] = prog[z];
                                                    }
                                                    m1 = 0;
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        if (mask[matHlp2[j, 1]][z])
                                                        {
                                                            if (prog[z] > m1) m1 = prog[z];
                                                        }
                                                    }
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        if (mask[matHlp2[j, 1]][z])
                                                        {
                                                            prog[z] = m1 + matHlp[i, 0];
                                                        }
                                                    }
                                                    J1 = j;
                                                    bz = 3;
                                                    fst = false;
                                                }
                                                else
                                                {
                                                    m2 = 0;
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        prog2[z] = lastprog[z];
                                                    }
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        if (mask[matHlp2[j, 1]][z])
                                                        {
                                                            if (prog2[z] > m2) m2 = prog2[z];
                                                        }
                                                    }
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        if (mask[matHlp2[j, 1]][z])
                                                        {
                                                            prog2[z] = m2 + matHlp[i, 0];
                                                        }
                                                    }
                                                    m2 = 0;
                                                    m1 = 0;
                                                    for (int z = 0; z < busCount; z++)
                                                    {
                                                        if (prog[z] > m1) m1 = prog[z];
                                                        if (prog2[z] > m2) m2 = prog2[z];
                                                    }
                                                    if (m2 < m1)
                                                    {
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            prog[z] = prog2[z];
                                                        }
                                                        J1 = j;
                                                        bz = 3;
                                                    }
                                                }
                                            }
                                            else
                                                if ((stUse2[matHlp2[j, 2] - 1] == 0) &&
                                                    (matHlp2[j, 3] == stZam[j1, 1]) &&
                                                    (matHlp2[j, 0] == 0))
                                                {
                                                    if (fst)
                                                    {
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            lastprog[z] = prog[z];
                                                        }
                                                        m1 = 0;
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            if (mask[matHlp2[j, 1]][z])
                                                            {
                                                                if (prog[z] > m1) m1 = prog[z];
                                                            }
                                                        }
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            if (mask[matHlp2[j, 1]][z])
                                                            {
                                                                prog[z] = m1 + matHlp[i, 0];
                                                            }
                                                        }
                                                        J1 = j;
                                                        bz = 2;
                                                        fst = false;
                                                    }
                                                    else
                                                    {
                                                        m2 = 0;
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            prog2[z] = lastprog[z];
                                                        }
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            if (mask[matHlp2[j, 1]][z])
                                                            {
                                                                if (prog2[z] > m2) m2 = prog2[z];
                                                            }
                                                        }
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            if (mask[matHlp2[j, 1]][z])
                                                            {
                                                                prog2[z] = m2 + matHlp[i, 0];
                                                            }
                                                        }
                                                        m2 = 0;
                                                        m1 = 0;
                                                        for (int z = 0; z < busCount; z++)
                                                        {
                                                            if (prog[z] > m1) m1 = prog[z];
                                                            if (prog2[z] > m2) m2 = prog2[z];
                                                        }
                                                        if (m2 < m1)
                                                        {
                                                            for (int z = 0; z < busCount; z++)
                                                            {
                                                                prog[z] = prog2[z];
                                                            }
                                                            J1 = j;
                                                            bz = 2;
                                                        }
                                                    }
                                                }
                                        }

                                        matHlp2[J1, 0] = matHlp[i, 0];
                                        stUse[matHlp[i, 2] - 1] = 1;
                                        stUse2[matHlp2[J1, bz] - 1] = 1;

                                        stZam[matHlp[i, 2] - 1, 1] = matHlp2[J1, bz];

                                    }
                    }
                }

                for (int z = 0; z < xxTmp; z++)
                {
                    zamen[st + 1, z] = matHlp2[z, 0];
                }

                for (int z = 0; z < stCount; z++)
                {
                    if (stZam[z, 1] == 0)
                    {
                        for (int h = 0; h < stCount; h++)
                        {
                            if (stUse2[h] == 0)
                            {
                                stUse[h] = 1;
                                zamenSt[st + 1, z] = h + 1;
                                stZam[z, 1] = h + 1;
                            }
                        }
                    }
                    else
                    {
                        zamenSt[st + 1, z] = stZam[z, 1];
                    }

                    for (int h = 0; h < stCount; h++)
                    {
                        if (stZam[h, 1] == z + 1)
                        {
                            aStr[st + 1] += (h + 1).ToString();
                            break;

                        }
                    }
                }

                for (int j = 0; j < busCount; j++)
                {
                    prog[j] = 0;
                }

                int mm;
                for (int i = 0; i < xxTmp; i++)
                {
                    if (matHlp2[i, 0] != 0)
                    {
                        mm = 0;
                        for (int j = 0; j < busCount; j++)
                        {
                            if (mask[matHlp2[i, 1]][j])
                            {
                                if (prog[j] > mm) mm = prog[j];
                            }
                        }
                        for (int j = 0; j < busCount; j++)
                        {
                            if (mask[matHlp2[i, 1]][j])
                            {
                                prog[j] = mm + matHlp2[i, 0];
                            }
                        }
                    }
                }
                mm = 0;
                for (int j = 0; j < busCount; j++)
                {
                    if (prog[j] > mm) mm = prog[j];
                }

                aStr[st + 1] += "  Р = " + (mm).ToString();

                if (st == 0)
                {
                    for (int j = 0; j < busCount; j++)
                    {
                        prog[j] = 0;
                    }
                    for (int i = 0; i < xxTmp; i++)
                    {
                        if (matHlp2[i, 0] != 0)
                        {
                            int max = 0;
                            for (int j = 0; j < busCount; j++)
                            {
                                if (mask[matHlp2[i, 1]][j])
                                {
                                    if (prog[j] > max) max = prog[j];
                                }
                            }
                            for (int j = 0; j < busCount; j++)
                            {
                                if (mask[matHlp2[i, 1]][j])
                                {
                                    prog[j] = max + matHlp2[i, 0];
                                }
                            }
                        }
                    }

                    for (int z = 0; z < xxTmp; z++)
                    {
                        matHlp3[z, 0] = matHlp2[z, 0];
                        matHlp3[z, 1] = matHlp2[z, 1];
                        matHlp3[z, 2] = matHlp2[z, 2];
                        matHlp3[z, 3] = matHlp2[z, 3];
                    }
                    for (int z = 0; z < stCount; z++)
                    {
                        stZam2[z, 0] = stZam[z, 0];
                        stZam2[z, 1] = stZam[z, 1];
                    }
                }

                if (st != 0)
                {
                    for (int j = 0; j < busCount; j++)
                    {
                        prog[j] = 0;
                        prog2[j] = 0;
                    }
                    for (int i = 0; i < xxTmp; i++)
                    {
                        if (matHlp2[i, 0] != 0)
                        {
                            int max = 0;
                            for (int j = 0; j < busCount; j++)
                            {
                                if (mask[matHlp2[i, 1]][j])
                                {
                                    if (prog2[j] > max) max = prog2[j];
                                }
                            }
                            for (int j = 0; j < busCount; j++)
                            {
                                if (mask[matHlp2[i, 1]][j])
                                {
                                    prog2[j] = max + matHlp2[i, 0];
                                }
                            }
                        }
                        if (matHlp3[i, 0] != 0)
                        {
                            int max = 0;
                            for (int j = 0; j < busCount; j++)
                            {
                                if (mask[matHlp3[i, 1]][j])
                                {
                                    if (prog[j] > max) max = prog[j];
                                }
                            }
                            for (int j = 0; j < busCount; j++)
                            {
                                if (mask[matHlp3[i, 1]][j])
                                {
                                    prog[j] = max + matHlp3[i, 0];
                                }
                            }
                        }
                    }

                    m1 = 0; m2 = 0;
                    for (int z = 0; z < busCount; z++)
                    {
                        if (m1 < prog[z]) m1 = prog[z];
                        if (m2 < prog2[z]) m2 = prog2[z];
                    }

                    if ((m2 < m1) && (m1 != 0) && (m2 != 0))
                    {
                        for (int z = 0; z < xxTmp; z++)
                        {
                            matHlp3[z, 0] = matHlp2[z, 0];
                            matHlp3[z, 1] = matHlp2[z, 1];
                            matHlp3[z, 2] = matHlp2[z, 2];
                            matHlp3[z, 3] = matHlp2[z, 3];
                        }
                        for (int z = 0; z < stCount; z++)
                        {
                            stZam2[z, 0] = stZam[z, 0];
                            stZam2[z, 1] = stZam[z, 1];
                        }
                    }
                }
            }
            var conflictNames = c.Select(netPath => netPath.Name).ToList();
            var lines = grd.Children.OfType<Line>().ToList();
            var res = new NetChartResult(conflictNames, conflictMatrix, 
                stList, busList, lines, mask, 
                stZam2, zamen, zamenSt, matHlp, newMatrixes, aStr,
                busCount, stCount, xxTmp);
            if (NavigationService != null) NavigationService.Navigate(res);
        }
    }
}
