using System;
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
      double sneakDies = Math.Round((double)(GetRogueLevel(damager) / 2), MidpointRounding.AwayFromZero);
  
      //ModuleSystem.Log.Info($"----------------sneak dies : {sneakDies}-----------------------");

      foreach (var frappe in frappeRuseeEffects)
      {
        //ModuleSystem.Log.Info($"----------------frappe : {frappe}-----------------------");
        switch (int.Parse(frappe))
        {
          case CustomSpell.FrappeRuseePoison: 
          case CustomSpell.FrappePerfidePoison:

            if (sneakDies < 1)
            {
              damager.LoginPlayer?.SendServerMessage($"Dés de sournoise insuffisants pour activer Poison", ColorConstants.Orange);
              return;
            }

            if (EffectSystem.ApplyPoison(damaged, damager, NwTimeSpan.FromRounds(10), Ability.Constitution, Ability.Dexterity, true) == SavingThrowResult.Failure
              && damager.KnowsFeat((Feat)CustomSkill.AssassinEnvenimer)) 
            {
              damaged.ApplyEffect(EffectDuration.Temporary, EffectSystem.VulnerabilitePoison, NwTimeSpan.FromRounds(1));
              damager.ApplyEffect(EffectDuration.Temporary, EffectSystem.VulnerabilitePoison, NwTimeSpan.FromRounds(1));
              NWScript.AssignCommand(damager, () => damaged.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 6, 2), CustomDamageType.Poison)));
            }

            sneakDies -= 1;

            break;

          case CustomSpell.FrappeRuseeBousculade: 
          case CustomSpell.FrappePerfideBousculade:

            if (sneakDies < 1)
            {
              damager.LoginPlayer?.SendServerMessage($"Dés de sournoise insuffisants pour activer Bousculade", ColorConstants.Orange);
              return;
            }

            EffectSystem.ApplyKnockdown(damaged, damager, Ability.Dexterity,  Ability.Dexterity, EffectSystem.Destabilisation);

            sneakDies -= 1;

            break;

          case CustomSpell.FrappeRuseeRetraite: 
          case CustomSpell.FrappePerfideRetraite:

            if (sneakDies < 1)
            {
              damager.LoginPlayer?.SendServerMessage($"Dés de sournoise insuffisants pour activer Retraite", ColorConstants.Orange);
              return;
            }

            damager.ApplyEffect(EffectDuration.Temporary, EffectSystem.Retraite, NwTimeSpan.FromRounds(1));

            sneakDies -= 1;

            break;

          case CustomSpell.FrappePerfideHebeter:

            if (sneakDies < 2)
            {
              damager.LoginPlayer?.SendServerMessage($"Dés de sournoise insuffisants pour activer Hébéter", ColorConstants.Orange);
              return;
            }

            if (CreatureUtils.GetSavingThrow(damager, damaged, Ability.Constitution, SpellUtils.GetCasterSpellDC(damager, Ability.Dexterity)) == SavingThrowResult.Failure)
              damaged.ApplyEffect(EffectDuration.Temporary, Effect.Slow(), NwTimeSpan.FromRounds(1));

            sneakDies -= 2;

            break;
          
          case CustomSpell.FrappePerfideObscurcir:

            if (sneakDies < 3)
            {
              damager.LoginPlayer?.SendServerMessage($"Dés de sournoise insuffisants pour activer Obscurcir", ColorConstants.Orange);
              return;
            }

            if (CreatureUtils.GetSavingThrow(damager, damaged, Ability.Dexterity, SpellUtils.GetCasterSpellDC(damager, Ability.Dexterity)) == SavingThrowResult.Failure)
              damaged.ApplyEffect(EffectDuration.Temporary, Effect.Blindness(), NwTimeSpan.FromRounds(1));

            sneakDies -= 3;

            break;

          case CustomSpell.FrappePerfideAssommer:

            if (sneakDies < 6)
            {
              damager.LoginPlayer?.SendServerMessage($"Dés de sournoise insuffisants pour activer Assommer", ColorConstants.Orange);
              return;
            }

            EffectSystem.ApplySommeil(damaged, damager, null, NwTimeSpan.FromRounds(10), Ability.Constitution, Ability.Dexterity, true);

            sneakDies -= 6;

            break;
        }
      }

      onDamage.DamagedBy.GetObjectVariable<LocalVariableString>(EffectSystem.FrappeRuseeVariable).Delete();
    }
  }
}
