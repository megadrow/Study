using System.IO;
using System.Windows;
using System.Windows.Navigation;
using System.Xml.Serialization;

namespace DiplomWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var settings = new Settings();
            //SerializeStatic.Load(settings.GetType(), "settings.xml");
            if (File.Exists("settings.xml"))
            {
                var writer = new StreamReader("settings.xml");
                var serializer = new XmlSerializer(typeof(Settings));

                settings = (Settings)serializer.Deserialize(writer);
                writer.Close();
            }
            else
            {
                settings.AreaHeight = 800;
                settings.AreaWidth = 600;
            }
            

            var result = new StationAndPoints(settings);
            if (frame.NavigationService != null) frame.NavigationService.Navigate(result);
        }
    }

}
