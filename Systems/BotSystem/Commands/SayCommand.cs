using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using NWN.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static async Task ExecuteSayCommand(SocketCommandContext context, string sPCName, string text)
    {
      await NwTask.SwitchToMainThread();

      int result = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, sPCName);
      if (result > 0)
      {
        NwPlayer player = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p, "characterId") == result);

        if(player != null)
        {
          await player.SpeakString(text);
          await context.Channel.SendMessageAsync("Texte en cours de relais vers votre personnage.");
          return;
        }
      }

      await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas, n'est pas connecté ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
    }
  }
}
