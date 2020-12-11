using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    private static void ExecuteSayCommand(string prout)
    {
      ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_DM_TALK, Module.textToSpeak, NWScript.GetFirstPC());
    }
  }
}
