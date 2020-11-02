using System;

namespace NWN.Systems
{
  public class BotAsyncCommand
  {
    public string name { get; }
    public Action<string> execute { get; }
    public BotAsyncCommand(
        string name,
        Action<string> execute
      )
    {
      this.name = name;
      this.execute = execute;
    }
  }
}
