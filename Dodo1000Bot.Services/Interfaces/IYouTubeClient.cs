using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Youtube;

namespace Dodo1000Bot.Services;

public interface IYouTubeClient
{
    Task<Video[]> SearchVideos(string channelId, CancellationToken cancellationToken);
}