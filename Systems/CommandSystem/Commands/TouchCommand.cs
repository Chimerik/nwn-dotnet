using NWN.API;
using System.Linq;
using NWN.API.Constants;
using System.Collections.Generic;

namespace NWN.Systems
{
  class TouchMode
  {
    public TouchMode(NwPlayer oPC)
    {
      List<Effect> effectList = oPC.ActiveEffects.Where(e => e.EffectType == EffectType.CutsceneGhost).ToList();

      if (effectList.Count == 0)
        oPC.ApplyEffect(EffectDuration.Permanent, Effect.CutsceneGhost());
      else
        foreach (Effect eff in effectList)
          oPC.RemoveEffect(eff);
    }
  }
}
