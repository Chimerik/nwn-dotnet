using NWN.API;
using NWN.API.Constants;
using System.Linq;

namespace NWN.Systems
{
  class DispelDisease
  {
    public DispelDisease(NwPlayer oPC)
    {
      foreach (API.Effect eff in oPC.ActiveEffects.Where(e => e.EffectType == EffectType.Disease))
        oPC.RemoveEffect(eff);
    }
  }
}
