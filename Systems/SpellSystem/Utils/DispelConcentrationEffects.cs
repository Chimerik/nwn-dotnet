using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void DispelConcentrationEffects(NwCreature caster)
    {
      foreach (var eff in caster.ActiveEffects.Where(e => e.Tag == EffectSystem.ConcentrationEffectTag))
        caster.RemoveEffect(eff);

      caster.OnCreatureAttack -= CreatureUtils.OnAttackSearingSmite;
      caster.OnCreatureAttack -= CreatureUtils.OnAttackBrandingSmite;
    }
  }
}
