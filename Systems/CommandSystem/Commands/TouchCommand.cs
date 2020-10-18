using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTouchCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (!Spells.GetHasEffect(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), ctx.oSender))
        NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, NWScript.SupernaturalEffect(NWScript.EffectCutsceneGhost()), ctx.oSender);
      else
        Spells.RemoveEffectOfType(NWScript.GetEffectType(NWScript.EffectCutsceneGhost()), ctx.oSender);
    }
  }
}
