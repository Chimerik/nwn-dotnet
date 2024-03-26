using Anvil.API.Events;
using Anvil.API;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void HandleTirChercheur(NwCreature caster)
    {
      if (ItemUtils.HasBowEquipped(caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType))
      {
        if (caster.GetObjectVariable<LocalVariableObject<NwCreature>>(TirChercheurVariable).HasNothing)
        {
          caster.LoginPlayer?.EnterTargetMode(SelectTirChercheurTarget, Config.attackCreatureTargetMode);
          caster.LoginPlayer?.SendServerMessage("Tête chercheuse - Veuillez choisir une cible", ColorConstants.Orange);
        }
        else
        {
          NwCreature target = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(TirChercheurVariable).Value;

          if (target is not null && target.IsValid)
          {
            if (caster.GetObjectVariable<LocalVariableInt>(TirArcaniqueCooldownVariable).HasNothing)
            {
              DealTirChercheurDamage(caster, target);
              StringUtils.DelayLocalVariableDeletion<LocalVariableInt>(caster, TirArcaniqueCooldownVariable, NwTimeSpan.FromRounds(1));
            }
            else
              caster.LoginPlayer?.SendServerMessage("Tir arcanique limité à un par round", ColorConstants.Red);
          }
          else
          {
            caster.GetObjectVariable<LocalVariableObject<NwCreature>>(TirChercheurVariable).Delete();
            caster.LoginPlayer?.EnterTargetMode(SelectTirChercheurTarget, Config.attackCreatureTargetMode);
            caster.LoginPlayer?.SendServerMessage("Tête chercheuse - La cible sélectionnée n'est plus valide - Veuillez choisir une nouvelle cible", ColorConstants.Red);
          }
        }
      }
      else
        caster.LoginPlayer?.SendServerMessage("Vous devez être équipé d'un arc", ColorConstants.Red);
    }
    private static void SelectTirChercheurTarget(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.TargetObject is not NwCreature creature || creature is null || !creature.IsValid)
        return;

      selection.Player.LoginCreature.GetObjectVariable<LocalVariableObject<NwCreature>>(TirChercheurVariable).Value = creature;
      selection.Player.SendServerMessage($"Tête chercheuse - cible : {StringUtils.ToWhitecolor(creature.Name)}", StringUtils.brightGreen);

      TirChercheurExpiration(selection.Player.LoginCreature, creature);
    }
    private static async void TirChercheurExpiration(NwCreature caster, NwCreature target)
    {
      await NwTask.Delay(NwTimeSpan.FromTurns(1));

      if (caster.GetObjectVariable<LocalVariableObject<NwCreature>>(TirChercheurVariable).Value == target)
        caster.GetObjectVariable<LocalVariableObject<NwCreature>>(TirChercheurVariable).Delete();
    }
    private static void DealTirChercheurDamage(NwCreature caster, NwCreature target)
    {
      LogUtils.LogMessage($"----------------------{caster.Name} - Tir Chercheur ----------------------", LogUtils.LogType.Combat);

      NwBaseItem weapon = caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem;
      int weaponDamage = NativeUtils.HandleWeaponDamageRerolls(caster, weapon, weapon.NumDamageDice, weapon.DieToRoll);
      int damage = weaponDamage + caster.GetAbilityModifier(Ability.Dexterity);
      int nbDice = caster.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18) ? 1 : 2;
      int forceDamage = NwRandom.Roll(Utils.random, 6, nbDice);

      LogUtils.LogMessage($"Dégâts initiaux : {damage} perçant - {forceDamage} force ({nbDice}d6)", LogUtils.LogType.Combat);

      SpellConfig.SavingThrowFeedback feedback = new();
      int tirDC = 8 + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Intelligence);
      int advantage = GetCreatureAbilityAdvantage(target, Ability.Dexterity);
      int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Dexterity, tirDC, advantage, feedback);
      bool saveFailed = totalSave < tirDC;

      SpellUtils.SendSavingThrowFeedbackMessage(caster, target, feedback, advantage, tirDC, totalSave, saveFailed, Ability.Dexterity);

      if (!saveFailed)
      {
        damage /= 2;
        forceDamage /= 2;
        LogUtils.LogMessage($"JDS réussi - Dégâts divisés par 2 - {damage} perçant - {forceDamage} force", LogUtils.LogType.Combat);
      }
      else
        LogUtils.LogMessage("JDS échoué", LogUtils.LogType.Combat);

      LogUtils.LogMessage($"Dégâts perçants {weapon.NumDamageDice}d{weapon.DieToRoll} ({weaponDamage}) + {caster.GetAbilityModifier(Ability.Dexterity)} (dex) +{forceDamage} (force)", LogUtils.LogType.Combat);

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant,
        Effect.Damage(damage, DamageType.Piercing)));

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant,
        Effect.Damage(forceDamage, DamageType.Magical)));

      caster.GetObjectVariable<LocalVariableObject<NwCreature>>(TirChercheurVariable).Delete();

      FeatUtils.DecrementTirArcanique(caster);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Tir Chercheur", StringUtils.gold, true);

      LogUtils.LogMessage($"Tir Chercheur : dégâts finaux {damage} (perçant) {forceDamage} (force)", LogUtils.LogType.Combat);
      LogUtils.LogMessage("-------------------------------------------------", LogUtils.LogType.Combat);
    }
  }
}
