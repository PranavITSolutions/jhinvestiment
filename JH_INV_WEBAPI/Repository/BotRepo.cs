﻿using Dapper;
using JH_INV_WEBAPI.Models;
using JH_INV_WEBAPI.Utility;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace JH_INV_WEBAPI.Repository
{
    public class BotRepo
    {
        private static DocumentClient client = new DocumentClient(new Uri(WebConfigurationManager.AppSettings["CosmosDBEndpoint"]), WebConfigurationManager.AppSettings["CosmosDBApiKey"]);
        private static string database = WebConfigurationManager.AppSettings["Database"];
        private static string collection = WebConfigurationManager.AppSettings["Collection"];

        public static FinanceModel GetFinanceByTicker(string ticker)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings[Constants.DB_CONNECTION_STR_NAME].ConnectionString))
                {
                    string query = " SELECT * FROM finance WHERE ticker = @ticker";
                    return db.Query<FinanceModel>(query, new { ticker = ticker }).FirstOrDefault();
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
        }

        public static List<FinanceModel> GetFinanceList()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings[Constants.DB_CONNECTION_STR_NAME].ConnectionString))
                {
                    string query = " SELECT * FROM finance";
                    return db.Query<FinanceModel>(query).ToList();
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
        }




        public static FinanceModel GetFactsheet(string name)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings[Constants.DB_CONNECTION_STR_NAME].ConnectionString))
                {
                    string query = " SELECT * FROM finance WHERE name like '%" + name + "%'";
                    return db.Query<FinanceModel>(query).FirstOrDefault();
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
        }

        public static ResearchDTO getCustomer(string ticker, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                //var results = fromDate != null && toDate != null ?
                //           (from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                //            where d.TICKER == ticker && (d.CREATE_TS > fromDate && d.CREATE_TS < toDate)
                //            select d).ToList() :
                //           (from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                //            where d.TICKER == ticker
                //            select d).ToList();

                var results = (from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                               where d.TICKER == ticker
                               select d).ToList();

                //var query = null;
                //if (fromDate != null && toDate != null)
                //{
                //    query = from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                //            where d.TICKER == ticker && (d.CREATE_TS > fromDate && d.CREATE_TS < toDate)
                //            select d;
                //}
                //else
                //{
                //    query = from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                //            where d.TICKER == ticker
                //            select d;
                //}

                if (fromDate != null && toDate != null)
                {
                    results = results.Where(c => c.CREATE_TS >= fromDate && c.CREATE_TS <= toDate).ToList();
                }

                return results.GroupBy(x => new { x.TICKER, x.Customer_Name, x.Research_Industry, x.Research_Sector })
                    .Select(y => new ResearchDTO
                    {
                        ticker = y.Key.TICKER,
                        stockName = y.Key.Customer_Name,
                        researchIndustry = y.Key.Research_Industry,
                        researchSector = y.Key.Research_Sector,
                        researchCount = y.ToList().Count(),
                        sentimentScore = y.ToList().Select(s => s.Sentiment_Score).Average()
                    }
                    ).FirstOrDefault();
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying index: {0}\r\n", exception.Message.ToString());
                throw exception;
            }
        }

        public static ResearchDTO getAnalyst(string name,DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var results = (from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                            select d).ToList();

                if (fromDate != null && toDate != null)
                {
                    results = results.Where(c => c.CREATE_TS >= fromDate && c.CREATE_TS <= toDate).ToList();
                }
                //var results = fromDate != null && toDate != null ?
                //         (from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                //          where d.CREATE_TS > fromDate && d.CREATE_TS < toDate
                //          select d).ToList() :
                //         (from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))                      
                //          select d).ToList();
                //where d.Keywords != null
                //             group d by new
                //            {
                //             d.TICKER,
                //             d.Customer_Name,
                //             d.Research_Industry,
                //            } into result
                //            select new { TICKER = result.Key.TICKER, Customer_Name = result.Key.Customer_Name,
                //                Research_Industry = result.Key.Research_Industry, Score = result.Select(s=>s.Sentiment_Score).Average()};

                var list = results.Where(c => c.Source.Contains(name) || c.Email.Contains(name) || c.Email.Equals(name) 
                || c.Source.Equals(name) || c.Source.Replace(" ", String.Empty).Equals(name)).ToList();
                return list.GroupBy(x => new { x.Source, x.ResearchSector,x.Email })
                    .Select(y => new ResearchDTO
                    {
                        source = y.Key.Source,
                        email = y.Key.Email,
                        researchCount = y.ToList().Count(),
                        researchSector = y.Key.ResearchSector,
                        //  re = y.ToList().Count(),
                        sentimentScore = y.ToList().Select(s => s.Sentiment_Score).Average()
                    }).FirstOrDefault();
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying index: {0}\r\n", exception.Message.ToString());
                throw exception;
            }
        }

        public static List<ResearchDTO> getCustomerList(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var results = (from d in client.CreateDocumentQuery<Research>(UriFactory.CreateDocumentCollectionUri(database, collection))
                            //where d.TICKER == ticker
                            select d).ToList();

                if (fromDate != null && toDate != null)
                {
                    results = results.Where(c => c.CREATE_TS >= fromDate && c.CREATE_TS <= toDate).ToList();
                }

                return results.GroupBy(x => new { x.TICKER, x.Customer_Name, x.Research_Industry, x.Research_Sector })
                    .Select(y => new ResearchDTO
                    {
                        ticker = y.Key.TICKER,
                        stockName = y.Key.Customer_Name,
                        researchIndustry = y.Key.Research_Industry,
                        researchSector = y.Key.Research_Sector,
                        researchCount = y.ToList().Count(),
                        //  re = y.ToList().Count(),
                        sentimentScore = y.ToList().Select(s => s.Sentiment_Score).Average()
                    }
                    ).ToList();
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error querying index: {0}\r\n", exception.Message.ToString());
                throw exception;
            }
        }


        public static List<ResearchDTO> GetStockPerformance(DateTime fromDate,DateTime toDate)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["AzureSqlConnString"].ConnectionString))
                {
                    List<ResearchDTO> list = db.Query<ResearchDTO>("SpBOTGetStockPerformance", new { start = fromDate, end = toDate },
                 commandType: CommandType.StoredProcedure).ToList();
                    return list;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
        }


        public static List<ResearchDTO> GetAnalystPerformance(DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["AzureSqlConnString"].ConnectionString))
                {
                    List<ResearchDTO> list = db.Query<ResearchDTO>("SpBOTGetAnalystPerformance", new { start = fromDate, end = toDate },
                 commandType: CommandType.StoredProcedure).ToList();
                    return list;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
        }


    }
}