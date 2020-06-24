using Jayrock.Json.Conversion.Converters;
using OsEngine.Charts.CandleChart.Indicators;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsEngine.Robots.BotExam
{
    public class BotExam : BotPanel
    {
        #region private fields
        private MovingAverage _moving;

        private Atr _atr;
        #endregion
        #region public properties
        public int Volume { get; set; } = 10;

        public bool IsOn { get; set; } = false;

        public decimal Value { get; set; } = 2.1M;
        #endregion

        #region private methods
        private void BotExam_NewTickEvent(Trade trade)
        {//Вход в лонг. Суммарный объем в заявках на продажу на 40% меньше чем на покупку.

            if (!IsOn)
            {
                return;
            }

            var positions = TabsSimple[0].PositionsOpenAll;

            if (positions.Any())
            {
                return;
            }

            if(trade.AsksVolume != 0 && trade.BidsVolume/trade.AsksVolume > 1.4M)
            {
                TabsSimple[0].BuyAtMarket(Volume);
            }
        }

        private void BotExam_CandleFinishedEvent(List<Candle> candleTabs)
        {
            if (!IsOn)
            {
                return;
            }

            var candles = TabsSimple[0].CandlesFinishedOnly;

            if (candles[candles.Count - 1].TimeStart == candleTabs[candleTabs.Count - 1].TimeStart)
            {
                TradeLogic(candles, candleTabs);
            }
        }

        private void BotExam_SpreadChangeEvent(List<Candle> candles)
        {
            if (!IsOn)
            {
                return;
            }

            var candleTabs = TabsSimple[0].CandlesFinishedOnly;

            if (candles[candles.Count - 1].TimeStart == candleTabs[candleTabs.Count - 1].TimeStart)
            {
                TradeLogic(candles, candleTabs);
            }
        }

        private void TradeLogic(List<Candle> indexes, List<Candle> tabCandles)
        {//Вход в лонг. Цена закрытия свечи ниже чем MA - ATR*Value
         //Выход. Цена зарытия свечи выше чем MA + ATR*Value.

            if (_moving.Lenght > tabCandles.Count ||
                _atr.Lenght > tabCandles.Count)
            {
                return;
            }

            var positions = TabsSimple[0].PositionsOpenAll;

            if (!positions.Any())
            {
                if(indexes[indexes.Count - 1].Close < 
                    _moving.Values[_moving.Values.Count - 1] - _atr.Values[_atr.Values.Count - 1] * Value)
                {
                    TabsSimple[0].BuyAtLimit(Volume, tabCandles[tabCandles.Count - 1].Close);
                }
            }
            else
            {
                if (indexes[indexes.Count - 1].Close >
                     _moving.Values[_moving.Values.Count - 1] + _atr.Values[_atr.Values.Count - 1] * Value)
                {
                    TabsSimple[0].CloseAtLimit(positions[0], tabCandles[tabCandles.Count -1].Close, positions[0].OpenVolume);
                }
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
                    Value = Convert.ToInt32(reader.ReadLine());

                    reader.Close();
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }
        #endregion

        #region public methods
        public BotExam(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Index);
            TabCreate(BotTabType.Simple);
            Load();

            _moving = (MovingAverage)TabsIndex[0].CreateCandleIndicator(new MovingAverage("moving", false), "Prime");
            _moving.Save();

            _atr = (Atr)TabsIndex[0].CreateCandleIndicator(new Atr("atr", false), "NewArea");
            _atr.Save();

            TabsIndex[0].SpreadChangeEvent += BotExam_SpreadChangeEvent;
            TabsSimple[0].CandleFinishedEvent += BotExam_CandleFinishedEvent;
            TabsSimple[0].NewTickEvent += BotExam_NewTickEvent;
        }


        public override string GetNameStrategyType()
        {
            return nameof(BotExam);
        }

        public override void ShowIndividualSettingsDialog()
        {
            var ui = new BotExamUi(this);
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
                    writer.WriteLine(Value);

                    writer.Close();
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }
        #endregion
    }
}
