using System.Threading.Tasks;
using Dodo1000Bot.Models;

namespace Dodo1000Bot.Services
{
    public interface IDialogflowService
    {
        Task<Dialog> GetResponseAsync(Request request);
    }
}