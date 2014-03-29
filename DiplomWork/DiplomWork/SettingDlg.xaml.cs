using System.IO;
using System.Windows;
using System.Xml.Serialization;
using Controls;

namespace DiplomWork
{
    /// <summary>
    /// Interaction logic for SettingDlg.xaml
    /// </summary>
    public partial class SettingDlg : Window
    {
        Settings settings = new Settings();
        Settings settingRes = new Settings();

        public SettingDlg(Settings setting, int type)
        {
            InitializeComponent();
            switch (type)
            {
                case 0:
                    {
                        settArea.Visibility = Visibility.Visible;
                        settAreaGpd.Visibility = Visibility.Hidden;
                    } break;
                case 1:
                    {
                        settArea.Visibility = Visibility.Hidden;
                        settAreaGpd.Visibility = Visibility.Visible;
                    } break;
            }
            UpdateTimer timer = new UpdateTimer();
            timer.Start(btnOk, mainGrid);
            settingRes = setting;
            settings = setting.Copy();
            DataContext = settings;
        }

        public Settings ReturnSetting()
        {
            return settingRes;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            //SerializeStatic.Save(settings.GetType(), "settings.xml");
            settingRes = settings;

            StreamWriter writer = new StreamWriter("settings.xml", false);
            XmlSerializer serializer = new XmlSerializer(settingRes.GetType());

            serializer.Serialize(writer, settingRes);
            writer.Close();
            
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
