using System.Collections.Generic;

using Anvil.Services;

namespace NWN.Systems
{
  public class ChatLine
  {
    public string portrait { get; set; }
    public string receiverPortrait { get; set; }
    public string name { get; set; }
    public string playerName { get; set; }
    public string receiverPlayerName { get; set; }
    public string text { get; set; }
    public string untranslatedText { get; set; }
    public List<string> textHistory { get; set; }
    public ChatChannel channel { get; set; }
    public ChatCategory category { get; set; }

    public ChatLine(string portrait, string name, string playerName, string text, string untranslatedText, ChatChannel channel, ChatCategory category, string receiverPlayerName = "", string receiverPortrait = "")
    {
      this.portrait = portrait;
      this.receiverPortrait = receiverPortrait;
      this.name = name;
      this.playerName = playerName;
      this.receiverPlayerName = receiverPlayerName;
      this.text = text.Trim();
      this.untranslatedText = untranslatedText.Trim();
      this.channel = channel;
      this.category = category;
      textHistory = new List<string>();
      textHistory.Add(this.text);
    }
    public enum ChatCategory
    {
      RolePlay,
      HorsRolePlay,
      Private
    }
  }
}
