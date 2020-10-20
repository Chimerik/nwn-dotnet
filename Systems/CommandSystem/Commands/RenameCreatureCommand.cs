using System;
using System.Numerics;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRenameCreatureCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        string newName = (string)options.positional[0];

        Action<uint, Vector3> callback = (uint target, Vector3 position) =>
        {
          var oCreature = target;
          if (NWScript.GetIsDM(player.oid) == 1 || NWScript.GetMaster(oCreature) == player.oid)
            NWScript.SetName(oCreature, newName);
          else
            NWScript.SendMessageToPC(player.oid, $"{NWScript.GetName(oCreature)} n'est pas une de vos invocations, vous ne pouvez pas modifier son nom");
        };

        player.SelectTarget(callback);
      }
    }
  }
}
