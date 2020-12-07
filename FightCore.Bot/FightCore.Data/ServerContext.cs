using FightCore.Models;
using Microsoft.EntityFrameworkCore;

namespace FightCore.Data
{
    public class ServerContext : DbContext
    {
        public ServerContext(DbContextOptions<ServerContext> options) : base(options)
        {
        }

        public DbSet<ServerSettings> ServerSettings { get; set; }
    }
}
