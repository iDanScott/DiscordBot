using Discord.WebSocket;
using DiscordBot.Interfaces;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Handlers
{
    public class RebootValheimServerHandler : IHandler
    {
        private ILogger _logger;
        private RestClient _rebootClient;

        public RebootValheimServerHandler(ILogger logger)
        {
            _logger = logger;
            _rebootClient = new RestClient("https://api.nitrado.net/services/8115561/gameservers/restart");
        }

        public async Task Handle(IDiscordClient client, SocketMessage arg)
        {
            var token = await GetToken();
            _rebootClient.Authenticator = new JwtAuthenticator(token);
            _logger.LogInformation($"Reboot requested");
            var request = new RestRequest(Method.POST);
            var response = await _rebootClient.ExecuteAsync(request);
            _logger.LogInformation($"Response from server: {response.Content}");
            await client.SendMessageAsync("Reboot requested!", arg.Channel.Id);
        }

        public Task<string> GetToken()
        {
            return Task.FromResult(Environment.GetEnvironmentVariable("Nitrado.Token"));
        }

        public bool Handles(string command)
        {
            return command == "reboot";
        }
    }
}
