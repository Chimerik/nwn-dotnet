using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  static public class TargetSystem
  {
    public static Dictionary<string, Func<PlayerSystem.Player, uint, Vector, int>> Register = new Dictionary<string, Func<PlayerSystem.Player, uint, Vector, int>>
    {
            { "renameCreature", HandleRenameCreatureCommand },
            
    };

    private static int HandleRenameCreatureCommand(PlayerSystem.Player player, uint oTarget, Vector vTarget)
    {
      NWCreature oCreature = oTarget.AsCreature();
      /*if (!player.IsDM && oCreature.Master != player)
      {
        player.SendMessage($"{oCreature.Name} n'est pas une de vos invocations, vous ne pouvez pas modifier son nom");
        return Entrypoints.SCRIPT_HANDLED;
      }*/

      oCreature.Name = player.lastTargetedCommandArgument;
      NWNX.Creature.SetOriginalName(oCreature, player.lastTargetedCommandArgument, false); // Juste pour le test, à supprimer pour la vraie commande

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
