using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;

using static NWN.Systems.ItemUtils;

namespace NWN.Systems
{
  public partial class LootSystem
  {
    private static async void UpdateDB(NwPlaceable oChest, NwCreature oClosedBy)
    {
      if (PlayerSystem.Players.TryGetValue(oClosedBy, out PlayerSystem.Player oPC))
      {
        string tag = oChest.Tag;
        string accountId = oPC.accountId.ToString();
        string serializedChest = oChest.Serialize().ToBase64EncodedString();
        string position = oChest.Position.ToString();
        string facing = oChest.Rotation.ToString();

        bool queryResult = await SqLiteUtils.InsertQueryAsync(SQL_TABLE,
          new List<string[]>() {
            new string[] { "chestTag", tag },
            new string[] { "accountId", accountId },
            new string[] { "serializedChest", serializedChest },
          new string[] { "position", position },
          new string[] { "facing", facing } },
          new List<string>() { "chestTag" },
          new List<string[]>() { new string[] { "serializedChest" }, new string[] { "position" }, new string[] { "facing" } },
          new List<string>() { "characterId", "grimoireName" });

        oPC.HandleAsyncQueryFeedback(queryResult, $"Coffre {tag} correctement sauvegardé.", "Erreur technique - Le coffre n'a pas pu être sauvegardé.");
      }
    }
    private void UpdateChestTagToLootsDic(NwPlaceable oChest)
    {
      var loots = new List<NwItem> { };

      foreach (NwItem item in oChest.Inventory.Items)
      {
        loots.Add(item);
      }
      chestTagToLootsDic[oChest.Tag] = loots;
    }
    private static void ThrowException(string message)
    {
      throw new ApplicationException($"LootSystem: {message}");
    }

