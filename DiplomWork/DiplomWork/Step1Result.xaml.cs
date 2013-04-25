using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        //private void cmdGetExceptions_Click(object sender, RoutedEventArgs e)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    GetErrors(sb, gridCarDetails);
        //    string message = sb.ToString();
        //    if (message != "") MessageBox.Show(message);
        //}


        public Step1Result(FirstStep step)
        {
            InitializeComponent();
            UpdateTimer.Start(Sol, grdMain);
            //сохраняем для последующего получения данных
            //station = step;
            //stations = sUse.ToList();
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
                ResList.Items.Clear();
                IntLinearEquationSolve.SetMin(stationMin);
                resultList = IntLinearEquationSolve.Solve();

                if (resultList.Count == 0)
                {
                    ErrorViewer.ShowInfo("Отсутствуют решения. Попробуйте увеличить количество используемых станций");
                    return;
                }

                foreach (int[] t in resultList)
                {
                    var str = string.Empty;
                    var pointCover = new int[fStep.GetPointCount()];
                    for (int j = 0; j < t.Length; j++)
                    {
                        str += fStep.GetStationName(j)  + " = " + t[j].ToString();
                        if (j == t.Length - 1)
                        {
                            str += ": ";
                        }
                        else
                        {
                            str += ", ";
                        }

                        for (int i = 0; i < fStep.GetPointCount(); i++)
                        {
                            pointCover[i] += fStep.GetPointNumber(j, i)*t[j];
                        }
                    }

                    str += "покрыто точек: ";
                    for (int i = 0; i < fStep.GetPointCount(); i++)
                    {
                        str += fStep.GetPointName(i) + " = " + pointCover[i].ToString() + "/" + fStep.GetPointTask(i);

                        if (i != fStep.GetPointCount() - 1)
                        {
                            str += ", ";
                        }
                    }
                    
                    ResList.Items.Add(str);
                }
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            var result = new SecondStep(fStep);
            if (NavigationService != null) NavigationService.Navigate(result);
        }

        private void ResList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var res = resultList;
            var list = sender as ListBox;
            for (int i = 0; i < fStep.GetStationCount(); i++)
            {
                fStep.Stations[i].Num = resultList[list.SelectedIndex][i];
            }
            
        }
    }
}
