using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JH_INV_WEBAPI.Utility
{
    public class Constants
    {

        public const string ACTION_CHECK_SCORE = "Check Sentiment Score";

        public static List<String> ACTION_CHECK_SCORE_OPTIONS = new List<string> { "Customer", "Agent", "Business Unit","Product" };
        public static List<String> CONFIRMATION_OPTIONS = new List<string> { "Yes", "No" };

        public const string ACTION_CHECK_SCORE_OPTION_CUSTOMER = "Customer";
        public const string ACTION_CHECK_SCORE_OPTION_AGENT = "Agent";
        public const string ACTION_CHECK_SCORE_OPTION_BU = "Business Unit";
        public const string ACTION_CHECK_SCORE_OPTION_PRODUCT = "Product";

        public const string CONFIRMATION_YES = "yes";
        public const string CONFIRMATION_NO = "no";

        public const string INTENT_GREETING = "Greeting";
        public const string INTENT_CHECK_SENTIMENT_SCORE = "CheckSentimentScore";
        public const string INTENT_THANK = "Thank";
        public const string INTENT_BYE = "Bye";
        public const string INTENT_NONE = "None";
    }
}