using System;
using System.Collections.Generic;
using System.Text;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class DialogSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { "diag_root", HandleDialogStart },
            { "intro_start", HandleIntroStart },
        };

    private static int HandleDialogStart(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(NWScript.GetLastSpeaker(), out player))
      {
        switch (NWScript.GetTag(oidSelf))
        {
          case "petitepatate":
            new Bank(player);
            break;
          case "micropatate":
            new Storage(player);
            break;
        }
      }
        
      return 0;
    }
    private static int HandleIntroStart(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(NWScript.GetLastSpeaker(), out player))
      {
        Area area; 
        if (Module.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(NWScript.GetArea(player.oid)), out area))
          area.StartEntryScene(player);
      }

      return 0;
    }
  }
}
