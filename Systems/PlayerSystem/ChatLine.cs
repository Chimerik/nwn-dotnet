using Anvil.Services;

namespace NWN.Systems
{
  public class ChatLine
  {
    public string portrait { get; set; }
    public string name { get; set; }
    public string playerName { get; set; }
    public string text { get; set; }
    public ChatChannel channel { get; set; }

    public ChatLine(string portrait, string name, string playerName, string text, ChatChannel channel)
    {
      this.portrait = portrait;
      this.name = name;
      this.playerName = playerName;
      this.text = text;
      this.channel = channel;
    }
  }
}
