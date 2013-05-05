using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
    public static class FindValidationError
    {
        public static void GetErrors(List<string> errors, DependencyObject obj)
        {
            foreach (object child in LogicalTreeHelper.GetChildren(obj))
            {
                var element = child as TextBox;
                var elementGrid = child as Grid;
                var elementStack = child as StackPanel;
                var elementDataGrid = child as DataGrid;
                var elementGroupBox = child as GroupBox;

                if (elementGroupBox != null)
                {
                    GetErrors(errors, elementGroupBox);
                }

                if (elementGrid != null)
                {
                    GetErrors(errors, elementGrid);
                }

                if (elementStack != null)
                {
                    GetErrors(errors, elementStack);
                }

                if (elementDataGrid != null)
                {
                    GetErrors(errors, elementDataGrid);
                }

                if (element == null) continue;

                if ((System.Windows.Controls.Validation.GetHasError(element)))
                {
                    errors.AddRange(System.Windows.Controls.Validation.GetErrors(element).Select(error => error.ErrorContent.ToString()));
                }

                GetErrors(errors, element);
            }
        }

        private static DataGridRow GetRow(DataGrid grid, int index)
        {
            var row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null) 
            {
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        private static void GetErrors(List<string> errors, DataGrid grid)
        {
            for (int i = 0; i < grid.Items.Count; i++)
            {
                var row = GetRow(grid, i);
                if ((row != null) && (System.Windows.Controls.Validation.GetHasError(row)))
                {
                    errors.AddRange(System.Windows.Controls.Validation.GetErrors(row).Select(error => error.ErrorContent.ToString()));
                    break;
                }
            }
        }
    }
}
