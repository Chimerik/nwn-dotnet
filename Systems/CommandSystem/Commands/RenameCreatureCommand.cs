using System;
using System.Numerics;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRenameCreatureCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if(CreaturePlugin.GetKnowsFeat(ctx.oSender, (int)API.Constants.Feat.SpellFocusConjuration) == 0)
      {
        ctx.oSender.SendServerMessage("Le don de spécialisation en invocation est nécessaire pour pouvoir renommer une invocation.", Color.ORANGE);
        return;
      }

      ctx.oSender.GetLocalVariable<string>("_RENAME_VALUE").Value = (string)options.positional[0];
      PlayerSystem.cursorTargetService.EnterTargetMode(ctx.oSender, SummonRenameTarget, ObjectTypes.Creature, MouseCursor.Create);
    }
    private static void SummonRenameTarget(CursorTargetData selection)
    {
      if (selection.TargetObj != null && ((NwCreature)selection.TargetObj).Master == selection.Player)
        selection.TargetObj.Name = selection.Player.GetLocalVariable<string>("_RENAME_VALUE").Value;
      else
        selection.Player.SendServerMessage("Veuillez sélectionner une cible valide.", Color.RED);
    }
  }
}
