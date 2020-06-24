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

namespace OsEngine.Robots.BearAbsorption
{
    /// <summary>
    /// Interaction logic for BearAbsorptionBotUi.xaml
    /// </summary>
    public partial class BearAbsorptionBotUi : Window
    {
        private BearAbsorptionBot _bearAbsorptionBot;
        public BearAbsorptionBotUi(BearAbsorptionBot bearAbsorptionBot)
        {
            InitializeComponent();

            _bearAbsorptionBot = bearAbsorptionBot;

            TextBoxStop.Text = bearAbsorptionBot.Stop.ToString();
            TextBoxProfit.Text = bearAbsorptionBot.Profit.ToString();
            TextBoxStopSleepage.Text = bearAbsorptionBot.StopSleepage.ToString();
            TextBoxProfitSleepage.Text = bearAbsorptionBot.ProfitSleepage.ToString();
            TextBoxVolume.Text = bearAbsorptionBot.Volume.ToString();
            CheckBoxIsOn.IsChecked = bearAbsorptionBot.IsOn;

            ButtomSave.Click += ButtonSave_Click;
        }

        private void ButtonSave_Click(Object sender, RoutedEventArgs e)
        {
            _bearAbsorptionBot.Stop = Convert.ToInt32(TextBoxStop.Text);
            _bearAbsorptionBot.Profit = Convert.ToInt32(TextBoxProfit.Text);
            _bearAbsorptionBot.StopSleepage = Convert.ToInt32(TextBoxStopSleepage.Text);
            _bearAbsorptionBot.ProfitSleepage = Convert.ToInt32(TextBoxProfitSleepage.Text);
            _bearAbsorptionBot.Volume = Convert.ToInt32(TextBoxVolume.Text);
            _bearAbsorptionBot.IsOn = CheckBoxIsOn.IsChecked.Value;

            _bearAbsorptionBot.Save();

            Close();
        }
    }
}
