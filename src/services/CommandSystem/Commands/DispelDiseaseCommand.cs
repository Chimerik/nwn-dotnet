using Anvil.API;
using System.Linq;

namespace NWN.Systems
{
  class DispelDisease
  {
    public DispelDisease(NwPlayer oPC)
    {
      foreach (Effect eff in oPC.ControlledCreature.ActiveEffects.Where(e => e.EffectType == EffectType.Disease))
        oPC.ControlledCreature.RemoveEffect(eff);
    }
  }
}
