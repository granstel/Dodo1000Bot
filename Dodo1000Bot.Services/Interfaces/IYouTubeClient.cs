using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.RealtimeBoard;

namespace Dodo1000Bot.Services;

public interface IYouTubeClient
{
    Task<Statistics> Videos(string channelId, CancellationToken cancellationToken);
}