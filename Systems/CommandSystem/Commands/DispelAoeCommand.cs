namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelAoeCommand(ChatSystem.Context ctx, Options.Result options)
    {
      foreach(NWObject oAoE in NWScript.GetArea(ctx.oSender).AsArea().Objects)
        if (NWScript.GetAreaOfEffectCreator(oAoE) == ctx.oSender)
          oAoE.Destroy();
    }
  }
}
