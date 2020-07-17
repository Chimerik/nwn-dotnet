using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTouchCommand(ChatSystem.Context chatContext)
    {
      if (!Spells.GetHasEffect(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), chatContext.oSender))
        NWScript.ApplyEffectToObject(DurationType.Permanent, NWScript.SupernaturalEffect(NWScript.EffectCutsceneGhost()), chatContext.oSender);
      else
        Spells.RemoveEffectOfType(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), chatContext.oSender);
    }
  }
}
