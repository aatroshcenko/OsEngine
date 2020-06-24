using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using System;
using System.Linq;
using System.Xml;

namespace OsEngine.Robots.BotMarketDepth
{
    public class BotMarketDepth : BotPanel
    {
        public BotMarketDepth(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);

            TabsSimple[0].MarketDepthUpdateEvent += BotMarketDepth_MarketDepthUpdateEvent;
            TabsSimple[0].ServerTimeChangeEvent += BotMarketDepth_ServerTimeChangeEvent;
        }

        private void BotMarketDepth_ServerTimeChangeEvent(DateTime serverTime)
        {
            var positions = TabsSimple[0].PositionsOpenAll;
            if (positions == null ||
                !positions.Any())
            {
                return;
            }

            if(positions[0].State != PositionStateType.Open)
            {
                return;
            }

            if(positions[0].TimeOpen.AddSeconds(20) < serverTime)
            {
                TabsSimple[0].CloseAtMarket(positions[0], positions[0].OpenVolume);
            }
        }

        private void BotMarketDepth_MarketDepthUpdateEvent(MarketDepth marketDepth)
        {
            var positions = TabsSimple[0].PositionsOpenAll;

            if(positions != null &&
                positions.Any())
            {
                return;
            }

            if(marketDepth.AskSummVolume*5 < marketDepth.BidSummVolume)
            {
                TabsSimple[0].BuyAtMarket(1);
            }
        }

        public override string GetNameStrategyType()
        {
            return nameof(BotMarketDepth);
        }

        public override void ShowIndividualSettingsDialog()
        {
            throw new System.NotImplementedException();
        }
    }
}
