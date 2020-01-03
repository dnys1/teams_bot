using Microsoft.EntityFrameworkCore;

namespace BC.ServerTeamsBot.Data
{
    public class ServerLinksDatabaseContext : DbContext
{
        public ServerLinksDatabaseContext(
            DbContextOptions<ServerLinksDatabaseContext> options
        ) : base(options)
        {
        }

        public DbSet<Models.ServerLink> ServerLinks { get; set; }
}
}
