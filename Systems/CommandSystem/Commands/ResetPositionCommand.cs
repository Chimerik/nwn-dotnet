using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteResetPositionCommand(ChatSystem.Context ctx, Options.Result options)
    {
      NWScript.SetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X, 0.0f);
      NWScript.SetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, 0.0f);
      NWScript.SetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, 0.0f);
      NWScript.SetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, 0.0f);
    }
  }
}
