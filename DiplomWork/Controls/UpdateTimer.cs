using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Controls
{
    public static class UpdateTimer
    {
        public static void Start(Button btn, DependencyObject obj)
        {
            var timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(250), IsEnabled = true};
            timer.Tick += delegate
            {
                var err = new List<string>();
                FindValidationError.GetErrors(err, obj);
                btn.IsEnabled = (err.Count == 0);
            };
        }
    }
}
