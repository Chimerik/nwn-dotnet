using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellCastAbjurationWard(NwCreature caster, NwSpell spell, int spellLevel)
    {
      if (spell.SpellSchool != SpellSchool.Abjuration || !caster.KnowsFeat((Feat)CustomSkill.AbjurationWard)
        || spellLevel < 1)
        return;

      int maxIntensity = 2 * caster.GetClassInfo(ClassType.Wizard).Level;
      NwCreature target = caster.GetObjectVariable<LocalVariableObject<NwCreature>>("_ABJURATION_WARD_TARGET").HasValue
        ? caster.GetObjectVariable<LocalVariableObject<NwCreature>>("_ABJURATION_WARD_TARGET").Value : caster;

      int intensity = 0;

      foreach (var eff in target.ActiveEffects)
      {
        if(eff.Tag == EffectSystem.AbjurationWardEffectTag && eff.Creator == caster)
        {
          intensity = spellLevel + eff.CasterLevel > maxIntensity ? maxIntensity : spellLevel + eff.CasterLevel;
          caster.RemoveEffect(eff);
        }
      }

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAbjurationWardEffect(intensity)));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurMindAffectingPositive));
    }
  }
}
