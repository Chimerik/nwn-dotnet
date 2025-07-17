using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void MurmuresDissonnants(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      int damageDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

      SpellUtils.SignalEventSpellCast(target, oCaster, spell.SpellType);

      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      var result = CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, caster, spellDC);

      SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, damageDice, oCaster, 1, result, casterClass: casterClass);

      if (result == SavingThrowResult.Failure)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfScreenBump));
        EffectSystem.ApplyEffroi(target, caster, SpellUtils.GetSpellDuration(oCaster, spellEntry), spellDC, spell: spell);
      }
      
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwstun));
    }
  }
}
