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
            { "intro_validation", HandleIntroValidation },
        };

    private static int HandleDialogStart(uint oidSelf)
    {
      uint oPC = NWScript.GetLastSpeaker();

      if(!Convert.ToBoolean(NWScript.GetIsObjectValid(oPC)))
        oPC = NWScript.GetLastUsedBy();

      Player player;
      if (Players.TryGetValue(oPC, out player))
      {
        switch (NWScript.GetTag(oidSelf))
        {
          case "petitepatate":
            new Bank(player);
            break;
          case "micropatate":
            new Storage(player);
            break;
          case "intro_mirror":
            new IntroMirror(player);
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
    private static int HandleIntroValidation(uint oidSelf)
    {
      uint oPC = NWScript.GetLastSpeaker();

      if (ObjectPlugin.GetInt(oPC, "_STARTING_SKILL_POINTS") > 0)
      {
        NWScript.SendMessageToPC(oPC, $"Il vous reste encore {ObjectPlugin.GetInt(oPC, "_STARTING_SKILL_POINTS")} points de compétences à dépenser auprès du reflet avant de pouvoir débarquer !");
        return 0;
      }
      else
        return -1;
    }
  }
}
