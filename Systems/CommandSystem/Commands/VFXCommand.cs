using System;
using System.Numerics;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Services;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteVFXCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (((string)options.positional[0]).Length != 0)
      {
        if (Int32.TryParse((string)options.positional[0], out int value))
        {
          ctx.oSender.GetLocalVariable<int>("_VXF_TEST_ID").Value = value;
          PlayerSystem.cursorTargetService.EnterTargetMode(ctx.oSender, VFXTarget, ObjectTypes.Creature, MouseCursor.Magic);
        }
      }
    }
    private static void VFXTarget(ModuleEvents.OnPlayerTarget selection)
    {
      ((NwGameObject)selection.TargetObject).ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect((VfxType)selection.Player.GetLocalVariable<int>("_VXF_TEST_ID").Value), TimeSpan.FromSeconds(10));
    }
  }
}
