﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using BC.ServerTeamsBot.Models;
using BC.ServerTeamsBot.Data;

namespace BC.ServerTeamsBot.Controllers
{
    // This ASP Controller is created to handle a request. Dependency Injection will provide the Adapter and IBot
    // implementation at runtime. Multiple different IBot implementations running at different endpoints can be
    // achieved by specifying a more specific type for the bot constructor argument.
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter Adapter;
        private readonly IBot Bot;

        private readonly ServerLinksDatabaseContext _context;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot, ServerLinksDatabaseContext context)
        {
            Adapter = adapter;
            Bot = bot;
            _context = context;
        }

        // TODO: Get the UNC path for the user if submitted a P:/ link
        private async Task<string> GetHomeUNC(string user)
        {
            return null;
        }

        // Returns the guid for the registered link in the database
        private async Task<string> RegisterLinkInDatabase(string from, string link)
        {
            link = LinkFormatter.FormatString(link);

            // Upload path to DB
            var guid = Guid.NewGuid().ToString();

            _context.Add(
                new ServerLink
                {
                    From = from,
                    Link = link,
                    ID = guid,
                });
            await _context.SaveChangesAsync();

            return $"{LinkFormatter.BaseUrl}/{guid}";
        }

        [HttpPost]
        public async Task PostAsync()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.

            // Enable rewind of the request body so we can read it multiple times.
            // We need to read it here in this method and then rewind it so that
            // Adapter.ProcessAsync() can read it from the beginning as well.
            Microsoft.AspNetCore.Http.Internal.BufferingHelper.EnableRewind(Request);

            RequestBody jsonObj;
            using (var mem = new MemoryStream())
            using (var reader = new StreamReader(mem))
            {
                Request.Body.CopyTo(mem);

                var body = reader.ReadToEnd();

                mem.Seek(0, SeekOrigin.Begin);

                body = reader.ReadToEnd();

                jsonObj = JsonConvert.DeserializeObject<RequestBody>(body);
            }

            if (jsonObj != null)
            {
                var from = jsonObj.From.Name;
                var link = jsonObj.Value.Data.OriginalLink;

                if (LinkFormatter.IsCopyAsPathLink(link))
                {
                    // Remove "s from link generated from "Copy as path" in Windows
                    link = link.Substring(1, link.Length - 2);
                    jsonObj.Value.Data.OriginalLink = link;
                }

                if (LinkFormatter.IsDocumentURN(link))
                {
                    // Remove 'url:' from beginning of link
                    link = link.Substring("url:".Length);
                    jsonObj.Value.Data.OriginalLink = link;
                }

                if (!LinkFormatter.IsProperlyFormatted(link))
                {
                    throw new Exception("Improperly formatted string. Please try again.");
                }

                if (LinkFormatter.IsProjectWiseLink(link))
                {
                    var fileLink = await RegisterLinkInDatabase(from, link);
                    jsonObj.Value.Data.FileLink = fileLink;
                }
                else
                {
                    // Retrieve user's home drive if a P:/ link
                    if (link.Contains("P:/"))
                    {
                        var UNCRoot = await GetHomeUNC(from);
                    }

                    // Create the links in the database and embed in Request.Body
                    if (Path.HasExtension(link))
                    {
                        // Create both a file and folder link for file paths given,
                        // in case the user would like to go to the enclosing folder.
                        var fileLink = await RegisterLinkInDatabase(from, link);
                        jsonObj.Value.Data.FileLink = fileLink;

                        var folderLink = await RegisterLinkInDatabase(from, Path.GetDirectoryName(link));
                        jsonObj.Value.Data.FolderLink = folderLink;
                    }
                    else
                    {
                        // Create only the folder link for folder paths sent.
                        var folderLink = await RegisterLinkInDatabase(from, link);
                        jsonObj.Value.Data.FolderLink = folderLink;
                    }
                }

                var json = JsonConvert.SerializeObject(jsonObj);
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                var stream = await requestContent.ReadAsStreamAsync();

                Request.Body = stream;

                await Adapter.ProcessAsync(Request, Response, Bot);
            }
            else
            {
                throw new Exception("Error processing request. Please try again.");
            }
        }
    }
}
