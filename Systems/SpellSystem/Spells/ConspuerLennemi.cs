﻿using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ConspuerLennemi(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target) 
        return;

      SpellUtils.SignalEventSpellCast(oCaster, target, spell.SpellType);

        int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Charisma);

      if (CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC) == SavingThrowResult.Failure)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFearS));
        EffectSystem.ApplyEffroi(target, caster, NwTimeSpan.FromRounds(spellEntry.duration));
      }
      else
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSlow));
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedDecrease(50), NwTimeSpan.FromRounds(spellEntry.duration)));
      }
      
      PaladinUtils.ConsumeOathCharge(caster);
    }
  }
}
