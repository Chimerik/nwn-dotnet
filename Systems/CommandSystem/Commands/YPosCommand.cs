using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteYPosCommand(ChatSystem.Context ctx, Options.Result options)
    {
      float zPos;
      if (((string)options.positional[0]).Length != 0)
      {
        float value;
        if (float.TryParse((string)options.positional[0], out value))
        {
          NWScript.SetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, NWScript.GetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y) + value);

          zPos = NWScript.GetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y);
          if (zPos < -10 || zPos > 10)
            Utils.LogMessageToDMs($"SIT COMMAND - Player {NWScript.GetName(ctx.oSender)} - Y translation = {zPos}");

          return;
        }
      }

      NWScript.SetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, NWScript.GetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y) + 1.0f);

      zPos = NWScript.GetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y);
      if (zPos < -10 || zPos > 10)
        Utils.LogMessageToDMs($"SIT COMMAND - Player {NWScript.GetName(ctx.oSender)} - X translation = {zPos}");
    }
  }
}
