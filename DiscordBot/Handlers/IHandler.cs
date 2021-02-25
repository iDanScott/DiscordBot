using Discord.WebSocket;
using DiscordBot.Interfaces;
using System.Threading.Tasks;

namespace DiscordBot.Handlers
{
    public interface IHandler
    {
        Task Handle(IDiscordClient client, SocketMessage arg);
        bool Handles(string command);
    }
}
