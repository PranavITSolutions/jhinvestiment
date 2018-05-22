using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JH_INV_WEBAPI.Utility
{
    public class Constants
    {

     
       // public const string ACTION_CHECK_SCORE = "Check Sentiment Score";
        public const string ACTION_CHECK_STOCK_PERFORMANCE = "Check Stock Performance";

        public const string ACTION_CHECK_ANALYST_PERFORMANCE = "Check Analyst Performance";

        public const string ACTION_HIGH_PERFORMING_STOCKS = "High Performing Stocks";

        public const string ACTION_STOCKS_TO_FOCUS = "Stocks To Focus";

        public const string ACTION_COMPARE_STOCKS = "Compare Stocks";

        public const string ACTION_INVESTING_GUIDE = "JH Investing Guide";

        public static List<String> ACTION_CHECK_SCORE_OPTIONS = new List<string> { "Ticker", "Analyst"};
        public static List<String> CONFIRMATION_OPTIONS = new List<string> { "Yes", "No" };

        public const string ACTION_CHECK_SCORE_OPTION_TICKER = "Ticker";
        public const string ACTION_CHECK_SCORE_OPTION_ANALYST = "Analyst";

        public const string CONFIRMATION_YES = "yes";
        public const string CONFIRMATION_NO = "no";

        public const string INTENT_GREETING = "Greeting";
        public const string INTENT_CHECK_STOCK_PERFORMANCE = "CheckStockPerformance";
        public const string INTENT_CHECK_ANALYST_PERFORMANCE = "CheckAnalystPerformance";
        public const string INTENT_HIGH_PERFORMING_STOCKS = "HighPerformingStock";
        public const string INTENT_STOCKS_TO_FOCUS = "StockToFocus";
        public const string INTENT_COMPARE_STOCKS = "CompareStocks";

        public const string INTENT_INVESTING_GUIDE = "JHInvestingGuide";
        public const string INTENT_MARKET_INSIGHTS = "MarketInsights";

        public const string INTENT_THANK = "Thank";
        public const string INTENT_BYE = "Bye";
        public const string INTENT_NONE = "None";
        public const string INTENT_SCHEDULE_MEETING = "ScheduleMeeting";


        public const string TIME_PERIOD_CHOICE_SIX_MONTH = "For last 6 months";

        public const string TIME_PERIOD_CHOICE_ONE_YEAR = "For last 1 year";

        public const string TIME_PERIOD_CHOICE_TWO_YEAR = "For last 2 years";



        //CRP BOT Constants
        public static string DB_CONNECTION_STR_NAME = "DBConnString";
        public static string JH_LOGO = "https://callcenterfunct82f0.blob.core.windows.net/logo/jh-logo.png";

        public static List<String> ACTIONS = new List<string> { "Get Performance", "Get MorningStar Ratings",
            "Get Pricing", "Get Factsheet", "Get Portfolio Commentary", "Get Annual Report", "Get Fund Holdings" };


        public static List<String> FUNDS = new List<string> { "High Yield Fund", "Global Equity Income Fund",
                                                              "Flexible Bond Fund", "Multi Sector Income Fund",
                                                              "Strategic Income Fund"};

        public const string ACTION_GET_PERFORMANCE = "Get Performance";
        public const string ACTION_GET_RATING = "Get MorningStar Ratings";
        public const string ACTION_GET_PRICING = "Get Pricing";
        public const string ACTION_GET_FACTSHEET = "Get Factsheet";
        public const string ACTION_GET_PORTFOLIO = "Get Portfolio Commentary";
        public const string ACTION_GET_REPORT = "Get Annual Report";
        public const string ACTION_GET_HOLDING = "Get Fund Holdings";
        public const string ACTION_SCHEDULE_MEETING = "SetUp Meeting";

        public const string ACTION_GET_DIVIDEND_STUDY = "JH's global dividend study";
        public const string ACTION_GET_MARKET_INSIGHTS = "Market Insights";

        public const string FACTSHEET = "FACTSHEET";
        public const string PORTFOLIO = "PORTFOLIO";
        public const string ANNUALREPORT = "ANNUALREPORT";
        public const string FUNDHOLDING = "FUNDHOLDING";

        public const string FUND_HIGH_YIELD = "High Yield Fund";
        public const string FUND_GLOBAL_EQUITY = "Global Equity Income Fund";
        public const string FUND_FLEXIBLE_BOND = "Flexible Bond Fund";
        public const string FUND_MULTISECTOR = "Multi Sector Income Fund";
        public const string FUND_STRATEGIC = "Strategic Income Fund";

        public const string REMEMBER_MEETING_TITLE = "meeting_title";
        public const string REMEMBER_MEETING_TIMES = "meeting_time";
        public static List<SendGrid.Helpers.Mail.EmailAddress> EMAILS = new List<SendGrid.Helpers.Mail.EmailAddress>
                    {
                        new SendGrid.Helpers.Mail.EmailAddress("paul.algreen@janushenderson.com","Paul Algreen"),
                        new SendGrid.Helpers.Mail.EmailAddress("cyril.obot@janushenderson.com", "Cyril Obot"),
                        new SendGrid.Helpers.Mail.EmailAddress("cesunder@microsoft.com","Cecil Sunder")
                    };


        //public static List<SendGrid.Helpers.Mail.EmailAddress> EMAILS = new List<SendGrid.Helpers.Mail.EmailAddress>
        //            {
        //                new SendGrid.Helpers.Mail.EmailAddress("cesunder@microsoft.com","Cecil Sunder"),
        //                new SendGrid.Helpers.Mail.EmailAddress("abinayas@cloudix.io", "Abinaya")
        //            };


        public const string REMEMBER_PERIOD = "period";
        public const string REMEMBER_NUMBER = "number";
        public const string REMEMBER_TICKER = "ticker";
        public const string REMEMBER_TICKERS = "tickers";
        public const string REMEMBER_ANALYST = "analyst";
        public const string REMEMBER_LAST_INTENT = "lastIntent";
    }
}