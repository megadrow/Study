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
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
