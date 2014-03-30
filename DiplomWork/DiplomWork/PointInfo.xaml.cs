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
            DialogResult = true;
        }
    }
}
