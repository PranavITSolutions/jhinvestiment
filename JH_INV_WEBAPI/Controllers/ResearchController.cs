using JH_INV_WEBAPI.Models;
using JH_INV_WEBAPI.Repository;
using JH_INV_WEBAPI.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace JH_INV_WEBAPI.Controllers
{
    public class ActivityController : ApiController
    {

        [Route("api/get/research")]
        public async Task<HttpResponseMessage> GetResearchList()
        {
            HttpResponseMessage response = null;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_001", "Success", ResearchRepo.getResearchDocuments()));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new JHResponseMessage("JH_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/get/research/{id?}")]
        public async Task<HttpResponseMessage> GetActivityById(string id)
        {
            HttpResponseMessage response = null;
            try
            {
                if (id != null)
                {
                    Research research = await ResearchRepo.getResearchDocument(id);
                    response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_001", "Success", research));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_104", "Invalid research Id", "Invalid research Id"));
                }

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new JHResponseMessage("JH_101", "Application Error", exception.Message));
            }
            return response;
        }
        [Route("api/get/keywords")]
        public async Task<HttpResponseMessage> GetKeywords()
        {

            HttpResponseMessage response = null;
            try
            {
                List<KeywordResult> keywordList = await ResearchRepo.getKeywords();
                response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_001", "Success", keywordList));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new JHResponseMessage("JH_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/get/customer/perf")]
        public async Task<HttpResponseMessage> GetCustomerPerformance()
        {
            HttpResponseMessage response = null;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_001", "Success", ResearchRepo.getCustomerPerformance()));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new JHResponseMessage("JH_101", "Application Error", exception.Message));
            }
            return response;
        }

        //from here it is my data --Avinash
        [Route("api/get/customer/analytics")]
        public async Task<HttpResponseMessage> GetCustomerRecords()
        {
            HttpResponseMessage response = null;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_001", "Success", ResearchRepo.getCustomerRecords()));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new JHResponseMessage("JH_101", "Application Error", exception.Message));
            }
            return response;
        }
        // from here you have to host
        [Route("api/get/stock/performance/{isin?}")]
        public async Task<HttpResponseMessage> GetStockPerformanceById(string isin)
        {
            HttpResponseMessage response = null;
            try
            {
                if (isin != null)
                {
                    var research = ResearchRepo.getStockPerformanceById(isin);
                    response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_001", "Success", research));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_104", "Invalid research Id", "Invalid research Id"));
                }

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new JHResponseMessage("JH_101", "Application Error", exception.Message));
            }
            return response;
        }
        [Route("api/get/analyst/perf")]
        public async Task<HttpResponseMessage> GetAnalystRecords()
        {
            HttpResponseMessage response = null;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_001", "Success", ResearchRepo.getAnalystPerformance()));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new JHResponseMessage("JH_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/get/analysts/count")]
        public async Task<HttpResponseMessage> GetAnalyticsCount()
        {
            HttpResponseMessage response = null;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_001", "Success", ResearchRepo.getAnalyticsCount()));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new JHResponseMessage("JH_101", "Application Error", exception.Message));
            }
            return response;
        }
        [Route("api/get/analyst/performance/{isin?}/{name?}")]
        public async Task<HttpResponseMessage> GetAnalystPerformanceById(string isin, string name)
        {
            HttpResponseMessage response = null;
            try
            {
                if (isin != null && name != null)
                {
                    var research = ResearchRepo.getAnalystPerformanceById(isin, name);
                    response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_001", "Success", research));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_104", "Invalid research Id", "Invalid research Id"));
                }

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new JHResponseMessage("JH_101", "Application Error", exception.Message));
            }
            return response;
        }



        [Route("api/get/sentiment/details/{From?}/{To?}")]
        public async Task<HttpResponseMessage> GetSentimentScoreChart(DateTime From, DateTime To)
        {
            HttpResponseMessage response = null;
            try
            {
                if (From != null && To != null)
                {
                    var research = ResearchRepo.getSentimentScoreChart(From, To);
                    response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_001", "Success", research));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_104", "Invalid research Id", "Invalid research Id"));
                }

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new JHResponseMessage("JH_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/get/sentiment/chart/{From?}/{To?}")]
        public async Task<HttpResponseMessage> GetSentimentScoreChartDetails(DateTime From, DateTime To)
        {
            HttpResponseMessage response = null;
            try
            {
                if (From != null && To != null)
                {
                    var research = ResearchRepo.getSentimentScoreChartDetails(From, To);
                    response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_001", "Success", research));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new JHResponseMessage("JH_104", "Invalid research Id", "Invalid research Id"));
                }

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new JHResponseMessage("JH_101", "Application Error", exception.Message));
            }
            return response;
        }
    }
}
