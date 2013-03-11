using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Calculation;
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

        List<Station> st = new List<Station>();

        private Station sta = new Station(true);

        public int stationMin { get; set; }

        public Step1Result(IEnumerable<Station> sUse, Station sMax)
        {
            InitializeComponent();

            sta = sMax;
            st = sUse.ToList();

            var matA = new List<int[]>();
            var pointCpunt = sMax.Points.Count;
            var enumerable = sUse as IList<Station> ?? sUse.ToList();
            for (int j = 0; j < enumerable.Count(); j++)
            {
                matA.Add(new int[pointCpunt]);
                for (int i = 0; i < pointCpunt; i++)
                {
                    matA[j][i] = enumerable[j].Points[i].CurrentUse;
                }
            }

            var matB = new int[pointCpunt];
            for (int i = 0; i < pointCpunt; i++)
            {
                matB[i] = sMax.Points[i].Max;
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
                    for (int j = 0; j < t.Length; j++)
                    {
                        str += t[j].ToString() + " ";
                    }

                    str += ": ";
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
