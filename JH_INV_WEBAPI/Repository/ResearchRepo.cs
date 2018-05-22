using Dapper;
using JH_INV_WEBAPI.Models;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace JH_INV_WEBAPI.Repository
{
    public class ResearchRepo
    {
        private static DocumentClient client = new DocumentClient(new Uri(WebConfigurationManager.AppSettings["CosmosDBEndpoint"]), WebConfigurationManager.AppSettings["CosmosDBApiKey"]);
        private static string database = WebConfigurationManager.AppSettings["Database"];
        private static string collection = WebConfigurationManager.AppSettings["Collection"];

        public static async Task<bool> insertResearchDocument(Research document)
        {
            try
            {
                await client.CreateDocumentAsync(
            UriFactory.CreateDocumentCollectionUri(database, collection), document);
                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying DB: {0}\r\n", exception.Message.ToString());
                throw exception;
            }
        }


        public static async Task<bool> updateResearchDocument(Research document)
        {
            try
            {
                await client.ReplaceDocumentAsync(
            UriFactory.CreateDocumentUri(database, collection, document.id), document);
                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying DB: {0}\r\n", exception.Message.ToString());
                throw exception;
            }
        }

        public static async Task<Research> getResearchDocument(string id)
        {
            try
            {
                Research result = await client.ReadDocumentAsync<Research>(UriFactory.CreateDocumentUri(database, collection, id));
                return result;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying DB: {0}\r\n", exception.Message.ToString());
                //throw exception;
                return null;
            }

        }

        public static List<Research> getResearchDocuments()
        {
            try
            {
                // return await client.ReadDocumentFeedAsync(UriFactory.CreateDocumentCollectionUri(database, collection), new FeedOptions { MaxItemCount = 10 });
                return client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection)).Take(20).ToList();
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying DB: {0}\r\n", exception.Message.ToString());
                throw exception;
            }
        }

        public static async Task<List<KeywordResult>> getKeywords()
        {
            try
            {
                var query = from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                            where d.Keywords != null
                            select d.Keywords;
                //   return query.ToList().SelectMany(x=>x).ToList().GroupBy(g => g).ToDictionary(x => x.Key, x => x.Count()); 

                return query.ToList().SelectMany(x => x.Split(',')).ToList().GroupBy(g => g).Select(s => new KeywordResult
                {
                    text = s.Key,
                    weight = s.Count()
                }).ToList().Where(w => w.text.Length > 15).OrderByDescending(o => o.weight).Take(50).ToList();
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying index: {0}\r\n", exception.Message.ToString());
                throw exception;
            }
        }

        public static dynamic getCustomerPerformance()
        {
            try
            {
                var query = from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                            select d;
                //where d.Keywords != null
                //             group d by new
                //            {
                //             d.TICKER,
                //             d.Customer_Name,
                //             d.Research_Industry,
                //            } into result
                //            select new { TICKER = result.Key.TICKER, Customer_Name = result.Key.Customer_Name,
                //                Research_Industry = result.Key.Research_Industry, Score = result.Select(s=>s.Sentiment_Score).Average()};

                return query.ToList().GroupBy(x => new { x.TICKER, x.Customer_Name, x.Research_Industry, x.ISIN })
                    .Select(y => new
                    {
                        isin = y.Key.ISIN,
                        ticker = y.Key.TICKER,
                        customerName = y.Key.Customer_Name,
                        researchIndustry = y.Key.Research_Industry,
                        researchCount = y.ToList().Count(),
                        score = y.ToList().Select(s => s.Sentiment_Score).Average()
                    }
                    );
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying index: {0}\r\n", exception.Message.ToString());
                throw exception;
            }
        }


        //from here it is my data --Avinash.
        public static dynamic getCustomerRecords()
        {
            //Customer, Analyst/Source, Industry, Ticker, Research Sector, Research Team, Email, Last Updated Date, Sentiment Score 
            try
            {
                var query = from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                            select d;

                return query.ToList().GroupBy(x => new {
                    x.TICKER,
                    x.Customer_Name,
                    x.Research_Industry,
                    x.Research_Sector,
                    x.Email,
                    x.ResearchTeam,
                    x.UPDATE_TS,
                    x.Source,
                    x.ResearchSector
                })
                    .Select(y => new
                    {
                        //securityLongDesc= y.Key.SECURITY_LONG_DESC,
                        researchIndustry = y.Key.Research_Industry,
                        reasearch_Sector = y.Key.Research_Sector,
                        customerName = y.Key.Customer_Name,
                        researchTeam = y.Key.ResearchTeam,
                        researchSector = y.Key.ResearchSector,
                        ticker = y.Key.TICKER,
                        email = y.Key.Email,
                        lastUpdatedDate = y.Key.UPDATE_TS,
                        source = y.Key.Source,
                        score = y.ToList().Select(s => s.Sentiment_Score).Average()
                    }
                    );
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying index: {0}\r\n", exception.Message.ToString());
                throw exception;
            }
        }
        //from here have to host
        public static dynamic getStockPerformanceById(string isin)
        {
            try
            {
                var query = from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                                //where d.ISIN == isin
                            select d;

                return query.ToList().Where(e => e.ISIN.Equals(isin));
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying index: {0}\r\n", exception.Message.ToString());
                throw exception;
            }
        }

        public static dynamic getAnalystPerformance()
        {
            try
            {
                var query = from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                            select d;

                return query.ToList().GroupBy(x => new { x.TICKER, x.Customer_Name, x.Research_Industry, x.ISIN, x.Source, x.ResearchTeam })
                    .Select(y => new
                    {
                        researchTeam = y.Key.ResearchTeam,
                        source = y.Key.Source,
                        isin = y.Key.ISIN,
                        customerName = y.Key.Customer_Name,
                        researchCount = y.ToList().Count(),
                        score = y.ToList().Select(s => s.Sentiment_Score).Average()
                    }
                    );
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying index: {0}\r\n", exception.Message.ToString());
                throw exception;
            }
        }

        //To get analysts, Research Count and Sentiment Scores
        public static dynamic getAnalyticsCount()
        {
            try
            {
                //var source = from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection)).ToList()
                //             .GroupBy(x => new { x.Customer_Name, x.Source }).Select(g => g.First())
                //             select d;


                //var researchCount = from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection)).ToList()
                //            select d;

                //var score = from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection)).GroupBy(x => new { x.Customer_Name, x.Source })
                //                        .Select(y => new { score = y.ToList().Select(s => s.Sentiment_Score).Average() })
                //            select d;
                //return "{" +"source" + ":" + source.Count() +","+ "researchCount" + ":" + researchCount.Count() + "," +"score"+":"+ score +"}";

                var query = from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection)).ToList()
                            .GroupBy(x => new { x.Customer_Name, x.Source }).Select(y => new { score = y.ToList().Select(s => s.Sentiment_Score).Average() })
                            select d;
                return query;

            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying index: {0}\r\n", exception.Message.ToString());
                throw exception;
            }
        }
        //Analyst values
        public static dynamic getAnalystPerformanceById(string isin, string name)
        {
            try
            {
                var query = from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                                //.Where(e => e.ISIN.Equals(isin)).Where(e => e.Customer_Name.Equals(name))
                            where (d.ISIN.Equals(isin)) && (d.Source.Equals(name))
                            select d;

                return query.ToList();
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying index: {0}\r\n", exception.Message.ToString());
                throw exception;
            }
        }

        public static dynamic getSentimentScoreChart(DateTime From, DateTime To)
        {
            using (IDbConnection db = new SqlConnection((ConfigurationManager.ConnectionStrings["AzureSqlConnString"].ConnectionString)))
            {
                //db.Open();
                var query = "SpGetSentimentScoreChart";

                // return db.Query<StockPrice>(query).SingleOrDefault();

                var parameters = new DynamicParameters();
                db.Open();
                parameters.Add("@start", From);
                parameters.Add("@end", To);
                //parameters.Add("@RESULT", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                return db.Query<StockPrice>(query, parameters, commandType: CommandType.StoredProcedure).ToList();

            }
        }

        public static dynamic getSentimentScoreChartDetails(DateTime From, DateTime To)
        {
            using (IDbConnection db = new SqlConnection((ConfigurationManager.ConnectionStrings["AzureSqlConnString"].ConnectionString)))
            {
                //db.Open();
                var query = "SpGetSentimentScoreChartDetails";

                // return db.Query<StockPrice>(query).SingleOrDefault();

                var parameters = new DynamicParameters();
                db.Open();
                parameters.Add("@start", From);
                parameters.Add("@end", To);
                // parameters.Add("@RESULT", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                return db.Query<StockPrice>(query, parameters, commandType: CommandType.StoredProcedure).ToList().OrderBy(o => o.Date);
            }
        }


    }
}
