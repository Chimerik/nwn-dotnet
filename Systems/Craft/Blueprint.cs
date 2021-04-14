using System;
using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
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

        int value;
        if (int.TryParse(NWScript.Get2DAString("armor", "COST", -baseItemType), out value))
        {
          this.mineralsCost = value * 10;
          this.goldCost = value * 5;
        }

        if (int.TryParse(NWScript.Get2DAString("armor", "NAME", -baseItemType), out value))
          this.name = NWScript.GetStringByStrRef(value);

        this.workshopTag = NWScript.Get2DAString("armor", "WORKSHOP", -baseItemType);
        this.craftedItemTag = NWScript.Get2DAString("armor", "CRAFTRESREF", -baseItemType);
      }
      else
      {
        int value;
        if (int.TryParse(NWScript.Get2DAString("baseitems", "Name", baseItemType), out value))
          this.name = NWScript.GetStringByStrRef(value);

        if (int.TryParse(NWScript.Get2DAString("baseitems", "BaseCost", baseItemType), out value))
        {
          this.mineralsCost = value * 10;
          this.goldCost = value * 5;
        }

        this.workshopTag = NWScript.Get2DAString("baseitems", "Category", baseItemType);
        this.craftedItemTag = NWScript.Get2DAString("baseitems", "label", baseItemType);
      }

      switch (workshopTag)
      {
        case "forge":
          jobFeat = CustomFeats.Forge;
          break;
        case "scierie":
          jobFeat = CustomFeats.Ebeniste;
          break;
      }
    }
    public static API.ItemProperty[] GetCraftItemProperties(string material, NwItem craftedItem)
    {
      ItemCategory itemCategory = GetItemCategory(craftedItem.BaseItemType);
      if (itemCategory == ItemCategory.Invalid)
      {
        Utils.LogMessageToDMs($"Item {craftedItem.Name} - Base {craftedItem.BaseItemType} - Category invalid");
        
        return new API.ItemProperty[]
        {
          API.ItemProperty.Quality(IPQuality.Unknown)
      };
      }

      if (material == "mauvais état")
        return GetBadItemProperties(itemCategory, craftedItem);
      else if (Enum.TryParse(material, out MineralType myMineralType))
      {
        switch (myMineralType)
        {
          case MineralType.Tritanium: return GetTritaniumItemProperties(craftedItem);
          case MineralType.Pyerite: return GetPyeriteItemProperties(itemCategory, craftedItem);
        }
      }
      else if (Enum.TryParse(material, out PlankType myPlankType))
      {
        switch (myPlankType)
        {
          case PlankType.Laurelinade: return GetTritaniumItemProperties(craftedItem);
          case PlankType.Telperionade: return GetPyeriteItemProperties(itemCategory, craftedItem);
        }
      }
      else if (Enum.TryParse(material, out LeatherType myLeatherType))
      {
        switch (myLeatherType)
        {
          case LeatherType.MauvaisCuir: return GetTritaniumItemProperties(craftedItem);
          case LeatherType.CuirCommun: return GetPyeriteItemProperties(itemCategory, craftedItem);
        }
      }

      Utils.LogMessageToDMs($"No craft property found for material {material} and item {itemCategory}");

      return new API.ItemProperty[]
      {
          API.ItemProperty.Quality(IPQuality.Unknown)
    };
    }
    public static void BlueprintValidation(NwPlayer oPlayer, NwGameObject target, Feat feat)
    {
      if (!(target is NwItem) || target.Tag != "blueprint")
      {
        oPlayer.SendServerMessage($"{target.Name} n'est pas un patron valide.");
        return;
      }

      NwItem item = (NwItem)target;

      if (item.Possessor != oPlayer)
      {
        oPlayer.SendServerMessage("Le patron doit se trouver dans votre inventaire afin d'effectuer cette opération.");
        return;
      }

      int baseItemType = target.GetLocalVariable<int>("_BASE_ITEM_TYPE").Value;

      if (Collect.System.blueprintDictionnary.TryGetValue(baseItemType, out Blueprint blueprint))
        if (PlayerSystem.Players.TryGetValue(oPlayer, out PlayerSystem.Player player))
          blueprint.StartJob(player, item, feat);
        else
        {
          oPlayer.SendServerMessage("[ERREUR HRP] - Le patron utilisé n'est pas correctement initialisé. Le bug a été remonté au staff.");
          Utils.LogMessageToDMs($"Blueprint Invalid : {item.Name} - Base Item Type : {baseItemType} - Used by : {oPlayer.Name}");
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
        $"Recherche d'efficacité matérielle niveau {oItem.GetLocalVariable<int>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value}\n\n" +
        $"Coût initial en {sMaterial} : {iMineralCost}.\n Puis 10 % de moins par amélioration vers un matériau supérieur.\n" +
        $"Recherche d'efficacité de temps niveau {oItem.GetLocalVariable<int>("_BLUEPRINT_TIME_EFFICIENCY").Value}\n\n" +
        $"Temps de fabrication et d'amélioration : {Utils.StripTimeSpanMilliseconds(DateTime.Now.AddSeconds(iJobDuration).Subtract(DateTime.Now))}.";
      
      int runs = oItem.GetLocalVariable<int>("_BLUEPRINT_RUNS").Value; 

      if (runs > 0)
        bpDescription += $"\n\nUtilisation(s) restante(s) : {runs}";

      return bpDescription;
    }
    public int GetBlueprintMineralCostForPlayer(NwPlayer oPC, NwItem item)
    {
      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return 999999999;

      int iSkillLevel = 0;

      if (player.learntCustomFeats.ContainsKey(jobFeat))
        iSkillLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(jobFeat, player.learntCustomFeats[jobFeat]);

      if (player.learntCustomFeats.ContainsKey(feat))
        iSkillLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(feat, player.learntCustomFeats[feat]);

      iSkillLevel += item.GetLocalVariable<int>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value;

      return (mineralsCost - iSkillLevel) / 100;
    }
    public float GetBlueprintTimeCostForPlayer(NwPlayer oPC, NwItem item)
    {
      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return 999999999;

      int iSkillLevel = 0;
      float fJobDuration = this.mineralsCost * 300;

      if (player.learntCustomFeats.ContainsKey(jobFeat))
        iSkillLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(jobFeat, player.learntCustomFeats[jobFeat]);

      if (player.learntCustomFeats.ContainsKey(feat))
        iSkillLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(feat, player.learntCustomFeats[feat]);

      iSkillLevel += item.GetLocalVariable<int>("_BLUEPRINT_TIME_EFFICIENCY").Value;

      return (fJobDuration - iSkillLevel) / 100;
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
        string material = oTarget.GetLocalVariable<string>("_ITEM_MATERIAL").Value;
        if (Enum.TryParse(material, out MineralType myMineralType))
          return Enum.GetName(typeof(MineralType), myMineralType + 1);
        else if (Enum.TryParse(material, out PlankType myPlankType))
          return Enum.GetName(typeof(PlankType), myPlankType + 1);
        else if (Enum.TryParse(material, out LeatherType myLeatherType))
          return Enum.GetName(typeof(LeatherType), myLeatherType + 1);
      }

      return "Invalid";
    }
    public static API.ItemProperty GetCraftEnchantementProperties(NwItem craftedItem, string ipString, int boost)
    {
      string enchTag = $"ENCHANTEMENT_{ipString}";
      API.ItemProperty currentIP = craftedItem.ItemProperties.FirstOrDefault(ip => ip.Tag == enchTag);

      if (currentIP != null)
      {
        craftedItem.RemoveItemProperty(currentIP);
        currentIP.CostTableValue += 1 + boost;

        return currentIP;
      }
      else
      {
        string[] IPproperties = ipString.Split("_");

        API.ItemProperty newIP = API.ItemProperty.Quality(IPQuality.Unknown);

        int value;
        if (Int32.TryParse(IPproperties[0], out value))
          newIP.PropertyType = (ItemPropertyType)value;
        else
          Utils.LogMessageToDMs($"Could not parse nProperty in : {ipString}");
        if (Int32.TryParse(IPproperties[0], out value))
          newIP.SubType = Int32.Parse(IPproperties[1]);
        else
          Utils.LogMessageToDMs($"Could not parse nSubType in : {ipString}");
        if (Int32.TryParse(IPproperties[0], out value))
          newIP.CostTable = Int32.Parse(IPproperties[2]);
        else
          Utils.LogMessageToDMs($"Could not parse nCostTable in : {ipString}");
        if (Int32.TryParse(IPproperties[0], out value))
          newIP.CostTableValue = Int32.Parse(IPproperties[3]) + boost;
        else
          Utils.LogMessageToDMs($"Could not parse nCostTableValue in : {ipString}");

        newIP.Tag = enchTag;

        return newIP;
      }
    }
  }
}
