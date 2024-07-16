using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class WizardUtils
  {
    public static void HandleEvocateurSurchargeSelfDamage(NwCreature caster, byte spellLevel, SpellSchool spellSchool)
    {
      if (caster.KnowsFeat((Feat)CustomSkill.EvocateurSurcharge) && spellSchool == SpellSchool.Evocation
          && 0 < spellLevel && spellLevel < 6 && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.EvocateurSurchargeEffectTag))
      {
        int selfDamageDice = caster.GetObjectVariable<LocalVariableInt>(EffectSystem.EvocateurSurchargeVariable).Value;

        if (selfDamageDice > 0)
        {
          NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Instant, Effect.Damage(
            NwRandom.Roll(Utils.random, 12, selfDamageDice * spellLevel), CustomDamageType.Necrotic)));

          caster.GetObjectVariable<LocalVariableInt>(EffectSystem.EvocateurSurchargeVariable).Value += 1;
        }
        else
          caster.GetObjectVariable<LocalVariableInt>(EffectSystem.EvocateurSurchargeVariable).Value = 2;
      }
    }
  }
}
