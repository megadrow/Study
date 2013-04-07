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

        List<StationNum> stations = new List<StationNum>();

        private StationNum station = new StationNum(true);

        public int stationMin { get; set; }

        //private void cmdGetExceptions_Click(object sender, RoutedEventArgs e)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    GetErrors(sb, gridCarDetails);
        //    string message = sb.ToString();
        //    if (message != "") MessageBox.Show(message);
        //}


        public Step1Result(IEnumerable<StationNum> sUse, StationNum sMax)
        {
            InitializeComponent();

            //сохраняем для последующего получения данных
            station = sMax;
            stations = sUse.ToList();

            //enumerable - матрица sUse, в виде списка
            //matA - матрица коэффициентов (каждый список - терм точка, а внутри списка - перечень станций)
            var matA = new List<int[]>();
            var pointCount = sMax.Station.Points.Count;
            var enumerable = sUse as IList<StationNum> ?? sUse.ToList();
            var stationCount = enumerable.Count();
            var matB = new int[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                matB[i] = sMax.Station.Points[i].Num;
                matA.Add(new int[stationCount]);

                for (int j = 0; j < stationCount; j++)
                {
                    matA[i][j] = enumerable[j].Station.Points[i].Num;
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
                List<string> sb = new List<string>();
                FindValidationError.GetErrors(sb, pa);
               // var err = Validation.GetErrors(pa);
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
                    var pointCover = new int[station.Station.Points.Count];
                    for (int j = 0; j < t.Length; j++)
                    {
                        str += stations[j].Station.Name  + " = " + t[j].ToString();
                        if (j == t.Length - 1)
                        {
                            str += ": ";
                        }
                        else
                        {
                            str += ", ";
                        }

                        for (int i = 0; i < station.Station.Points.Count; i++)
                        {
                            pointCover[i] += stations[j].Station.Points[i].Num*t[j];
                        }
                    }

                    str += "покрыто точек: ";
                    for (int i = 0; i < station.Station.Points.Count; i++)
                    {
                        str += station.Station.Points[i].Point.Name + " = " + pointCover[i].ToString() + "/" + station.Station.Points[i].Num;

                        if (i != station.Station.Points.Count - 1)
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
    }
}
