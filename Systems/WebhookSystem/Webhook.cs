using JNogueira.Discord.Webhook.Client;
using System;
using System.Threading.Tasks;

namespace NWN.Systems
{
  public static partial class WebhookSystem
  {
    private static DiscordWebhookClient client = new DiscordWebhookClient(Environment.GetEnvironmentVariable("WEBHOOK"));
    //"discordapp.com//api/webhooks/737378235402289264/3-nDoj7dEw-edzjM-DDyjWFCZbs6LXACoJ9vFnOWXc8Pn2nArFEt3HiVIhHyu_lYiNUt/slack"

    public static void StartSendingAsyncDiscordMessage(string message, string userName)
    {
      var discordMessage = new DiscordMessage(
        "Discord Webhook Client sent this message! " + DiscordEmoji.Grinning,
        username: "Username",
        avatarUrl: "http://www.spellholdstudios.net/images/icons/icon_nwn.png",
        tts: false
      );

      var test = SendDiscordMessage(discordMessage);
    }
    public static async Task SendDiscordMessage(DiscordMessage message)
    {
      await client.SendToDiscord(message);
    }
  }
}
