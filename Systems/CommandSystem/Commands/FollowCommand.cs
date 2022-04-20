using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteFollowCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oSender.ControlledCreature.MovementRate == MovementRate.Immobile 
        || Encumbrance2da.IsCreatureHeavilyEncumbred(ctx.oSender.ControlledCreature))
      {
        ctx.oSender.SendServerMessage("Cette commande ne peut être utilisée en étant surchargé.", ColorConstants.Red);
        return;
      }

      ctx.oSender.EnterTargetMode(FollowTarget, ObjectTypes.Creature, MouseCursor.Follow);
    }
    private static async void FollowTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled)
        return;

      if (selection.Player.ControlledCreature.MovementRate == MovementRate.Immobile
          || Encumbrance2da.IsCreatureHeavilyEncumbred(selection.Player.ControlledCreature))
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
