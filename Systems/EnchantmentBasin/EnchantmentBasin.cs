using System;
using System.Collections.Generic;
using NWN.Core;

namespace NWN.Systems
{
    public static partial class EnchantmentBasinSystem
    {
        public class EnchantmentBasin
        {
            private float costRate;
            private int minSuccessPercent;
            private int maxSuccessPercent;
            private int maxCostRateForSuccessRateMin;

            // Enabled enchantment
            private bool isAttackBonusEnabled;
            private bool isACBonusEnabled;
            private bool isAbilityBonusEnabled;
            private bool isDamageBonusEnabled;
            private bool isSavingThrowBonusEnabled;
            private bool isRegenBonusEnabled;

            // maximum values
            private int maxAttackBonus;
            private int maxACBonus;
            private int maxAbilityBonus;
            private ItemPropertyUtils.DamageBonus maxDamageBonus;
            private int maxSavingThrowBonus;
            private int maxRegenBonus;

            public PlayerSystem.Player player;
            public uint oItem;
            public uint oContainer;

            public EnchantmentBasin(
              float costRate = 1.0f,
              int minSuccessPercent = 1,
              int maxSuccessPercent = 99,
              int maxCostRateForSuccessRateMin = 100000,
              bool isAttackBonusEnabled = true,
              bool isACBonusEnabled = true,
              bool isAbilityBonusEnabled = true,
              bool isDamageBonusEnabled = true,
              bool isSavingThrowBonusEnabled = true,
              bool isRegenBonusEnabled = true,
              int maxAttackBonus = 20,
              int maxACBonus = 20,
              int maxAbilityBonus = 12,
              ItemPropertyUtils.DamageBonus maxDamageBonus = ItemPropertyUtils.DamageBonus.D2d12,
              int maxSavingThrowBonus = 20,
              int maxRegenBonus = 20
            )
            {
                this.costRate = costRate;
                this.minSuccessPercent = minSuccessPercent;
                this.maxSuccessPercent = maxSuccessPercent;
                this.maxCostRateForSuccessRateMin = maxCostRateForSuccessRateMin;
                this.isAttackBonusEnabled = isAttackBonusEnabled;
                this.isACBonusEnabled = isACBonusEnabled;
                this.isAbilityBonusEnabled = isAbilityBonusEnabled;
                this.isDamageBonusEnabled = isDamageBonusEnabled;
                this.isSavingThrowBonusEnabled = isSavingThrowBonusEnabled;
                this.isRegenBonusEnabled = isRegenBonusEnabled;
                this.maxAttackBonus = maxAttackBonus;
                this.maxACBonus = maxACBonus;
                this.maxAbilityBonus = maxAbilityBonus;
                this.maxDamageBonus = maxDamageBonus;
                this.maxSavingThrowBonus = maxSavingThrowBonus;
                this.maxRegenBonus = maxRegenBonus;
            }

            public void DrawMenu(PlayerSystem.Player player, uint oItem, uint oContainer)
            {
                this.player = player;
                this.oItem = oItem;
                this.oContainer = oContainer;

                DrawMenuPage();
            }

            private void DrawMenuPage()
            {
                player.menu.Clear();
                player.menu.titleLines.Add("Choisissez un enchantement a appliquer sur votre objet");

                bool isWeapon = ItemUtils.IsWeapon(oItem);
                bool isMeleeWeapon = ItemUtils.IsMeleeWeapon(oItem);

                if (isAttackBonusEnabled && isWeapon) AddAttackBonusToMenu();
                if (isDamageBonusEnabled && isMeleeWeapon) AddDamageBonusToMenu();
                if (isACBonusEnabled) AddACBonusToMenu();
                if (isAbilityBonusEnabled) AddAbilityBonusToMenu();
                if (isSavingThrowBonusEnabled) AddSavingThrowBonusToMenu();
                if (isRegenBonusEnabled) AddRegenBonusToMenu();

                player.menu.Draw();
            }

            private void DrawCostPage(ItemProperty ip, string enchantmentName)
            {
                // Create a copy item to evaluate enchantment cost
                uint oCopy = NWScript.CopyItem(oItem, player.oid.LoginCreature);

                // Enchant copy item to evaluate cost
                ItemPropertyUtils.ReplaceItemProperty(oCopy, ip);

                // Calculate cost
                int baseCost = ItemUtils.GetIdentifiedGoldPieceValue(oCopy) - ItemUtils.GetIdentifiedGoldPieceValue(oItem);
                int cost = (int)(baseCost * costRate);

                // Destroy the cloned item
                NWScript.DestroyObject(oCopy);

                // Calculate success rate
                int successPercent = (int)Utils.ScaleToRange(cost, maxCostRateForSuccessRateMin, 0, minSuccessPercent, maxSuccessPercent);
                successPercent = Math.Clamp(successPercent, minSuccessPercent, maxSuccessPercent);

                player.menu.Clear();
                player.menu.titleLines = new List<string> {
          $"Desirez vous ajouter l'enchantement {enchantmentName} a l'item {NWScript.GetName(oItem)} ? ",
          $"Il vous en coutera {cost} Pieces d'or. ",
          $"Vous avez {successPercent}% de chance de reussite. ",
          $"En cas d'échec, l'item sera detruit de facon permanente."
        };

                player.menu.choices.Add(("Confirmer", () => HandleConfirm(ip, cost, successPercent)));
                player.menu.choices.Add(("Retour", () => DrawMenuPage()));
                player.menu.Draw();
            }

