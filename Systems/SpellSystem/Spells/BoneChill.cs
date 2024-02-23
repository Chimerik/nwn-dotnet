using Anvil.API;
using System;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BoneChill(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature oCaster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);

      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, onSpellCast.Spell);

      Ability spellCastingAbility = oCaster.GetAbilityModifier(Ability.Intelligence) > oCaster.GetAbilityModifier(Ability.Charisma)
            ? Ability.Intelligence : Ability.Charisma;

      switch(SpellUtils.GetSpellAttackRoll(onSpellCast.TargetObject, oCaster, onSpellCast.Spell, spellCastingAbility))
      {
        case TouchAttackResult.CriticalHit: SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); ; break;
        case TouchAttackResult.Hit: break;
        default: return;
      }

      onSpellCast.TargetObject.OnHeal += PreventHeal;
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, EffectSystem.boneChillEffect, NwTimeSpan.FromRounds(spellEntry.duration));
      SpellUtils.DealSpellDamage(onSpellCast.TargetObject, oCaster.CasterLevel, spellEntry, nbDice, oCaster);
    }
  }
}
