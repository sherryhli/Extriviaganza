using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Threading.Tasks;

using GameBackend.APIClient;
using GameBackend.Models;

namespace GameBackend.Hubs
{
    public class GameHub : Hub
    {
        // TODO
        // public async Task AddToGame(string gameId)
        // {
        //     await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        // }

        // public async Task RemoveFromGroup(string gameId)
        // {
        //     await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        // }

        public async Task SendQuestion(string gameId)
        {
            // TODO: Pre-fetch questions
            QbQuestionsClient client = new QbQuestionsClient();
            var response = await client.GetRandomQuestionAsync();
            var json = await response.Content.ReadAsStringAsync();
            QbQuestion question = JsonConvert.DeserializeObject<QbQuestion>(json);
            await Clients.All.SendAsync("ReceiveQuestion", question);

            // TODO
            //await Clients.Group(gameId).SendAsync("ReceiveQuestion", question);
        }
    }
}
