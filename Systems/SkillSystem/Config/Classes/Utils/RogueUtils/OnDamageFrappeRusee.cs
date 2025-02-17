using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class RogueUtils
  {
    public static void OnDamageFrappeRusee(OnCreatureDamage onDamage)
    {
      if (onDamage.DamagedBy.GetObjectVariable<LocalVariableString>(EffectSystem.FrappeRuseeVariable).HasNothing || onDamage.Target is not NwCreature damaged || onDamage.DamagedBy is not NwCreature damager)
        return;

      var frappeRuseeEffects = damager.GetObjectVariable<LocalVariableString>(EffectSystem.FrappeRuseeVariable).Value.Split("_");
      
      foreach (var frappe in frappeRuseeEffects)
      {
        switch(int.Parse(frappe))
        {
          case CustomSpell.FrappeRuseePoison: 
          case CustomSpell.FrappePerfidePoison: 
            
            if(EffectSystem.ApplyPoison(damaged, damager, NwTimeSpan.FromRounds(2), Ability.Constitution, Ability.Dexterity, true) == SavingThrowResult.Failure
              && damager.KnowsFeat((Feat)CustomSkill.AssassinEnvenimer)) 
            {
              damaged.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.Icon(CustomEffectIcon.PoisonVulnerability), Effect.DamageImmunityDecrease(CustomDamageType.Poison, 50)) , NwTimeSpan.FromRounds(1));
              NWScript.AssignCommand(damager, () => damaged.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 6, 2), CustomDamageType.Poison)));
            }

            break;

          case CustomSpell.FrappeRuseeBousculade: 
          case CustomSpell.FrappePerfideBousculade: 
            EffectSystem.ApplyKnockdown(damaged, damager, Ability.Dexterity,  Ability.Dexterity, EffectSystem.Destabilisation); 
            break;

          case CustomSpell.FrappeRuseeRetraite: 
          case CustomSpell.FrappePerfideRetraite: 
            damager.ApplyEffect(EffectDuration.Temporary, EffectSystem.Retraite, NwTimeSpan.FromRounds(1)); 
            break;

          case CustomSpell.FrappePerfideHebeter:

            if (CreatureUtils.GetSavingThrow(damager, damaged, Ability.Constitution, SpellUtils.GetCasterSpellDC(damager, Ability.Dexterity)) == SavingThrowResult.Failure)
              damaged.ApplyEffect(EffectDuration.Temporary, Effect.Slow(), NwTimeSpan.FromRounds(1)); 
            
            break;
          
          case CustomSpell.FrappePerfideObscurcir:

            if (CreatureUtils.GetSavingThrow(damager, damaged, Ability.Dexterity, SpellUtils.GetCasterSpellDC(damager, Ability.Dexterity)) == SavingThrowResult.Failure)
              damaged.ApplyEffect(EffectDuration.Temporary, Effect.Blindness(), NwTimeSpan.FromRounds(1));

            break;

          case CustomSpell.FrappePerfideAssommer: EffectSystem.ApplySommeil(damaged, damager, null, NwTimeSpan.FromRounds(10), Ability.Dexterity, Ability.Constitution); break;
        }
      }

      onDamage.DamagedBy.GetObjectVariable<LocalVariableString>(EffectSystem.FrappeRuseeVariable).Delete();
    }
  }
}
