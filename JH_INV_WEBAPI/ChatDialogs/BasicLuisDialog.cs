using System;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using JH_INV_WEBAPI.Utility;
using System.Diagnostics;
using JH_INV_WEBAPI.Models;
using JH_INV_WEBAPI.Repository;
using AdaptiveCards;

namespace JH_INV_WEBAPI.ChatDialogs
{
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings["LuisAppId"], ConfigurationManager.AppSettings["LuisAPIKey"])))
        {
        }

        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                //context.ConversationData.SetValue("orderNumber", ""); --> To have some data in the current session 
                //context.Activity.From.Id  --> To get user id 

                List<ResearchDTO> researchList = BotRepo.GetStockPerformance(DateTime.Now.Date.AddMonths(-6), DateTime.Now.Date).Take(2).ToList();
                //List<Column> columns = new List<Column>();
                //columns.Add(new Column()
                //{
                //    Size = ColumnSize.Auto,
                //    Items = new List<CardElement>()
                //                                {
                //                                   new TextBlock()
                //                                   {
                //                                       Text = "Ticker",
                //                                       Separation = SeparationStyle.Strong
                //                                   },
                //                                   new TextBlock()
                //                                   {
                //                                       Text = "Stock Name",
                //                                       Separation = SeparationStyle.Strong
                //                                   },
                //                                   new TextBlock()
                //                                   {
                //                                       Text = "Research Sector",
                //                                       Separation = SeparationStyle.Strong
                //                                   },
                //                                   new TextBlock()
                //                                   {
                //                                       Text = "Intr. Sentiment Score",
                //                                       Separation = SeparationStyle.Strong
                //                                   },
                //                                   new TextBlock()
                //                                   {
                //                                       Text = "Extr. Sentiment Score",
                //                                       Separation = SeparationStyle.Strong
                //                                   },
                //                                   new TextBlock()
                //                                   {
                //                                       Text = "Performance",
                //                                       Separation = SeparationStyle.Strong
                //                                   }

                //                                },
                //    Separation = SeparationStyle.Strong

                //});



                //foreach (ResearchDTO researchDTO in researchList)
                //{
                //    Column column = new Column
                //    {
                //        Size = ColumnSize.Auto,
                //        Items = new List<CardElement>()
                //                                {
                //                                   new TextBlock()
                //                                   {
                //                                       Text = researchDTO.ticker.ToUpper(),
                //                                       Separation = SeparationStyle.Strong
                //                                   },
                //                                   new TextBlock()
                //                                   {
                //                                       Text = researchDTO.stockName,
                //                                       Separation = SeparationStyle.Strong
                //                                   },
                //                                   new TextBlock()
                //                                   {
                //                                       Text = researchDTO.researchSector,
                //                                       Separation = SeparationStyle.Strong
                //                                   },
                //                                   new TextBlock()
                //                                   {                       
                //                                       Text = String.Format("{0:0.##}", researchDTO.internalSentimentScore.Value),
                //                                       Separation = SeparationStyle.Strong
                //                                   },
                //                                   new TextBlock()
                //                                   {
                //                                       Text = String.Format("{0:0.##}", researchDTO.externalSentimentScore.Value),
                //                                       Separation = SeparationStyle.Strong
                //                                   },
                //                                   new TextBlock()
                //                                   {
                //                                       Text = researchDTO.performance.ToString()+"%",
                //                                       Separation = SeparationStyle.Strong
                //                                   }

                //                                },
                //        Separation = SeparationStyle.Strong
                //    };
                //    columns.Add(column);
                //}

                //AdaptiveCard card = new AdaptiveCard()
                //{
                //    Body = new List<CardElement>()
                //        {
                //            new Container()
                //            {
                //                Speak = "<s>Hello!</s><s>Are you looking for a flight or a hotel?</s>",
                //                Items = new List<CardElement>()
                //                {
                //                    new ColumnSet()
                //                    {
                //                        Separation = SeparationStyle.Strong,
                //                        Columns = columns
                //                    }
                //                }
                //            }
                //    }
                //};

                //Attachment attachment = new Attachment()
                //{
                //    ContentType = AdaptiveCard.ContentType,
                //    Content = card
                //};

                //var reply = context.MakeMessage();
                //reply.Attachments.Add(attachment);
                //await context.PostAsync(reply);

                await context.PostAsync($"Hey there! I am your virtual chatbot.");
                await this.ShowActionChoices(context, "I can assist you with:");
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        [LuisIntent("HighPerformingStock")]
        public async Task HighPerformingStockIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                List<ResearchDTO> researchList = BotRepo.GetStockPerformance(DateTime.Now.Date.AddYears(-2), DateTime.Now.Date)
                    .Where(c => (c.internalSentimentScore.Value >= 0.55m && c.externalSentimentScore.Value >= 0.5m)).ToList().OrderByDescending(o=>o.internalSentimentScore.Value).ToList();
                if (researchList != null && researchList.Count > 0)
                {
                    var message = context.MakeMessage();
                    message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    foreach (ResearchDTO researchDTO in researchList)
                    {
                        List<string> dataList = new List<string>();
                        dataList.Add("Stock : " + researchDTO.stockName + "<br>");
                       // dataList.Add("Research Industry : " + researchDTO.researchIndustry + ",<br>");
                        dataList.Add("Research sector : " + researchDTO.researchSector + ",<br>");
                        dataList.Add("Research count : " + researchDTO.researchCount + ",<br>");
                        dataList.Add("Internal Sentiment Score : " + String.Format("{0:0.##}", researchDTO.internalSentimentScore.Value) + "<br>");
                        dataList.Add("External Sentiment Score : " + String.Format("{0:0.##}", researchDTO.externalSentimentScore.Value) + "<br>");
                        dataList.Add("Performance : " + researchDTO.performance + "%  ");

                        HeroCard card = new HeroCard()
                        {
                            Title = researchDTO.stockName,
                            Subtitle ="For last 2 years",
                            Text = string.Join("     ", dataList)
                        };

                        message.Attachments.Add(card.ToAttachment());
                    }
                    await context.PostAsync(message);
                }

            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        [LuisIntent("StockToFocus")]
        public async Task StockToFocusIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                List<ResearchDTO> researchList = BotRepo.GetStockPerformance(DateTime.Now.Date.AddYears(-2), DateTime.Now.Date)
                       .Where(c => (c.internalSentimentScore.Value <= 0.55m && c.externalSentimentScore.Value <= 0.5m)).ToList().OrderByDescending(o => o.internalSentimentScore.Value).ToList();
                if (researchList != null && researchList.Count > 0)
                {
                    var message = context.MakeMessage();
                    message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    foreach (ResearchDTO researchDTO in researchList)
                    {
                        List<string> dataList = new List<string>();
                        dataList.Add("Stock : " + researchDTO.stockName + "<br>");
                      //  dataList.Add("Research Industry : " + researchDTO.researchIndustry + ",<br>");
                        dataList.Add("Research sector : " + researchDTO.researchSector + ",<br>");
                        dataList.Add("Research count : " + researchDTO.researchCount + ",<br>");
                        dataList.Add("Internal Sentiment Score : " + String.Format("{0:0.##}", researchDTO.internalSentimentScore.Value) + "<br>");
                        dataList.Add("External Sentiment Score : " + String.Format("{0:0.##}", researchDTO.externalSentimentScore.Value) + "<br>");
                        dataList.Add("Performance : " + researchDTO.performance + "%  ");

                        HeroCard card = new HeroCard()
                        {
                            Title = researchDTO.stockName,
                            Subtitle = "For last 2 years",
                            Text = string.Join("     ", dataList)
                        };

                        message.Attachments.Add(card.ToAttachment());
                    }
                    await context.PostAsync(message);
                }
                else
                {
                    await context.PostAsync(" ");
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        [LuisIntent("CompareStocks")]
        public async Task CompareStocksIntent(IDialogContext context, LuisResult result)
        {
            try
            {

                string period = null;
                int number = 0;
                List<string> tickers = new List<string>();
                if (result.Entities != null && result.Entities.Count > 0)
                {
                    foreach (EntityRecommendation entity in result.Entities)
                    {
                        if (entity.Type.Equals("Ticker"))
                        {
                            tickers.Add(entity.Entity);
                        }

                        if (entity.Type.Equals("builtin.number"))
                        {
                            if (int.TryParse(entity.Entity, out number))
                            {
                                number = Convert.ToInt32(entity.Entity);

                            }
                            else
                            {
                                number = WordToNumber(entity.Entity);
                            }

                        }

                        if (entity.Type.Equals("Period"))
                        {
                            period = entity.Entity;
                        }
                    }
                }

                if (tickers.Count == 0)
                {
                    await context.PostAsync("Please provide valid tickers");
                }
                else if (period == null || number == 0)
                {
                    context.ConversationData.SetValue(Constants.REMEMBER_TICKERS, tickers);
                    await this.ShowTimePeriodChoices(context, "Choose time period: ");
                }
                else
                {
                    await this.ShowComparisonPerformance(context, tickers, period, number);
                    context.ConversationData.SetValue(Constants.REMEMBER_TICKERS, tickers);
                    context.ConversationData.SetValue(Constants.REMEMBER_PERIOD, period);
                    context.ConversationData.SetValue(Constants.REMEMBER_NUMBER, number);
                }
                context.ConversationData.SetValue(Constants.REMEMBER_LAST_INTENT, Constants.INTENT_COMPARE_STOCKS);


                //List<ResearchDTO> researchList = BotRepo.getCustomerList(null, null).Where(c => tickers.Contains(c.ticker.ToLower())).ToList();
                //if (researchList != null && researchList.Count > 0)
                //{
                //    var message = context.MakeMessage();
                //    message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                //    foreach (ResearchDTO researchDTO in researchList)
                //    {
                //        List<string> dataList = new List<string>();
                //        dataList.Add("Stock : " + researchDTO.stockName + "<br>");
                //        dataList.Add("Research Industry : " + researchDTO.researchIndustry + ",<br>");
                //        dataList.Add("Research sector : " + researchDTO.researchSector + ",<br>");
                //        dataList.Add("Research count : " + researchDTO.researchCount + ",<br>");
                //        dataList.Add("Internal Sentiment Score : " + String.Format("{0:0.##}", researchDTO.sentimentScore.Value) + "<br>");

                //        HeroCard card = new HeroCard()
                //        {
                //            Title = "Performance of "+ researchDTO.ticker,
                //            Text = string.Join("     ", dataList)
                //        };

                //        message.Attachments.Add(card.ToAttachment());
                //    }
                //    await context.PostAsync(message);
                //}

            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }




        [LuisIntent("CheckStockPerformance")]
        public async Task CheckStockPerformanceIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                string ticker = null;
                string period = null;
                int number = 0;
                if (result.Entities != null && result.Entities.Count > 0)
                {
                    foreach (EntityRecommendation entity in result.Entities)
                    {
                        if (entity.Type.Equals("Ticker"))
                        {
                            ticker = entity.Entity;
                        }

                        if (entity.Type.Equals("builtin.number"))
                        {
                            if (int.TryParse(entity.Entity, out number))
                            {
                                number = Convert.ToInt32(entity.Entity);

                            }
                            else
                            {
                                number = WordToNumber(entity.Entity);
                            }

                        }

                        if (entity.Type.Equals("Period"))
                        {
                            period = entity.Entity;
                        }
                    }
                }

                //if (period == null)
                //{
                //    period = "year";
                //}else
                //{
                //    if (number == 0)
                //    {
                //        number = 1;
                //    }
                //}              

                //if (number == 0)
                //{
                //    number = 2;
                //}



                if (ticker == null)
                {
                    PromptDialog.Text(context, ResumeAfterGetTicker, "Ticker?");
                }
                else if (period == null || number == 0)
                {
                    context.ConversationData.SetValue(Constants.REMEMBER_TICKER, ticker);
                    await this.ShowTimePeriodChoices(context, "Choose time period: ");
                }
                else
                {
                    await this.ShowStockPerformance(context, ticker, period, number);
                    context.ConversationData.SetValue(Constants.REMEMBER_TICKER, ticker);
                    context.ConversationData.SetValue(Constants.REMEMBER_PERIOD, period);
                    context.ConversationData.SetValue(Constants.REMEMBER_NUMBER, number);
                }
                context.ConversationData.SetValue(Constants.REMEMBER_LAST_INTENT, Constants.INTENT_CHECK_STOCK_PERFORMANCE);

                //if (ticker != null && period != null && number > 0)
                //{
                //    ResearchDTO research = null;
                //    if (period.Contains("month"))
                //    {
                //        research = BotRepo.getCustomer(ticker.ToUpper(), DateTime.Now.Date.AddMonths(-number), DateTime.Now.Date);
                //    }else if (period.Contains("year"))
                //    {
                //        research = BotRepo.getCustomer(ticker.ToUpper(), DateTime.Now.Date.AddYears(-number), DateTime.Now.Date);
                //    }

                //    if (research != null)
                //    {
                //        var message = context.MakeMessage();
                //        List<string> dataList = new List<string>();
                //        dataList.Add("Stock : " + research.stockName + ",<br>  ");
                //        dataList.Add("Research Industry : " + research.researchIndustry + ",<br>     ");
                //        dataList.Add("Research Sector : " + research.researchSector + ",<br>      ");
                //        dataList.Add("Total Research Count : " + research.researchCount + ",<br>     ");
                //        dataList.Add("Internal Sentiment Score : " + String.Format("{0:0.##}", research.sentimentScore.Value) + "  ");

                //        HeroCard card = new HeroCard()
                //        {
                //            Title = "Stock Performance for Ticker - " + ticker.ToUpper(),
                //            Subtitle = "For last " + number + " " + (number > 1 ? period + "s" : period),
                //            Text = string.Join("     ", dataList)
                //        };

                //        message.Attachments.Add(card.ToAttachment());
                //        await context.PostAsync(message);
                //        //PromptDialog.Choice<string>(context, ResumeAfterCustomerCheckConfirmation,
                //        //               new PromptOptions<string>("Would you like to check for another Stock?",
                //        //               "Selected action not available. Please choose another.", "Let me get you there...",
                //        //               Constants.CONFIRMATION_OPTIONS, 0));
                //    }
                //    else
                //    {
                //        Boolean isIntentMatched = await this.CheckForIntent(context);
                //        if (!isIntentMatched)
                //        {
                //            await context.PostAsync("Sorry. I could not find data for given Ticker. Please ask me with a valid Ticker. For ex, **Stock performance of NVDA for last 6 moths ");
                //            //PromptDialog.Choice<string>(context, ResumeAfterCustomerCheckConfirmation,
                //            //               new PromptOptions<string>("Invalid Ticker. Would you like to check for another Stock?",
                //            //               "Selected action not available. Please choose another.", "Let me get you there...",
                //            //               Constants.CONFIRMATION_OPTIONS, 0));
                //        }
                //    }
                //}

            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        [LuisIntent("CheckAnalystPerformance")]
        public async Task CheckAnalystPerformanceIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                string analyst = null;
                string period = null;
                int number = 0;
                if (result.Entities != null && result.Entities.Count > 0)
                {
                    foreach (EntityRecommendation entity in result.Entities)
                    {
                        if (entity.Type.Equals("Communication.SenderName"))
                        {
                            analyst = entity.Entity;
                        }

                        if (entity.Type.Equals("builtin.number"))
                        {
                            if (int.TryParse(entity.Entity, out number))
                            {
                                number = Convert.ToInt32(entity.Entity);

                            }
                            else
                            {
                                number = WordToNumber(entity.Entity);
                            }

                        }

                        if (entity.Type.Equals("Period"))
                        {
                            period = entity.Entity;
                        }
                    }
                }

                //if (period == null)
                //{
                //    period = "year";
                //}
                //else
                //{
                //    if (number == 0)
                //    {
                //        number = 1;
                //    }
                //}

                //if (number == 0)
                //{
                //    number = 2;
                //}



                if (analyst == null)
                {
                    PromptDialog.Text(context, ResumeAfterGetAnalystName, "Analyst Name or Email?");
                }
                else if (period == null || number == 0)
                {
                    context.ConversationData.SetValue(Constants.REMEMBER_ANALYST, analyst);
                    await this.ShowTimePeriodChoices(context, "Choose time period: ");
                }
                else
                {
                    await this.ShowAnalystPerformance(context, analyst, period, number);
                    context.ConversationData.SetValue(Constants.REMEMBER_ANALYST, analyst);
                    context.ConversationData.SetValue(Constants.REMEMBER_PERIOD, period);
                    context.ConversationData.SetValue(Constants.REMEMBER_NUMBER, number);
                }
                context.ConversationData.SetValue(Constants.REMEMBER_LAST_INTENT, Constants.INTENT_CHECK_ANALYST_PERFORMANCE);
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        private async Task ResumeAfterGetTicker(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string ticker = await result;
                if (ticker != null)
                {
                    context.ConversationData.SetValue(Constants.REMEMBER_TICKER, ticker);
                    string period = null;
                    int number = 0;
                    if (context.ConversationData.TryGetValue<string>(Constants.REMEMBER_PERIOD, out period) && context.ConversationData.TryGetValue<Int32>(Constants.REMEMBER_NUMBER, out number))
                    {
                        await this.ShowStockPerformance(context, ticker, period, number);
                    }
                    else
                    {
                        await this.ShowTimePeriodChoices(context, "Choose time period: ");
                    }
                }
                else
                {
                    Boolean isIntentMatched = await this.CheckForIntent(context);
                    if (!isIntentMatched)
                    {
                        PromptDialog.Choice<string>(context, ResumeAfterCustomerCheckConfirmation,
                                       new PromptOptions<string>("Invalid Ticker. Would you like to check for another Stock?",
                                       "Selected action not available. Please choose another.", "Let me get you there...",
                                       Constants.CONFIRMATION_OPTIONS, 0));
                    }
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        private async Task ResumeAfterCustomerCheckConfirmation(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string confirmation = await result;
                switch (confirmation.ToLower())
                {
                    case Constants.CONFIRMATION_YES:
                        PromptDialog.Text(context, ResumeAfterGetTicker, "Ticker?");
                        break;
                    case Constants.CONFIRMATION_NO:
                        await this.ShowActionChoices(context, "What do you want to do next?");
                        break;
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.CheckForIntent(context);
            }
            catch (Exception exception)
            {

                Debug.WriteLine(exception.GetBaseException());
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }



        private async Task ResumeAfterGetAnalystName(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string analystName = await result;
                if (analystName != null)
                {
                    string[] nameSplits = analystName.Split(',');
                    if (nameSplits.Count() > 1)
                    {
                        analystName = nameSplits[1].ToLower().Trim();
                    }
                    //string period = context.ConversationData.GetValue<string>(Constants.REMEMBER_PERIOD);
                    //int number = context.ConversationData.GetValue<Int32>(Constants.REMEMBER_NUMBER);

                    context.ConversationData.SetValue(Constants.REMEMBER_ANALYST, analystName);
                    string period = null;
                    int number = 0;
                    if (context.ConversationData.TryGetValue<string>(Constants.REMEMBER_PERIOD, out period) && context.ConversationData.TryGetValue<Int32>(Constants.REMEMBER_NUMBER, out number))
                    {
                        await this.ShowAnalystPerformance(context, analystName, period, number);
                    }
                    else
                    {
                        await this.ShowTimePeriodChoices(context, "Choose time period: ");
                    }
                }
                else
                {
                    Boolean isIntentMatched = await this.CheckForIntent(context);
                    if (!isIntentMatched)
                    {
                        PromptDialog.Choice<string>(context, ResumeAfterAgentCheckConfirmation,
                                       new PromptOptions<string>("Invalid Analyst name. Would you like to check for another Analyst?",
                                       "Selected action not available. Please choose another.", "Let me get you there...",
                                       Constants.CONFIRMATION_OPTIONS, 0));
                    }
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        private async Task ResumeAfterAgentCheckConfirmation(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string confirmation = await result;
                switch (confirmation.ToLower())
                {
                    case Constants.CONFIRMATION_YES:
                        PromptDialog.Text(context, ResumeAfterGetAnalystName, "Analyst Name or Email?");
                        break;
                    case Constants.CONFIRMATION_NO:
                        await this.ShowActionChoices(context, "What do you want to do next?");
                        break;
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.CheckForIntent(context);
            }
            catch (Exception exception)
            {

                Debug.WriteLine(exception.GetBaseException());
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");

            }
        }


        [LuisIntent("TimePeriodIntent")]
        public async Task TimePeriodIntentIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                //string ticker = context.ConversationData.GetValue<string>(Constants.REMEMBER_TICKER);
                string period = null;
                int number = 0;

                if (result.Entities != null && result.Entities.Count > 0)
                {
                    foreach (EntityRecommendation entity in result.Entities)
                    {
                        if (entity.Type.Equals("builtin.number"))
                        {
                            if (int.TryParse(entity.Entity, out number))
                            {
                                number = Convert.ToInt32(entity.Entity);

                            }
                            else
                            {
                                number = WordToNumber(entity.Entity);
                            }

                        }

                        if (entity.Type.Equals("Period"))
                        {
                            period = entity.Entity;
                        }
                    }
                }

                if (period == null)
                {
                    period = "year";
                }
                else
                {
                    if (number == 0)
                    {
                        number = 1;
                    }
                }

                if (number == 0)
                {
                    number = 2;
                }

                context.ConversationData.SetValue(Constants.REMEMBER_PERIOD, period);
                context.ConversationData.SetValue(Constants.REMEMBER_NUMBER, number);

                if (context.ConversationData.GetValue<string>(Constants.REMEMBER_LAST_INTENT).Equals(Constants.INTENT_CHECK_STOCK_PERFORMANCE))
                {
                    await this.ShowStockPerformance(context, context.ConversationData.GetValue<string>(Constants.REMEMBER_TICKER), period, number);
                }
                else if (context.ConversationData.GetValue<string>(Constants.REMEMBER_LAST_INTENT).Equals(Constants.INTENT_CHECK_ANALYST_PERFORMANCE))
                {
                    await this.ShowAnalystPerformance(context, context.ConversationData.GetValue<string>(Constants.REMEMBER_ANALYST), period, number);
                }
                else if (context.ConversationData.GetValue<string>(Constants.REMEMBER_LAST_INTENT).Equals(Constants.INTENT_COMPARE_STOCKS))
                {
                    await this.ShowComparisonPerformance(context, context.ConversationData.GetValue<List<string>>(Constants.REMEMBER_TICKERS), period, number);
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        [LuisIntent("TickerIntent")]
        public async Task TickerIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                //string ticker = context.ConversationData.GetValue<string>(Constants.REMEMBER_TICKER);
                string ticker = null;
                if (result.Entities != null && result.Entities.Count > 0)
                {
                    foreach (EntityRecommendation entity in result.Entities)
                    {
                        if (entity.Type.Equals("Ticker"))
                        {
                            ticker = entity.Entity;
                        }
                    }
                }

                string period = null;
                int number = 0;
                if (context.ConversationData.TryGetValue<string>(Constants.REMEMBER_PERIOD, out period) && context.ConversationData.TryGetValue<Int32>(Constants.REMEMBER_NUMBER, out number))
                {
                    await this.ShowStockPerformance(context, ticker, period, number);
                }
                else
                {
                    await this.ShowTimePeriodChoices(context, "Choose time period: ");
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        [LuisIntent("AnalystIntent")]
        public async Task AnalystIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                //string ticker = context.ConversationData.GetValue<string>(Constants.REMEMBER_TICKER);
                string period = null;
                string analyst = null;
                int number = 0;
                if (result.Entities != null && result.Entities.Count > 0)
                {
                    foreach (EntityRecommendation entity in result.Entities)
                    {
                        if (entity.Type.Equals("Communication.SenderName"))
                        {
                            analyst = entity.Entity;
                        }                       
                    }
                }

                if (analyst != null)
                {
                    string[] nameSplits = analyst.Split(',');
                    if (nameSplits.Count() > 1)
                    {
                        analyst = nameSplits[1].ToLower().Trim();
                    }
                }
                

                context.ConversationData.SetValue(Constants.REMEMBER_ANALYST, analyst);
                context.ConversationData.SetValue(Constants.REMEMBER_LAST_INTENT, Constants.INTENT_CHECK_ANALYST_PERFORMANCE);
                if (context.ConversationData.TryGetValue<string>(Constants.REMEMBER_PERIOD, out period) && context.ConversationData.TryGetValue<Int32>(Constants.REMEMBER_NUMBER, out number))
                {
                    await this.ShowAnalystPerformance(context, analyst, period, number);
                }
                else
                {
                    await this.ShowTimePeriodChoices(context, "Choose time period: ");
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        private async Task ShowStockPerformance(IDialogContext context, string ticker, string period, int number)
        {
            try
            {

                if (ticker != null && period != null && number > 0)
                {

                    ResearchDTO research = null;
                    if (period.Contains("month"))
                    {
                        //research = BotRepo.getCustomer(ticker.ToUpper(), DateTime.Now.Date.AddMonths(-number), DateTime.Now.Date);
                        research = BotRepo.GetStockPerformance(DateTime.Now.Date.AddMonths(-number), DateTime.Now.Date).Where(c => c.ticker.Equals(ticker.ToUpper())).FirstOrDefault();
                    }
                    else if (period.Contains("year"))
                    {
                        //  research = BotRepo.getCustomer(ticker.ToUpper(), DateTime.Now.Date.AddYears(-number), DateTime.Now.Date);
                        research = BotRepo.GetStockPerformance(DateTime.Now.Date.AddYears(-number), DateTime.Now.Date).Where(c => c.ticker.Equals(ticker.ToUpper())).FirstOrDefault();
                    }
                    else if (period.Contains("day"))
                    {
                        //  research = BotRepo.getCustomer(ticker.ToUpper(), DateTime.Now.Date.AddYears(-number), DateTime.Now.Date);
                        research = BotRepo.GetStockPerformance(DateTime.Now.Date.AddDays(-number), DateTime.Now.Date).Where(c => c.ticker.Equals(ticker.ToUpper())).FirstOrDefault();
                    }

                    if (research != null)
                    {
                        await context.PostAsync("Checking performance of ticker -" + ticker.ToUpper());
                        var message = context.MakeMessage();
                        List<string> dataList = new List<string>();
                        dataList.Add("Stock : " + research.stockName + ",<br>  ");
                        //dataList.Add("Research Industry : " + research.researchIndustry + ",<br>     ");
                        dataList.Add("Research Sector : " + research.researchSector + ",<br>      ");
                        dataList.Add("Total Research Count : " + research.researchCount + ",<br>     ");
                        dataList.Add("Internal Sentiment Score : " + String.Format("{0:0.##}", research.internalSentimentScore.Value) + ",<br>     ");
                        dataList.Add("External Sentiment Score : " + String.Format("{0:0.##}", research.externalSentimentScore.Value) + ",<br>     ");
                        dataList.Add("Performance : " + research.performance + "%  ");

                        HeroCard card = new HeroCard()
                        {
                            Title = "Stock Performance for Ticker - " + ticker.ToUpper(),
                            Subtitle = "For last " + number + " " + ((number > 1 && !period.Contains("s")) ? period + "s" : period),
                            Text = string.Join("     ", dataList)
                        };

                        message.Attachments.Add(card.ToAttachment());
                        await context.PostAsync(message);
                        //PromptDialog.Choice<string>(context, ResumeAfterCustomerCheckConfirmation,
                        //               new PromptOptions<string>("Would you like to check for another Stock?",
                        //               "Selected action not available. Please choose another.", "Let me get you there...",
                        //               Constants.CONFIRMATION_OPTIONS, 0));
                    }
                    else
                    {
                        Boolean isIntentMatched = await this.CheckForIntent(context);
                        if (!isIntentMatched)
                        {
                            await context.PostAsync("Sorry. I could not find data for given Ticker. Please ask me with a valid Ticker. For ex: Stock performance of NVDA for last 6 months, Stock performance of AVGO for last year ");
                            //PromptDialog.Choice<string>(context, ResumeAfterCustomerCheckConfirmation,
                            //               new PromptOptions<string>("Invalid Ticker. Would you like to check for another Stock?",
                            //               "Selected action not available. Please choose another.", "Let me get you there...",
                            //               Constants.CONFIRMATION_OPTIONS, 0));
                        }
                    }
                }

            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        private async Task ShowAnalystPerformance(IDialogContext context, string analyst, string period, int number)
        {
            try
            {
                if (analyst != null && period != null && number > 0)
                {
                    string[] nameSplits = analyst.Split(',');
                    if (nameSplits.Count() > 1) {
                        analyst = nameSplits[1].ToLower().Trim();
                    }
                    ResearchDTO research = null;
                    if (period.Contains("month"))
                    {
                        //research = BotRepo.getAnalyst(analyst, DateTime.Now.Date.AddMonths(-number), DateTime.Now.Date);
                        var list = BotRepo.GetAnalystPerformance(DateTime.Now.Date.AddMonths(-number), DateTime.Now.Date).Where(c => c.source.ToLower().Contains(analyst) || c.source.ToLower().Contains(analyst) || c.email.ToLower().Contains(analyst) || c.email.ToLower().Contains(analyst) || c.email.ToLower().Equals(analyst)
                         || c.source.ToLower().Equals(analyst) || c.source.ToLower().Replace(" ", String.Empty).Equals(analyst)).ToList();
                        research = list.GroupBy(x => new { x.source, x.researchSector, x.email })
                            .Select(dd => new ResearchDTO
                            {
                                source = dd.Key.source,
                                email = dd.Key.email,
                                researchCount = dd.ToList().Select(s => s.researchCount).Sum(),
                                researchSector = dd.Key.researchSector,
                                internalSentimentScore = dd.ToList().Select(s => s.internalSentimentScore).Average(),
                                externalSentimentScore = dd.ToList().Select(s => s.externalSentimentScore).Average(),
                                ticker = string.Join(" & ", dd.Select(ee => ee.ticker))
                            }).FirstOrDefault();
                    }
                    else if (period.Contains("year"))
                    {
                        //  research = BotRepo.getAnalyst(analyst, DateTime.Now.Date.AddYears(-number), DateTime.Now.Date);
                        //research = BotRepo.getAnalyst(analyst, DateTime.Now.Date.AddMonths(-number), DateTime.Now.Date);
                        var list = BotRepo.GetAnalystPerformance(DateTime.Now.Date.AddYears(-number), DateTime.Now.Date).Where(c => c.source.ToLower().Contains(analyst) || c.source.ToLower().Contains(analyst) || c.email.ToLower().Contains(analyst) || c.email.ToLower().Contains(analyst) || c.email.ToLower().Equals(analyst)
                        || c.source.ToLower().Equals(analyst) || c.source.ToLower().Replace(" ", String.Empty).Equals(analyst)).ToList();
                        research = list.GroupBy(x => new { x.source, x.researchSector,x.email})
                            .Select(dd => new ResearchDTO
                            {
                                source = dd.Key.source,
                                email = dd.Key.email,
                                researchCount = dd.ToList().Select(s => s.researchCount).Sum(),
                                researchSector = dd.Key.researchSector,
                                internalSentimentScore = dd.ToList().Select(s => s.internalSentimentScore).Average(),
                                externalSentimentScore = dd.ToList().Select(s => s.externalSentimentScore).Average(),
                                ticker = string.Join(" & ", dd.Select(ee => ee.ticker))
                            }).FirstOrDefault();
                    }
                    else if (period.Contains("day"))
                    {
                        //  research = BotRepo.getCustomer(ticker.ToUpper(), DateTime.Now.Date.AddYears(-number), DateTime.Now.Date);
                        var list = BotRepo.GetAnalystPerformance(DateTime.Now.Date.AddDays(-number), DateTime.Now.Date).Where(c => c.source.Contains(analyst) || c.source.ToLower().Contains(analyst) || c.email.Contains(analyst) || c.email.ToLower().Contains(analyst) || c.email.Equals(analyst)
                            || c.source.Equals(analyst) || c.source.Replace(" ", String.Empty).Equals(analyst)).ToList();
                        research = list.GroupBy(x => new { x.source, x.researchSector, x.email })
                            .Select(dd => new ResearchDTO
                            {
                                source = dd.Key.source,
                                email = dd.Key.email,
                                researchCount = dd.ToList().Select(s => s.researchCount).Sum(),
                                researchSector = dd.Key.researchSector,
                                internalSentimentScore = dd.ToList().Select(s => s.internalSentimentScore).Average(),
                                externalSentimentScore = dd.ToList().Select(s => s.externalSentimentScore).Average(),
                                ticker = string.Join(" & ", dd.Select(ee => ee.ticker))
                            }).FirstOrDefault();
                    }

                    if (research != null)
                    {
                        await context.PostAsync("Checking performance of Analyst - " + analyst);
                        var message = context.MakeMessage();
                        List<string> dataList = new List<string>();
                        dataList.Add("Analyst Name : " + research.source + "<br>");
                        dataList.Add("Analyst Email : " + research.email + ",<br>");
                        dataList.Add("Research Sector : " + research.researchSector + ",<br>");
                        dataList.Add("Research Count : " + research.researchCount + ",<br>");
                        dataList.Add("Tickers : " + research.ticker + ",<br>");
                        dataList.Add("Internal Sentiment Score : " + String.Format("{0:0.##}", research.internalSentimentScore.Value) + "<br>");
                        dataList.Add("External Sentiment Score : " + String.Format("{0:0.##}", research.externalSentimentScore.Value) + "<br>");
                        HeroCard card = new HeroCard()
                        {
                            Title = "Performance of Analyst - " + analyst,
                            Subtitle = "For last " + number + " " + ((number > 1 && !period.Contains("s")) ? period + "s" : period),
                            Text = string.Join("     ", dataList)
                        };

                        message.Attachments.Add(card.ToAttachment());
                        await context.PostAsync(message);
                        //PromptDialog.Choice<string>(context, ResumeAfterAgentCheckConfirmation,
                        //                   new PromptOptions<string>("Would you like to check for another Analyst?",
                        //                   "Selected action not available. Please choose another.", "Let me get you there...",
                        //                   Constants.CONFIRMATION_OPTIONS, 0));
                    }
                    else
                    {
                        Boolean isIntentMatched = await this.CheckForIntent(context);
                        if (!isIntentMatched)
                        {
                            //PromptDialog.Text(context, ResumeAfterGetAnalystName, "Sorry. I could not find data for given Analyst. Please provide valid Analyst Name or Email?");
                             await context.PostAsync("Sorry. I could not find data for given Analyst. Please ask me with a valid Analyst name. For ex: Lebo, Jessica");
                            //PromptDialog.Choice<string>(context, ResumeAfterCustomerCheckConfirmation,
                            //               new PromptOptions<string>("Invalid Ticker. Would you like to check for another Stock?",
                            //               "Selected action not available. Please choose another.", "Let me get you there...",
                            //               Constants.CONFIRMATION_OPTIONS, 0));
                        }
                    }
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        private async Task ShowComparisonPerformance(IDialogContext context, List<string> tickers, string period, int number)
        {
            try
            {
                if (tickers != null && period != null && number > 0)
                {
                    await context.PostAsync("Checking performance of provided tickers...");
                    List<ResearchDTO> researchList = null;

                    if (period.Contains("month"))
                    {
                        //  researchList = BotRepo.getCustomerList(DateTime.Now.Date.AddMonths(-number), DateTime.Now.Date).Where(c => tickers.Contains(c.ticker.ToLower())).ToList();
                        researchList = BotRepo.GetStockPerformance(DateTime.Now.Date.AddMonths(-number), DateTime.Now.Date).Where(c => tickers.Contains(c.ticker.ToLower())).ToList();

                    }
                    else if (period.Contains("year"))
                    {
                        // researchList = BotRepo.getCustomerList(DateTime.Now.Date.AddYears(-number), DateTime.Now.Date).Where(c => tickers.Contains(c.ticker.ToLower())).ToList();
                        researchList = BotRepo.GetStockPerformance(DateTime.Now.Date.AddYears(-number), DateTime.Now.Date).Where(c => tickers.Contains(c.ticker.ToLower())).ToList();
                    }
                    else if (period.Contains("day"))
                    {
                        researchList = BotRepo.GetStockPerformance(DateTime.Now.Date.AddDays(-number), DateTime.Now.Date).Where(c => tickers.Contains(c.ticker.ToLower())).ToList();
                    }

                    if (researchList != null && researchList.Count > 0)
                    {
                        //var message = context.MakeMessage();
                        //message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        //foreach (ResearchDTO researchDTO in researchList)
                        //{
                        //    List<string> dataList = new List<string>();
                        //    dataList.Add("Stock : " + researchDTO.stockName + ",<br>  ");
                        //    //dataList.Add("Research Industry : " + research.researchIndustry + ",<br>     ");
                        //    dataList.Add("Research Sector : " + researchDTO.researchSector + ",<br>      ");
                        //    dataList.Add("Total Research Count : " + researchDTO.researchCount + ",<br>     ");
                        //    dataList.Add("Internal Sentiment Score : " + String.Format("{0:0.##}", researchDTO.internalSentimentScore.Value) + ",<br>     ");
                        //    dataList.Add("External Sentiment Score : " + String.Format("{0:0.##}", researchDTO.externalSentimentScore.Value));

                        //    HeroCard card = new HeroCard()
                        //    {
                        //        Title = "Performance of " + researchDTO.ticker,
                        //        Subtitle = "For last " + number + " " + ((number > 1 && !period.Contains("s")) ? period + "s" : period),
                        //        Text = string.Join("     ", dataList)
                        //    };

                        //    message.Attachments.Add(card.ToAttachment());
                        //}
                        //await context.PostAsync(message);

                        List<Column> columns = new List<Column>();
                        columns.Add(new Column()
                        {
                            Size = ColumnSize.Auto,
                            Items = new List<CardElement>()
                                                {
                                                   new TextBlock()
                                                   {
                                                       Text = "Ticker",
                                                       Separation = SeparationStyle.Strong
                                                   },
                                                   new TextBlock()
                                                   {
                                                       Text = "Stock Name",
                                                       Separation = SeparationStyle.Strong
                                                   },
                                                   new TextBlock()
                                                   {
                                                       Text = "Research Sector",
                                                       Separation = SeparationStyle.Strong
                                                   },
                                                   new TextBlock()
                                                   {
                                                       Text = "Intr. Sentiment Score",
                                                       Separation = SeparationStyle.Strong
                                                   },
                                                   new TextBlock()
                                                   {
                                                       Text = "Extr. Sentiment Score",
                                                       Separation = SeparationStyle.Strong
                                                   },
                                                   new TextBlock()
                                                   {
                                                       Text = "Performance",
                                                       Separation = SeparationStyle.Strong
                                                   }

                                                },
                            Separation = SeparationStyle.Strong
                        });

                        foreach (ResearchDTO researchDTO in researchList)
                        {
                            Column column = new Column
                            {
                                Size = ColumnSize.Auto,
                                Items = new List<CardElement>()
                                                {
                                                   new TextBlock()
                                                   {
                                                       Text = researchDTO.ticker.ToUpper(),
                                                       Separation = SeparationStyle.Strong
                                                   },
                                                   new TextBlock()
                                                   {
                                                       Text = researchDTO.stockName,
                                                       Separation = SeparationStyle.Strong
                                                   },
                                                   new TextBlock()
                                                   {
                                                       Text = researchDTO.researchSector,
                                                       Separation = SeparationStyle.Strong
                                                   },
                                                   new TextBlock()
                                                   {
                                                       Text = String.Format("{0:0.##}", researchDTO.internalSentimentScore.Value),
                                                       Separation = SeparationStyle.Strong
                                                   },
                                                   new TextBlock()
                                                   {
                                                       Text = String.Format("{0:0.##}", researchDTO.externalSentimentScore.Value),
                                                       Separation = SeparationStyle.Strong
                                                   },
                                                   new TextBlock()
                                                   {
                                                       Text = researchDTO.performance.ToString()+"%",
                                                       Separation = SeparationStyle.Strong
                                                   }

                                                },
                                Separation = SeparationStyle.Strong
                            };
                            columns.Add(column);
                        }

                        AdaptiveCard card = new AdaptiveCard()
                        {
                            Body = new List<CardElement>()
                        {                               
                            new Container()
                            {
                               // Speak = "<s>Hello!</s><s>Are you looking for a flight or a hotel?</s>",                               
                                Items = new List<CardElement>()
                                {
                                    new ColumnSet()
                                    {
                                        Separation = SeparationStyle.Strong,
                                        Columns = columns
                                    }
                                }
                            }
                          }
                        };

                        Attachment attachment = new Attachment()
                        {
                            ContentType = AdaptiveCard.ContentType,
                            Content = card                            
                        };

                        var reply = context.MakeMessage();
                        reply.Text = "Performance of "+  string.Join(" & ", tickers.Select(c=> c.ToUpper()).ToList()) + " for last " + number + " " + ((number > 1 && !period.Contains("s")) ? period + "s" : period);
                        //reply.Summary = "For last " + number + " " + ((number > 1 && !period.Contains("s")) ? period + "s" : period);
                        reply.Attachments.Add(attachment);
                        await context.PostAsync(reply);
                    }
                    else
                    {
                        //Boolean isIntentMatched = await this.CheckForIntent(context);
                        //if (!isIntentMatched)
                        //{
                        await context.PostAsync("I could not find the stocks.");
                        //  }
                    }
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        [LuisIntent("Thank")]
        public async Task ThankIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, "Happy to help :)");
        }


        [LuisIntent("SayBye")]
        public async Task SayByeIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, "Bye. See you soon!");
        }

        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, "Sorry. I didn't understand. Try again using different words :(");
        }




        private async Task ShowActionChoices(IDialogContext context, string message)
        {
            HeroCard card = new HeroCard
            {
                Title = message,
                Buttons = new List<CardAction> {
                     new CardAction(ActionTypes.ImBack, title: Constants.ACTION_CHECK_STOCK_PERFORMANCE, value: Constants.ACTION_CHECK_STOCK_PERFORMANCE),
                     new CardAction(ActionTypes.ImBack, title: Constants.ACTION_CHECK_ANALYST_PERFORMANCE, value: Constants.ACTION_CHECK_ANALYST_PERFORMANCE),
                     new CardAction(ActionTypes.ImBack, title: Constants.ACTION_COMPARE_STOCKS, value: Constants.ACTION_COMPARE_STOCKS),
                     new CardAction(ActionTypes.ImBack, title: Constants.ACTION_HIGH_PERFORMING_STOCKS, value: Constants.ACTION_HIGH_PERFORMING_STOCKS),
                     new CardAction(ActionTypes.ImBack, title: Constants.ACTION_STOCKS_TO_FOCUS, value: Constants.ACTION_STOCKS_TO_FOCUS),
                     new CardAction(ActionTypes.ImBack, title: Constants.ACTION_SCHEDULE_MEETING, value: Constants.ACTION_SCHEDULE_MEETING)
                     //new CardAction(ActionTypes.ImBack, Constants.ACTION_GET_MARKET_INSIGHTS, value: Constants.ACTION_GET_MARKET_INSIGHTS),
                     //new CardAction(ActionTypes.ImBack, Constants.ACTION_INVESTING_GUIDE, value: Constants.ACTION_INVESTING_GUIDE),
                    //new CardAction(ActionTypes.ImBack, title: Constants.ACTION_GET_PERFORMANCE, value: Constants.ACTION_GET_PERFORMANCE),
                    //new CardAction(ActionTypes.ImBack, Constants.ACTION_GET_PRICING, value: Constants.ACTION_GET_PRICING),
                    //new CardAction(ActionTypes.ImBack, Constants.ACTION_GET_RATING, value: Constants.ACTION_GET_RATING),
                    //new CardAction(ActionTypes.ImBack, title: Constants.ACTION_GET_FACTSHEET, value: Constants.ACTION_GET_FACTSHEET),
                    //new CardAction(ActionTypes.ImBack, Constants.ACTION_GET_PORTFOLIO, value: Constants.ACTION_GET_PORTFOLIO),
                    //new CardAction(ActionTypes.ImBack, Constants.ACTION_GET_REPORT, value: Constants.ACTION_GET_REPORT),
                    //new CardAction(ActionTypes.ImBack, Constants.ACTION_GET_HOLDING, value: Constants.ACTION_GET_HOLDING)
                }
            };
            var reply = context.MakeMessage();
            reply.Attachments.Add(card.ToAttachment());
            await context.PostAsync(reply);
        }

        private async Task ShowTimePeriodChoices(IDialogContext context, string message)
        {
            HeroCard card = new HeroCard
            {
                Title = message,
                Buttons = new List<CardAction> {
                     new CardAction(ActionTypes.ImBack, title: Constants.TIME_PERIOD_CHOICE_SIX_MONTH, value: Constants.TIME_PERIOD_CHOICE_SIX_MONTH),
                     new CardAction(ActionTypes.ImBack, title: Constants.TIME_PERIOD_CHOICE_ONE_YEAR, value: Constants.TIME_PERIOD_CHOICE_ONE_YEAR),
                     new CardAction(ActionTypes.ImBack, title: Constants.TIME_PERIOD_CHOICE_TWO_YEAR, value: Constants.TIME_PERIOD_CHOICE_TWO_YEAR)
                }
            };
            var reply = context.MakeMessage();
            reply.Attachments.Add(card.ToAttachment());
            await context.PostAsync(reply);
        }

        private async Task ShowLuisResult(IDialogContext context, string message)
        {
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        //CRP BOT Sceanrios

        [LuisIntent("GetPricing")]
        public async Task GetPricingIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                context.ConversationData.SetValue("lastIntent", Constants.ACTION_GET_PRICING);
                await context.PostAsync("Ticker ?");
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        [LuisIntent("GetRating")]
        public async Task GetRatingIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                context.ConversationData.SetValue("lastIntent", Constants.ACTION_GET_RATING);
                await context.PostAsync("Ticker ?");
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        [LuisIntent("GetPerformance")]
        public async Task GetPerformanceIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                context.ConversationData.SetValue("lastIntent", Constants.ACTION_GET_PERFORMANCE);
                await context.PostAsync("Ticker ?");
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        [LuisIntent("GetFactSheet")]
        public async Task GetFactSheetIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                if (result.Entities.Count > 0)
                {
                    if (result.Entities[0].Type.Equals("FundName"))
                    {
                        await this.ShowFactsheet(context, result.Entities[0].Entity);
                    }
                }
                else
                {
                    await this.ShowAllFactsheets(context);
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        [LuisIntent("GetPortfolio")]
        public async Task GetPortfolioIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                if (result.Entities.Count > 0)
                {
                    if (result.Entities[0].Type.Equals("FundName"))
                    {
                        await this.ShowPortfolio(context, result.Entities[0].Entity);
                    }
                }
                else
                {
                    await this.ShowAllPortfolios(context);
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        [LuisIntent("GetReport")]
        public async Task GetReportIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                if (result.Entities.Count > 0)
                {
                    if (result.Entities[0].Type.Equals("FundName"))
                    {
                        await this.ShowAnnualReport(context, result.Entities[0].Entity);
                    }
                }
                else
                {
                    await this.ShowAllAnnualReports(context);
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        [LuisIntent("GetHoldings")]
        public async Task GetHoldingsIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                if (result.Entities.Count > 0)
                {
                    if (result.Entities[0].Type.Equals("FundName"))
                    {
                        await this.ShowFundHolding(context, result.Entities[0].Entity);
                    }
                }
                else
                {
                    await this.ShowAllFundHoldings(context);
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        [LuisIntent("FundnameIntent")]
        public async Task FundnameIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                if (result.Entities.Count > 0)
                {
                    if (result.Entities[0].Type.Equals("FundName"))
                    {
                        //if (Constants.FUNDS.Contains(result.Entities[0].Entity))
                        //{
                        string lastIntent = context.ConversationData.GetValue<string>("lastIntent");

                        switch (lastIntent)
                        {
                            case Constants.FACTSHEET:
                                await this.ShowFactsheet(context, result.Entities[0].Entity);
                                break;
                            case Constants.PORTFOLIO:
                                await this.ShowPortfolio(context, result.Entities[0].Entity);
                                break;
                            case Constants.ANNUALREPORT:
                                await this.ShowAnnualReport(context, result.Entities[0].Entity);
                                break;
                            case Constants.FUNDHOLDING:
                                await this.ShowFundHolding(context, result.Entities[0].Entity);
                                break;
                                //   }
                        }
                    }
                }
                else
                {
                    //  context.ConversationData.SetValue("lastIntent", Constants.ACTION_GET_PRICING);
                    await context.PostAsync("Given FundName is invalid. Would like to check for another FundName?");
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        //[LuisIntent("TickerIntent")]
        //public async Task TickerIntent(IDialogContext context, LuisResult result)
        //{
        //    try
        //    {
        //        if (result.Entities.Count > 0)
        //        {
        //            if (result.Entities[0].Type.Equals("Ticker"))
        //            {
        //                string lastIntent = context.ConversationData.GetValue<string>("lastIntent");

        //                switch (lastIntent)
        //                {
        //                    case Constants.ACTION_GET_PERFORMANCE:
        //                        await this.ShowPerformance(context, result.Entities[0].Entity);
        //                        break;
        //                    case Constants.ACTION_GET_PRICING:
        //                        await this.ShowPricing(context, result.Entities[0].Entity);
        //                        break;
        //                    case Constants.ACTION_GET_RATING:
        //                        await this.ShowRating(context, result.Entities[0].Entity);
        //                        break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            await context.PostAsync("Given Ticker # is invalid. Would like to check for another Ticker?");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
        //    }
        //}

        private async Task ShowPerformance(IDialogContext context, string ticker)
        {
            try
            {
                FinanceModel financeModel = BotRepo.GetFinanceByTicker(ticker);
                if (financeModel != null)
                {
                    var message = context.MakeMessage();
                    List<string> dataList = new List<string>();

                    // dataList.Add("Here is the performance for ticker #" + option + " :    ");
                    dataList.Add("Name : " + financeModel.name + ",         ");
                    dataList.Add("MorningStar Category : " + financeModel.ms_category + ",      ");
                    dataList.Add("YTD : " + financeModel.ytd + "%,      ");
                    dataList.Add("Annual total return since inception : " + financeModel.since_inception + "%,    ");
                    dataList.Add("SEC Yield (With Waivers): " + financeModel.sec_yield_waivers + "%,    ");
                    dataList.Add("SEC Yield (Without Waivers): " + financeModel.sec_yield_wo_waivers + "%,    ");
                    dataList.Add("Expense Ratio (Gross): " + financeModel.expense_ratio_gross + "%,    ");
                    dataList.Add("Expense Ratio (Net): " + financeModel.expense_ratio_net + "%   ");
                    //await this.ShowLuisResult(context, String.Join("\n ", dataList));
                    var heroCard = new HeroCard
                    {
                        Title = "Performance for ticker #" + ticker + ",",
                        Text = String.Join("\n          ", dataList),
                        Tap = new CardAction()
                        {
                            Type = ActionTypes.OpenUrl,
                            Value = "https://en-us.janushenderson.com/advisor/products?vehicle_type=MF"
                        }
                    };

                    message.Attachments.Add(heroCard.ToAttachment());
                    await context.PostAsync(message);
                }
                else
                {
                    context.ConversationData.SetValue("lastIntent", Constants.ACTION_GET_PERFORMANCE);
                    await context.PostAsync("Given Ticker # is invalid. Would like to check for another Ticker?");
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        private async Task ShowRating(IDialogContext context, string ticker)
        {
            try
            {
                FinanceModel financeModel = BotRepo.GetFinanceByTicker(ticker);
                if (financeModel != null)
                {
                    var message = context.MakeMessage();
                    List<string> dataList = new List<string>();
                    dataList.Add("Name : " + financeModel.name + " ,     ");
                    dataList.Add("MorningStar Category : " + financeModel.ms_category + " ,       ");
                    dataList.Add("Overall Ratings : " + financeModel.overall_rating + ",        ");
                    dataList.Add("Funds Rated Count : " + financeModel.rating_count + "        ");
                    var heroCard = new HeroCard
                    {
                        Title = "Morningstar ratings for ticker #" + ticker + ",",
                        Text = String.Join("\n   ", dataList),
                        Tap = new CardAction()
                        {
                            Type = ActionTypes.OpenUrl,
                            Value = "https://en-us.janushenderson.com/advisor/products?vehicle_type=MF"
                        }
                    };

                    message.Attachments.Add(heroCard.ToAttachment());
                    await context.PostAsync(message);
                }
                else
                {
                    context.ConversationData.SetValue("lastIntent", Constants.ACTION_GET_RATING);
                    await context.PostAsync("Given Ticker # is invalid. Would like to check for another Ticker?");
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        private async Task ShowPricing(IDialogContext context, string ticker)
        {
            try
            {
                FinanceModel financeModel = BotRepo.GetFinanceByTicker(ticker);

                if (financeModel != null)
                {
                    var message = context.MakeMessage();
                    List<string> dataList = new List<string>();
                    //dataList.Add("Here is the pricing details for ticker # " + option + " :    ");
                    dataList.Add("Name : " + financeModel.name + ",     ");
                    dataList.Add("MorningStar Category : " + financeModel.ms_category + ",     ");
                    dataList.Add("Market Price :  -  ,  ");
                    dataList.Add("Nav (as of 04/03/18) : $" + financeModel.nav + ",     ");
                    dataList.Add("Nav Change Percentage (as of 04/03/18)  : " + financeModel.nav_change_per + "%,       ");
                    dataList.Add("Nav Change Amount (as of 04/03/18) : $" + financeModel.nav_change_price + ",      ");
                    dataList.Add("Premium Discount :  -  ");

                    var heroCard = new HeroCard
                    {
                        Title = "Pricing details for ticker #" + ticker + ",",
                        //Subtitle = "Your bots — wherever your users are talking",
                        Text = String.Join("\n   ", dataList),
                        Tap = new CardAction()
                        {
                            Type = ActionTypes.OpenUrl,
                            Value = "https://en-us.janushenderson.com/advisor/products?vehicle_type=MF"
                        }
                    };

                    message.Attachments.Add(heroCard.ToAttachment());
                    await context.PostAsync(message);
                    //await this.ShowLuisResult(context, String.Join("\n ", dataList));
                }
                else
                {
                    context.ConversationData.SetValue("lastIntent", Constants.ACTION_GET_PRICING);
                    await context.PostAsync("Given Ticker # is invalid. Would like to check for another Ticker?");
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        private async Task ShowFactsheet(IDialogContext context, string fundName)
        {
            try
            {
                FinanceModel financeModel = BotRepo.GetFactsheet(fundName);
                if (financeModel != null)
                {
                    var message = context.MakeMessage();
                    ThumbnailCard thumbnailCard = new ThumbnailCard
                    {
                        Title = "Fact Sheet ",
                        //  Subtitle = "Your bots — wherever your users are talking",
                        Text = "See attached document for Factsheet of " + fundName + ", ",
                        Images = new List<CardImage> { new CardImage(Constants.JH_LOGO) },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Open",
                                value: financeModel.fact_sheet_url) },
                        Tap = new CardAction(ActionTypes.OpenUrl, "",
                        value: financeModel.fact_sheet_url)
                    };
                    message.Attachments.Add(thumbnailCard.ToAttachment());
                    await context.PostAsync(message);
                }
                else
                {
                    //  context.ConversationData.SetValue("lastIntent", Constants.ACTION_GET_PRICING);
                    await context.PostAsync("Given FundName is invalid. Would like to check for another FundName?");
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        private async Task ShowAllFactsheets(IDialogContext context)
        {
            try
            {
                var financeList = BotRepo.GetFinanceList();
                await context.PostAsync("Top " + financeList.Count + " factsheets");
                var message = context.MakeMessage();
                message.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                foreach (FinanceModel finance in financeList)
                {
                    message.Attachments.Add(new ThumbnailCard
                    {
                        Title = finance.name,
                        //  Subtitle = "Your bots — wherever your users are talking",
                        Text = "See attached document for Fact Sheet ",
                        Images = new List<CardImage> { new CardImage(Constants.JH_LOGO) },

                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "View",
                                value: finance.fact_sheet_url) },
                        Tap = new CardAction(ActionTypes.OpenUrl, "",
                        value: finance.fact_sheet_url)
                    }.ToAttachment());
                }
                await context.PostAsync(message);
                context.ConversationData.SetValue("lastIntent", Constants.FACTSHEET);
                //await context.PostAsync("Want to check specific factsheet?");
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        private async Task ShowPortfolio(IDialogContext context, string fundName)
        {
            try
            {
                FinanceModel financeModel = BotRepo.GetFactsheet(fundName);
                if (financeModel != null)
                {
                    var message = context.MakeMessage();
                    ThumbnailCard thumbnailCard = new ThumbnailCard
                    {
                        Title = " Portfolio Commentary ",
                        //  Subtitle = "Your bots — wherever your users are talking",
                        Text = "See attached document for Portfolio commentary of " + fundName + ", ",
                        Images = new List<CardImage> { new CardImage(Constants.JH_LOGO) },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Open",
                                value: financeModel.portfolio_commentary_url) },
                        Tap = new CardAction(ActionTypes.OpenUrl, "",
                        value: financeModel.portfolio_commentary_url)
                    };
                    message.Attachments.Add(thumbnailCard.ToAttachment());
                    await context.PostAsync(message);
                }
                else
                {
                    //await this.ShowLuisResult(context, "Given FundName is invalid.");
                    await context.PostAsync("Given FundName is invalid. Would like to check for another FundName?");
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        private async Task ShowAllPortfolios(IDialogContext context)
        {
            try
            {
                var financeList = BotRepo.GetFinanceList();
                await context.PostAsync("Top " + financeList.Count + " portfolio commentaries");
                var postfolioMessage = context.MakeMessage();
                postfolioMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                foreach (FinanceModel finance in financeList)
                {
                    postfolioMessage.Attachments.Add(new ThumbnailCard
                    {
                        Title = finance.name,
                        //  Subtitle = "Your bots — wherever your users are talking",
                        Text = "See attached document for Portfolio Commentary ",
                        Images = new List<CardImage> { new CardImage(Constants.JH_LOGO) },

                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "View",
                                value: finance.portfolio_commentary_url) },
                        Tap = new CardAction(ActionTypes.OpenUrl, "",
                        value: finance.portfolio_commentary_url)
                    }.ToAttachment());
                }

                await context.PostAsync(postfolioMessage);
                context.ConversationData.SetValue("lastIntent", Constants.PORTFOLIO);
                //await context.PostAsync("Want to check specific portfolio?");
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        private async Task ShowAnnualReport(IDialogContext context, string fundName)
        {
            try
            {
                FinanceModel financeModel = BotRepo.GetFactsheet(fundName);
                if (financeModel != null)
                {
                    var message = context.MakeMessage();
                    ThumbnailCard thumbnailCard = new ThumbnailCard
                    {
                        Title = "Annual Report ",
                        //  Subtitle = "Your bots — wherever your users are talking",
                        Text = "See attached document for Annual report of " + fundName + ", ",
                        Images = new List<CardImage> { new CardImage(Constants.JH_LOGO) },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Open",
                                value: financeModel.annual_report_url) },
                        Tap = new CardAction(ActionTypes.OpenUrl, "",
                        value: financeModel.annual_report_url)
                    };
                    message.Attachments.Add(thumbnailCard.ToAttachment());
                    await context.PostAsync(message);
                }
                else
                {
                    //  await this.ShowLuisResult(context, "Given FundName is invalid.");
                    await context.PostAsync("Given FundName is invalid. Would like to check for another FundName?");
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        private async Task ShowAllAnnualReports(IDialogContext context)
        {
            try
            {
                var financeList = BotRepo.GetFinanceList();
                await context.PostAsync("Top " + financeList.Count + " annual reports");
                var reportMessage = context.MakeMessage();
                reportMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                foreach (FinanceModel finance in financeList)
                {
                    reportMessage.Attachments.Add(new ThumbnailCard
                    {
                        Title = finance.name,
                        //  Subtitle = "Your bots — wherever your users are talking",
                        Text = "See attached document for Annual Report ",
                        Images = new List<CardImage> { new CardImage(Constants.JH_LOGO) },

                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "View",
                                value: finance.annual_report_url) },
                        Tap = new CardAction(ActionTypes.OpenUrl, "",
                            value: finance.annual_report_url)
                    }.ToAttachment());
                }
                await context.PostAsync(reportMessage);
                context.ConversationData.SetValue("lastIntent", Constants.ANNUALREPORT);
                //await context.PostAsync("Want to check specific annual report?");
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        private async Task ShowFundHolding(IDialogContext context, string fundName)
        {
            try
            {
                FinanceModel financeModel = BotRepo.GetFactsheet(fundName);
                if (financeModel != null)
                {
                    var message = context.MakeMessage();
                    ThumbnailCard thumbnailCard = new ThumbnailCard
                    {
                        Title = "Fund Holdings ",
                        //  Subtitle = "Your bots — wherever your users are talking",
                        Text = "Click the button to see Fund holdings of " + fundName + ", ",
                        Images = new List<CardImage> { new CardImage(Constants.JH_LOGO) },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Open",
                                value: financeModel.full_holdings_url) },
                        Tap = new CardAction(ActionTypes.OpenUrl, "",
                        value: financeModel.full_holdings_url)
                    };
                    message.Attachments.Add(thumbnailCard.ToAttachment());
                    await context.PostAsync(message);
                }
                else
                {
                    // await this.ShowLuisResult(context, "Given FundName is invalid.");
                    await context.PostAsync("Given FundName is invalid. Would like to check for another FundName?");

                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        private async Task ShowAllFundHoldings(IDialogContext context)
        {
            try
            {
                var financeList = BotRepo.GetFinanceList();

                await context.PostAsync("Top " + financeList.Count + " fund holdings");
                var holdingsMessage = context.MakeMessage();
                holdingsMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                foreach (FinanceModel finance in financeList)
                {
                    holdingsMessage.Attachments.Add(new ThumbnailCard
                    {
                        Title = finance.name,
                        //  Subtitle = "Your bots — wherever your users are talking",
                        Text = "Click the button to see Fund holdings ",
                        Images = new List<CardImage> { new CardImage(Constants.JH_LOGO) },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Open",
                                value: finance.full_holdings_url) },
                        Tap = new CardAction(ActionTypes.OpenUrl, "",
                            value: finance.full_holdings_url)
                    }.ToAttachment());
                }
                await context.PostAsync(holdingsMessage);
                context.ConversationData.SetValue("lastIntent", Constants.FUNDHOLDING);
                //await context.PostAsync("Want to check specific fund holding?");
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }



        //Common Scenarios

        [LuisIntent("DividendStudy")]
        public async Task DividendStudyIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                var message = context.MakeMessage();
                message.Attachments = new List<Attachment>();
                message.Attachments.Add(new VideoCard
                {
                    Title = "Janus Henderson Global Dividend Study",
                    // Subtitle = "by the Blender Institute",
                    Text = "JHGDS is a long-term study into global dividend trends. It is a measure of the progress that global firms are making in paying their investors an income on their capital. It analyses dividends paid every quarter by the 1,200 largest firms by capitalisation.",
                    //Image = new ThumbnailUrl
                    //{
                    //    Url = System.Web.HttpContext.Current.Server.MapPath("~/ App_Data / Images / Cisco_logo.png")
                    //},
                    Media = new List<MediaUrl>
            {
                new MediaUrl()
                {
                    Url = "https://www.youtube.com/watch?v=cSdsDrOxn2g"
                }
            },
                    Buttons = new List<CardAction>
            {
                new CardAction()
                {
                    Title = "Open in YouTube",
                    Type = ActionTypes.OpenUrl,
                    Value = "https://www.youtube.com/watch?v=cSdsDrOxn2g"
                }
            }
                }.ToAttachment()
            );
                await context.PostAsync(message);

            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        [LuisIntent("JHInvestingGuide")]
        public async Task JHInvestingGuideIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                var message = context.MakeMessage();
                message.Attachments = new List<Attachment>();
                message.Attachments.Add(new VideoCard
                {
                    Title = "Janus Henderson - Steps to investing guide",
                    // Subtitle = "by the Blender Institute",
                    Text = "This Janus Henderson guide takes you through the steps required to purchase an investment trust, and reminds you of points to consider before investing, and explains the different types of investment strategies, inviting you to consider the most appropriate one for you.",
                    //Image = new ThumbnailUrl
                    //{
                    //    Url = System.Web.HttpContext.Current.Server.MapPath("~/ App_Data / Images / Cisco_logo.png")
                    //},
                    Media = new List<MediaUrl>
            {
                new MediaUrl()
                {
                    Url = "https://www.youtube.com/watch?v=S1_GLOJxLEc"
                }
            },
                    Buttons = new List<CardAction>
            {
                new CardAction()
                {
                    Title = "Open in YouTube",
                    Type = ActionTypes.OpenUrl,
                    Value = "https://www.youtube.com/watch?v=S1_GLOJxLEc"
                }
            }
                }.ToAttachment()
            );
                await context.PostAsync(message);

            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }




        [LuisIntent("MarketInsights")]
        public async Task MarketInsightsIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                var message = context.MakeMessage();
                message.Attachments = new List<Attachment>();
                message.Attachments.Add(new VideoCard
                {
                    Title = "PM Perspectives: Rising Demand, Capacity Constraints Create Opportunity for Industrials",
                    // Subtitle = "by the Blender Institute",
                    Text = "With aggregate demand improving globally, George Maris explains why long-shunned industrials are positioned to benefit after a decade focusing on cost control.",
                    //Image = new ThumbnailUrl
                    //{
                    //    Url = System.Web.HttpContext.Current.Server.MapPath("~/ App_Data / Images / Cisco_logo.png")
                    //},
                    Media = new List<MediaUrl>
            {
                new MediaUrl()
                {
                    Url = "https://www.youtube.com/watch?v=Fgr_nKi0Hqg"
                }
            },
                    Buttons = new List<CardAction>
            {
                new CardAction()
                {
                    Title = "Open in YouTube",
                    Type = ActionTypes.OpenUrl,
                    Value = "https://www.youtube.com/watch?v=Fgr_nKi0Hqg"
                }
            }
                }.ToAttachment()
            );



                message.Attachments.Add(new VideoCard
                {
                    Title = "PM Perspectives: As Rates Rise, Floating-Rate Instruments May Provide Income Potential",
                    // Subtitle = "by the Blender Institute",
                    Text = "Portfolio Manager Seth Meyer shares why he believes investors shouldn’t shun fixed income as rates rise and instead, focus on assets that leverage an improving economic outlook.",
                    //Image = new ThumbnailUrl
                    //{
                    //    Url = System.Web.HttpContext.Current.Server.MapPath("~/ App_Data / Images / Cisco_logo.png")
                    //},
                    Media = new List<MediaUrl>
            {
                new MediaUrl()
                {
                    Url = "https://www.youtube.com/watch?v=IHECseRaoMU"
                }
            },
                    Buttons = new List<CardAction>
            {
                new CardAction()
                {
                    Title = "Open in YouTube",
                    Type = ActionTypes.OpenUrl,
                    Value = "https://www.youtube.com/watch?v=IHECseRaoMU"
                }
            }
                }.ToAttachment()
            );
                await context.PostAsync(message);

            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }



        [LuisIntent("GlobalEquities ")]
        public async Task GlobalEquitiesIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                var message = context.MakeMessage();
                message.Attachments = new List<Attachment>();
                message.Attachments.Add(new VideoCard
                {
                    Title = "How Will Global Equities Fare in a Rising Rates Environment?",
                    // Subtitle = "by the Blender Institute",
                    Text = "With the 10-year Treasury approaching 3% and yields rising globally, Adam Schor shares why investors should consider how equities will perform in a higher rate environment.",
                    //Image = new ThumbnailUrl
                    //{
                    //    Url = System.Web.HttpContext.Current.Server.MapPath("~/ App_Data / Images / Cisco_logo.png")
                    //},
                    Media = new List<MediaUrl>
            {
                new MediaUrl()
                {
                    Url = "https://www.youtube.com/watch?v=NtGurwqLFmw"
                }
            },
                    Buttons = new List<CardAction>
            {
                new CardAction()
                {
                    Title = "Open in YouTube",
                    Type = ActionTypes.OpenUrl,
                    Value = "https://www.youtube.com/watch?v=NtGurwqLFmw"
                }
            }
                }.ToAttachment()
            );
                await context.PostAsync(message);

            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        [LuisIntent("ScheduleMeeting")]
        public async Task ScheduleMeetingIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                PromptDialog.Text(context, ResumeAfterGetMeetingTitleValue, "Please give a meeting title");
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        private async Task ResumeAfterGetMeetingTitleValue(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string title = await result;
                //await context.PostAsync("Checking for availability...");
                if (title != null)
                {
                    //Boolean isIntentMatched =  await this.CheckForIntent(context);
                    //if (!isIntentMatched)
                    //{
                    context.ConversationData.SetValue(Constants.REMEMBER_MEETING_TITLE, title);
                    List<string> MEETING_TIMES = new List<string>();
                    MEETING_TIMES.Add(Utils.GetNextWeekday(DateTime.UtcNow.DayOfWeek).Date.ToString("ddd, dd MMMM yyyy") + " | 11:00 - 11:30");
                    MEETING_TIMES.Add(Utils.GetNextWeekday(DateTime.UtcNow.DayOfWeek).Date.ToString("ddd, dd MMMM yyyy") + " | 13:30 - 14:00");
                    context.ConversationData.SetValue(Constants.REMEMBER_MEETING_TIMES, MEETING_TIMES);
                    PromptDialog.Choice<string>(context, MeetingTimeChoiceReceivedAsync,
                            new PromptOptions<string>("Choose from availability",
                            "Invalid choice. Please choose another.", "Let me get you there...",
                           MEETING_TIMES, 0));
                    // }
                }
                else
                {
                    PromptDialog.Text(context, ResumeAfterGetMeetingTitleValue, "Please give a valid meeting title");
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        private async Task MeetingTimeChoiceReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string time = await result;
                await context.PostAsync("Setting up the meeting...");
                if (time != null)
                {
                    if (context.ConversationData.GetValue<List<string>>(Constants.REMEMBER_MEETING_TIMES).Contains(time))
                    {

                        if (time.Contains("11:00 - 11:30"))
                        {
                            //                    TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow + new TimeSpan(11, 0, 0),
                            //TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                            //new DateTime("2018-04-24 11:12 PM")

                            await context.PostAsync("Sending the meeting invite...");
                            await MailHandler.SendMeetingInvite(context.ConversationData.GetValue<string>(Constants.REMEMBER_MEETING_TITLE),
                               Utils.GetNextWeekday(DateTime.UtcNow.DayOfWeek).Date + new TimeSpan(11, 0, 0),
                               Utils.GetNextWeekday(DateTime.UtcNow.DayOfWeek).Date + new TimeSpan(11, 30, 0));
                            await context.PostAsync("Scheduled meeting On " + time);
                        }
                        else if (time.Contains("13:30 - 14:00"))
                        {
                            await context.PostAsync("Sending the meeting invite...");
                            await MailHandler.SendMeetingInvite(context.ConversationData.GetValue<string>(Constants.REMEMBER_MEETING_TITLE),
                                Utils.GetNextWeekday(DateTime.UtcNow.DayOfWeek).Date + new TimeSpan(13, 30, 0),
                               Utils.GetNextWeekday(DateTime.UtcNow.DayOfWeek).Date + new TimeSpan(14, 0, 0));
                            await context.PostAsync("Scheduled meeting On " + time);
                        }
                    }
                    else
                    {
                        await this.CheckForIntent(context);
                    }
                }
                else
                {
                    PromptDialog.Choice<string>(context, MeetingTimeChoiceReceivedAsync,
                           new PromptOptions<string>("Choose from availability",
                           "Invalid choice. Please choose another.", "Let me get you there...",
                          context.ConversationData.GetValue<List<string>>(Constants.REMEMBER_MEETING_TIMES), 0));
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.CheckForIntent(context);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.GetBaseException());
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

        private static int WordToNumber(String number)
        {
            int returnVal = 0;
            switch (number.ToLower())
            {

                case "One":
                    returnVal = 1;
                    break;
                case "two":
                    returnVal = 2;
                    break;
                case "three":
                    returnVal = 3;
                    break;
                case "four":
                    returnVal = 4;
                    break;
                case "five":
                    returnVal = 5;
                    break;
                case "six":
                    returnVal = 6;
                    break;
                case "seven":
                    returnVal = 7;
                    break;
                case "eight":
                    returnVal = 8;
                    break;
                case "nine":
                    returnVal = 9;
                    break;
                case "ten":
                    returnVal = 10;
                    break;
            }
            return returnVal;
        }


        private async Task<Boolean> CheckForIntent(IDialogContext context)
        {
            Boolean isIntentMatched = false;
            try
            {
                LUIS objLUISResult = await LUISHandler.QueryLUIS(context.Activity.AsMessageActivity().Text);

                if (objLUISResult.topScoringIntent != null)
                {
                    switch (objLUISResult.topScoringIntent.intent)
                    {
                        case Constants.INTENT_GREETING:
                            isIntentMatched = true;
                            await this.GreetingIntent(context, null);
                            break;

                        case Constants.INTENT_CHECK_STOCK_PERFORMANCE:
                            isIntentMatched = true;
                            await this.CheckStockPerformanceIntent(context, null);
                            break;
                        case Constants.INTENT_CHECK_ANALYST_PERFORMANCE:
                            isIntentMatched = true;
                            await this.CheckAnalystPerformanceIntent(context, null);
                            break;
                        case Constants.INTENT_HIGH_PERFORMING_STOCKS:
                            isIntentMatched = true;
                            await this.HighPerformingStockIntent(context, null);
                            break;
                        case Constants.INTENT_STOCKS_TO_FOCUS:
                            isIntentMatched = true;
                            await this.StockToFocusIntent(context, null);
                            break;
                        case Constants.INTENT_INVESTING_GUIDE:
                            isIntentMatched = true;
                            await this.JHInvestingGuideIntent(context, null);
                            break;
                        case Constants.INTENT_MARKET_INSIGHTS:
                            isIntentMatched = true;
                            await this.MarketInsightsIntent(context, null);
                            break;
                        case Constants.INTENT_SCHEDULE_MEETING:
                            isIntentMatched = true;
                            await this.ScheduleMeetingIntent(context, null);
                            break;
                            //case Constants.INTENT_THANK:
                            //    isIntentMatched = true;
                            //    await this.ShowLuisResult(context, "Happy to help!");
                            //    break;
                            //case Constants.INTENT_BYE:
                            //    isIntentMatched = true;
                            //    await this.ShowLuisResult(context, "Bye! See you soon.");
                            //    break;
                    }
                }
                else
                {
                    await this.ShowLuisResult(context, "I'm sorry, I don't understand :(");
                }
            }
            catch (Exception exception)
            {

                Debug.WriteLine(exception.GetBaseException());
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
            return isIntentMatched;
        }
    }
}