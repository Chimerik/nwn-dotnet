using System;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.Items.Utils;
using static NWN.Systems.PlayerSystem;

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
          this.mineralsCost = value * 1000;
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
          this.mineralsCost = value * 1000;
          this.goldCost = value * 5;
        }

        this.workshopTag = NWScript.Get2DAString("baseitems", "Category", baseItemType);
        this.craftedItemTag = NWScript.Get2DAString("baseitems", "label", baseItemType);
      }

      switch (workshopTag)
      {
        case "forge":
          jobFeat = Feat.Forge;
          break;
        case "scierie":
          jobFeat = Feat.Ebeniste;
          break;
      }
    }
    public static ItemProperty[] GetCraftItemProperties(string material, ItemCategory itemCategory)
    {
      if(itemCategory == ItemCategory.Invalid)
      {
        Utils.LogMessageToDMs($"Item Category invalid");

        return new ItemProperty[]
        {
          NWScript.ItemPropertyVisualEffect(NWScript.VFX_NONE)
        };
      }

      if (Enum.TryParse(material, out MineralType myMineralType))
      {
        switch (myMineralType)
        {
          case MineralType.Tritanium: return GetTritaniumItemProperties();
          case MineralType.Pyerite: return GetPyeriteItemProperties(itemCategory);
        }
      }
      else if (Enum.TryParse(material, out PlankType myPlankType))
      {
        switch (myPlankType)
        {
          case PlankType.Laurelinade: return GetTritaniumItemProperties();
          case PlankType.Telperionade: return GetPyeriteItemProperties(itemCategory);
        }
      }

      Utils.LogMessageToDMs($"No craft property found for material {material} and item {itemCategory}");

      return new ItemProperty[]
      {
          NWScript.ItemPropertyVisualEffect(NWScript.VFX_NONE)
      };
    }
    public static void BlueprintValidation(uint oidSelf, uint oTarget, Feat feat)
    {
      if (Players.TryGetValue(oidSelf, out Player oPC))
      {
        if (Convert.ToBoolean(NWScript.GetIsObjectValid(oTarget)) && NWScript.GetTag(oTarget) == "blueprint")
        {
          if(NWScript.GetItemPossessor(oTarget) != oidSelf)
          {
            NWScript.SendMessageToPC(oidSelf, "Le patron doit se trouver dans votre inventaire afin d'effectuer cette opération.");
            return;
          }

          int baseItemType = NWScript.GetLocalInt(oTarget, "_BASE_ITEM_TYPE");
           
          if (Collect.System.blueprintDictionnary.ContainsKey(baseItemType))
            Collect.System.blueprintDictionnary[baseItemType].StartJob(oPC, oTarget, feat);
          else
          {
            NWScript.SendMessageToPC(oidSelf, "[ERREUR HRP] - Le patron utilisé n'est pas correctement initialisé. Le bug a été remonté au staff.");
            Utils.LogMessageToDMs($"Blueprint Invalid : {NWScript.GetName(oTarget)} - Base Item Type : {baseItemType} - Used by : {NWScript.GetName(oidSelf)}");
          }
        }
        else
          NWScript.SendMessageToPC(oidSelf, "Vous devez sélectionner un patron valide.");
      }
    }
    private void StartJob(Player player, uint blueprint, Feat feat)
    {
      switch (feat)
      {
        case Feat.BlueprintCopy:
        case Feat.BlueprintCopy2:
        case Feat.BlueprintCopy3:
        case Feat.BlueprintCopy4:
        case Feat.BlueprintCopy5:
          player.craftJob.Start(Job.JobType.BlueprintCopy, this, player, blueprint);
          break;
        case Feat.Research:
        case Feat.Research2:
        case Feat.Research3:
        case Feat.Research4:
        case Feat.Research5:
          player.craftJob.Start(Job.JobType.BlueprintResearchTimeEfficiency, this, player, blueprint);
          break;
        case Feat.Metallurgy:
        case Feat.Metallurgy2:
        case Feat.Metallurgy3:
        case Feat.Metallurgy4:
        case Feat.Metallurgy5:
          player.craftJob.Start(Job.JobType.BlueprintResearchMaterialEfficiency, this, player, blueprint);
          break;
      }
    }
    public string DisplayBlueprintInfo(Player player, uint oItem)
    {
      int iMineralCost = this.GetBlueprintMineralCostForPlayer(player, oItem);
      float iJobDuration = this.GetBlueprintTimeCostForPlayer(player, oItem);

      string bpDescription = $"Patron de création de l'objet artisanal : {name}\n\n\n" +
        $"Recherche d'efficacité matérielle niveau {NWScript.GetLocalInt(oItem, "_BLUEPRINT_MATERIAL_EFFICIENCY")}\n\n" +
        $"Coût initial en Tritanium : {iMineralCost}.\n Puis 10 % de moins par amélioration vers un matériau supérieur.\n" +
        $"Recherche d'efficacité de temps niveau {NWScript.GetLocalInt(oItem, "_BLUEPRINT_TIME_EFFICIENCY")}\n\n" +
        $"Temps de fabrication et d'amélioration : {Utils.StripTimeSpanMilliseconds(DateTime.Now.AddSeconds(iJobDuration).Subtract(DateTime.Now))}.";

      int runs = NWScript.GetLocalInt(oItem, "_BLUEPRINT_RUNS");

      if (runs > 0)
        bpDescription += $"\n\nUtilisation(s) restante(s) : {runs}";

      return bpDescription;
    }
    public int GetBlueprintMineralCostForPlayer(Player player, uint item)
    {
      int iSkillLevel = 1;

      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)jobFeat)), out value))
        iSkillLevel += value;

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)feat)), out value))
        iSkillLevel += value;

      return this.mineralsCost - (this.mineralsCost * (iSkillLevel + NWScript.GetLocalInt(item, "_BLUEPRINT_MATERIAL_EFFICIENCY")) / 100);
    }
    public float GetBlueprintTimeCostForPlayer(Player player, uint item)
    {
      int iSkillLevel = 1;
      float fJobDuration = this.mineralsCost;

      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)jobFeat)), out value))
        iSkillLevel += value;

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)feat)), out value))
        iSkillLevel += value;

      return fJobDuration - (fJobDuration * (iSkillLevel + NWScript.GetLocalInt(item, "_BLUEPRINT_TIME_EFFICIENCY")) / 100);
    }
    public string GetMaterialFromTargetItem(uint oTarget)
    {
      if (NWScript.GetTag(oTarget) == workshopTag)
      {
        switch (workshopTag)
        {
          case "forge":
            return Enum.GetName(typeof(MineralType), MineralType.Tritanium);
          case "scierie":
            return Enum.GetName(typeof(PlankType), PlankType.Laurelinade);
        }
      }
      else if (NWScript.GetTag(oTarget) == this.craftedItemTag)
      {
        string material = NWScript.GetLocalString(oTarget, "_ITEM_MATERIAL");
        if (Enum.TryParse(material, out MineralType myMineralType))
          return Enum.GetName(typeof(MineralType), myMineralType + 1);
        else if (Enum.TryParse(material, out PlankType myPlankType))
          return Enum.GetName(typeof(PlankType), myPlankType + 1);
      }
      
      return "Invalid";
    }
  }
}
