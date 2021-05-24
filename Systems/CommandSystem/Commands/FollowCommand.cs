using System;
using NWN.Core;
using NWN.API.Constants;
using NWN.Services;
using NWN.API;
using NWN.API.Events;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteFollowCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oSender.ControlledCreature.MovementRate == MovementRate.Immobile
        || ctx.oSender.ControlledCreature.TotalWeight > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", ctx.oSender.ControlledCreature.GetAbilityScore(Ability.Strength))))
      {
        ctx.oSender.SendServerMessage("Cette commande ne peut être utilisée en étant surchargé.", Color.RED);
        return;
      }

      PlayerSystem.cursorTargetService.EnterTargetMode(ctx.oSender, FollowTarget, ObjectTypes.Creature, MouseCursor.Follow);
    }
    private static void FollowTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.Player.ControlledCreature.MovementRate == MovementRate.Immobile
            || selection.Player.ControlledCreature.TotalWeight > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", selection.Player.ControlledCreature.GetAbilityScore(Ability.Strength))))
      {
        selection.Player.SendServerMessage("Cette commande ne peut être utilisée en étant surchargé.", Color.RED);
        return;
      }

      /*if(selection.Player.Area != ((NwCreature)selection.TargetObj).Area // TODO : A DECOMMENTER A LA FIN DE L'ALPHA
      {
        selection.Player.SendServerMessage("Vous ne pouvez pas suivre quelqu'un qui ne se trouve pas dans la même zone que vous.", Color.RED);
        return;
      }*/

      selection.Player.ControlledCreature.ActionForceFollowObject((NwGameObject)selection.TargetObject, 3.0f);
    }
  }
}
