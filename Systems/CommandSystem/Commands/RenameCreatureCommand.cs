using System;
using NWN.Enums;
using NWN.NWNX;

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

        Action<uint, Vector> callback = (uint target, Vector position) =>
        {
          NWCreature oCreature = target.AsCreature();
          if (player.IsDM && oCreature.Master == player)
            oCreature.Name = newName;
          else
            player.SendMessage($"{oCreature.Name} n'est pas une de vos invocations, vous ne pouvez pas modifier son nom");
        };

        player.SelectTarget(callback);
      }
    }
  }
}
