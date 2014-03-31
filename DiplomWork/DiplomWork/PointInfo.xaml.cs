using System.Windows;
using System.Windows.Controls;
using Controls;

namespace DiplomWork
{
    /// <summary>
    /// Interaction logic for PointInfo.xaml
    /// </summary>
    public partial class PointInfo : Window
    {
        public PointInfo(int type)
        {
            InitializeComponent();
            switch (type)
            {
                case 0:
                    {
                        gbCommInfo.Visibility = Visibility.Visible;
                        gbCoords.Visibility = Visibility.Visible;
                        gbData.Visibility = Visibility.Hidden;
                    } break;
                case 1:
                    {
                        gbCommInfo.Visibility = Visibility.Hidden;
                        gbCoords.Visibility = Visibility.Hidden;
                        gbData.Visibility = Visibility.Visible;
                    } break;
                case 2:
                    {
                        gbCommInfo.Visibility = Visibility.Hidden;
                        gbCoords.Visibility = Visibility.Hidden;
                        gbData.Visibility = Visibility.Hidden;
                    } break;
            }
            UpdateTimer timer = new UpdateTimer();
            timer.Start(btnOk, mainGrid);
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            var bindEx = tboxCicle.GetBindingExpression(TextBox.TextProperty);
            bindEx.UpdateSource();
            bindEx = cbInOut.GetBindingExpression(CheckBox.IsCheckedProperty);
            bindEx.UpdateSource();
            DialogResult = true;
        }

        private void InOutChecked(object sender, RoutedEventArgs e)
        {
            tboxCicle.IsEnabled = true;
        }

        private void InOutUnchacked(object sender, RoutedEventArgs e)
        {
            tboxCicle.IsEnabled = false;
        }
    }
}
