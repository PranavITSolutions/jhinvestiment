using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JH_INV_WEBAPI.Models
{
    public class StockPrice
    {
        public string Rating { get; set; }
        public string TickerName { get; set; }
        public string Ticker { get; set; }
        public string AnalystName { get; set; }
        public DateTime Date { get; set; }
        public decimal SentimentScore { get; set; }
        public decimal ExternalSentimentScore { get; set; }
        public decimal Performance { get; set; }
    }
}