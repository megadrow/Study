﻿using System.Linq;
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
        public static DataInit Step { get; set; }

        private Settings Settings { get; set; }

        public enum DeleteMode
        {
            DeleteRow,
            DeleteCol,
            Undelete
        }

        private DeleteMode Mode = DeleteMode.Undelete;
        public StationAndPoints(Settings settings)
        {
            InitializeComponent();
            Settings = settings;

            if (!DataInit.IsAvailable())
            {
                Step = new DataInit();
                Step.AddStation();
                Step.AddPoint();
                AddCol(Step.GetPointCount() - 1);
            }
            else
            {
                RepearDataGrid();
            }
            var timer = new UpdateTimer();
            timer.Start(Next, grMain);

            mainGrid.ItemsSource = Step.Stations;
            pointGrid.ItemsSource = Step.PointsTask;
        }

        private void RepearDataGrid()
        {
            for (int i = 0; i < Step.GetPointCount(); i++)
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
                Header = Step.GetPointName(numberCol),
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
            Mode = DeleteMode.Undelete;
            Step.AddStation();
        }

        private void AddColOnClick(object sender, RoutedEventArgs e)
        {
            Mode = DeleteMode.Undelete;
            Step.AddPoint();

            AddCol(Step.GetPointCount() - 1);
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            if (Step.GetAllPoints().Sum(pts => pts.Num) == 0)
            {
                ErrorViewer.ShowInfo("Не задано количество требуемых терминальных точек");
                return;
            }


            for (int i = 0; i < Step.GetPointCount(); i++)
            {
                var ptsNum = 0;
                if (Step.GetPointTask(i) != 0)
                {
                    ptsNum += Step.Stations.Sum(st => st.GetPoint(i).Num);

                    if (ptsNum == 0)
                    {
                        ErrorViewer.ShowInfo(
                            "Решение задачи невозможно\nОтсутствуют станции, к которым можно подключить точку: " +
                            Step.GetPoint(i).GetName());
                        return;
                    }
                }
            }

            var result = new ChooseInitTask(Step, Settings);
            if (NavigationService != null) NavigationService.Navigate(result);
        }

        private void DelRowOnClick(object sender, RoutedEventArgs e)
        {
            mainGrid.SelectedIndex = -1;
            Mode = DeleteMode.DeleteRow;
        }

        private void DelColOnClick(object sender, RoutedEventArgs e)
        {
            mainGrid.SelectedIndex = -1;
            Mode = DeleteMode.DeleteCol;
        }

        private void UndeleteOnClick(object sender, RoutedEventArgs e)
        {
            Mode = DeleteMode.Undelete;
        }

        private void DeliteClick(object sender, SelectionChangedEventArgs e)
        {
            if (mainGrid.SelectedIndex != -1)
            {
                if (Mode == DeleteMode.DeleteCol)
                {
                    if ((mainGrid.CurrentColumn.DisplayIndex != 0) && (mainGrid.Columns.Count > 2))
                    {
                        var delPoint =mainGrid.CurrentColumn.DisplayIndex - 1;
                        foreach (var st in Step.Stations)
                        {
                            st.GetAllPoints().RemoveAt(delPoint);
                        }
                        Step.PointsTask[0].GetAllPoints().RemoveAt(delPoint);
                        pointGrid.Columns.RemoveAt(mainGrid.CurrentColumn.DisplayIndex);
                        mainGrid.Columns.RemoveAt(mainGrid.CurrentColumn.DisplayIndex);
                    }
                    mainGrid.SelectedIndex = -1;
                    return;
                }
                if (Mode == DeleteMode.DeleteRow)
                {
                    if (Step.GetStationCount() > 1)
                    {
                        Step.Stations.RemoveAt(mainGrid.SelectedIndex);
                    }
                    return;
                }
            }
        }

        

    }
}
