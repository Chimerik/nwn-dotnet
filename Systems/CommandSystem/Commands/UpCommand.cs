using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteUpCommand(ChatSystem.Context ctx, Options.Result options)
    {
      float zPos;
      if (((string)options.positional[0]).Length != 0)
      {
        float value;
        if (float.TryParse((string)options.positional[0], out value))
        {
          NWScript.SetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, NWScript.GetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z) + value);

          zPos = NWScript.GetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z);
          if (zPos < 0 || zPos > 5)
            Utils.LogMessageToDMs($"SIT COMMAND - Player {NWScript.GetName(ctx.oSender)} - Z translation = {zPos}");
          
          return;
        }
      }

      NWScript.SetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, NWScript.GetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z) + 0.1f);

      zPos = NWScript.GetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z);
      if (zPos < 0 || zPos > 5)
        Utils.LogMessageToDMs($"SIT COMMAND - Player {NWScript.GetName(ctx.oSender)} - Z translation = {zPos}");
    }
  }
}
