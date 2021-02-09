using Discord.Commands;
using System.Threading.Tasks;

namespace NWN.Systems
{
    public static partial class BotSystem
    {
        public static async Task ExecuteRefillCommand(SocketCommandContext context)
        {
            if (DiscordUtils.GetPlayerStaffRankFromDiscord(context.User.Id) != "admin")
            {
                await context.Channel.SendMessageAsync("Noooon, vous n'êtes pas la maaaaaître ! Le maaaaître est bien plus poli, d'habitude !");
                return;
            }
            
            await ModuleSystem.SpawnCollectableResources(0.0f);
            await context.Channel.SendMessageAsync("Refill terminé");
        }
    }
}
