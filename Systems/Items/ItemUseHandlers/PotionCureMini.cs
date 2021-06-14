using System;
using System.Linq;
using NWN.API;

namespace NWN.Systems
{
  class PotionCureMini
  {
    public PotionCureMini(NwPlayer oPC)
    {
      foreach (Effect arenaMalus in oPC.ControlledCreature.ActiveEffects.Where(f => f.Tag == "CUSTOM_EFFECT_MINI"))
        oPC.ControlledCreature.RemoveEffect(arenaMalus);

      oPC.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(API.Constants.VfxType.ImpRemoveCondition));
    }
  }
}
