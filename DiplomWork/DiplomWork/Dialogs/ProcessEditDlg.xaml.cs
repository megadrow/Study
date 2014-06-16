using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Controls;

namespace DiplomWork.Dialogs
{
    /// <summary>
    /// Interaction logic for ProcessEditDlg.xaml
    /// </summary>
    public partial class ProcessEditDlg : Window
    {

        public ProcessEditDlg()
        {
            InitializeComponent();
            var timer = new UpdateTimer();
            timer.Start(BtnOk, MainGrid);
        }

        private void LoadedEvent(object sender, RoutedEventArgs e)
        {
            var list = DataContext as List<ProcessesInfo>;
            if (list == null) return;
            DataGridModule.ItemsSource = list;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
