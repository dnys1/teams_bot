using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using BC.ServerTeamsBot.Models;

namespace BC.ServerTeamsBot.Data
{
    public static class SeedData
{
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ServerLinksDatabaseContext(
                serviceProvider.GetRequiredService<DbContextOptions<ServerLinksDatabaseContext>>()))
            {
                // Look for any data
                if (context.ServerLink.Any())
                {
                    return; // DB has been seeded
                }

                context.ServerLink.AddRange(
                    new ServerLink
                    {
                        ID = Guid.NewGuid().ToString(),
                        Link = @"file:\\bcphxfp01\projects\Phoenix, City of\154095 - WSD-WWTPs Update Plans\Drawings\ES\91st"
                    },
                    new ServerLink
                    {
                        ID = Guid.NewGuid().ToString(),
                        Link = @"pw://brwncald-pw.bentley.com:brwncald-pw-01/Documents/P{bb47a88e-c112-4ec7-9a35-95d95b3b4bba}/"
                    }
                );

                context.SaveChanges();
            }
        }
}
}