    private static LootQuality GetLootQualityFromAreaLevel(int areaLevel)
    {
      int roll = NwRandom.Roll(Utils.random, 100) + areaLevel * Config.mobQualityRollMultiplier;

      if (roll < (int)LootQuality.Inutile)
        return LootQuality.Inutile;
      else if (roll < (int)LootQuality.Simple)
        return LootQuality.Simple;
      else if (roll < (int)LootQuality.Raffiné)
        return LootQuality.Raffiné;
      else if (roll < (int)LootQuality.Superbe)
        return LootQuality.Superbe;
      else if (roll < (int)LootQuality.Rare)
        return LootQuality.Rare;
      else if (roll < (int)LootQuality.Exotique)
        return LootQuality.Exotique;
      else if (roll < (int)LootQuality.Transcendé)
        return LootQuality.Transcendé;
      else return LootQuality.Légendaire;
    }
    private static int GetItemStatsDowngradeFromQuality(LootQuality quality, int stat)
    {
      if (quality == LootQuality.Inutile)
        return Utils.random.Next((int)((double)stat * 0.05), stat + 1);
      else
        return Utils.random.Next((int)((double)stat * (((double)quality / 100 + 1) / 10)), stat + 1); 
    }
    private static LootableType GetLootType()
    {
      return (LootableType)lootTypeArray.GetValue(Utils.random.Next(lootTypeArray.Length));
    }
    /*private static NwItem CreateLootItem(int areaLevel)
    {
      NwItem lootItem;
      LootQuality itemQuality = GetLootQualityFromAreaLevel(areaLevel);

      switch (GetLootType())
      {
        case LootableType.Equipement:

          lootItem = GetEquipementItem(itemQuality);
          SetItemName(itemQuality, lootItem);
          SetItemDurability(itemQuality, lootItem);
          SetItemNBSlots(itemQuality, lootItem);
          AddItemProperties(itemQuality, lootItem);

          if (itemDamageDictionary.TryGetValue(lootItem.BaseItem.ItemType, out int[,] weaponStats)) // Si l'objet est une arme
            SetWeaponRandomDamage(itemQuality, lootItem, weaponStats);

          // TODO : Déterminer les enchantements

          break;
      }

      return lootItem;
    }*/
    private static NwItem GetEquipementItem(LootQuality itemQuality)
    {
      var lootList = BaseItems2da.lootableEquipement;
      int armorQuality = 9;

      if (itemQuality < LootQuality.Simple)
      {
        lootList = lootList.Where(i => i.PrerequisiteFeats.Any(f => f.FeatType != Feat.WeaponProficiencyMartial
        && f.FeatType != Feat.WeaponProficiencyExotic && i.ItemType != BaseItemType.LargeShield && i.ItemType != BaseItemType.TowerShield)).ToList();
        armorQuality = 4;
      }
      else if (itemQuality < LootQuality.Raffiné)
      {
        lootList = lootList.Where(i => i.PrerequisiteFeats.Any(f => f.FeatType != Feat.WeaponProficiencyExotic && i.ItemType != BaseItemType.TowerShield)).ToList();
        armorQuality = 6;
      }
      else if (itemQuality < LootQuality.Rare)
        armorQuality = 8;

      NwBaseItem baseItem = lootList[Utils.random.Next(lootList.Count)];
      string itemResref = BaseItems2da.baseItemTable.GetRow((int)lootList[Utils.random.Next(lootList.Count)].Id).craftedItem;

      if (baseItem.ItemType == BaseItemType.Armor)
        itemResref = Armor2da.armorTable.GetRow(Utils.random.Next(armorQuality)).craftResRef;

      return NwItem.Create(itemResref, ModuleSystem.placeholderTemplate.Location);
    }
    private static void SetWeaponRandomDamage(LootQuality itemQuality, NwItem lootItem, int[,] weaponStats)
    {
      int grade = (int)itemQuality / 100 - 1;
      int minDamage = weaponStats[grade, 0];
      int maxDamage = weaponStats[grade, 1];

      lootItem.GetObjectVariable<LocalVariableInt>("_MIN_WEAPON_DAMAGE").Value = GetItemStatsDowngradeFromQuality(itemQuality, minDamage);
      lootItem.GetObjectVariable<LocalVariableInt>("_MAX_WEAPON_DAMAGE").Value = GetItemStatsDowngradeFromQuality(itemQuality, maxDamage);
    }
    private static void SetItemName(LootQuality itemQuality, NwItem lootItem)
    {
      lootItem.Name += $" {itemQuality}";
      lootItem.Name = lootItem.Name.ColorString(lootColor[itemQuality]);
    }
    private static void SetItemDurability(LootQuality itemQuality, NwItem lootItem)
    {
      int grade = (int)itemQuality / 100 - 1;

      lootItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value = GetItemStatsDowngradeFromQuality(itemQuality, ItemUtils.GetBaseItemCost(lootItem) * 100 * grade);
      lootItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = GetItemStatsDowngradeFromQuality(itemQuality, ItemUtils.GetBaseItemCost(lootItem) * 100 * grade);
    }
    private static void SetItemNBSlots(LootQuality itemQuality, NwItem lootItem)
    {
      switch(itemQuality)
      {
        case LootQuality.Raffiné:
          lootItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value = 1;
          lootItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value = 1;
          break;

        case LootQuality.Superbe:
          lootItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value = 3;
          lootItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value = 3;
          break;

        case LootQuality.Rare:
          lootItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value = 4;
          lootItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value = 4;
          break;

        case LootQuality.Exotique:
          lootItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value = 5;
          lootItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value = 5;
          break;

        case LootQuality.Transcendé:
          lootItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value = 6;
          lootItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value = 6;
          break;

        case LootQuality.Légendaire:
          lootItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value = 8;
          lootItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value = 8;
          break;
      }

      ItemCategory itemCategory = GetItemCategory(lootItem.BaseItem.ItemType);

      if (itemCategory == ItemCategory.TwoHandedMeleeWeapon || itemCategory == ItemCategory.RangedWeapon)
      {
        lootItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += lootItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value / 2;
        lootItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value += lootItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value / 2;
      }
    }
    private static void AddItemProperties(LootQuality itemQuality, NwItem lootItem)
    {
      int grade = (int)itemQuality / 100 - 1;

      switch (lootItem.BaseItem.ItemType)
      {
        case BaseItemType.Armor: AddArmorItemProperties(itemQuality, lootItem, grade); break;
        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield: AddShieldItemProperties(itemQuality, lootItem, grade); break;
        case BaseItemType.Helmet:
        case BaseItemType.Cloak:
        case BaseItemType.Boots:
        case BaseItemType.Gloves:
        case BaseItemType.Belt:
        case BaseItemType.Bracer: lootItem.AddItemProperty(ItemProperty.ACBonus(GetItemStatsDowngradeFromQuality(itemQuality, 7 * grade)), EffectDuration.Permanent); break;
      }

      switch (GetItemCategory(lootItem.BaseItem.ItemType))
      {
        case ItemCategory.OneHandedMeleeWeapon: lootItem.AddItemProperty(ItemProperty.AttackBonus(GetItemStatsDowngradeFromQuality(itemQuality, 2 * grade)), EffectDuration.Permanent); break;

        case ItemCategory.TwoHandedMeleeWeapon:
        case ItemCategory.RangedWeapon: lootItem.AddItemProperty(ItemProperty.AttackBonus(GetItemStatsDowngradeFromQuality(itemQuality, 4 * grade)), EffectDuration.Permanent); break;
        case ItemCategory.Ammunition: lootItem.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1), EffectDuration.Permanent); break;
      }
    }
    private static void AddArmorItemProperties(LootQuality itemQuality, NwItem lootItem, int grade)
    {
      lootItem.AddItemProperty(ItemProperty.ACBonus(GetItemStatsDowngradeFromQuality(itemQuality, lootItem.BaseACValue * 3 + 7 * grade)), EffectDuration.Permanent);

      switch (lootItem.BaseACValue)
      {
        case 1:
        case 2:
        case 3:
        case 4: lootItem.AddItemProperty(ItemProperty.ACBonusVsDmgType((IPDamageType)14, lootItem.BaseACValue * 5), EffectDuration.Permanent); break;
        case 5:
          lootItem.AddItemProperty(ItemProperty.ACBonusVsDmgType((IPDamageType)14, 30), EffectDuration.Permanent);
          lootItem.AddItemProperty(ItemProperty.ACBonusVsDmgType((IPDamageType)4, 5), EffectDuration.Permanent);
          break;
        case 6: lootItem.AddItemProperty(ItemProperty.ACBonusVsDmgType((IPDamageType)4, 10), EffectDuration.Permanent); break;
        case 7: lootItem.AddItemProperty(ItemProperty.ACBonusVsDmgType((IPDamageType)4, 15), EffectDuration.Permanent); break;
        case 8: lootItem.AddItemProperty(ItemProperty.ACBonusVsDmgType((IPDamageType)4, 20), EffectDuration.Permanent); break;
      }
    }
    private static void AddShieldItemProperties(LootQuality itemQuality, NwItem lootItem, int grade)
    {
      switch (lootItem.BaseItem.ItemType)
      {
        case BaseItemType.SmallShield:
          lootItem.AddItemProperty(ItemProperty.ACBonus(GetItemStatsDowngradeFromQuality(itemQuality, 2 + 2 * grade)), EffectDuration.Permanent);
          lootItem.AddItemProperty(ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 5), EffectDuration.Permanent);
          break;
        case BaseItemType.LargeShield:
          lootItem.AddItemProperty(ItemProperty.ACBonus(GetItemStatsDowngradeFromQuality(itemQuality, 4 + 2 * grade)), EffectDuration.Permanent);
          lootItem.AddItemProperty(ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 10), EffectDuration.Permanent);
          break;
        case BaseItemType.TowerShield:
          lootItem.AddItemProperty(ItemProperty.ACBonus(GetItemStatsDowngradeFromQuality(itemQuality, 2 * grade)), EffectDuration.Permanent);
          lootItem.AddItemProperty(ItemProperty.ACBonusVsDmgType(IPDamageType.Piercing, 30), EffectDuration.Permanent);
          break;
      }
    }
  }
}
