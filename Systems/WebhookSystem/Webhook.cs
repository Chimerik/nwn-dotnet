using JNogueira.Discord.Webhook.Client;
using System;
using System.Threading.Tasks;

namespace NWN.Systems
{
  public static partial class WebhookSystem
  {
    private static DiscordWebhookClient client = new DiscordWebhookClient(Environment.GetEnvironmentVariable("WEBHOOK"));

    public static void StartSendingAsyncDiscordMessage(string message, string userName)
    {
      var discordMessage = new DiscordMessage(
        message,
        username: userName,
        avatarUrl: "http://www.spellholdstudios.net/images/icons/icon_nwn.png",
        tts: false
      );

      SendDiscordMessage(discordMessage);
    }
    public static async Task SendDiscordMessage(DiscordMessage message)
    {
      await client.SendToDiscord(message);
    }
  }
}
