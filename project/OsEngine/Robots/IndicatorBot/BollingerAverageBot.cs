using OsEngine.Charts.CandleChart.Indicators;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsEngine.Robots.IndicatorBot
{
    public class BollingerAverageBot : BotPanel
    {
        private MovingAverage _moving;

        private Bollinger _bollinger;

        public int Volume = 10;

        public bool IsOn = false;
        public BollingerAverageBot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            Load();

            TabCreate(BotTabType.Simple);

            _moving = (MovingAverage)TabsSimple[0].CreateCandleIndicator(new MovingAverage("moving1", false), "Prime");
            _moving.Save();

            _bollinger = (Bollinger)TabsSimple[0].CreateCandleIndicator(new Bollinger("bollinger1", false), "Prime");
            _bollinger.Save();

            TabsSimple[0].CandleFinishedEvent += BollingerAverageBot_CandleFinishedEvent;
        }

        public override string GetNameStrategyType()
        {
            return nameof(BollingerAverageBot);
        }

        public override void ShowIndividualSettingsDialog()
        {
            var ui = new BollingerAverageBotUi(this);
            ui.ShowDialog();
        }

        /// <summary>
        /// save settings
        /// сохранить настройки
        /// </summary>
        public void Save()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(@"Engine\" + NameStrategyUniq + @"SettingsBot.txt", false)
                    )
                {
                    writer.WriteLine(Volume);
                    writer.WriteLine(IsOn);

                    writer.Close();
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }

        /// <summary>
        /// load settings
        /// загрузить настройки
        /// </summary>
        private void Load()
        {
            if (!File.Exists(@"Engine\" + NameStrategyUniq + @"SettingsBot.txt"))
            {
                return;
            }
            try
            {
                using (StreamReader reader = new StreamReader(@"Engine\" + NameStrategyUniq + @"SettingsBot.txt"))
                {
                    Volume = Convert.ToInt32(reader.ReadLine());
                    IsOn = Convert.ToBoolean(reader.ReadLine());

                    reader.Close();
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }


        private void BollingerAverageBot_CandleFinishedEvent(List<Candle> candles)
        {
            if (!IsOn)
            {
                return;
            }

            if (_moving.Lenght > candles.Count ||
                _bollinger.Lenght > candles.Count)
            {
                return;
            }

            List<Position> positions = TabsSimple[0].PositionsOpenAll;

            if (positions != null &&
                positions.Count != 0)
            {
                if (positions[0].State != PositionStateType.Open)
                {
                    return;
                }
            }

            if (positions == null ||
                positions.Count == 0)
            {
                if (candles[candles.Count - 1].Close > _bollinger.ValuesUp[_bollinger.ValuesUp.Count - 1])
                {
                    TabsSimple[0].BuyAtLimit(Volume, candles[candles.Count - 1].Close);
                }
            }
            else
            {
                if (candles[candles.Count - 1].Close < _moving.Values[_moving.Values.Count - 1])
                {
                    TabsSimple[0].CloseAtLimit(positions[0], candles[candles.Count - 1].Close, positions[0].OpenVolume);
                }
            }
        }
    }
}
