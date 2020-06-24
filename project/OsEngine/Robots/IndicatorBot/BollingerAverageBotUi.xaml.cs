using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OsEngine.Robots.IndicatorBot
{
    /// <summary>
    /// Interaction logic for BollingerAverageBotUi.xaml
    /// </summary>
    public partial class BollingerAverageBotUi : Window
    {
        private BollingerAverageBot _bollingerAverageBot;
        public BollingerAverageBotUi(BollingerAverageBot bollingerAverageBot)
        {
            InitializeComponent();

            TextBoxVolume.Text = bollingerAverageBot.Volume.ToString();
            CheckBoxIsOn.IsChecked = bollingerAverageBot.IsOn;

            _bollingerAverageBot = bollingerAverageBot;

            ButtonSave.Click += ButtonSave_Click;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            _bollingerAverageBot.Volume = Convert.ToInt32(TextBoxVolume.Text);
            _bollingerAverageBot.IsOn = CheckBoxIsOn.IsChecked.Value;

            _bollingerAverageBot.Save();

            Close();
        }
    }
}
