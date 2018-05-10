using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JH_INV_WEBAPI.Models
{
    public class FinanceModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string ticker { get; set; }
        public string ms_category { get; set; }
        public decimal nav { get; set; }
        public decimal nav_change_per { get; set; }
        public decimal nav_change_price { get; set; }
        public decimal premium_discount { get; set; }

        public int rating_count { get; set; }
        public int rating_three_yr_count { get; set; }
        public int rating_five_yr_count { get; set; }
        public int rating_ten_yr_count { get; set; }

        public decimal overall_rating { get; set; }
        public decimal rating_three_yr { get; set; }
        public decimal rating_five_yr { get; set; }
        public decimal rating_ten_yr { get; set; }

        public string fact_sheet_url { get; set; }
        public string portfolio_commentary_url { get; set; }
        public string annual_report_url { get; set; }
        public string full_holdings_url { get; set; }

        public decimal market_price { get; set; }
        public decimal ytd { get; set; }
        public decimal since_inception { get; set; }
        public decimal sec_yield_waivers { get; set; }
        public decimal sec_yield_wo_waivers { get; set; }
        public decimal expense_ratio_gross { get; set; }
        public decimal expense_ratio_net { get; set; }
    }
}