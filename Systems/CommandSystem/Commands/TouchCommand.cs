using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTouchCommand(ChatSystem.ChatEventArgs e, Options.Result options)
    {
      if (!Spells.GetHasEffect(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), e.oSender))
        NWScript.ApplyEffectToObject(DurationType.Permanent, NWScript.SupernaturalEffect(NWScript.EffectCutsceneGhost()), e.oSender);
      else
        Spells.RemoveEffectOfType(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), e.oSender);
    }
  }
}
