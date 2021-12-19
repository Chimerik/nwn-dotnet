using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.ItemUtils;

namespace NWN.Systems.Craft
{
  public class Blueprint
  {
    public readonly int baseItemType;
    public readonly string name;
    public string workshopTag { get; set; }
    public string craftedItemTag { get; set; }
    public int mineralsCost { get; set; }
    public Feat feat { get; set; }
    public Feat jobFeat { get; set; }
    public int goldCost { get; }
    public Blueprint(int baseItemType)
    {
      this.baseItemType = baseItemType;
      this.feat = Collect.System.craftBaseItemFeatDictionnary[baseItemType];

      if (baseItemType < 0) // il s'agit d'une armure. Vu que les références ne se trouvent pas dans le même 2da, je triche en utilisant des valeurs négatives
      {
        if (baseItemType == -9) // cas particulier des vêtements, je triche pour ne pas doubler la valeur 0 dans le dictionnary
          baseItemType = 0;

        ArmorTable.Entry armorEntry = Armor2da.armorTable.GetDataEntry(-baseItemType);

        this.mineralsCost = armorEntry.cost * 10;
        this.goldCost = armorEntry.cost * 5;
        this.name = armorEntry.name;
        this.workshopTag = armorEntry.workshop;
        this.craftedItemTag = armorEntry.craftResRef;
      }
      else
      {
        BaseItemTable.Entry baseItemEntry = BaseItems2da.baseItemTable.GetBaseItemDataEntry((BaseItemType)baseItemType);
        NwBaseItem baseItem = NwBaseItem.FromItemType((BaseItemType)baseItemType);
        this.name = baseItem.Name;
        this.mineralsCost = (int)(baseItem.BaseCost * 10);
        this.goldCost = (int)(baseItem.BaseCost * 5);
        this.workshopTag = baseItemEntry.workshop;
        this.craftedItemTag = baseItemEntry.craftedItem;
      }

      switch (workshopTag)
      {
        case "forge":
          jobFeat = CustomFeats.Forge;
          break;
        case "scierie":
          jobFeat = CustomFeats.Ebeniste;
          break;
        case "tannerie":
          jobFeat = CustomFeats.Tanner;
          break;
        case "enchant":
          jobFeat = CustomFeats.Enchanteur;
          break;
      }
    }
    public static List<ItemProperty> GetCraftItemProperties(string material, NwItem craftedItem)
    {
      ItemCategory itemCategory = GetItemCategory(craftedItem.BaseItem.ItemType);

      int materialTier = 0;

      if (material == "mauvais état")
        materialTier = 0;
      else if (Enum.TryParse(material, out MineralType myMineralType))
        materialTier = (int)myMineralType;
      else if (Enum.TryParse(material, out PlankType myPlankType))
        materialTier = (int)myPlankType;
      else if (Enum.TryParse(material, out LeatherType myLeatherType))
        materialTier = (int)myLeatherType;

      craftedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value = GetBaseItemCost(craftedItem) * 100 * materialTier;
      craftedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = GetBaseItemCost(craftedItem) * 100 * materialTier;

      if (materialTier == 0)
        craftedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value /= 2;
      else
      {
        if (craftedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").HasValue)
          craftedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
        else
          craftedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value = 1;
      }

      switch (craftedItem.BaseItem.ItemType)
      {
        case BaseItemType.Armor:
          return GetArmorProperties(craftedItem, materialTier);
        case BaseItemType.SmallShield:
          return GetSmallShieldProperties(materialTier);
        case BaseItemType.LargeShield:
          return GetLargeShieldProperties(materialTier);
        case BaseItemType.TowerShield:
          return GetTowerShieldProperties(materialTier);
        case BaseItemType.Helmet:
        case BaseItemType.Cloak:
        case BaseItemType.Boots:
          return GetArmorPartProperties();
        case BaseItemType.Gloves:
          return GetGlovesProperties();
        case BaseItemType.Amulet:
          return GetAmuletProperties(materialTier);
        case BaseItemType.Ring:
          return GetRingProperties(materialTier);
        case BaseItemType.Belt:
          return GetBeltProperties(materialTier);
      }

      switch(GetItemCategory(craftedItem.BaseItem.ItemType))
      {
        case ItemCategory.CraftTool:
          return GetToolProperties(craftedItem, materialTier);
        case ItemCategory.OneHandedMeleeWeapon:
          return GetOneHandedMeleeWeaponProperties();
        case ItemCategory.TwoHandedMeleeWeapon:
          return GetTwoHandedMeleeWeaponProperties();
        case ItemCategory.RangedWeapon:
          return GetRangedWeaponProperties();
        case ItemCategory.Ammunition:
          return GetAmmunitionProperties();
      }

      Utils.LogMessageToDMs($"No craft property found for material {material} and item {itemCategory}");

      return new List<ItemProperty>();
    }
    public static void BlueprintValidation(NwPlayer oPlayer, NwGameObject target, Feat feat)
    {
      if (!(target is NwItem) || target.Tag != "blueprint")
      {
        oPlayer.SendServerMessage($"{target.Name} n'est pas un patron valide.");
        return;
      }

      NwItem item = (NwItem)target;

      if (item.Possessor != oPlayer.LoginCreature)
      {
        oPlayer.SendServerMessage("Le patron doit se trouver dans votre inventaire afin d'effectuer cette opération.");
        return;
      }

      int baseItemType = target.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;

      if (Collect.System.blueprintDictionnary.TryGetValue(baseItemType, out Blueprint blueprint))
        if (PlayerSystem.Players.TryGetValue(oPlayer.LoginCreature, out PlayerSystem.Player player))
          blueprint.StartJob(player, item, feat);
        else
        {
          oPlayer.SendServerMessage("[ERREUR HRP] - Le patron utilisé n'est pas correctement initialisé. Le bug a été remonté au staff.");
          Utils.LogMessageToDMs($"Blueprint Invalid : {item.Name} - Base Item Type : {baseItemType} - Used by : {oPlayer.LoginCreature.Name}");
        }
    }
    private void StartJob(PlayerSystem.Player player, NwItem blueprint, Feat feat)
    {
      switch (feat)
      {
        case CustomFeats.BlueprintCopy:
          player.craftJob.Start(Job.JobType.BlueprintCopy, this, player, blueprint);
          break;
        case CustomFeats.Research:
          player.craftJob.Start(Job.JobType.BlueprintResearchTimeEfficiency, this, player, blueprint);
          break;
        case CustomFeats.Metallurgy:
          player.craftJob.Start(Job.JobType.BlueprintResearchMaterialEfficiency, this, player, blueprint);
          break;
      }
    }
    public string DisplayBlueprintInfo(NwPlayer player, NwItem oItem)
    {
      int iMineralCost = this.GetBlueprintMineralCostForPlayer(player, oItem);
      float iJobDuration = this.GetBlueprintTimeCostForPlayer(player, oItem);
      string sMaterial = GetMaterialFromTargetItem(NwModule.FindObjectsWithTag<NwPlaceable>(workshopTag).FirstOrDefault());
      string bpDescription = $"Patron de création de l'objet artisanal : {name}\n\n\n" +
        $"Recherche d'efficacité matérielle niveau {oItem.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value}\n\n" +
        $"Coût initial en {sMaterial} : {iMineralCost}.\n Puis 10 % de moins par amélioration vers un matériau supérieur.\n" +
        $"Recherche d'efficacité de temps niveau {oItem.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value} \n\n" +
        $"Temps de fabrication et d'amélioration : {Utils.StripTimeSpanMilliseconds(DateTime.Now.AddSeconds(iJobDuration).Subtract(DateTime.Now))}.";
      
      int runs = oItem.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value; 

      if (runs > 0)
        bpDescription += $"\n\nUtilisation(s) restante(s) : {runs}";

      return bpDescription;
    }
    public int GetBlueprintMineralCostForPlayer(NwPlayer oPC, NwItem item)
    {
      if (!PlayerSystem.Players.TryGetValue(oPC.LoginCreature, out PlayerSystem.Player player))
        return 999999999;

      int iSkillLevel = 0;

      if (player.learntCustomFeats.ContainsKey(jobFeat))
        iSkillLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(jobFeat, player.learntCustomFeats[jobFeat]);

      if (player.learntCustomFeats.ContainsKey(feat))
        iSkillLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(feat, player.learntCustomFeats[feat]);

      iSkillLevel += item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value;

      return mineralsCost * (100 - iSkillLevel) / 100;
    }
    public float GetBlueprintTimeCostForPlayer(NwPlayer oPC, NwItem item)
    {
      if (!PlayerSystem.Players.TryGetValue(oPC.LoginCreature, out PlayerSystem.Player player))
        return 999999999;

      int iSkillLevel = 0;
      float fJobDuration = this.mineralsCost * 300;

      if (player.learntCustomFeats.ContainsKey(jobFeat))
        iSkillLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(jobFeat, player.learntCustomFeats[jobFeat]);

      if (player.learntCustomFeats.ContainsKey(feat))
        iSkillLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(feat, player.learntCustomFeats[feat]);

      iSkillLevel += item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value;

      return fJobDuration * (100 - iSkillLevel) / 100;
    }
    public string GetMaterialFromTargetItem(NwGameObject oTarget)
    {
      if (oTarget.Tag == workshopTag)
      {
        switch (workshopTag)
        {
          case "forge":
            return Enum.GetName(typeof(MineralType), MineralType.Tritanium);
          case "scierie":
            return Enum.GetName(typeof(PlankType), PlankType.Laurelinade);
          case "tannerie":
            return Enum.GetName(typeof(LeatherType), LeatherType.MauvaisCuir);
        }
      }
      else if (oTarget.Tag == this.craftedItemTag)
      {
        string material = oTarget.GetObjectVariable<LocalVariableString>("_ITEM_MATERIAL").Value;

        if (Enum.TryParse(material, out MineralType myMineralType))
          return Enum.GetName(typeof(MineralType), myMineralType + 1);
        else if (Enum.TryParse(material, out PlankType myPlankType))
          return Enum.GetName(typeof(PlankType), myPlankType + 1);
        else if (Enum.TryParse(material, out LeatherType myLeatherType))
          return Enum.GetName(typeof(LeatherType), myLeatherType + 1);
      }

      return "Invalid";
    }
    public static ItemProperty GetCraftEnchantementProperties(NwItem craftedItem, string ipString, int boost, int enchanterId)
    {
      string[] IPproperties = ipString.Split("_");
      string enchTag = $"ENCHANTEMENT_{IPproperties[0]}";

      ItemProperty newIP = ItemProperty.Quality(IPQuality.Unknown);

      int value;
      if (int.TryParse(IPproperties[1], out value))
      {
        newIP.PropertyType = (ItemPropertyType)value;
        enchTag += $"_{newIP.PropertyType}";
      }
      else
        Utils.LogMessageToDMs($"Could not parse nProperty in : {ipString}");
      if (int.TryParse(IPproperties[2], out value))
      {
        newIP.SubType = value;
        enchTag += $"_{newIP.SubType}";
      }
      else
        Utils.LogMessageToDMs($"Could not parse nSubType in : {ipString}");
      if (int.TryParse(IPproperties[3], out value))
      {
        newIP.CostTable = value;
        enchTag += $"_{newIP.CostTable}";
      }
      else
        Utils.LogMessageToDMs($"Could not parse nCostTable in : {ipString}");
      if (int.TryParse(IPproperties[4], out value))
        newIP.CostTableValue = value + boost;
      else
        Utils.LogMessageToDMs($"Could not parse nCostTableValue in : {ipString}");

      ItemProperty existingIP = craftedItem.ItemProperties.FirstOrDefault(i => i.DurationType == EffectDuration.Permanent && i.PropertyType == newIP.PropertyType && i.SubType == newIP.SubType && i.Param1Table == newIP.Param1Table);

      if (existingIP != null)
      {
        craftedItem.RemoveItemProperty(existingIP);

        if(newIP.PropertyType == ItemPropertyType.DamageBonus 
          || newIP.PropertyType == ItemPropertyType.DamageBonusVsAlignmentGroup
          || newIP.PropertyType == ItemPropertyType.DamageBonusVsRacialGroup
          || newIP.PropertyType == ItemPropertyType.DamageBonusVsSpecificAlignment)
        {
          int newRank = ItemPropertyDamageCost2da.ipDamageCost.GetRankFromCostValue(newIP.CostTableValue);
          int existingRank = ItemPropertyDamageCost2da.ipDamageCost.GetRankFromCostValue(existingIP.CostTableValue);
          
          if (existingRank > newRank)
            newRank = existingRank + 1;
          else
            newRank += 1;

          newIP.CostTableValue = ItemPropertyDamageCost2da.ipDamageCost.GetDamageCostValueFromRank(newRank);
        }
        else
        {
          if (existingIP.CostTableValue > newIP.CostTableValue)
            newIP.CostTableValue = existingIP.CostTableValue + 1;
          else
            newIP.CostTableValue += 1;
        }
      }

      enchTag += $"_{newIP.CostTableValue}_{enchanterId}";
      newIP.Tag = enchTag;
      
      return newIP;
    }
  }
}
