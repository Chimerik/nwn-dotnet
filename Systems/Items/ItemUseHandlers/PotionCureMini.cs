using System;
using System.Linq;
using NWN.API;
using NWN.API.Constants;

namespace NWN.Systems
{
  class PotionCureMini
  {
    public PotionCureMini(NwPlayer oPC)
    {
      foreach (Effect arenaMalus in oPC.ControlledCreature.ActiveEffects.Where(f => f.Tag == "CUSTOM_EFFECT_MINI"))
        oPC.ControlledCreature.RemoveEffect(arenaMalus);

      oPC.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRemoveCondition));
    }
  }
}
