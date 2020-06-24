using OsEngine.Charts.CandleChart.Indicators;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsEngine.Robots.IndicatorBot
{
    public class MultipalIndicatorBot : BotPanel
    {
        private MovingAverage _moving;

        private Atr _atr;

        private Envelops _envelops;

        public MultipalIndicatorBot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);

            _moving =(MovingAverage)TabsSimple[0].CreateCandleIndicator(new MovingAverage("moving1", false), "Prime");
            _moving.Save();

            _atr = (Atr)TabsSimple[0].CreateCandleIndicator(new Atr("atr1", false), "NewArea");
            _atr.Save();

            _envelops = (Envelops)TabsSimple[0].CreateCandleIndicator(new Envelops("envelops1", false), "Prime");
            _envelops.Save();

            TabsSimple[0].CandleFinishedEvent += MultipalIndicatorBot_CandleFinishedEvent;

        }

        private void MultipalIndicatorBot_CandleFinishedEvent(List<Candle> candles)
        {
            if(_moving.Lenght > candles.Count ||
                _atr.Lenght > candles.Count)
            {
                return;
            }

            //if(candles[candles.Count - 1].TimeStart.Hour < 11)
            //{
            //    return;
            //}

            //входим в лонг, когда закрытие свечи выше верхнего значаения конверта
            //выходим, когда закрытие свечи ниже мувинг - атр*2

            List<Position> positions = TabsSimple[0].PositionsOpenAll;

            if(positions != null &&
                positions.Count != 0)
            {
                if(positions[0].State != PositionStateType.Open)
                {
                    return;
                }
            }

            if(positions == null ||
                positions.Count == 0)
            {
                if(candles[candles.Count - 1].Close > _envelops.ValuesUp[_envelops.ValuesUp.Count - 1])
                {
                    TabsSimple[0].BuyAtLimit(1, candles[candles.Count - 1].Close);
                }
            }
            else
            {
                if(candles[candles.Count - 1].Close < 
                    _moving.Values[_moving.Values.Count - 1] - _atr.Values[_atr.Values.Count - 1] * 2)
                {
                    TabsSimple[0].CloseAtLimit(positions[0], candles[candles.Count - 1].Close, positions[0].OpenVolume);
                }
            }
        }

        public override string GetNameStrategyType()
        {
            return nameof(MultipalIndicatorBot);
        }

        public override void ShowIndividualSettingsDialog()
        {
        }
    }
}
