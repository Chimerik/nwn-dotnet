using System.Collections.Generic;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteListenCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWScript.GetIsDM(ctx.oSender))
      {
        PlayerSystem.Player oDM;
        if (PlayerSystem.Players.TryGetValue(ctx.oSender, out oDM))
        {
          if (!NWScript.GetIsObjectValid(ctx.oTarget) == 1)
          {
            if (oDM.listened.Count > 0)
              oDM.listened.Clear();
            else
            {
              foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
              {
                if (!NWScript.GetIsDM(PlayerListEntry.Key))
                  oDM.listened.Add(PlayerListEntry.Key, PlayerListEntry.Value);
              }
            }
          }
          else
          {
            if (oDM.listened.ContainsKey(ctx.oTarget))
              oDM.listened.Remove(ctx.oTarget);
            else
            {
              PlayerSystem.Player oPC;
              if (PlayerSystem.Players.TryGetValue(ctx.oTarget, out oPC))
                oDM.listened.Add(ctx.oTarget, oPC);
            }
          }
        }
      }
    }
  }
}
