using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FightCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FightCore.Bot
{
    public class ServerContextFactory : IDesignTimeDbContextFactory<ServerContext>
    {
        public ServerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ServerContext>();
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=FightCoreBot;Trusted_Connection=True;MultipleActiveResultSets=true;");

            return new ServerContext(optionsBuilder.Options);
        }
    }
}
