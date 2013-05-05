using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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

        public SettingDlg(Settings setting)
        {
            InitializeComponent();
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
