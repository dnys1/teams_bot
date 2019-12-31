// Copyright (c) Microsoft Corporation. All rights reserved.
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

        private async Task<string> GetHomeUNC(string user)
        {
            return null;
        }

        // Returns the guid for the registered link in the database
        private async Task<string> CreateGuidInDatabase(string from, string link)
        {
            // Format path as file URI
            if (!link.Contains("file:"))
            {
                if (link.Contains("P:"))
                {
                    link = "file:///" + link.Replace('\\', '/');
                } else if (link.Contains(@"\\"))
                {
                    link = "file:" + link.Replace('\\', '/');
                }
            }
            // Upload path to DB
            var newGuid = Guid.NewGuid().ToString();
            _context.Add(
                new ServerLink
                {
                    From = from,
                    Link = link,
                    ID = newGuid,
                });
            await _context.SaveChangesAsync();
            return newGuid;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.

            // Enable rewind of the request body so we can read it multiple times
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
                var link = jsonObj.Value.Data.Link;

                // Retrieve user's home drive if a P:/ link
                if (link.Contains("P:/"))
                {
                    var UNCRoot = await GetHomeUNC(from);
                }

                // Create the link in the database and embed in Request.Body
                var guid = await CreateGuidInDatabase(from, link);

                var newLink = $"http://localhost:3978/link/{guid}";
                jsonObj.Value.Data.Link = newLink;

                var json = JsonConvert.SerializeObject(jsonObj);
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                var stream = await requestContent.ReadAsStreamAsync();

                Request.Body = stream;

                await Adapter.ProcessAsync(Request, Response, Bot);
            } else
            {
                throw new Exception("Error processing request. Please try again.");
            }
        }
    }
}
