using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private void MainBindings()
        {
          portrait.SetBindValue(player.oid, nuiToken.Token, $"{target.PortraitResRef}m");

          classRow.Children.Clear();
          classRow.Children.Add(new NuiSpacer());

          classRow.Children.Add(new NuiColumn() { Margin = 0.0f, Width = windowWidth / 7, Children = new List<NuiElement>()
          {
            new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiButtonImage(Races2da.raceTable[target.Race.Id].icon) { Width = windowWidth / 7 } } },
            new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel($"{target.Race.Name}") { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center } } },
            new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel("") } }
          } });

          classRow.Children.Add(new NuiSpacer());

          foreach (var playerClass in target.Classes)
          {
            if (playerClass.Class.Id == CustomClass.Adventurer)
              continue;

            classRow.Children.Add(new NuiColumn() { Margin = 0.0f, Width = windowWidth / 7, Children = new List<NuiElement>()
            {
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiButtonImage(targetPlayer.GetIconOverride(playerClass.Class.IconResRef)) { Width = windowWidth / 7, Tooltip = $"{targetPlayer.GetTlkOverride(playerClass.Class.Name)}" } } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel($"{targetPlayer.GetTlkOverride(playerClass.Class.Name)}") { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center, Tooltip = $"{targetPlayer.GetTlkOverride(playerClass.Class.Name)}" } } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel($"{playerClass.Level}") { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center, Tooltip = $"{targetPlayer.GetTlkOverride(playerClass.Class.Name)}" } } }
            } });

            classRow.Children.Add(new NuiSpacer());
          }

          classGroup.SetLayout(player.oid, nuiToken.Token, classRow);

          /*backgroundRow.Children.Clear();
          backgroundRow.Children.Add(new NuiSpacer());

          if (Players.TryGetValue(target, out var targetPlayer))
          {
            foreach (var learnable in targetPlayer.learnableSkills.Values.Where(s => s.category == SkillSystem.Category.StartingTraits))
            {
              backgroundRow.Children.Add(new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
              {
                new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiButtonImage(learnable.icon) { Width = windowWidth / 8 } } },
                new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel($"{learnable.name}") { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center } } },
              }});

              backgroundRow.Children.Add(new NuiSpacer());
            }
          }

          backgroundGroup.SetLayout(player.oid, nuiToken.Token, backgroundRow);*/

          str.SetBindValue(player.oid, nuiToken.Token, target.GetAbilityScore(Ability.Strength).ToString());
          dex.SetBindValue(player.oid, nuiToken.Token, target.GetAbilityScore(Ability.Dexterity).ToString());
          con.SetBindValue(player.oid, nuiToken.Token, target.GetAbilityScore(Ability.Constitution).ToString());
          intel.SetBindValue(player.oid, nuiToken.Token, target.GetAbilityScore(Ability.Intelligence).ToString());
          wis.SetBindValue(player.oid, nuiToken.Token, target.GetAbilityScore(Ability.Wisdom).ToString());
          cha.SetBindValue(player.oid, nuiToken.Token, target.GetAbilityScore(Ability.Charisma).ToString());

          hp.SetBindValue(player.oid, nuiToken.Token, $"Points de vie : {target.HP} / {target.MaxHP}");
          ac.SetBindValue(player.oid, nuiToken.Token, $"Classe d'armure : {target.AC}");
          init.SetBindValue(player.oid, nuiToken.Token, $"Initiative : {target.GetAbilityModifier(Ability.Dexterity)}");
          proficiency.SetBindValue(player.oid, nuiToken.Token, $"Bonus de maîtrise : {NativeUtils.GetCreatureProficiencyBonus(target)}");

          NwItem mainWeapon = target.GetItemInSlot(InventorySlot.RightHand);
          NwItem secondaryWeapon = target.GetItemInSlot(InventorySlot.LeftHand);

          if (mainWeapon is not null && ItemUtils.IsWeapon(mainWeapon.BaseItem))
          {
            bool isRangedWeapon = !ItemUtils.IsMeleeWeapon(mainWeapon.BaseItem);
            var damageAbility = NativeUtils.GetAttackAbility(target, isRangedWeapon, mainWeapon);
            int damageDie = NativeUtils.GetShillelaghDamageDie(target, mainWeapon.BaseItem);
            damageDie = ItemUtils.IsVersatileWeapon(mainWeapon.BaseItem.ItemType) && secondaryWeapon is null ? damageDie + 2 : damageDie;

            int numDamageDice = NativeUtils.GetShillelaghNumDice(target, mainWeapon.BaseItem);

            mainDamage.SetBindValue(player.oid, nuiToken.Token, $"Dégâts : {numDamageDice}d{damageDie} + {target.GetAbilityModifier(damageAbility)}");
            mainCriticalRange.SetBindValue(player.oid, nuiToken.Token, $"Critique : {NativeUtils.GetCriticalRange(target, mainWeapon, isRangedWeapon)} - 20");
          }
          else
          {
            var damageAbility = NativeUtils.GetAttackAbility(target, false, mainWeapon);
            mainDamage.SetBindValue(player.oid, nuiToken.Token, $"Dégâts : {1}d{CreatureUtils.GetUnarmedDamage(target)} + {target.GetAbilityModifier(damageAbility)}");
            mainCriticalRange.SetBindValue(player.oid, nuiToken.Token, "Critique : 20 - 20");
          }

          if (secondaryWeapon is not null && ItemUtils.IsWeapon(secondaryWeapon.BaseItem))
          {
            bool isRangedWeapon = !ItemUtils.IsMeleeWeapon(secondaryWeapon.BaseItem);
            var damageAbility = NativeUtils.GetAttackAbility(target, isRangedWeapon, secondaryWeapon);
            int damageDie = NativeUtils.GetShillelaghDamageDie(target, secondaryWeapon.BaseItem);
            int numDamageDice = NativeUtils.GetShillelaghNumDice(target, secondaryWeapon.BaseItem);

            secondaryDamage.SetBindValue(player.oid, nuiToken.Token, $"Main secondaire : {numDamageDice}d{damageDie} + {target.GetAbilityModifier(damageAbility)}");
            secondaryCriticalRange.SetBindValue(player.oid, nuiToken.Token, $"Critique : {NativeUtils.GetCriticalRange(target, secondaryWeapon, !ItemUtils.IsMeleeWeapon(mainWeapon.BaseItem))} - 20");
          }
          else
          {
            secondaryDamage.SetBindValue(player.oid, nuiToken.Token, "Main secondaire : -");
            secondaryCriticalRange.SetBindValue(player.oid, nuiToken.Token, "Critique : -");
          }

          // TODO : penser à prendre en compte les effets et items

          var saveBonus = target.GetAbilityModifier(Ability.Strength) + SpellUtils.GetSavingThrowProficiencyBonus(target, Ability.Strength);
          strSave.SetBindValue(player.oid, nuiToken.Token, saveBonus > 0 ? $"+{saveBonus}" : $"{saveBonus}");
          saveBonus = target.GetAbilityModifier(Ability.Dexterity) + SpellUtils.GetSavingThrowProficiencyBonus(target, Ability.Dexterity);
          dexSave.SetBindValue(player.oid, nuiToken.Token, saveBonus > 0 ? $"+{saveBonus}" : $"{saveBonus}");
          saveBonus = target.GetAbilityModifier(Ability.Constitution) + SpellUtils.GetSavingThrowProficiencyBonus(target, Ability.Constitution);
          conSave.SetBindValue(player.oid, nuiToken.Token, saveBonus > 0 ? $"+{saveBonus}" : $"{saveBonus}");
          saveBonus = target.GetAbilityModifier(Ability.Intelligence) + SpellUtils.GetSavingThrowProficiencyBonus(target, Ability.Intelligence);
          intSave.SetBindValue(player.oid, nuiToken.Token, saveBonus > 0 ? $"+{saveBonus}" : $"{saveBonus}");
          saveBonus = target.GetAbilityModifier(Ability.Wisdom) + SpellUtils.GetSavingThrowProficiencyBonus(target, Ability.Wisdom);
          wisSave.SetBindValue(player.oid, nuiToken.Token, saveBonus > 0 ? $"+{saveBonus}" : $"{saveBonus}");
          saveBonus = target.GetAbilityModifier(Ability.Charisma) + SpellUtils.GetSavingThrowProficiencyBonus(target, Ability.Charisma);
          chaSave.SetBindValue(player.oid, nuiToken.Token, saveBonus > 0 ? $"+{saveBonus}" : $"{saveBonus}");
        }
      }
    }
  }
}
