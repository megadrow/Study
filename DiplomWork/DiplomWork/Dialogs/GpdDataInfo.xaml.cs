using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Controls;

namespace DiplomWork.Dialogs
{
    /// <summary>
    /// Interaction logic for GpdDataInfo.xaml
    /// </summary>
    public partial class GpdDataInfo
    {
        public GpdDataInfo()
        {
            InitializeComponent();
            var timer = new UpdateTimer();
            timer.Start(BtnOk, MainGrid);
        }

        private void InOutChecked(object sender, RoutedEventArgs e)
        {
            TboxCicle.IsEnabled = true;
        }

        private void InOutUnchacked(object sender, RoutedEventArgs e)
        {
            TboxCicle.IsEnabled = false;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {

        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            var bindEx = TboxCicle.GetBindingExpression(TextBox.TextProperty);
            bindEx.UpdateSource();
            bindEx = CbInOut.GetBindingExpression(CheckBox.IsCheckedProperty);
            bindEx.UpdateSource();
            var itemData = (this.DataContext as GpdData);
            if (itemData != null)
            {
                itemData.Process.Ind = PropProc.SelectedIndex;
                itemData.Process.Str = (PropProc.SelectedIndex > -1) ? PropProc.SelectedItem.ToString() : null;
                itemData.Stan.Ind = PropStan.SelectedIndex;
                itemData.Stan.Str = (PropStan.SelectedIndex > -1) ? PropStan.SelectedItem.ToString() : null;
            }
            DialogResult = true;
        }

        private void LoadedEvent(object sender, RoutedEventArgs e)
        {
            if (DataContext == null) return;
            var data = DataContext as GpdData;
            if (data == null)
                return;
            foreach (var item in GpdData.ProcNames.Select(procName => new ComboBoxItem {Content = procName}))
            {
                PropProc.Items.Add(item);
            }
            foreach (var item in GpdData.StanNames.Select(stanName => new ComboBoxItem {Content = stanName}))
            {
                PropStan.Items.Add(item);
            }
            PropProc.SelectedIndex = data.Process.Ind;
            PropStan.SelectedIndex = data.Stan.Ind;
        }
    }
}
