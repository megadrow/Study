using System.Windows;
using Controls;

namespace DiplomWork.Dialogs
{
    /// <summary>
    /// Interaction logic for PointInfo.xaml
    /// </summary>
    public partial class PointInfo
    {
        public PointInfo()
        {
            InitializeComponent();
            var timer = new UpdateTimer();
            timer.Start(BtnOk, MainGrid);
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
