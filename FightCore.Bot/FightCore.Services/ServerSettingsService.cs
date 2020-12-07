using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FightCore.Data;
using FightCore.Models;
using Microsoft.EntityFrameworkCore;

namespace FightCore.Services
{
    public interface IServerSettingsService
    {
        Task<ServerSettings> GetForServerId(ulong serverId);

        ServerSettings CreateDefaultSettings(ulong serverId);

        ServerSettings CreateDefaultSettings();

        Task SetPrefix(ulong serverId, char prefix);
    }

    public class ServerSettingsService : IServerSettingsService
    {
        private readonly ServerContext _serverContext;
        private readonly Dictionary<ulong, ServerSettings> _cache;

        public ServerSettingsService(ServerContext databaseContext)
        {
            _serverContext = databaseContext;
            _cache = new Dictionary<ulong, ServerSettings>();
        }

        public async Task<ServerSettings> GetForServerId(ulong serverId)
        {
            if (_cache.ContainsKey(serverId))
            {
                return _cache[serverId];
            }

            var serverSettings =
                await _serverContext.ServerSettings.FirstOrDefaultAsync(settings => settings.ServerId == serverId);

            if (serverSettings == null)
            {
                return CreateDefaultSettings(serverId);
            }

            _cache.TryAdd(serverId, serverSettings);
            return serverSettings;

        }

        public ServerSettings CreateDefaultSettings(ulong serverId)
        {
            return new ServerSettings()
            {
                Prefix = "-",
                ServerId = serverId
            };
        }

        public ServerSettings CreateDefaultSettings()
        {
            return new ServerSettings()
            {
                Prefix = "-"
            };
        }

        public async Task SetPrefix(ulong serverId, char prefix)
        {
            var serverSettings = await GetForServerId(serverId);
            serverSettings.Prefix = prefix.ToString();

            _serverContext.Update(serverSettings);
            _cache[serverId] = serverSettings;
            await _serverContext.SaveChangesAsync();
        }
    }
}
