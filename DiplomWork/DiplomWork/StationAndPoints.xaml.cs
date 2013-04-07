using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Controls;
using Controls.Validation;
using DiplomWork.Objects;

namespace DiplomWork
{
    /// <summary>
    /// Interaction logic for StationAndPoints.xaml
    /// </summary>
    public partial class StationAndPoints : Page
    {
        public static FirstStep Step { get; set; }
        public StationAndPoints()
        {
            InitializeComponent();
            if (!FirstStep.IsAvailable())
            {
                Step = new FirstStep();
            }
            else
            {
                RepearDataGrid();
            }

            mainGrid.ItemsSource = Step.Stations;
            pointGrid.ItemsSource = Step.PointsTask;
        }

        private void RepearDataGrid()
        {
            for (int i = 0; i < Step.PointsTask[0].GetPointCount(); i++)
            {
                AddCol(i);
            }
        }

        private void AddCol(int numberCol)
        {
            var bind = new Binding("Station.Points[" + numberCol.ToString() + "].Num");
            bind.ValidationRules.Clear();
            bind.ValidationRules.Add(new ValidationNumber());
            var textColumn = new DataGridTextColumn
            {
                Header = Step.PointsTask[0].GetPointName(numberCol),
                Width = new DataGridLength(60),
                Binding = bind
            };
            
            mainGrid.Columns.Add(textColumn);

            bind = new Binding("Station.Points[" + numberCol.ToString() + "].Num");
            bind.ValidationRules.Clear();
            bind.ValidationRules.Add(new ValidationNumber());

            textColumn = new DataGridTextColumn
            {
                Width = new DataGridLength(60),
                Binding = bind
            };
            pointGrid.Columns.Add(textColumn);
        }

        private void AddRowOnClick(object sender, RoutedEventArgs e)
        {
            var station = new StationNum();
            foreach (var point in Step.PointsTask[0].GetAllPoints())
            {
                station.AddPoint(point);
            }
            Step.Stations.Add(station);
        }

        private void AddColOnClick(object sender, RoutedEventArgs e)
        {
            Step.PointsTask[0].Station.AddPoint();
            foreach (var station in Step.Stations)
            {
                station.AddPoint(Step.PointsTask[0].GetPoint(Step.PointsTask[0].GetPointCount() - 1));
            }

            AddCol(Step.PointsTask[0].GetPointCount() - 1);
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            List<string> err = new List<string>();
            FindValidationError.GetErrors(err, gr);

            
            
            //var matA = new List<int[]>();
            //var pointCpunt = TempStation[0].Points.Count;
            //for (int j = 0; j < Stations.Count; j++)
            //{
            //    matA.Add(new int[pointCpunt]);
            //    for (int i = 0; i < pointCpunt; i++)
            //    {
            //        matA[j][i] = Stations[j].Points[i].CurrentUse;
            //    }
            //}

            //var matB = new int[pointCpunt];
            //for (int i = 0; i < pointCpunt; i++)
            //{
            //    matB[i] = TempStation[0].Points[i].Max;
            //}

            var result = new Step1Result(Step.Stations, Step.PointsTask[0]);
            if (NavigationService != null) NavigationService.Navigate(result);
        }
    }
}
