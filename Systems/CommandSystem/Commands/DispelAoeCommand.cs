namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelAoeCommand(ChatSystem.ChatEventArgs e, Options.Result options)
    {
      foreach(NWObject oAoE in NWScript.GetArea(e.oSender).AsArea().Objects)
        if (NWScript.GetAreaOfEffectCreator(oAoE) == e.oSender)
          oAoE.Destroy();
    }
  }
}
