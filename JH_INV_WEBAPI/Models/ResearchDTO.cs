using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JH_INV_WEBAPI.Models
{
    public class ResearchDTO
    {
        public string researchId { get; set; }
        public string customerName { get; set; }
        public string researchIndustry { get; set; }
        public string researchSector { get; set; }
        public string ticker { get; set; }

        public string stockName { get; set; }

        public string source { get; set; }
        public string email { get; set; }
        public string researchTeam { get; set; }
        public string category { get; set; }
        public int researchCount { get; set; }
        public decimal? sentimentScore { get; set; }

    }
}