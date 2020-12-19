using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    private static void ExecuteSayCommand(string sayText)
    {
      string text = sayText.Remove(0, 4);
      int pcId;
      if(int.TryParse(text.Substring(0, text.IndexOf("_")), out pcId))
      {
        text = text.Remove(0, text.IndexOf("_") + 1);

        uint oPC = NWScript.GetFirstPC();

        while(Convert.ToBoolean(NWScript.GetIsObjectValid(oPC)))
        {
          if (pcId == ObjectPlugin.GetInt(oPC, "characterId"))
          {
            ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, text, oPC);
            break;
          }
          oPC = NWScript.GetNextPC();
        }
      }
    }
  }
}
