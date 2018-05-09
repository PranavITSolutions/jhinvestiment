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


        [LuisIntent("CheckSentimentScore")]
        public async Task CheckSentimentScoreIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                PromptDialog.Choice<string>(context, CheckScoreChoiceReceivedAsync,
                            new PromptOptions<string>("Whom do you want to check score? ",
                            "Invalid option. Please choose another.", "Let me get you there...",
                            Constants.ACTION_CHECK_SCORE_OPTIONS, 0));
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        private async Task CheckScoreChoiceReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string option = await result;
                switch (option)
                {
                    case Constants.ACTION_CHECK_SCORE_OPTION_CUSTOMER:
                        PromptDialog.Text(context, ResumeAfterGetCustomerName, "Customer name?");
                        break;

                    case Constants.ACTION_CHECK_SCORE_OPTION_AGENT:
                        PromptDialog.Text(context, ResumeAfterGetAgentID, "Agent Sales Connect Rep ID #?");
                        break;
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.CheckForIntent(context);
            }
            catch (Exception)
            {
                await this.ShowLuisResult(context, "Bot returning an error. Please check later. Sorry!");
            }
        }


        private async Task ResumeAfterGetCustomerName(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string customerName = await result;
                if (customerName != null)
                {
                    // Get customer's Info and return
                }
                else
                {
                    Boolean isIntentMatched = await this.CheckForIntent(context);
                    if (!isIntentMatched)
                    {
                        PromptDialog.Choice<string>(context, ResumeAfterCustomerCheckConfirmation,
                                       new PromptOptions<string>("Invalid Customer Name. Would you like to check for another customer?",
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
                        PromptDialog.Text(context, ResumeAfterGetCustomerName, "Customer name?");
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



        private async Task ResumeAfterGetAgentID(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string customerName = await result;
                if (customerName != null)
                {
                    // Get agent's Info and return
                }
                else
                {
                    Boolean isIntentMatched = await this.CheckForIntent(context);
                    if (!isIntentMatched)
                    {
                        PromptDialog.Choice<string>(context, ResumeAfterAgentCheckConfirmation,
                                       new PromptOptions<string>("Invalid Agent ID. Would you like to check for another customer?",
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
                        PromptDialog.Text(context, ResumeAfterGetAgentID, "Agent Sales Connect Rep ID #?");
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
                    new CardAction(ActionTypes.ImBack, title: Constants.ACTION_CHECK_SCORE, value: Constants.ACTION_CHECK_SCORE)
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
                        case Constants.INTENT_THANK:
                            isIntentMatched = true;
                            await this.ShowLuisResult(context, "Happy to help!");
                            break;
                        case Constants.INTENT_BYE:
                            isIntentMatched = true;
                            await this.ShowLuisResult(context, "Bye! See you soon.");
                            break;
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