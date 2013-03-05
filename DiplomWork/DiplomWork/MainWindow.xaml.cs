using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DiplomWork.Objects;

namespace DiplomWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Station> Stations { get; set; }
        public ObservableCollection<Station> TempStation { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Stations = new ObservableCollection<Station>();
            TempStation = new ObservableCollection<Station> {new Station(true)};
            mainGrid.ItemsSource = Stations;
            pointGrid.ItemsSource = TempStation;
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

            var textColumn = new DataGridTextColumn
                {
                    Header = TempStation[0].Points[TempStation[0].Points.Count - 1].Name,
                    Width = new DataGridLength(60),
                    Binding = new Binding("Points[" + (TempStation[0].Points.Count - 1).ToString() + "].CurrentUse")
                    
                };
            mainGrid.Columns.Add(textColumn);

            textColumn = new DataGridTextColumn
                {
                    Width = new DataGridLength(60),
                    Binding = new Binding("Points[" + (TempStation[0].Points.Count - 1).ToString() + "].Max")
                };
            pointGrid.Columns.Add(textColumn);
        }
    }
}
