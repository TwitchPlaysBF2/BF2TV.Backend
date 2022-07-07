using System.Threading.Tasks;
using Refit;

namespace BF2TV.Backend.Functions.DiscordApi;

public interface IDiscordApi
{
    [Get("/channels/{channelId}/messages")]
    Task<string> GetMessagesForChannelId(string channelId, [Header("Authorization")] string authorization);
}