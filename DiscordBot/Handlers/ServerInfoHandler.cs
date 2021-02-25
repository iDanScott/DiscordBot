using Discord.WebSocket;
using DiscordBot.Interfaces;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Handlers
{
    class ServerInfoHandler : IHandler
    {
        private ILogger _logger;
        private RestClient _infoClient;

        public ServerInfoHandler(ILogger logger)
        {
            _logger = logger;
            _infoClient = new RestClient("https://api.nitrado.net/services/8115561/gameservers");
        }

        public async Task Handle(IDiscordClient client, SocketMessage arg)
        {
            var token = await GetToken();
            _infoClient.Authenticator = new JwtAuthenticator(token);
            var request = new RestRequest(Method.GET);
            _logger.LogInformation($"requesting: {_infoClient.BaseUrl}");
            var response = await _infoClient.ExecuteAsync(request);
            _logger.LogInformation($"response: {response.Content}");
            await client.SendMessageAsync($"Server info response: ```{response.Content}```", arg.Channel.Id);
        }

        public Task<string> GetToken()
        {
            return Task.FromResult(Environment.GetEnvironmentVariable("Nitrado.Token"));
        }

        public bool Handles(string command)
        {
            return command == "info";
        }
    }
}
