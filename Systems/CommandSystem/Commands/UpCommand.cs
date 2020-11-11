using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteUpCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (((string)options.positional[0]).Length != 0)
      {
        float value;
        if (float.TryParse((string)options.positional[0], out value))
        {
          NWScript.SetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, NWScript.GetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z) + value);
          return;
        }
      }

      NWScript.SetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, NWScript.GetObjectVisualTransform(ctx.oSender, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z) + 0.1f);
    }
  }
}
