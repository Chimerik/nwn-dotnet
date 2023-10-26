﻿using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ElectricJolt(SpellEvents.OnSpellCast onSpellCast)
    {
      if (onSpellCast.Caster is not NwCreature oCaster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpLightningS));

      int nbDice = 1;

      if (oCaster.LastSpellCasterLevel > 16)
        nbDice = 4;
      else if (oCaster.LastSpellCasterLevel > 10)
        nbDice = 3;
      if (oCaster.LastSpellCasterLevel > 4)
        nbDice = 2;

      Ability spellCastingAbility = oCaster.GetAbilityModifier(Ability.Intelligence) > oCaster.GetAbilityModifier(Ability.Charisma)
        ? Ability.Intelligence : Ability.Charisma;

      switch (SpellUtils.GetSpellAttackRoll(onSpellCast, oCaster, spellCastingAbility))
      {
        case TouchAttackResult.CriticalHit: nbDice *= 2; break;
        case TouchAttackResult.Hit: break;
        default: return;
      }

      int damage = Utils.random.Roll(Spells2da.spellTable[onSpellCast.Spell.Id].damageDice, nbDice);
      //int nDamage = SpellUtils.MaximizeOrEmpower(4, 1 + nCasterLevel / 6, onSpellCast.MetaMagicFeat);
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, DamageType.Electrical));
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, EffectSystem.noReactions, NwTimeSpan.FromRounds(1)) ;
      onSpellCast.TargetObject.GetObjectVariable<LocalVariableInt>("_REACTION").Value = 0;
      LogUtils.LogMessage($"Dégâts : {nbDice}d{Spells2da.spellTable[onSpellCast.Spell.Id].damageDice} (caster lvl {oCaster.LastSpellCasterLevel}) = {damage}", LogUtils.LogType.Combat);
    }
  }
}
