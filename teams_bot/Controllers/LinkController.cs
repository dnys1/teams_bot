using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BC.ServerTeamsBot.Models;
using BC.ServerTeamsBot.Data;

namespace BC.ServerTeamsBot.Controllers
{
    public class LinkController : Controller
    {
        private readonly ServerLinksDatabaseContext _context;

        public LinkController(ServerLinksDatabaseContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string id)
        {
            if (id == null)
            {
                return NotFound(null);
            }

            var serverLink = await _context.ServerLinks
                .FindAsync(id);

            if (serverLink == null)
            {
                return NotFound(id);
            }

            return View(serverLink);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View(new ErrorViewModel { RequestId = RequestId });
        }
    }
}
