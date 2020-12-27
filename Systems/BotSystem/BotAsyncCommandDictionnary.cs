using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    public static Dictionary<string, BotAsyncCommand> commandDic = new Dictionary<string, BotAsyncCommand>
    {
      {
        "reboot", new BotAsyncCommand(
          name: "reboot",
          execute: ExecuteRebootCommand
        )
      },
      {
        "say", new BotAsyncCommand(
          name: "say",
          execute: ExecuteSayCommand
        )
      },
    };
  }
}
