using NWN.API;
using System.Linq;
using NWN.API.Constants;
using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTouchCommand(ChatSystem.Context ctx, Options.Result options)
    {

      List<Effect> effectList = ctx.oSender.ActiveEffects.Where(e => e.EffectType == EffectType.CutsceneGhost).ToList();

      if (effectList.Count == 0)
        ctx.oSender.ApplyEffect(EffectDuration.Permanent, Effect.CutsceneGhost());
      else
        foreach (Effect eff in effectList)
          ctx.oSender.RemoveEffect(eff);
    }
  }
}
