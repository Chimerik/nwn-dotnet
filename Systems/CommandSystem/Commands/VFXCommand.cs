using System;
using System.Collections.Generic;
using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteVFXCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (((string)options.positional[0]).Length != 0)
      {
        Log.Info($"positionnal : {options.positional[0]}");

        if (Int32.TryParse((string)options.positional[0], out int value))
        {
          ctx.oSender.LoginCreature.GetLocalVariable<int>("_VFX_ID").Value = value;
          PlayerSystem.cursorTargetService.EnterTargetMode(ctx.oSender, playerVFXTarget, ObjectTypes.Creature, MouseCursor.Magic);
        }
        else if (ctx.oSender.IsDM && ((string)options.positional[0]).Length > 0
          && PlayerSystem.Players.TryGetValue(ctx.oSender.LoginCreature, out PlayerSystem.Player player))
        {
          string vfxName = (string)options.positional[0];

          var result = SqLiteUtils.SelectQuery("dmVFX",
            new List<string>() { { "vfxId" } },
            new List<string[]>() { new string[] { "playerName", player.oid.PlayerName }, new string[] { "vfxName", vfxName } });

          if (result != null && result.Count() > 0)
          {
            ctx.oSender.LoginCreature.GetLocalVariable<int>("_VFX_ID").Value = result.FirstOrDefault().GetInt(0);
            PlayerSystem.cursorTargetService.EnterTargetMode(ctx.oSender, dmVFXTarget, ObjectTypes.All, MouseCursor.Magic);
          }
          else
            ctx.oSender.SendServerMessage($"Vous n'avez pas enregistré de vfx correspondant au nom {vfxName.ColorString(ColorConstants.White)}", ColorConstants.Red);
        }
      }
    }
    private static void playerVFXTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled)
        return;

      if (selection.Player.LoginCreature.GetLocalVariable<int>("_VFX_ID").HasValue)
      {
        selection.Player.ApplyInstantVisualEffectToObject((VfxType)selection.Player.LoginCreature.GetLocalVariable<int>("_VXF_TEST_ID").Value, (NwGameObject)selection.TargetObject);
        selection.Player.LoginCreature.GetLocalVariable<int>("_VFX_ID").Delete();
      }
    }
    private static void dmVFXTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.Player.LoginCreature.GetLocalVariable<int>("_VFX_ID").HasNothing)
        return;

      VfxType vfxId = (VfxType)selection.Player.LoginCreature.GetLocalVariable<int>("_VFX_ID").Value;
      selection.Player.LoginCreature.GetLocalVariable<string>("_VFX_ID").Delete();

      var result = SqLiteUtils.SelectQuery("dmVFXDuration",
        new List<string>() { { "vfxDuration" } },
        new List<string[]>() { new string[] { "playerName", selection.Player.PlayerName } });

      int vfxDuration = 0;

      if (result != null && result.Count() > 0)
        vfxDuration = result.FirstOrDefault().GetInt(0);

      if (selection.TargetObject is NwGameObject target)
      {
        if (vfxDuration <= 0)
          target.ApplyEffect(EffectDuration.Permanent, API.Effect.VisualEffect(vfxId));
        else
          target.ApplyEffect(EffectDuration.Temporary, API.Effect.VisualEffect(vfxId), TimeSpan.FromSeconds(vfxDuration));
      }
      else
        API.Location.Create(selection.Player.ControlledCreature.Location.Area, selection.TargetPosition, selection.Player.ControlledCreature.Rotation)
          .ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(vfxId));
    }
  }
}
