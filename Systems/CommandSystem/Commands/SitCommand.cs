using System;
using NWN.Core;
using NWN.Core.NWNX;
using System.Numerics;
using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ActionFacing(uint player, uint oObject)
    {
      NWScript.DelayCommand(0.1f, () => NWScript.AssignCommand(player, () => NWScript.SetFacing(-NWScript.GetFacing(oObject))));
      //ObjectPlugin.SetPosition(player, NWScript.GetPosition(oObject));
    }
    private static void ExecuteSitCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        var animOpt = (bool)options.named.GetValueOrDefault("down");
        int animation;

        if(animOpt)
          animation = NWScript.ANIMATION_LOOPING_SIT_CROSS;
        else
          animation = NWScript.ANIMATION_LOOPING_SIT_CHAIR;

        Action<uint, Vector3> callback = (uint target, Vector3 position) =>
        {
          if (target == NWScript.GetArea(player.oid))
          {
            var location = NWScript.Location(NWScript.GetArea(player.oid), position, NWScript.GetFacing(player.oid));
            //var oSitter = NWScript.GetNearestObjectToLocation(NWScript.OBJECT_TYPE_PLACEABLE, location);
            var oSitter = NWScript.GetFirstObjectInShape(NWScript.SHAPE_CUBE, 2.0f, location, 0, NWScript.OBJECT_TYPE_PLACEABLE);
            NWScript.AssignCommand(player.oid, () => NWScript.ActionMoveToLocation(location));

            if (NWScript.GetTag(oSitter) == "chair")
              NWScript.AssignCommand(player.oid, () => NWScript.ActionDoCommand(() => ActionFacing(player.oid, oSitter)));

            NWScript.AssignCommand(player.oid, () => NWScript.ActionPlayAnimation(animation, 1.0f, 999999.0f));
          }
          else
            NWScript.AssignCommand(player.oid, () => NWScript.ActionSit(target));

          player.LoadMenuQuickbar(QuickbarType.Sit);
        };

        player.targetEvent = TargetEvent.SitTarget;
        player.SelectTarget(callback);
      }
    }
  }
}
