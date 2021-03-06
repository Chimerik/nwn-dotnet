﻿using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  class PotionCureFrog
  {
    public PotionCureFrog(NwPlayer oPC)
    {
      foreach (Effect arenaMalus in oPC.ControlledCreature.ActiveEffects.Where(f => f.Tag == "CUSTOM_EFFECT_FROG"))
        oPC.ControlledCreature.RemoveEffect(arenaMalus);

      oPC.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRemoveCondition));
    }
  }
}
