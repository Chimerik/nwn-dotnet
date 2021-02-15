using System.Collections.Generic;
using System.Linq;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteListenCommand(ChatSystem.Context ctx, Options.Result options)
    {
      NwPlayer oDM = ctx.oSender.ToNwObject<NwPlayer>();
      if (oDM.IsDM || oDM.IsDMPossessed || oDM.IsPlayerDM)
      {
        if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player dungeonMaster))
        {
          if (NWScript.GetIsObjectValid(ctx.oTarget) != 1)
          {
            if (dungeonMaster.listened.Count > 0)
            {
              oDM.SendServerMessage("Vous cessez d'écouter les conversations des joueurs?");
              dungeonMaster.listened.Clear();
            }
            else
            {
              foreach(NwPlayer oPC in NwModule.Instance.Players.Where(p => !p.IsDM && !p.IsDMPossessed && !p.IsPlayerDM))
                dungeonMaster.listened.Add(oPC);
            }
          }
          else
          {
            NwPlayer oPC = ctx.oTarget.ToNwObject<NwPlayer>();
            if (dungeonMaster.listened.Contains(oPC))
              dungeonMaster.listened.Remove(oPC);
            else
                dungeonMaster.listened.Add(oPC);
          }
        }
      }
    }
  }
}
