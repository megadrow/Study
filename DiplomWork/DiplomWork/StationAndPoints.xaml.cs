using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DiplomWork.Objects;

namespace DiplomWork
{
    /// <summary>
    /// Interaction logic for StationAndPoints.xaml
    /// </summary>
    public partial class StationAndPoints : Page
    {
        public static ObservableCollection<Station> Stations { get; set; }
        public static ObservableCollection<Station> TempStation { get; set; }
        public StationAndPoints()
        {
            InitializeComponent();
            if (Stations == null)
            {
                Stations = new ObservableCollection<Station>();
                TempStation = new ObservableCollection<Station> { new Station(true) };
            }
            else
            {
                RepearDataGrid();
            }
            
            mainGrid.ItemsSource = Stations;
            pointGrid.ItemsSource = TempStation;
        }

        private void RepearDataGrid()
        {
            for (int i = 0; i < TempStation[0].Points.Count; i++)
            {
                AddCol(i);
            }
        }

        private void AddCol(int numberCol)
        {
            var textColumn = new DataGridTextColumn
            {
                Header = TempStation[0].Points[numberCol].Name,
                Width = new DataGridLength(60),
                Binding = new Binding("Points[" + numberCol.ToString() + "].CurrentUse")

            };
            mainGrid.Columns.Add(textColumn);

            textColumn = new DataGridTextColumn
            {
                Width = new DataGridLength(60),
                Binding = new Binding("Points[" + numberCol.ToString() + "].Max")
            };
            pointGrid.Columns.Add(textColumn);
        }

        private void AddRowOnClick(object sender, RoutedEventArgs e)
        {
            var station = new Station();
            foreach (var point in TempStation[0].Points)
            {
                station.AddPoint(point);
            }
            Stations.Add(station);
        }

        private void AddColOnClick(object sender, RoutedEventArgs e)
        {
            TempStation[0].AddPoint();
            foreach (var station in Stations)
            {
                station.AddPoint(TempStation[0].Points[TempStation[0].Points.Count - 1]);
            }

            AddCol(TempStation[0].Points.Count - 1);
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
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

            var result = new Step1Result(Stations, TempStation[0]);
            if (NavigationService != null) NavigationService.Navigate(result);
        }
    }
}