            private void HandleConfirm(ItemProperty ip, int cost, int successPercent)
            {
                if (NWScript.GetGold(player.oid.LoginCreature) <= cost)
                {
                    NWScript.SendMessageToPC(player.oid.LoginCreature, "Vous n'avez pas la quantité d'or requise pour effectuer cet enchantement.");
                    return;
                }

                NWScript.TakeGoldFromCreature(cost, player.oid.LoginCreature, 1);

                var roll = Utils.random.Next(1, 100);
                if (roll <= successPercent)
                {
                    // Success
                    ItemPropertyUtils.ReplaceItemProperty(oItem, ip);
                    NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_HOLY_AID), oContainer);
                }
                else
                {
                    // Failure
                    NWScript.DestroyObject(oItem);
                    NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_FLAME_M), oContainer);
                }

                player.menu.Close();
            }

            private void AddAttackBonusToMenu()
            {
                var currentAttackBonus = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_ATTACK_BONUS);
                if (currentAttackBonus < maxAttackBonus)
                {
                    player.menu.choices.Add((
                      "Bonus d'attaque",
                      () => DrawCostPage(
                        NWScript.ItemPropertyAttackBonus(currentAttackBonus + 1),
                        $"Bonus d'attaque +{currentAttackBonus + 1}"
                      )
                    ));
                }
            }

            private void AddACBonusToMenu()
            {
                var currentAC = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_AC_BONUS);
                if (currentAC < maxACBonus)
                {
                    player.menu.choices.Add((
                      "Bonus d'armure",
                      () => DrawCostPage(
                        NWScript.ItemPropertyACBonus(currentAC + 1),
                        $"Bonus d'armure +{currentAC + 1}"
                      )
                    ));
                }
            }

            private void AddAbilityBonusToMenu()
            {
                player.menu.choices.Add((
                  "Bonus de caracteristique",
                  () => DrawAbilityBonusPage()
                ));
            }

            private void DrawAbilityBonusPage()
            {
                player.menu.Clear();
                player.menu.titleLines.Add("Choisissez une caracteristique");

                AddAbilityBonusTypeToMenu(NWScript.ABILITY_STRENGTH);
                AddAbilityBonusTypeToMenu(NWScript.ABILITY_DEXTERITY);
                AddAbilityBonusTypeToMenu(NWScript.ABILITY_CONSTITUTION);
                AddAbilityBonusTypeToMenu(NWScript.ABILITY_INTELLIGENCE);
                AddAbilityBonusTypeToMenu(NWScript.ABILITY_CHARISMA);
                AddAbilityBonusTypeToMenu(NWScript.ABILITY_WISDOM);

                player.menu.choices.Add((
                  "Retour",
                  () => DrawMenuPage()
                ));
                player.menu.Draw();
            }

            private void AddAbilityBonusTypeToMenu(int abilityBonusType)
            {
                var currentAbilityBonus = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_ABILITY_BONUS, abilityBonusType);
                if (currentAbilityBonus < maxAbilityBonus)
                {
                    var name = ItemPropertyUtils.AbilityBonusToString(abilityBonusType);

                    player.menu.choices.Add((
                      StringUtils.FirstCharToUpper(name),
                      () => DrawCostPage(
                        NWScript.ItemPropertyAbilityBonus(abilityBonusType, currentAbilityBonus + 1),
                        $"Bonus de {name} +{currentAbilityBonus + 1}"
                      )
                    ));
                }
            }

            private void AddDamageBonusToMenu()
            {
                player.menu.choices.Add((
                  "Bonus de degats",
                  () => DrawDamageBonusPage()
                ));
            }

            private void DrawDamageBonusPage()
            {
                player.menu.Clear();
                player.menu.titleLines.Add("Choisissez un type de degats");

                AddDamageBonusTypeToMenu(NWScript.IP_CONST_DAMAGETYPE_ACID);
                AddDamageBonusTypeToMenu(NWScript.IP_CONST_DAMAGETYPE_COLD);
                AddDamageBonusTypeToMenu(NWScript.IP_CONST_DAMAGETYPE_ELECTRICAL);
                AddDamageBonusTypeToMenu(NWScript.IP_CONST_DAMAGETYPE_FIRE);
                AddDamageBonusTypeToMenu(NWScript.IP_CONST_DAMAGETYPE_SONIC);
                AddDamageBonusTypeToMenu(NWScript.IP_CONST_DAMAGETYPE_BLUDGEONING);
                AddDamageBonusTypeToMenu(NWScript.IP_CONST_DAMAGETYPE_PIERCING);
                AddDamageBonusTypeToMenu(NWScript.IP_CONST_DAMAGETYPE_SLASHING);

                player.menu.choices.Add((
                  "Retour",
                  () => DrawMenuPage()
                ));
                player.menu.Draw();
            }

            private void AddDamageBonusTypeToMenu(int damageBonusType)
            {
                int nCurrentDamageBonus = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_DAMAGE_BONUS, damageBonusType);

                if (Enum.IsDefined(typeof(ItemPropertyUtils.DamageBonus), nCurrentDamageBonus))
                {
                    var currentDamageBonus = (ItemPropertyUtils.DamageBonus)nCurrentDamageBonus;

                    if (ItemPropertyUtils.GetAverageDamageFromDamageBonus(currentDamageBonus) < ItemPropertyUtils.GetAverageDamageFromDamageBonus(maxDamageBonus))
                    {
                        var nextDamageBonus = ItemPropertyUtils.GetNextAverageDamageBonus(currentDamageBonus);
                        var name = ItemPropertyUtils.DamageBonusTypeToString(damageBonusType);

                        player.menu.choices.Add((
                          StringUtils.FirstCharToUpper(name),
                          () => DrawCostPage(
                            NWScript.ItemPropertyDamageBonus(damageBonusType, (int)nextDamageBonus),
                            $"Bonus de degats {name} +{ItemPropertyUtils.DamageBonusToString(nextDamageBonus)}"
                          )
                        ));
                    }
                }
            }

            private void AddSavingThrowBonusToMenu()
            {
                player.menu.choices.Add((
                  "Bonus aux jets de sauvegarde",
                  () => DrawSavingThrowBonusPage()
                ));
            }

            private void DrawSavingThrowBonusPage()
            {
                player.menu.Clear();
                player.menu.titleLines.Add("Choisissez un type de jet de sauvegarde");

                var currentUniversalSavingThrowBonus = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_SAVING_THROW_BONUS, NWScript.IP_CONST_SAVEVS_UNIVERSAL);

                if (currentUniversalSavingThrowBonus < maxSavingThrowBonus)
                {
                    player.menu.choices.Add((
                      "Universel",
                      () => DrawCostPage(
                        NWScript.ItemPropertyBonusSavingThrowVsX(NWScript.IP_CONST_SAVEVS_UNIVERSAL, currentUniversalSavingThrowBonus + 1),
                        $"Bonus de jet universel +{currentUniversalSavingThrowBonus + 1}"
                      )
                    ));
                }

                AddSavingThrowBonusTypeToMenu(NWScript.IP_CONST_SAVEBASETYPE_FORTITUDE);
                AddSavingThrowBonusTypeToMenu(NWScript.IP_CONST_SAVEBASETYPE_WILL);
                AddSavingThrowBonusTypeToMenu(NWScript.IP_CONST_SAVEBASETYPE_REFLEX);

                player.menu.choices.Add((
                  "Retour",
                  () => DrawMenuPage()
                ));
                player.menu.Draw();
            }

            private void AddSavingThrowBonusTypeToMenu(int savingThrowBonusType)
            {
                var currentSavingThrowBonus = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_SAVING_THROW_BONUS_SPECIFIC, savingThrowBonusType);

                if (currentSavingThrowBonus < maxSavingThrowBonus)
                {
                    var name = ItemPropertyUtils.SavingThrowBonusToString(savingThrowBonusType);

                    player.menu.choices.Add((
                      StringUtils.FirstCharToUpper(name),
                      () => DrawCostPage(
                        NWScript.ItemPropertyBonusSavingThrow(savingThrowBonusType, currentSavingThrowBonus + 1),
                        $"Bonus de {name} +{currentSavingThrowBonus + 1}"
                      )
                    ));
                }
            }

            private void AddRegenBonusToMenu()
            {
                var currentRegenBonus = ItemUtils.GetItemPropertyBonus(oItem, NWScript.ITEM_PROPERTY_REGENERATION);

                if (currentRegenBonus < maxRegenBonus)
                {
                    player.menu.choices.Add((
                      "Regeneration",
                      () => DrawCostPage(
                        NWScript.ItemPropertyRegeneration(currentRegenBonus + 1),
                        $"Bonus de regeneration +{currentRegenBonus + 1}"
                      )
                    ));
                }
            }
        }
    }
}
