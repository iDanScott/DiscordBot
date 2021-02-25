using System.Threading.Tasks;

namespace DiscordBot.Interfaces
{
    public interface IDiscordClient
    {
        void Run();
        Task SendMessageAsync(string message, ulong channelId);
    }
}
