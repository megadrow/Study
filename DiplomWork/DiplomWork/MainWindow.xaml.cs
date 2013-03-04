using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace DiplomWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Class1> obs { get; set; }  
        public MainWindow()
        {
            InitializeComponent();
            obs = new ObservableCollection<Class1>();
            obs.Add(new Class1(2));
            obs.Add(new Class1(3));
            obs.Add(new Class1(4));
            mainGrid.ItemsSource = obs;
        }
    }
}
