using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JH_INV_WEBAPI.Models
{
    public class Research
    {
        public string ResearchId { get; set; }
        public string Customer_Name { get; set; }
        public string Research_Industry { get; set; }
        public string Research_Sector { get; set; }
        public string BB_UNIQUE_ID { get; set; }
        public string CUSIP { get; set; }
        public string ISIN { get; set; }
        public string SEDOL { get; set; }
        public int SECURITY_ALIAS { get; set; }
        public string TICKER { get; set; }
        public string EXCH_CODE { get; set; }
        public string TICKER_AND_EXCH_CODE { get; set; }
        public string Source { get; set; }
        public string Email { get; set; }
        public string Symbol { get; set; }
        public object Description { get; set; }
        public string ResearchTeam { get; set; }
        public string Category { get; set; }
        public string ResearchSector { get; set; }
        public DateTime CREATE_TS { get; set; }
        public DateTime UPDATE_TS { get; set; }
        public int AttachmentSize { get; set; }
        public string AttachmentType { get; set; }
        public decimal? Sentiment_Score { get; set; }
        public string Keywords { get; set; }
        public int Is_Processed { get; set; }
        public decimal? SummarySentimentScore { get; set; }
        public string SummaryKeywords { get; set; }
        public decimal? AttachmentSentimentScore { get; set; }
        public string AttachmentKeywords { get; set; }
        public string id { get; set; }
    }
}