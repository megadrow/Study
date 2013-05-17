using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Calculation;
using Controls;
using DiplomWork.Objects;

namespace DiplomWork
{
    /// <summary>
    /// Interaction logic for Step1Result.xaml
    /// </summary>
    public partial class Step1Result : Page
    {
        List<int[]> pointsUse = new List<int[]>();

        List<int[]> resultList = new List<int[]>();

        private int[] pointsMax;

        public int stationMin { get; set; }

        private FirstStep fStep;

        private Settings Settings { get; set; }

        public Step1Result(FirstStep step, Settings settings)
        {
            InitializeComponent();
            UpdateTimer timer = new UpdateTimer();
            timer.Start(Sol, grdMain);
            Settings = settings;
            //сохраняем для последующего получения данных

            fStep = step;
            //enumerable - матрица sUse, в виде списка
            //matA - матрица коэффициентов (каждый список - терм точка, а внутри списка - перечень станций)
            var matA = new List<int[]>();
            var pointCount = fStep.GetPointCount();
            var stationCount = fStep.GetStationCount();
            var matB = new int[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                matB[i] = fStep.GetPointTask(i);
                matA.Add(new int[stationCount]);

                for (int j = 0; j < stationCount; j++)
                {
                    matA[i][j] = fStep.GetPointNumber(j, i);
                }
            }
            
            pointsUse = matA;
            pointsMax = matB;
            DataContext = this;
            IntLinearEquationSolve.SetParam(pointsUse, pointsMax);
            stationMin = IntLinearEquationSolve.GetMin();
        }

        private void SolveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ResListV.ItemsSource = null;
                ResListV.Items.Clear();
                gridV.Columns.Clear();
                IntLinearEquationSolve.SetMin(stationMin);
                resultList = IntLinearEquationSolve.Solve();

                if (resultList.Count == 0)
                {
                    ErrorViewer.ShowInfo("Отсутствуют решения. Попробуйте увеличить количество используемых станций");
                    return;
                }

                var results = new ObservableCollection<Result1View>();
                ResListV.ItemsSource = results;
                bool chk = false;
                foreach (int[] t in resultList)
                {
                    results.Add(new Result1View(fStep.GetStationCount(), fStep.GetPointCount()));
                    var str = string.Empty;
                    var pointCover = new int[fStep.GetPointCount()];
                    for (int j = 0; j < t.Length; j++)
                    {
                        if (!chk)
                        {
                            gridV.Columns.Add(new GridViewColumn()
                            {
                                Header = fStep.GetStationName(j),
                                DisplayMemberBinding = new Binding("StationCount[" + j.ToString() + "]")
                            });   
                        }

                        results.Last().StationCount[j] = t[j];

                        for (int i = 0; i < fStep.GetPointCount(); i++)
                        {
                            results.Last().PointCover[i] += fStep.GetPointNumber(j, i) * t[j];
                        }
                    }
                    for (int i = 0; i < fStep.GetPointCount(); i++)
                    {
                        results.Last().PointCount[i] = fStep.GetPointTask(i);

                        if (!chk)
                        {
                            gridV.Columns.Add(new GridViewColumn()
                            {
                                Header = fStep.GetPointName(i),
                                DisplayMemberBinding = new Binding("PointView[" + i.ToString() + "]")
                            });   
                        }
                    }
                    results.Last().SetPtView();
                    chk = true;
                }
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            if (ResListV.SelectedIndex == -1)
            {
                ErrorViewer.ShowInfo("Решение не выбрано");
                return;
            }
            var result = new SecondStep(fStep, Settings);
            if (NavigationService != null) NavigationService.Navigate(result);
        }

        private void ResList_OnSelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            var list = sender as ListView;
            if (list.SelectedIndex != -1)
            {
                for (int i = 0; i < fStep.GetStationCount(); i++)
                {
                    fStep.Stations[i].Num = resultList[list.SelectedIndex][i];
                }
            }
        }
    }
}
