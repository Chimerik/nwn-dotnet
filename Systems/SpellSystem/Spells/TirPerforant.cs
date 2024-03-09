using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void TirPerforant(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      NwBaseItem weapon = caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem;

      if (!ItemUtils.HasBowEquipped(weapon?.ItemType))
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être équipé d'un arc", ColorConstants.Red);
        return;
      }

      LogUtils.LogMessage($"--- {caster.Name} Tir Perforant ---", LogUtils.LogType.Combat);

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = 8 + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Intelligence);
      int nbDice = caster.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18) ? 1 : 2;
      int dexDamage = caster.GetAbilityModifier(Ability.Dexterity);

      foreach (NwCreature target in onSpellCast.TargetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCylinder, 9, false, caster.Location.Position))
      {
        if(target == caster)
          continue;

        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, caster);
        int weaponDamage = 0;
        int bonusDamage = 0;
        int damage = 0;

        if (advantage < -900)
        {
          weaponDamage = NativeUtils.HandleWeaponDamageRerolls(caster, weapon, weapon.NumDamageDice, weapon.DieToRoll);
          bonusDamage = NwRandom.Roll(Utils.random, 6, nbDice);
          damage = weaponDamage + bonusDamage + dexDamage;
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, DamageType.Piercing)));

          LogUtils.LogMessage($"{target.Name} (incapable d'agir) : {weapon.DieToRoll}d{weapon.NumDamageDice} + {dexDamage} + {nbDice}d6 ({weaponDamage} + {dexDamage} + {bonusDamage}) = {damage}", LogUtils.LogType.Combat);
          continue;
        }

        int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback);
        bool saveFailed = totalSave < spellDC;

        weaponDamage = NativeUtils.HandleWeaponDamageRerolls(caster, weapon, weapon.NumDamageDice, weapon.DieToRoll);
        bonusDamage = NwRandom.Roll(Utils.random, 6, nbDice);
        damage = weaponDamage + bonusDamage + dexDamage;
        damage /= saveFailed ? 2 : 1;

        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, DamageType.Piercing)));

        SpellUtils.SendSavingThrowFeedbackMessage(caster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
        SpellUtils.DealSpellDamage(target, caster.CasterLevel, spellEntry, nbDice, caster, saveFailed);

        LogUtils.LogMessage($"{target.Name}: {weapon.DieToRoll}d{weapon.NumDamageDice} + {dexDamage} + {nbDice}d6 ({weaponDamage} + {dexDamage} + {bonusDamage}) = {damage}", LogUtils.LogType.Combat);
      }

      caster.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirPerforant)); // Je redonne une utilisatation de tir perforant pour compenser la consommation du sort spécifique
      FeatUtils.DecrementTirArcanique(caster);

      LogUtils.LogMessage($"------", LogUtils.LogType.Combat);
    }
  }
}
