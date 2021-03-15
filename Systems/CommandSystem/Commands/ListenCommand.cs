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
      if (ctx.oSender.IsDM || ctx.oSender.IsDMPossessed || ctx.oSender.IsPlayerDM)
      {
        if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player dungeonMaster))
        {
          if (ctx.oTarget != null)
          {
            if (dungeonMaster.listened.Count > 0)
            {
              ctx.oSender.SendServerMessage("Vous cessez d'écouter les conversations des joueurs", Color.CYAN);
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
            if (dungeonMaster.listened.Contains(ctx.oTarget))
              dungeonMaster.listened.Remove(ctx.oTarget);
            else
                dungeonMaster.listened.Add(ctx.oTarget);
          }
        }
      }
    }
  }
}
