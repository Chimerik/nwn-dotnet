using Anvil.API.Events;
using Anvil.API;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using Discord;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnDamageGlobal(OnCreatureDamage onDamage)
    {
      if (onDamage.Target is not NwCreature target)
        return;

      Effect resist = target.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ResistanceEffectTag);

      if (resist is not null && !target.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSpell.Resistance))
      {
        switch (resist.IntParams[6])
        {
          case CustomSpell.ResistanceContondant: ApplyResistanceReduction(onDamage.DamageData, DamageType.Bludgeoning, target, resist.IntParams[6]); break;
          case CustomSpell.ResistanceAcide: ApplyResistanceReduction(onDamage.DamageData, DamageType.Acid, target, resist.IntParams[6]); break;
          case CustomSpell.ResistanceElec: ApplyResistanceReduction(onDamage.DamageData, DamageType.Electrical, target, resist.IntParams[6]); break;
          case CustomSpell.ResistanceFeu: ApplyResistanceReduction(onDamage.DamageData, DamageType.Fire, target, resist.IntParams[6]); break;
          case CustomSpell.ResistanceFroid: ApplyResistanceReduction(onDamage.DamageData, DamageType.Cold, target, resist.IntParams[6]); break;
          case CustomSpell.ResistancePercant: ApplyResistanceReduction(onDamage.DamageData, DamageType.Piercing, target, resist.IntParams[6]); break;
          case CustomSpell.ResistanceTranchant: ApplyResistanceReduction(onDamage.DamageData, DamageType.Slashing, target, resist.IntParams[6]); break;
          case CustomSpell.ResistancePoison: ApplyResistanceReduction(onDamage.DamageData, DamageType.Custom1, target, resist.IntParams[6]); break;
        }
      }

      if (onDamage.DamagedBy is not NwCreature damager)
        return;

      int baseWeaponDamage = onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon);

      if (baseWeaponDamage > -1)
      {
        if(target.ActiveEffects.Any(e => e.Tag == EffectSystem.DefensesEnjoleusesEffectTag))
        {
          EffectUtils.RemoveTaggedEffect(target, EffectSystem.DefensesEnjoleusesEffectTag);

          int spellDC = SpellUtils.GetCasterSpellDC(damager, Ability.Charisma);
          int reducedDamage = baseWeaponDamage / 2;
          onDamage.DamageData.SetDamageByType(DamageType.BaseWeapon, reducedDamage);
          
          LogUtils.LogMessage($"Defenses Enjoleuses - Dégâts initiaux : {baseWeaponDamage} / 2 = {reducedDamage}", LogUtils.LogType.Combat);
          StringUtils.DisplayStringToAllPlayersNearTarget(target, "Défenses Enjôleuses", ColorConstants.Pink, true, true);
          
          if(GetSavingThrow(damager, target, Ability.Wisdom, spellDC) == SavingThrowResult.Failure)
          {
            NWScript.AssignCommand(target, () => damager.ApplyEffect(EffectDuration.Instant, Effect.Damage(reducedDamage, CustomDamageType.Psychic)));
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
    }
    private static void ApplyResistanceReduction(DamageData<int> damageData, DamageType damageType, NwCreature target, int spellId)
    {
      int damage = damageData.GetDamageByType(damageType);
      
      if (damage > 0)
      {
        int damageReduction = Utils.Roll(4);
        int total = damage - damageReduction;
        damageData.SetDamageByType(damageType, total);

        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(target, 6, CustomSpell.Resistance, spellId));

        LogUtils.LogMessage($"Résistance : {StringUtils.GetDamageTypeTraduction(damageType)} réduits de {damageReduction} ({damage} - {damageReduction} = {total})", LogUtils.LogType.Combat);
      }
    }
  }
}
