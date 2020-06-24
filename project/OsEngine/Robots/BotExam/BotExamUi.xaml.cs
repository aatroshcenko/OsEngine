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

namespace OsEngine.Robots.BotExam
{
    /// <summary>
    /// Interaction logic for BotExamUi.xaml
    /// </summary>
    public partial class BotExamUi : Window
    {
        private BotExam _botExam;
        public BotExamUi(BotExam botExam)
        {
            InitializeComponent();

            TextBoxVolume.Text = botExam.Volume.ToString();
            CheckBoxIsOn.IsChecked = botExam.IsOn;
            TextBoxValue.Text = botExam.Value.ToString();

            _botExam = botExam;

            ButtonSave.Click += ButtonSave_Click;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            _botExam.Volume = Convert.ToInt32(TextBoxVolume.Text);
            _botExam.IsOn = CheckBoxIsOn.IsChecked.Value;
            _botExam.Value = Convert.ToDecimal(TextBoxValue.Text);

            _botExam.Save();

            Close();
        }
    }
}
