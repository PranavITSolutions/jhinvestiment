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
                await context.PostAsync($"Hi! I am your virtual chat Bot.");
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
                List<ResearchDTO> researchList = BotRepo.getCustomerList().Where(c => c.sentimentScore.Value > 0.55m).ToList();
                if (researchList != null && researchList.Count > 0)
                {
                    var message = context.MakeMessage();
                    message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    foreach (ResearchDTO researchDTO in researchList)
                    {
                        List<string> dataList = new List<string>();
                        dataList.Add("Stock : " + researchDTO.stockName + "<br>");
                        dataList.Add("Research Industry : " + researchDTO.researchIndustry + ",<br>");
                        dataList.Add("Research sector : " + researchDTO.researchSector + ",<br>");
                        dataList.Add("Overall Sentiment Score : " + String.Format("{0:0.##}", researchDTO.sentimentScore.Value) + "<br>");

                        HeroCard card = new HeroCard()
                        {
                            Title = researchDTO.stockName ,
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
                List<ResearchDTO> researchList = BotRepo.getCustomerList().Where(c => c.sentimentScore.Value < 0.55m).ToList();
                if (researchList != null && researchList.Count > 0)
                {
                    var message = context.MakeMessage();
                    message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    foreach (ResearchDTO researchDTO in researchList)
                    {
                        List<string> dataList = new List<string>();
                        dataList.Add("Stock : " + researchDTO.stockName + "<br>");
                        dataList.Add("Research Industry : " + researchDTO.researchIndustry + ",<br>");
                        dataList.Add("Research sector : " + researchDTO.researchSector + ",<br>");
                        dataList.Add("Overall Sentiment Score : " + String.Format("{0:0.##}", researchDTO.sentimentScore.Value) + "<br>");

                        HeroCard card = new HeroCard()
                        {
                            Title = researchDTO.stockName,
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

        [LuisIntent("CheckStockPerformance")]
        public async Task CheckStockPerformanceIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                //PromptDialog.Choice<string>(context, CheckScoreChoiceReceivedAsync,
                //            new PromptOptions<string>("Whom do you want to check score? ",
                //            "Invalid option. Please choose another.", "Let me get you there...",
                //            Constants.ACTION_CHECK_SCORE_OPTIONS, 0));
                PromptDialog.Text(context, ResumeAfterGetTicker, "Ticker?");
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
                //PromptDialog.Choice<string>(context, CheckScoreChoiceReceivedAsync,
                //            new PromptOptions<string>("Whom do you want to check score? ",
                //            "Invalid option. Please choose another.", "Let me get you there...",
                //            Constants.ACTION_CHECK_SCORE_OPTIONS, 0));
                PromptDialog.Text(context, ResumeAfterGetAnalystName, "Analyst name?");

            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }



        //private async Task CheckScoreChoiceReceivedAsync(IDialogContext context, IAwaitable<string> result)
        //{
        //    try
        //    {
        //        string option = await result;
        //        switch (option)
        //        {
        //            case Constants.ACTION_CHECK_SCORE_OPTION_TICKER:
        //                PromptDialog.Text(context, ResumeAfterGetTicker, "Ticker?");
        //                break;

        //            case Constants.ACTION_CHECK_SCORE_OPTION_ANALYST:
        //                PromptDialog.Text(context, ResumeAfterGetAnalystName, "Analyst name?");
        //                break;
        //        }
        //    }
        //    catch (TooManyAttemptsException)
        //    {
        //        await this.CheckForIntent(context);
        //    }
        //    catch (Exception)
        //    {
        //        await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
        //    }
        //}


        private async Task ResumeAfterGetTicker(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string ticker = await result;
                if (ticker != null)
                {
                    await context.PostAsync("Checking performance for Ticker " + ticker);
                    // Get customer's Info and return
                    ResearchDTO research = BotRepo.getCustomer(ticker.ToUpper());              
                    if (research != null )
                    {
                        var message = context.MakeMessage();
                        List<string> dataList = new List<string>();

                        dataList.Add("Stock : " + research.stockName + ",<br>  ");
                        dataList.Add("Research Industry : " + research.researchIndustry + ",<br>     ");
                        dataList.Add("Research Sector : " + research.researchSector + ",<br>      ");
                        dataList.Add("Total Research Count : " + research.researchCount + ",<br>     ");
                        dataList.Add("Overall Sentiment Score : " + String.Format("{0:0.##}", research.sentimentScore.Value) + "  ");

                        HeroCard card = new HeroCard()
                        {
                            Title = "Stock Performance for Ticker "+ticker,
                            Text = string.Join("     ", dataList)                           
                        };

                        message.Attachments.Add(card.ToAttachment());
                        await context.PostAsync(message);
                        PromptDialog.Choice<string>(context, ResumeAfterCustomerCheckConfirmation,
                                       new PromptOptions<string>("Would you like to check for another Stock?",
                                       "Selected action not available. Please choose another.", "Let me get you there...",
                                       Constants.CONFIRMATION_OPTIONS, 0));
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
                    await context.PostAsync("Checking performance for Analyst "+analystName);
                    ResearchDTO research = BotRepo.getAnalyst(analystName);

                    if (research != null)
                    { 
                        var message = context.MakeMessage();
                        List<string> dataList = new List<string>();
                        dataList.Add("Analyst Name : " + research.source + "<br>");
                        dataList.Add("Analyst Email : " + research.email + ",<br>");
                        dataList.Add("Research Sector : " + research.researchSector + ",<br>");
                        dataList.Add("Research Count : " + research.researchCount + ",<br>");
                        dataList.Add("Sentiment Score : " + String.Format("{0:0.##}", research.sentimentScore.Value) + "<br>");                     
                        HeroCard card = new HeroCard()
                        {
                            Title = "Performance of Analyst " + analystName,
                            Text = string.Join("     ", dataList)
                        };

                        message.Attachments.Add(card.ToAttachment());
                        await context.PostAsync(message);
                        PromptDialog.Choice<string>(context, ResumeAfterAgentCheckConfirmation,
                                           new PromptOptions<string>("Would you like to check for another Analyst?",
                                           "Selected action not available. Please choose another.", "Let me get you there...",
                                           Constants.CONFIRMATION_OPTIONS, 0));
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
                        PromptDialog.Text(context, ResumeAfterGetAnalystName, "Analyst Name?");
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
                     new CardAction(ActionTypes.ImBack, title: Constants.ACTION_HIGH_PERFORMING_STOCKS, value: Constants.ACTION_HIGH_PERFORMING_STOCKS),
                     new CardAction(ActionTypes.ImBack, title: Constants.ACTION_STOCKS_TO_FOCUS, value: Constants.ACTION_STOCKS_TO_FOCUS),
                     new CardAction(ActionTypes.ImBack, Constants.ACTION_GET_MARKET_INSIGHTS, value: Constants.ACTION_GET_MARKET_INSIGHTS),
                     new CardAction(ActionTypes.ImBack, Constants.ACTION_INVESTING_GUIDE, value: Constants.ACTION_INVESTING_GUIDE),
                    //new CardAction(ActionTypes.ImBack, title: Constants.ACTION_GET_PERFORMANCE, value: Constants.ACTION_GET_PERFORMANCE),
                    //new CardAction(ActionTypes.ImBack, Constants.ACTION_GET_PRICING, value: Constants.ACTION_GET_PRICING),
                    //new CardAction(ActionTypes.ImBack, Constants.ACTION_GET_RATING, value: Constants.ACTION_GET_RATING),
                    new CardAction(ActionTypes.ImBack, title: Constants.ACTION_GET_FACTSHEET, value: Constants.ACTION_GET_FACTSHEET),
                    new CardAction(ActionTypes.ImBack, Constants.ACTION_GET_PORTFOLIO, value: Constants.ACTION_GET_PORTFOLIO),
                    new CardAction(ActionTypes.ImBack, Constants.ACTION_GET_REPORT, value: Constants.ACTION_GET_REPORT),
                    new CardAction(ActionTypes.ImBack, Constants.ACTION_GET_HOLDING, value: Constants.ACTION_GET_HOLDING)
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


        [LuisIntent("TickerIntent")]
        public async Task TickerIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                if (result.Entities.Count > 0)
                {
                    if (result.Entities[0].Type.Equals("Ticker"))
                    {
                        string lastIntent = context.ConversationData.GetValue<string>("lastIntent");

                        switch (lastIntent)
                        {
                            case Constants.ACTION_GET_PERFORMANCE:
                                await this.ShowPerformance(context, result.Entities[0].Entity);
                                break;
                            case Constants.ACTION_GET_PRICING:
                                await this.ShowPricing(context, result.Entities[0].Entity);
                                break;
                            case Constants.ACTION_GET_RATING:
                                await this.ShowRating(context, result.Entities[0].Entity);
                                break;
                        }
                    }
                }
                else
                {
                    await context.PostAsync("Given Ticker # is invalid. Would like to check for another Ticker?");
                }
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }

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