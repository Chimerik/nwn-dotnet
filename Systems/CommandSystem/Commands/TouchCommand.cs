using Anvil.API;
using System.Linq;
using System.Collections.Generic;

namespace NWN.Systems
{
  class TouchMode
  {
    public TouchMode(NwPlayer oPC)
    {
      List<Effect> effectList = oPC.ControlledCreature.ActiveEffects.Where(e => e.EffectType == EffectType.CutsceneGhost).ToList();

      if (effectList.Count == 0)
        oPC.ControlledCreature.ApplyEffect(EffectDuration.Permanent, Effect.CutsceneGhost());
      else
        foreach (Effect eff in effectList)
          oPC.ControlledCreature.RemoveEffect(eff);
    }
  }
}
