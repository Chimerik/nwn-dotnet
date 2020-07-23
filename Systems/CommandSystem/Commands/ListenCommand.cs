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
          if (!ctx.oTarget.IsValid)
          {
            if (oDM.Listened.Count > 0)
              oDM.Listened.Clear();
            else
            {
              foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
              {
                if (!NWScript.GetIsDM(PlayerListEntry.Key))
                  oDM.Listened.Add(PlayerListEntry.Key, PlayerListEntry.Value);
              }
            }
          }
          else
          {
            if (oDM.Listened.ContainsKey(ctx.oTarget))
              oDM.Listened.Remove(ctx.oTarget);
            else
            {
              PlayerSystem.Player oPC;
              if (PlayerSystem.Players.TryGetValue(ctx.oTarget, out oPC))
                oDM.Listened.Add(ctx.oTarget, oPC);
            }
          }
        }
      }
    }
  }
}
