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

        public static List<String> ACTION_CHECK_SCORE_OPTIONS = new List<string> { "Ticker", "Analyst"};
        public static List<String> CONFIRMATION_OPTIONS = new List<string> { "Yes", "No" };

        public const string ACTION_CHECK_SCORE_OPTION_TICKER = "Ticker";
        public const string ACTION_CHECK_SCORE_OPTION_ANALYST = "Analyst";

        public const string CONFIRMATION_YES = "yes";
        public const string CONFIRMATION_NO = "no";

        public const string INTENT_GREETING = "Greeting";
        public const string INTENT_CHECK_STOCK_PERFORMANCE = "CheckStockPerformance";

        public const string INTENT_CHECK_ANALYST_PERFORMANCE = "CheckAnalystPerformance";
        public const string INTENT_THANK = "Thank";
        public const string INTENT_BYE = "Bye";
        public const string INTENT_NONE = "None";






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
    }
}