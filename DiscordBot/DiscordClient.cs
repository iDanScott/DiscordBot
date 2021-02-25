using Discord;
using Discord.WebSocket;
using DiscordBot.Handlers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot
{
    class DiscordClient : Interfaces.IDiscordClient
    {
        private DiscordSocketClient _client;
        private readonly ILogger _logger;
        private readonly IEnumerable<IHandler> _handlers;

        public DiscordClient(ILogger logger, IEnumerable<IHandler> handlers)
        {
            _logger = logger;
            _handlers = handlers;
            _logger.LogInformation("Waking up....");
            _client = new DiscordSocketClient();

            var token = Environment.GetEnvironmentVariable("DiscordBot.Token");

            _client.MessageReceived += _client_MessageReceived;
            _client.LoggedIn += _client_LoggedIn;

            _client.LoginAsync(TokenType.Bot, token).ConfigureAwait(false).GetAwaiter().GetResult();
            _client.StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            _logger.LogInformation("Discord Bot Ready!");
        }

        private Task _client_LoggedIn()
        {
            _logger.LogInformation("Discord Bot Logged In!");

            return Task.CompletedTask;
        }

        public void Run() { }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            _logger.LogInformation($"Message Received: {arg.Content} from {arg.Author.Id} in channel {arg.Channel.Id}");

            var message = arg.Content;

            if (!message.StartsWith("!"))
            {
                return;
            }

            var command = message[1..];

            var tasks = new List<Task>();

            foreach (var handler in _handlers.Where(x => x.Handles(command)))
            {
                tasks.Add(handler.Handle(this, arg));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        public Task SendMessageAsync(string message, ulong channelId)
        {
            return ((ITextChannel)_client.GetChannel(channelId)).SendMessageAsync(message);
        }
    }
}
