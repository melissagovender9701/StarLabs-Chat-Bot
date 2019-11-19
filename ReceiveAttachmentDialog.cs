using Newtonsoft.Json.Linq;
using ReceiveAttachmentBot.ClientService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Configuration;
using System.Threading;

namespace ReceiveAttachmentBot
{

    [Serializable]
    internal class ReceiveAttachmentDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            #region Typing
            var typingMsg = context.MakeMessage();
            typingMsg.Type = ActivityTypes.Typing;
            typingMsg.Text = null;
            await context.PostAsync(typingMsg);
            #endregion

            var message = await argument;

            var UserName = message.From.Name;

            #region Not Active
            if ((message.Text != null && message.Text.ToLower() == "clear") || (message.Text != null && message.Text.ToLower() == ""))
            {
                GlobalSettings.MessageLevel = "Not Active";
            }
            #endregion

            #region Active
            if (GlobalSettings.MessageLevel == "Not Active")
            {
                GlobalSettings.MessageLevel = "Active";

                Activity reply = ((Activity)context.Activity).CreateReply();

                await context.PostAsync($"Hello {UserName} :grin:! Welcome to StarLabs Shopping Bot! :dress: :necktie:");
                #region Carousel Card
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                var actions = new List<CardAction>();
                var Images = new List<CardImage>();

                Images.Add(new CardImage(url: "https://starlabschatbotapp.azurewebsites.net/image/eli.jpg")
                {

                });

                actions.Add(new CardAction
                {
                    Value = "Yes",
                    Type = "postBack",
                    Title = "Yes",
                });
                actions.Add(new CardAction
                {
                    Value = "No",
                    Type = "postBack",
                    Title = "No",
                });

                reply.Attachments.Add(
                    new HeroCard
                    {
                        Title = "My name is Eli! Would you like to take a look at our range of clothing? ",
                        Buttons = actions,
                        Images = Images,
                    }.ToAttachment()
                );
                #endregion
                await context.PostAsync(reply);
            }
            #endregion

            #region message_level_webview
            else if (GlobalSettings.MessageLevel == "Active" && message.Text != null && message.Text.ToLower() == "yes")
            {
                GlobalSettings.MessageLevel = "message_level_webview";
                #region Webview
                var reply = ((Activity)context.Activity).CreateReply();

                var attachment = new
                {
                    type = "template",
                    payload = new
                    {
                        template_type = "button",
                        text = "View our online clothing store to see our newest range of clothing!",
                        buttons = new[]
                        {
                        new
                        {
                            type = "web_url",
                            url = "https://ultd.co.za/",
                            title = "View",
                            webview_height_ratio = "full",
                            messenger_extensions = true,

                        },
                        },
                    },
                };

                reply.ChannelData = JObject.FromObject(new { attachment });
                #endregion

                #region Main Menu Button
                var actions = new List<CardAction>();

                actions.Add(new CardAction
                {
                    Value = "Main Menu",
                    Type = "postBack",
                    Title = "Main Menu",
                });
                reply.Attachments.Add(
                    new HeroCard
                    {
                        Title = "If you'd like to go back, click Main Menu!",
                        Buttons = actions,
                    }.ToAttachment()
                );
                #endregion
                await context.PostAsync(reply);
            }
            #endregion

            #region message_level_close
            else if (GlobalSettings.MessageLevel == "Active" && message.Text != null && message.Text.ToLower() == "no")
            {
                GlobalSettings.MessageLevel = "message_level_close";

                Activity reply = ((Activity)context.Activity).CreateReply();

                reply.Text = "Thank you for chatting with me! I hope to hear from you next time :blush:";

                #region Main Menu Quick Reply
                var channelData = JObject.FromObject(new
                {
                    quick_replies = new dynamic[]
                    {
                        new
                        {
                            content_type = "text",
                            title = "Main Menu",
                            payload = "Main Menu"
                        }
                    }
                });

                reply.ChannelData = channelData;
                #endregion

                await context.PostAsync(reply);
            }
            #endregion

            #region Main Menu
            else if ((GlobalSettings.MessageLevel == "message_level_close"&& message.Text != null && message.Text.ToLower() == "main menu") || (GlobalSettings.MessageLevel == "message_level_webview" && message.Text != null && message.Text.ToLower() == "main menu")) 
            {
                #region Active
                GlobalSettings.MessageLevel = "Active";

                Activity reply = ((Activity)context.Activity).CreateReply();

                await context.PostAsync($"Hello {UserName} :grin:! Welcome to StarLabs Shopping Bot! :dress: :necktie:");

                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                var actions = new List<CardAction>();
                var Images = new List<CardImage>();

                Images.Add(new CardImage(url: "https://starlabschatbotapp.azurewebsites.net/image/eli.jpg")
                {

                });

                actions.Add(new CardAction
                {
                    Value = "Yes",
                    Type = "postBack",
                    Title = "Yes",
                });
                actions.Add(new CardAction
                {
                    Value = "No",
                    Type = "postBack",
                    Title = "No",
                });

                reply.Attachments.Add(
                    new HeroCard
                    {
                        Title = "My name is Eli! Would you like to take a look at our range of clothing? ",
                        Buttons = actions,
                        Images = Images,
                    }.ToAttachment()
                );

                await context.PostAsync(reply);
                #endregion
            }
            #endregion

            context.Wait(this.MessageReceivedAsync);
        }
    }
}