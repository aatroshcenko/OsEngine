using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsEngine.Robots.BotAllTrades
{
    public class BotAllTrades : BotPanel
    {
        public BotAllTrades(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);

            TabsSimple[0].NewTickEvent += BotAllTrades_NewTickEvent;
            TabsSimple[0].PositionOpeningSuccesEvent += BotAllTrades_PositionOpeningSuccesEvent;
        }

        private void BotAllTrades_PositionOpeningSuccesEvent(Position position)
        {
            TabsSimple[0].CloseAtStop(
                position,
                position.EntryPrice - 20 * TabsSimple[0].Securiti.PriceStep,
                position.EntryPrice - 20 * TabsSimple[0].Securiti.PriceStep);
            TabsSimple[0].CloseAtProfit(
                position,
                position.EntryPrice + 20 * TabsSimple[0].Securiti.PriceStep,
                position.EntryPrice + 20 * TabsSimple[0].Securiti.PriceStep);
        }

        private DateTime _timeTrade;

        private int _countTradesInSecond;

        private void BotAllTrades_NewTickEvent(Trade trade)
        {
            var positions = TabsSimple[0].PositionsOpenAll;

            if(positions != null &&
                positions.Any())
            {
                return;
            }

            if(trade.Time == _timeTrade)
            {
                if(trade.Side == Side.Buy)
                {
                    _countTradesInSecond++;
                }
            }
            else
            {
                _timeTrade = trade.Time;
                _countTradesInSecond = 0;
            }

            if(_countTradesInSecond > 50)
            {
                TabsSimple[0].BuyAtMarket(1);
            }
        }

        public override string GetNameStrategyType()
        {
            throw new NotImplementedException();
        }

        public override void ShowIndividualSettingsDialog()
        {
            throw new NotImplementedException();
        }
    }
}
