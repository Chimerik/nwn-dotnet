using System.Threading.Tasks;
using Discord.WebSocket;
using System.Linq;
using Discord;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteRefreshLearnableDescriptionsCommand(SocketSlashCommand command)
    {
      SkillSystem.RefreshLearnableDescriptions();
      await command.RespondAsync("Mise à jour des descriptions effectuée", ephemeral: true);
    }
  }
}
