using BC.ServerTeamsBot.Data;
using BC.ServerTeamsBot.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BC.ServerTeamsBot.Controllers
{
    // Not used for anything right now, but can be used to create a DB
    // link using the API instead of using a DbContext object, i.e.
    // from another program.

    [Route("api/create")]
    [ApiController]
    public class LinkAPIController : ControllerBase
{
        private readonly ServerLinksDatabaseContext _context;

        public LinkAPIController (ServerLinksDatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = reader.ReadToEnd();

                var serverLink = JsonConvert.DeserializeObject<ServerLink>(body);

                if (ModelState.IsValid)
                {
                    if (serverLink.ID == null)
                    {
                        serverLink.ID = Guid.NewGuid().ToString();
                    }
                    if (serverLink.Link == null || serverLink.From == null)
                    {
                        return BadRequest();
                    }
                    _context.Add(serverLink);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok();
        }
    }
}
