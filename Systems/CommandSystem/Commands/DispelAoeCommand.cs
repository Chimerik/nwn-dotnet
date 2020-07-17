using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelAoeCommand(ChatSystem.Context chatContext)
    {
      foreach(NWObject oAoE in NWScript.GetArea(chatContext.oSender).AsArea().Objects)
        if (NWScript.GetAreaOfEffectCreator(oAoE) == chatContext.oSender)
          oAoE.Destroy();
    }
  }
}
