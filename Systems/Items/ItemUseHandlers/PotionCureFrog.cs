using System;
using System.Linq;
using NWN.API;

namespace NWN.Systems
{
  class PotionCureFrog
  {
    public PotionCureFrog(NwPlayer oPC)
    {
      foreach (Effect arenaMalus in oPC.ActiveEffects.Where(f => f.Tag == "CUSTOM_EFFECT_FROG"))
        oPC.RemoveEffect(arenaMalus);

      oPC.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(API.Constants.VfxType.ImpRemoveCondition));
    }
  }
}
