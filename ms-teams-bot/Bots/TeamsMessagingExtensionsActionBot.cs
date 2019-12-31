// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;

namespace BC.ServerTeamsBot.Bots
{
    public class TeamsMessagingExtensionsActionBot : TeamsActivityHandler
    {
        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            switch (action.CommandId)
            {
                // These commandIds are defined in the Teams App Manifest.
                case "insertServerLink":
                    return InsertServerLink(turnContext, action);
                default:
                    throw new NotImplementedException($"Invalid CommandId: {action.CommandId}");
            }
        }

        private MessagingExtensionActionResponse InsertServerLink(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action)
        {
            // The user has chosen to create a card by choosing the 'Create Card' context menu command.
            var serverLinkData = ((JObject)action.Data).ToObject<ServerLinkData>();
            
            var card = new HeroCard
            {
                Title = "Project Link",
                Subtitle = "Link to our server",
                Tap = new CardAction
                {
                    Type = ActionTypes.OpenUrl,
                    Title = "Open Link",
                    Value = serverLinkData.Link,
                }
            };

            var attachments = new List<MessagingExtensionAttachment>();
            attachments.Add(new MessagingExtensionAttachment
            {
                Content = card,
                ContentType = HeroCard.ContentType,
                Preview = card.ToAttachment(),
            });
            
            return new MessagingExtensionActionResponse
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    AttachmentLayout = "list",
                    Type = "result",
                    Attachments = attachments,
                },
            };
        }

        private class ServerLinkData
        {
            public string Link { get; set; }
        }
    }
}
