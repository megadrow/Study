using System.Windows.Input;

namespace Controls.Commands
{
    class NextCommand
    {
                // Создание команды requery
        private static RoutedUICommand next;

        static NextCommand()
        {
            // Инициализация команды
            var inputs = new InputGestureCollection();
            //inputs.Add(new KeyGesture(Key.R, ModifierKeys.Control, "Ctrl + R"));
            next = new RoutedUICommand("Next", "Next", typeof(NextCommand), inputs);
        }

        public static RoutedUICommand Next
        {
            get { return next; }
        }

        //private bool CanExecuteNextCommand(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = true;
        //    e.Handled = true;
        //}
    }
}
