using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OsEngine.Robots.BearAbsorption
{
    public class BearAbsorptionBot : BotPanel
    {
        public int Stop = 10;

        public int Profit = 20;

        public int StopSleepage = 5;

        public int ProfitSleepage = 5;

        public int Volume = 10;

        public bool IsOn = false;

        public BearAbsorptionBot(string name, StartProgram startProgram) : base(
            name,
            startProgram)
        {
            Load();

            TabCreate(BotTabType.Simple); //создание вкладки;

            TabsSimple[0].CandleFinishedEvent += BearAbsorptionBot_CandleFinishedEvent;

            TabsSimple[0].PositionOpeningSuccesEvent += BearAbsorptionBot_CandleOpeningSuccessEvent;
        }

        private void BearAbsorptionBot_CandleOpeningSuccessEvent(Position position)
        {
            TabsSimple[0].CloseAtStop(
                position,
                position.EntryPrice - Stop * TabsSimple[0].Securiti.PriceStep,
                position.EntryPrice - (Stop + StopSleepage) * TabsSimple[0].Securiti.PriceStep);
            TabsSimple[0].CloseAtProfit(
                position,
                position.EntryPrice + Profit * TabsSimple[0].Securiti.PriceStep,
                position.EntryPrice + (Profit - ProfitSleepage) * TabsSimple[0].Securiti.PriceStep);
        }

        private void BearAbsorptionBot_CandleFinishedEvent(List<Candle> candles)
        {
            if(candles.Count < 5)
            {
                return;
            }

            if (!IsOn)
            {
                return;
            }

            if(TabsSimple[0].PositionsOpenAll != null &&
                TabsSimple[0].PositionsOpenAll.Any())
            {
                return;
            }

            Candle lastCandle = candles.Last();
            Candle secondCandle = candles[candles.Count - 2];

            if(lastCandle.Open > lastCandle.Close &&
                secondCandle.Open < secondCandle.Close)
            {
                if((lastCandle.Open - lastCandle.Close)/3 >
                    secondCandle.Close - secondCandle.Open)
                {
                    if(candles[candles.Count - 5].Low < lastCandle.Low)
                    {
                        TabsSimple[0].SellAtLimit(Volume, lastCandle.Close - TabsSimple[0].Securiti.PriceStep * 5);
                    }
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
                    Stop = Convert.ToInt32(reader.ReadLine());
                    Profit = Convert.ToInt32(reader.ReadLine());
                    StopSleepage = Convert.ToInt32(reader.ReadLine());
                    ProfitSleepage = Convert.ToInt32(reader.ReadLine());
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
                    writer.WriteLine(Stop);
                    writer.WriteLine(Profit);
                    writer.WriteLine(StopSleepage);
                    writer.WriteLine(ProfitSleepage);
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


        public override string GetNameStrategyType()
        {
            return nameof(BearAbsorptionBot);
        }

        public override void ShowIndividualSettingsDialog()
        {
            var ui = new BearAbsorptionBotUi(this);
            ui.ShowDialog();
        }
    }
}
