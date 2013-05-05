using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Controls
{
    public class UpdateTimer
    {
        public void Start(Button btn, DependencyObject obj)
        {
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50), IsEnabled = true };
            timer.Tick += delegate
            {
                var err = new List<string>();
                FindValidationError.GetErrors(err, obj);
                btn.IsEnabled = (err.Count == 0);
            };
            timer.Start();
        }
    }
}
