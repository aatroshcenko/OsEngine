﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsEngine.Market.Servers.MFD
{
    public class MfdServerPermission : IServerPermission
    {
        public ServerType ServerType
        {
            get { return ServerType.MfdWeb; }
        }

        #region DataFeedPermissions
        public bool DataFeedTf1SecondCanLoad
        {
            get { return false; }
        }
        public bool DataFeedTf2SecondCanLoad
        {
            get { return false; }
        }
        public bool DataFeedTf5SecondCanLoad
        {
            get { return false; }
        }
        public bool DataFeedTf10SecondCanLoad
        {
            get { return false; }
        }
        public bool DataFeedTf15SecondCanLoad
        {
            get { return false; }
        }
        public bool DataFeedTf20SecondCanLoad
        {
            get { return false; }
        }
        public bool DataFeedTf30SecondCanLoad
        {
            get { return false; }
        }
        public bool DataFeedTf1MinuteCanLoad
        {
            get { return true; }
        }
        public bool DataFeedTf2MinuteCanLoad
        {
            get { return false; }
        }
        public bool DataFeedTf5MinuteCanLoad
        {
            get { return true; }
        }
        public bool DataFeedTf10MinuteCanLoad
        {
            get { return true; }
        }
        public bool DataFeedTf15MinuteCanLoad
        {
            get { return true; }
        }
        public bool DataFeedTf30MinuteCanLoad
        {
            get { return true; }
        }
        public bool DataFeedTf1HourCanLoad
        {
            get { return true; }
        }
        public bool DataFeedTf2HourCanLoad
        {
            get { return false; }
        }
        public bool DataFeedTf4HourCanLoad
        {
            get { return false; }
        }
        public bool DataFeedTfTickCanLoad
        {
            get { return true; }
        }
        public bool DataFeedTfMarketDepthCanLoad
        {
            get { return false; }
        }

        #endregion

        #region Trade permission

        public bool IsTradeServer
        {
            get { return false; }
        }

        #endregion
    }
}
