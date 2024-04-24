using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void TirPerforant(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, Location targetLocation)
    {
      if (oCaster is not NwCreature caster)
        return;

      NwBaseItem weapon = caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem;

      if (!ItemUtils.HasBowEquipped(weapon?.ItemType))
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être équipé d'un arc", ColorConstants.Red);
        return;
      }

      SpellUtils.SignalEventSpellCast(oTarget, caster, spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = 8 + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Intelligence);
      int nbDice = caster.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18) ? 1 : 2;
      int dexDamage = caster.GetAbilityModifier(Ability.Dexterity);

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCylinder, 9, false, caster.Location.Position))
      {
        if(target == caster)
          continue;

        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, caster);

        if (advantage < -900)
          continue;

        int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback);
        bool saveFailed = totalSave < spellDC;

        int weaponDamage = NativeUtils.HandleWeaponDamageRerolls(caster, weapon, weapon.NumDamageDice, weapon.DieToRoll);
        int bonusDamage = NwRandom.Roll(Utils.random, 6, nbDice);
        int damage = weaponDamage + bonusDamage + dexDamage;
        damage = ItemUtils.GetShieldMasterReducedDamage(target, damage, saveFailed, spellEntry.savingThrowAbility);
        damage /= saveFailed ? 2 : 1;

        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, DamageType.Piercing)));

        SpellUtils.SendSavingThrowFeedbackMessage(caster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
        LogUtils.LogMessage($"{target.Name}: {weapon.NumDamageDice}d{weapon.DieToRoll} + {dexDamage} + {nbDice}d6 ({weaponDamage} + {dexDamage} + {bonusDamage}) = {damage}", LogUtils.LogType.Combat);
      }

      caster.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirPerforant)); // Je redonne une utilisation de tir perforant pour compenser la consommation du sort spécifique
      FeatUtils.DecrementTirArcanique(caster);

      LogUtils.LogMessage($"------", LogUtils.LogType.Combat);
    }
    public static void ApplyTirPerforantDamage(NwCreature caster, NwCreature target)
    {
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem;
      int nbDice = caster.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18) ? 1 : 2;
      int weaponDamage = NativeUtils.HandleWeaponDamageRerolls(caster, weapon, weapon.NumDamageDice, weapon.DieToRoll);
      int dexDamage = caster.GetAbilityModifier(Ability.Dexterity);
      int bonusDamage = NwRandom.Roll(Utils.random, 6, nbDice);
      int damage = weaponDamage + bonusDamage + dexDamage;
      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, DamageType.Piercing)));

      LogUtils.LogMessage($"{target.Name} (incapable d'agir) : {weapon.NumDamageDice}d{weapon.DieToRoll} + {dexDamage} + {nbDice}d6 ({weaponDamage} + {dexDamage} + {bonusDamage}) = {damage}", LogUtils.LogType.Combat);
    }
  }
}
