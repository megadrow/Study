using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Controls;

namespace DiplomWork
{
    /// <summary>
    /// Interaction logic for SecondStep.xaml
    /// </summary>
    public partial class SecondStep : Page
    {
        List<Shape> list = new List<Shape>();

        private bool con = false;

        public SecondStep()
        {
            InitializeComponent();
        }

        private void Ima_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!con)
            {
                var circle = new StationEll(e.GetPosition(ima).X, e.GetPosition(ima).Y);
                circle.Text = "123";

                grd.Children.Add(circle);
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var stan in grd.Children)
                {
                    var st = stan as StationEll;
                    if (st != null)
                    {
                        st.ToMoveMode();
                        con = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }

        private void ConOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var stan in grd.Children)
                {
                    var st = stan as StationEll;
                    if (st != null)
                    {
                        st.ToConnectMode();
                        con = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorViewer.ShowError(ex);
            }
        }

    }
}
