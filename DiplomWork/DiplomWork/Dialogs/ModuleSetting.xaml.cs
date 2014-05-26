using System.Collections.Generic;
using System.Windows;
using Controls;

namespace DiplomWork.Dialogs
{
    /// <summary>
    /// Interaction logic for ModuleSetting.xaml
    /// </summary>
    public partial class ModuleSetting
    {
        public ModuleSetting()
        {
            InitializeComponent();
            var timer = new UpdateTimer();
            timer.Start(BtnOk, MainGrid);
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private new void LoadedEvent(object sender, RoutedEventArgs e)
        {
            var list = DataContext as List<GpdModule>;
            if (list == null) return;
            DataGridModule.ItemsSource = list;
        }
    }
}
