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
      int result = DiscordUtils.CheckPlayerCredentialsFromDiscord(context, sPCName);
      if (result > 0)
      {
        await NwTask.SwitchToMainThread();

        foreach (NwPlayer player in NwModule.Instance.Players)
        {
          if (ObjectPlugin.GetInt(player, "characterId") == result)
          {
            await player.SpeakString(text);
            break;
          }
        }

        await context.Channel.SendMessageAsync("Texte en cours de relais vers votre personnage.");
        return;
      }

      await context.Channel.SendMessageAsync("Le personnage indiqué n'existe pas, n'est pas connecté ou n'a pas été enregistré avec votre code Discord et votre clef cd.");
    }
  }
}
