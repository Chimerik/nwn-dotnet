namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelAoeCommand(ChatSystem.Context chatContext, Options.Result options)
    {
      foreach(NWObject oAoE in NWScript.GetArea(chatContext.oSender).AsArea().Objects)
        if (NWScript.GetAreaOfEffectCreator(oAoE) == chatContext.oSender)
          oAoE.Destroy();
    }
  }
}
