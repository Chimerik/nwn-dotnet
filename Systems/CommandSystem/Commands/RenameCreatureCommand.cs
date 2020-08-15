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
        player.lastTargetedCommandArgument = (string)options.positional[0]; // je vois toujours pas comment tu récupères cet argument sans le coller dans une propriété de player
        player.OnTargetSelection += HandleRenameCreatureSelection;

        //NWScript.EnterTargetingMode(player, ObjectType.Creature);
        NWScript.ExecuteScript("on_pc_target", player); // bouchon en attendant d'avoir la vraie fonction
      }
    }
    private static void HandleRenameCreatureSelection(object sender, PlayerSystem.Player.TargetSelectionEventArgs e)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue((uint)sender, out player)) // Est-ce que sender c'est bien le player avec lequel on a déclenché l'événement ?
      {
        NWCreature oCreature = e.target.AsCreature();

        /*if (!player.IsDM && oCreature.Master != player)
        {
          player.SendMessage($"{oCreature.Name} n'est pas une de vos invocations, vous ne pouvez pas modifier son nom");
          return Entrypoints.SCRIPT_HANDLED;
        }*/

        oCreature.Name = player.lastTargetedCommandArgument;
        NWNX.Creature.SetOriginalName(oCreature, player.lastTargetedCommandArgument, false); // Juste pour le test, à supprimer pour la vraie commande
        player.OnTargetSelection -= HandleRenameCreatureSelection;
      }
    }
  }
}
