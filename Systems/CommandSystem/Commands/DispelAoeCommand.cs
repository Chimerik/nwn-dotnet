using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelAoeCommand(ChatSystem.Context ctx, Options.Result options)
    {
      var oArea = NWScript.GetArea(ctx.oSender);
      var oAoE = NWScript.GetFirstObjectInArea(oArea);
      while (NWScript.GetIsObjectValid(oAoE) == 1)
      {
        if (NWScript.GetAreaOfEffectCreator(oAoE) == ctx.oSender)
          NWScript.DestroyObject(oAoE);
        oAoE = NWScript.GetNextObjectInArea(oArea);
      }
    }
  }
}
