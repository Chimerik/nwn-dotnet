using Anvil.API.Events;
using Anvil.API;
using System.Linq;
using NWN.Core;
using System;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnDamageGlobal(OnCreatureDamage onDamage)
    {
      if (onDamage.Target is not NwCreature target)
        return;

      int totalDamage = 0;

      foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
      {
        var damage = onDamage.DamageData.GetDamageByType(damageType);
        totalDamage += damage > 0 ? damage : 0;
      }

      Effect resist = target.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ResistanceEffectTag);

      if (resist is not null && !target.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSpell.Resistance))
      {
        switch (resist.Spell.Id)
        {
          case CustomSpell.ResistanceContondant: ApplyResistanceReduction(onDamage.DamageData, DamageType.Bludgeoning, target, resist.Spell); break;
          case CustomSpell.ResistanceAcide: ApplyResistanceReduction(onDamage.DamageData, DamageType.Acid, target, resist.Spell); break;
          case CustomSpell.ResistanceElec: ApplyResistanceReduction(onDamage.DamageData, DamageType.Electrical, target, resist.Spell); break;
          case CustomSpell.ResistanceFeu: ApplyResistanceReduction(onDamage.DamageData, DamageType.Fire, target, resist.Spell); break;
          case CustomSpell.ResistanceFroid: ApplyResistanceReduction(onDamage.DamageData, DamageType.Cold, target, resist.Spell); break;
          case CustomSpell.ResistancePercant: ApplyResistanceReduction(onDamage.DamageData, DamageType.Piercing, target, resist.Spell); break;
          case CustomSpell.ResistanceTranchant: ApplyResistanceReduction(onDamage.DamageData, DamageType.Slashing, target, resist.Spell); break;
          case CustomSpell.ResistancePoison: ApplyResistanceReduction(onDamage.DamageData, DamageType.Custom1, target, resist.Spell); break;
        }
      }

      if(totalDamage >= target.HP && target.ActiveEffects.Any(e => e.EffectType == EffectType.Polymorph))
      {
        int remainingDamage = totalDamage - target.HP;
        target.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").Value -= remainingDamage;
        EffectUtils.RemoveTaggedEffect(target, EffectSystem.PolymorphEffectTag);
        target.ApplyEffect(EffectDuration.Temporary, Effect.TemporaryHitpoints(totalDamage), TimeSpan.FromSeconds(0.59f));
      }

      if (onDamage.DamagedBy is not NwCreature damager)
        return;

      int baseWeaponDamage = onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon);

      if (baseWeaponDamage > -1)
      {
        foreach(var eff in target.ActiveEffects)
        {
          switch(eff.Tag)
          {
            case EffectSystem.DefensesEnjoleusesEffectTag:

              EffectUtils.RemoveTaggedEffect(target, EffectSystem.DefensesEnjoleusesEffectTag);

              int spellDC = SpellUtils.GetCasterSpellDC(target, Ability.Charisma);
              int reducedDamage = baseWeaponDamage / 2;
              onDamage.DamageData.SetDamageByType(DamageType.BaseWeapon, reducedDamage);

              LogUtils.LogMessage($"Defenses Enjoleuses - Dégâts initiaux : {baseWeaponDamage} / 2 = {reducedDamage}", LogUtils.LogType.Combat);
              StringUtils.DisplayStringToAllPlayersNearTarget(target, "Défenses Enjôleuses", ColorConstants.Pink, true, true);

              if (GetSavingThrow(target, damager, Ability.Wisdom, spellDC) == SavingThrowResult.Failure)
              {
                NWScript.AssignCommand(target, () => damager.ApplyEffect(EffectDuration.Instant, Effect.Damage(reducedDamage, CustomDamageType.Psychic)));
              }

              break;

            case EffectSystem.FureurDelOuraganEffectTag:

              var weapon = damager.GetItemInSlot(InventorySlot.RightHand);

              if (weapon is null || ItemUtils.IsMeleeWeapon(weapon.BaseItem.ItemType))
              {
                if (HandleReactionUse(target))
                {
                  int ouraganDC = SpellUtils.GetCasterSpellDC(target, Ability.Wisdom);
                  int ouraganDamage = GetSavingThrow(target, damager, Ability.Dexterity, ouraganDC) == SavingThrowResult.Failure ? Utils.Roll(8, 2) : Utils.Roll(8, 1);
                  NWScript.AssignCommand(target, () => damager.ApplyEffect(EffectDuration.Instant, Effect.Damage(ouraganDamage, eff.Spell == NwSpell.FromSpellId(CustomSpell.FureurDelOuraganFoudre) ? DamageType.Electrical : DamageType.Sonic)));

                  EffectUtils.RemoveTaggedEffect(target, EffectSystem.FureurDelOuraganEffectTag);
                }
              }

              break;
          }
        }
      }
      else
      {
        int psyDamage = onDamage.DamageData.GetDamageByType(CustomDamageType.Psychic);

        if (psyDamage > 0 && target.ActiveEffects.Any(e => e.Tag == EffectSystem.BouclierPsychiqueEffectTag))
        {
          StringUtils.DisplayStringToAllPlayersNearTarget(target, "Bouclier Psychique", ColorConstants.Pink, true, true);
          NWScript.AssignCommand(target, () => damager.ApplyEffect(EffectDuration.Instant, Effect.Damage(psyDamage, CustomDamageType.Psychic)));
        }
      }

      NWScript.AssignCommand(damager, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.DamagedBy, NwTimeSpan.FromRounds(1)));
    }
    private static void ApplyResistanceReduction(DamageData<int> damageData, DamageType damageType, NwCreature target, NwSpell spell)
    {
      int damage = damageData.GetDamageByType(damageType);
      
      if (damage > 0)
      {
        int damageReduction = Utils.Roll(4);
        int total = damage - damageReduction;
        damageData.SetDamageByType(damageType, total);

        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(target, 6, CustomSpell.Resistance, spell));

        LogUtils.LogMessage($"Résistance : {StringUtils.GetDamageTypeTraduction(damageType)} réduits de {damageReduction} ({damage} - {damageReduction} = {total})", LogUtils.LogType.Combat);
      }
    }
  }
}
