using System.Collections.Generic;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteListenCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWScript.GetIsDM(ctx.oSender) == 1)
      {
        PlayerSystem.Player oDM;
        if (PlayerSystem.Players.TryGetValue(ctx.oSender, out oDM))
        {
          if (NWScript.GetIsObjectValid(ctx.oTarget) != 1)
          {
            if (oDM.listened.Count > 0)
              oDM.listened.Clear();
            else
            {
              foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
              {
                if (NWScript.GetIsDM(PlayerListEntry.Key) != 1)
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
