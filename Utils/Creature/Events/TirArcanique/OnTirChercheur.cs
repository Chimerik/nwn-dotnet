using Anvil.API.Events;
using Anvil.API;
using NativeUtils = NWN.Systems.NativeUtils;
using NWN.Systems;
using NWN.Core;
using System.Linq;

namespace NWN
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
            caster.LoginPlayer?.SendServerMessage("Tête chercheuse - La cible sélectionnée n'est plus valide", ColorConstants.Red);
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

      SpellConfig.SavingThrowFeedback feedback = new();
      int tirDC = 8 + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Intelligence);
      int advantage = GetCreatureAbilityAdvantage(target, Ability.Dexterity);
      int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Dexterity, tirDC, advantage, feedback);
      bool saveFailed = totalSave < tirDC;

      SpellUtils.SendSavingThrowFeedbackMessage(caster, target, feedback, advantage, tirDC, totalSave, saveFailed, Ability.Dexterity);

      NwBaseItem weapon = caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem;
      int damage = NativeUtils.HandleWeaponDamageRerolls(caster, weapon, weapon.NumDamageDice, weapon.DieToRoll);

      if (!saveFailed)
      {
        damage /= 2;
        LogUtils.LogMessage($"JDS réussi : dégâts {damage}", LogUtils.LogType.Combat);
      }
      else
      {
        int forceDamage = caster.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 18)
          ? NwRandom.Roll(Utils.random, 6, 2) : NwRandom.Roll(Utils.random, 6, 4);

        LogUtils.LogMessage($"JDS échoué : dégâts +{forceDamage} (force)", LogUtils.LogType.Combat);

        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant,
        Effect.Damage(forceDamage, DamageType.Magical)));
      }

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant,
        Effect.Damage(damage, DamageType.Piercing)));

      FeatUtils.DecrementTirArcanique(caster);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Tir Chercheur", StringUtils.gold, true);

      LogUtils.LogMessage($"Tir Chercheur : dégâts finaux {damage} (perçant)", LogUtils.LogType.Combat);
      LogUtils.LogMessage("-------------------------------------------------", LogUtils.LogType.Combat);
    }
  }
}
