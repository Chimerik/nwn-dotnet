using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellCastAbjurationWard(NwCreature caster, SpellEvents.OnSpellCast spellCast)
    {
      if (spellCast.Spell.SpellSchool != SpellSchool.Abjuration || !caster.KnowsFeat((Feat)CustomSkill.AbjurationWard)
        || spellCast.SpellLevel < 1)
        return;

      int maxIntensity = 2 * caster.GetClassInfo(ClassType.Wizard).Level;
      NwCreature target = caster.GetObjectVariable<LocalVariableObject<NwCreature>>("_ABJURATION_WARD_TARGET").HasValue
        ? caster.GetObjectVariable<LocalVariableObject<NwCreature>>("_ABJURATION_WARD_TARGET").Value : caster;

      int intensity = 0;

      foreach (var eff in target.ActiveEffects)
      {
        if(eff.Tag == EffectSystem.AbjurationWardEffectTag && eff.Creator == caster)
        {
          intensity = spellCast.SpellLevel + eff.CasterLevel > maxIntensity ? maxIntensity : spellCast.SpellLevel + eff.CasterLevel;
          caster.RemoveEffect(eff);
        }
      }

      target.OnDamaged -= WizardUtils.OnDamageAbjurationWard;
      target.OnDamaged += WizardUtils.OnDamageAbjurationWard;

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAbjurationWardEffect(intensity)));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurMindAffectingPositive));
    }
  }
}
