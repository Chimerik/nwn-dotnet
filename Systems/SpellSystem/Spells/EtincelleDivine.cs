﻿using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void EtincelleDivine(NwGameObject oCaster, NwGameObject oTarget, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int damage = NwRandom.Roll(Utils.random, spellEntry.damageDice) + CreatureUtils.GetAbilityModifierMin1(caster, Ability.Wisdom);

      if(spell.Id == CustomSpell.EtincelleDivineSoins)
      {
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Heal(damage)));
      }
      else
      {
        int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Wisdom);
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, 1, caster, 2,
          CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry));
      }

      caster.IncrementRemainingFeatUses((Feat)CustomSkill.ClercEtincelleDivine);
      ClercUtils.ConsumeConduitDivin(caster);
    }
  }
}
