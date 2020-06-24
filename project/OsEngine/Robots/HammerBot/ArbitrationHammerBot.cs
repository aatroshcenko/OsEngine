using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsEngine.Robots.HammerBot
{
    public class ArbitrationHammerBot : BotPanel
    {
        private const int LimitCandlesCount = 20;

        private int candlesCountTab1;
        private int candlesCountTab2;
        public ArbitrationHammerBot(string name, StartProgram startProgram) : base(
            name,
            startProgram)
        {
            TabCreate(BotTabType.Simple); //создание вкладки;
            TabCreate(BotTabType.Simple);

            TabsSimple[0].CandleFinishedEvent += ArbitrationHammerBotTab1_CandleFinishedEvent;
            TabsSimple[1].CandleFinishedEvent += ArbitrationHammerBotTab2_CandleFinishedEvent;

            TabsSimple[0].PositionOpeningSuccesEvent += ArbitrationHammerBotTab1_CandleOpeningSuccessEvent;
            TabsSimple[1].PositionOpeningSuccesEvent += ArbitrationHammerBotTab2_CandleOpeningSuccessEvent;

        }

        private void ArbitrationHammerBotTab2_CandleFinishedEvent(List<Candle> candlesTab2)
        {
            if (TabsSimple[1].PositionsOpenAll != null &&
                TabsSimple[1].PositionsOpenAll.Any())
            {
                if (candlesTab2.Count - candlesCountTab2 >= LimitCandlesCount)
                {
                    TabsSimple[1].CloseAllAtMarket();
                }
                return;
            }

            List<Candle> candlesTab1 = TabsSimple[0].CandlesFinishedOnly;
            if(candlesTab2.Last().TimeStart == candlesTab1.Last().TimeStart)
            {
                TradeLogic(candlesTab1, candlesTab2);
            }
        }

        private void ArbitrationHammerBotTab1_CandleOpeningSuccessEvent(Position position)
        {
            TabsSimple[0].CloseAtStop(
                position,
                position.EntryPrice + TabsSimple[0].Securiti.PriceStep * 50,
                position.EntryPrice + TabsSimple[0].Securiti.PriceStep * 40);
        }

        private void ArbitrationHammerBotTab2_CandleOpeningSuccessEvent(Position position)
        {
            TabsSimple[1].CloseAtStop(
                position,
                position.EntryPrice - TabsSimple[0].Securiti.PriceStep * 50,
                position.EntryPrice - TabsSimple[0].Securiti.PriceStep * 60);
        }

        private void ArbitrationHammerBotTab1_CandleFinishedEvent(List<Candle> candlesTab1)
        {
            if (TabsSimple[0].PositionsOpenAll != null &&
                TabsSimple[0].PositionsOpenAll.Any())
            {
                if (candlesTab1.Count - candlesCountTab1  >= LimitCandlesCount)
                {
                    TabsSimple[0].CloseAllAtMarket();
                }
                return;
            }

            List<Candle> candlesTab2 = TabsSimple[1].CandlesFinishedOnly;
            if (candlesTab1.Last().TimeStart == candlesTab2.Last().TimeStart)
            {
                TradeLogic(candlesTab1, candlesTab2);
            }
        }

        private void TradeLogic(List<Candle> candlesTab1, List<Candle> candlesTab2)
        {
            if (candlesTab1.Count < 21 && candlesTab2.Count < 21)
            {//если свечей меньше 21, то не входим.
                return;
            }

            if (candlesTab1.Last().Close <= candlesTab1.Last().Open &&
                candlesTab2.Last().Close <= candlesTab2.Last().Open)
            {
                //если последняя свеча не растущая
                return;
            }
            //проверяем чтобы последний лой был самой нижней точкой за последние 20 свечек

            decimal lastLowTab1 = candlesTab1.Last().Low;
            decimal lastLowTab2 = candlesTab2.Last().Low;
            int i;
            int j = candlesTab2.Count - 1;
            for (i = candlesTab1.Count - 1; i > candlesTab1.Count - 20; i--, j--)
            {
                if (lastLowTab1 > candlesTab1[i].Low &&
                    lastLowTab2 > candlesTab2[j].Low)
                {
                    return;
                }
            }

            //проверяем чтобы тело было в три раза меньше хвоста снизу и не было больше хвоста сверху

            Candle candle1 = candlesTab1.Last();
            decimal body1 = candle1.Close - candle1.Open;
            decimal shadowLow1 = candle1.Open - candle1.Low;
            decimal shadowHigh1 = candle1.High - candle1.Close;

            Candle candle2 = candlesTab2.Last();
            decimal body2 = candle2.Close - candle2.Open;
            decimal shadowLow2 = candle2.Open - candle2.Low;
            decimal shadowHigh2 = candle2.High - candle2.Close;

            if (body1 < shadowHigh1 && 
                body2 > shadowHigh2 /3 )
            {
                return;
            }

            if (shadowLow1 / 3 < body1 &&
                body2 < shadowLow1)
            {
                return;
            }

            TabsSimple[0].BuyAtLimit(1, TabsSimple[0].PriceBestBid);//открываем позицию
            TabsSimple[1].SellAtLimit(1, TabsSimple[1].PriceBestAsk);

            candlesCountTab1 = candlesTab1.Count;
            candlesCountTab2 = candlesTab2.Count;
        }

        public override string GetNameStrategyType()
        {
            return nameof(ArbitrationHammerBot);
        }

        public override void ShowIndividualSettingsDialog()
        {
        }
    }
}
