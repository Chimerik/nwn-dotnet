using System.Collections.Generic;

using Anvil.Services;

namespace NWN.Systems
{
  public class ChatLine
  {
    public string portrait { get; set; }
    public string name { get; set; }
    public string playerName { get; set; }
    public string text { get; set; }
    public List<string> textHistory { get; set; }
    public ChatChannel channel { get; set; }
    public ChatCategory category { get; set; }

    public ChatLine(string portrait, string name, string playerName, string text, ChatChannel channel, ChatCategory category)
    {
      this.portrait = portrait;
      this.name = name;
      this.playerName = playerName;
      this.text = text;
      this.channel = channel;
      this.category = category;
      textHistory = new List<string>();
    }
    public enum ChatCategory
    {
      RolePlay,
      HorsRolePlay,
      Private
    }
  }
}
