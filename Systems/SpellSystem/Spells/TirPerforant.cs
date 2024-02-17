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

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = 8 + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Intelligence);
      int nbDice = caster.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18) ? 2 : 4;

      ModuleSystem.Log.Info($"onSpellCast.TargetLocation : {onSpellCast.TargetLocation}");

      foreach (NwCreature target in onSpellCast.TargetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCylinder, 9, false, caster.Location.Position))
      {
        ModuleSystem.Log.Info($"target found : {target.Name}");
        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, caster);

        if (advantage < -900)
        {
          int weaponDamage = NativeUtils.HandleWeaponDamageRerolls(caster, weapon, weapon.NumDamageDice, weapon.DieToRoll);
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(weaponDamage, DamageType.Piercing)));
          continue;
        }

        int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback);
        bool saveFailed = totalSave < spellDC;

        int damage = NativeUtils.HandleWeaponDamageRerolls(caster, weapon, weapon.NumDamageDice, weapon.DieToRoll);
        damage /= saveFailed ? 2 : 1;

        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, DamageType.Piercing)));

        SpellUtils.SendSavingThrowFeedbackMessage(caster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
        SpellUtils.DealSpellDamage(target, caster.LastSpellCasterLevel, spellEntry, nbDice, caster, saveFailed);
      }

      caster.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirPerforant)); // Je redonne une utilisatation de tir perforant pour compenser la consommation du sort spécifique
      FeatUtils.DecrementTirArcanique(caster);
    }
  }
}
