using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteFollowCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oSender.ControlledCreature.MovementRate == MovementRate.Immobile
        || ctx.oSender.ControlledCreature.TotalWeight > Encumbrance2da.encumbranceTable.GetDataEntry(ctx.oSender.ControlledCreature.GetAbilityScore(Ability.Strength)).heavy)
      {
        ctx.oSender.SendServerMessage("Cette commande ne peut être utilisée en étant surchargé.", ColorConstants.Red);
        return;
      }

      PlayerSystem.cursorTargetService.EnterTargetMode(ctx.oSender, FollowTarget, ObjectTypes.Creature, MouseCursor.Follow);
    }
    private static async void FollowTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled)
        return;

      if (selection.Player.ControlledCreature.MovementRate == MovementRate.Immobile
            || selection.Player.ControlledCreature.TotalWeight > Encumbrance2da.encumbranceTable.GetDataEntry(selection.Player.ControlledCreature.GetAbilityScore(Ability.Strength)).heavy)
      {
        selection.Player.SendServerMessage("Cette commande ne peut être utilisée en étant surchargé.", ColorConstants.Red);
        return;
      }

      if(selection.Player.ControlledCreature.Area != ((NwCreature)selection.TargetObject).Area) // TODO : A DECOMMENTER A LA FIN DE L'ALPHA
      {
        selection.Player.SendServerMessage("Vous ne pouvez pas suivre quelqu'un qui ne se trouve pas dans la même zone que vous.", ColorConstants.Red);
        return;
      }

      await selection.Player.ControlledCreature.ActionForceFollowObject((NwGameObject)selection.TargetObject, 3.0f);
    }
  }
}
