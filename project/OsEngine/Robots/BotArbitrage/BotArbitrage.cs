using OsEngine.Charts.CandleChart.Indicators;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsEngine.Robots.BotArbitrage
{
    public class BotArbitrage : BotPanel
    {
        private IvashovRange _range;

        private MovingAverage _moving;
        public BotArbitrage(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Index);

            _range = (IvashovRange)TabsIndex[0].CreateCandleIndicator(new IvashovRange("range", false), "RangeArea");
            _range.Save();

            _moving = (MovingAverage)TabsIndex[0].CreateCandleIndicator(new MovingAverage("moving", false), "Prime");
            _moving.Save();

            TabCreate(BotTabType.Simple);

            TabsSimple[0].CandleFinishedEvent += BotArbitrage_CandleFinishedEvent;
            TabsIndex[0].SpreadChangeEvent += BotArbitrage_SpreadChangeEvent;
        }

        private void BotArbitrage_SpreadChangeEvent(List<Candle> candles)
        {
            var candleTabs = TabsSimple[0].CandlesFinishedOnly;

            if(candles[candles.Count - 1].TimeStart == candleTabs[candleTabs.Count - 1].TimeStart)
            {
                TradeLogic(candles, candleTabs);
            }
        }

        private void BotArbitrage_CandleFinishedEvent(List<Candle> candleTabs)
        {
            var candles = TabsSimple[0].CandlesFinishedOnly;

            if (candles[candles.Count - 1].TimeStart == candleTabs[candleTabs.Count - 1].TimeStart)
            {
                TradeLogic(candles, candleTabs);
            }
        }

        private void TradeLogic(List<Candle> indexs, List<Candle> tabCandles)
        {
            if(_range.LenghtAverage > tabCandles.Count + 5 ||
                _range.LenghtMa > tabCandles.Count + 5)
            {
                return;
            }

            var positions = TabsSimple[0].PositionsOpenAll;

            if (!positions.Any())
            {// открытие позиции
                if(indexs[indexs.Count - 1].Close > 
                    _moving.Values[_moving.Values.Count - 1] + _range.Values[_range.Values.Count - 1])
                {//шорт. Находимся выше канала среднеквадратичного отклонения
                    TabsSimple[0].SellAtLimit(1, tabCandles[tabCandles.Count - 1].Close);
                }else if(indexs[indexs.Count - 1].Close < 
                    _moving.Values[_moving.Values.Count - 1] - _range.Values[_range.Values.Count - 1])
                {//лонг
                    TabsSimple[0].BuyAtLimit(1, tabCandles[tabCandles.Count - 1].Close);
                }
            }
            else
            {// закрытие позиции
                if(positions[0].State != PositionStateType.Open)
                {
                    return;
                }

                if (positions[0].Direction == Side.Buy && indexs[indexs.Count - 1].Close >
                    _moving.Values[_moving.Values.Count - 1] + _range.Values[_range.Values.Count - 1])
                {//
                    TabsSimple[0].CloseAtLimit(positions[0], tabCandles[tabCandles.Count - 1].Close, positions[0].OpenVolume);
                }
                else if (positions[0].Direction == Side.Sell && indexs[indexs.Count - 1].Close <
                   _moving.Values[_moving.Values.Count - 1] - _range.Values[_range.Values.Count - 1])
                {//
                    TabsSimple[0].CloseAtLimit(positions[0], tabCandles[tabCandles.Count - 1].Close, positions[0].OpenVolume);
                }
            }


        }

        public override string GetNameStrategyType()
        {
            return nameof(BotArbitrage);
        }

        public override void ShowIndividualSettingsDialog()
        {
        }
    }
}
