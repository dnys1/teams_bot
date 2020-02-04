// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;
using BC.ServerTeamsBot.Data;

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

            var link = serverLinkData.OriginalLink;

            string icon = null, title = "Project Folder Link";

            if (LinkFormatter.IsDirPath(link))
            {
                if (Path.HasExtension(link))
                {
                    title = Path.GetFileName(link);

                    switch (Path.GetExtension(link).ToLower())
                    {
                        case ".xls":
                        case ".xlsx":
                            icon = "xlsx_icon.png";
                            break;
                        case ".doc":
                        case ".docx":
                            icon = "docx_icon.png";
                            break;
                        case ".ppt":
                        case ".pptx":
                            icon = "pptx_icon.png";
                            break;
                        case ".pdf":
                            icon = "pdf_icon.png";
                            break;
                        case ".zip":
                            icon = "zip_icon_small.png";
                            break;
                        default:
                            icon = "file_icon.png";
                            break;
                    }
                }
                else
                {
                    var pathParts = link.Split(Path.DirectorySeparatorChar);
                    title = pathParts[pathParts.Length - 1];
                    icon = "folder_icon.png";
                }
            }
            else
            {
                icon = "projectwise_icon.png";
            }

            List<CardImage> cardImages = new List<CardImage>();
            if (!string.IsNullOrEmpty(icon))
            {
                cardImages.Add(new CardImage { Url = $"{LinkFormatter.ImageBaseUrl}/{icon}" });
            }

            ThumbnailCard card;

            if (LinkFormatter.IsDirPath(link))
            {
                if (Path.HasExtension(link))
                {
                    var buttons = new List<CardAction>();
                    buttons.Add(new CardAction
                    {
                        Type = ActionTypes.OpenUrl,
                        Title = "Open File",
                        Value = serverLinkData.FileLink,
                    });

                    buttons.Add(new CardAction
                    {
                        Type = ActionTypes.OpenUrl,
                        Title = "Open Folder",
                        Value = serverLinkData.FolderLink,
                    });

                    card = new ThumbnailCard
                    {
                        Title = title,
                        Subtitle = serverLinkData.OriginalLink,
                        Text = "Use one of the buttons below to open this link.",
                        Images = cardImages,
                        Buttons = buttons
                    };
                }

                else
                {
                    var buttons = new List<CardAction>();
                    buttons.Add(new CardAction
                    {
                        Type = ActionTypes.OpenUrl,
                        Title = "Open Folder",
                        Value = serverLinkData.FolderLink,
                    });

                    card = new ThumbnailCard
                    {
                        Title = title,
                        Subtitle = serverLinkData.OriginalLink,
                        Text = "Use the button below to open this link.",
                        Images = cardImages,
                        Buttons = buttons
                    };
                }
            }
            else
            {
                var buttons = new List<CardAction>();
                buttons.Add(new CardAction
                {
                    Type = ActionTypes.OpenUrl,
                    Title = "Open ProjectWise Link",
                    Value = serverLinkData.FileLink,
                });

                card = new ThumbnailCard
                {
                    Title = title,
                    Subtitle = serverLinkData.OriginalLink,
                    Text = "Use the button below to open this link.",
                    Images = cardImages,
                    Buttons = buttons
                };
            }

            var attachments = new List<MessagingExtensionAttachment>();
            attachments.Add(new MessagingExtensionAttachment
            {
                Content = card,
                ContentType = ThumbnailCard.ContentType,
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
            public string FolderLink { get; set; }
            public string FileLink { get; set; }
            public string OriginalLink { get; set; }
        }
    }
}
