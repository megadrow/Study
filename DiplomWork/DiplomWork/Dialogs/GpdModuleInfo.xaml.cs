using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Controls;

namespace DiplomWork.Dialogs
{
    /// <summary>
    /// Interaction logic for GpdModuleInfo.xaml
    /// </summary>
    public partial class GpdModuleInfo
    {
        public GpdModuleInfo()
        {
            InitializeComponent();

            var timer = new UpdateTimer();
            timer.Start(BtnOk, MainGrid);
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            var bindEx = TboxOutData.GetBindingExpression(TextBox.TextProperty);
            bindEx.UpdateSource();
            bindEx = TboxProcTime.GetBindingExpression(TextBox.TextProperty);
            bindEx.UpdateSource();
            var itemData = (DataContext as GpdModule);
            if (itemData != null)
            {
                itemData.Process.Ind = PropProc.SelectedIndex;
                itemData.Process.Str = (PropProc.SelectedIndex > -1) ? PropProc.SelectedItem.ToString() : null;
                itemData.Stan.Ind = PropStan.SelectedIndex;
                itemData.Stan.Str = (PropStan.SelectedIndex > -1) ? PropStan.SelectedItem.ToString() : null;
            }
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {

        }

        private void LoadedEvent(object sender, RoutedEventArgs e)
        {
            if (DataContext == null) return;
            var data = DataContext as GpdModule;
            
            if (data == null)
                return;
            foreach (var item in GpdData.ProcNames.Select(procName => new ComboBoxItem { Content = procName }))
            {
                PropProc.Items.Add(item);
            }
            foreach (var item in GpdData.StanNames.Select(stanName => new ComboBoxItem { Content = stanName }))
            {
                PropStan.Items.Add(item);
            }
            PropProc.SelectedIndex = data.Process.Ind;
            PropStan.SelectedIndex = data.Stan.Ind;
        }
    }
}
