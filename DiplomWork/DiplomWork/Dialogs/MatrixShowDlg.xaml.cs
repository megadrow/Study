using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Controls.Validation;

namespace DiplomWork.Dialogs
{
    class MatrixView
    {
        public string Name { get; set; }
        public int[] Value { get; set; }

        public MatrixView(int valcount)
        {
            Value = new int[valcount];
        }
    }
    /// <summary>
    /// Interaction logic for MatrixShowDlg.xaml
    /// </summary>
    public partial class MatrixShowDlg : Window
    {
        private List<MatrixView> _mtx = new List<MatrixView>();
        public MatrixShowDlg(ICollection<string> names, int[,] matrix)
        {
            InitializeComponent();
            foreach (var name in names)
            {
                var mView = new MatrixView(names.Count);
                for (int i = 0; i < names.Count; i++)
                {
                    mView.Value[i] = matrix[_mtx.Count, i];
                }
                mView.Name = name;
                

                var bind = new Binding("Value[" + _mtx.Count + "]");
                var textColumn = new DataGridTextColumn
                {
                    Header = name,
                    Width = new DataGridLength(100),
                    Binding = bind,
                    IsReadOnly = true
                };
            
                _mtx.Add(mView);
                DataGridMatrix.Columns.Add(textColumn);
            }
            DataGridMatrix.ItemsSource = _mtx;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
