using System;
using System.Collections.Generic;
using System.Text;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.CollectSystem;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public class Blueprint
  {
    public BlueprintType type;
    public string workshopTag { get; set; }
    public string craftedItemTag { get; set; }
    public int mineralsCost { get; set; }
    public Feat feat { get; set; }
    public Blueprint(BlueprintType type)
    {
      this.type = type;
      this.InitiateBluePrintCosts();
    }

    public void InitiateBluePrintCosts()
    {
      switch (this.type)
      {
        case BlueprintType.Longsword:
          this.mineralsCost = 15000;
          this.workshopTag = "forge";
          this.craftedItemTag = "longsword";
          this.feat = Feat.ForgeLongsword;
          break;
        case BlueprintType.Fullplate:
          this.mineralsCost = 1500000;
          this.workshopTag = "forge";
          this.craftedItemTag = "fullplate";
          this.feat = Feat.ForgeFullplate;
          break;
      }
    }
    public enum BlueprintType
    {
      Invalid = 0,
      Dagger = 1,
      Longsword = 2,
      Chainshirt = 3,
      Fullplate = 4,
    }
    public static BlueprintType GetBlueprintTypeFromName(string name)
    {
      switch (name)
      {
        case "Longsword": return BlueprintType.Longsword;
        case "Fullplate": return BlueprintType.Fullplate;
        case "Dagger": return BlueprintType.Dagger;
        case "Chainshirt": return BlueprintType.Chainshirt;
      }

      return BlueprintType.Invalid;
    }
    public static string GetNameFromBlueprintType(BlueprintType type)
    {
      switch (type)
      {
        case BlueprintType.Longsword: return "Longsword";
        case BlueprintType.Fullplate: return "Fullplate";
        case BlueprintType.Dagger: return "Dagger";
        case BlueprintType.Chainshirt: return "Chainshirt";
      }

      return "";
    }

    public static ItemProperty[] GetCraftItemProperties(MineralType material, ItemSystem.ItemCategory itemCategory)
    {
      switch (material)
      {
        case MineralType.Tritanium: return GetTritaniumItemProperties();
        case MineralType.Pyerite: return GetPyeriteItemProperties(itemCategory);
      }

      Utils.LogMessageToDMs($"No craft property found for material {material.ToString()} and item {itemCategory.ToString()}");
      
      return new ItemProperty[0];
    }
    public static void BlueprintValidation(uint oidSelf, uint oTarget, Feat feat)
    {
      Player oPC;

      if (Players.TryGetValue(oidSelf, out oPC))
      {
        if (Convert.ToBoolean(NWScript.GetIsObjectValid(oTarget)) && NWScript.GetTag(oTarget) == "blueprint")
        {
          Blueprint blueprint;
          BlueprintType blueprintType = GetBlueprintTypeFromName(NWScript.GetName(oTarget));

          if (blueprintType != BlueprintType.Invalid)
          {
            if (CollectSystem.blueprintDictionnary.ContainsKey(blueprintType))
              blueprint = CollectSystem.blueprintDictionnary[blueprintType];
            else
              blueprint = new Blueprint(blueprintType);

            blueprint.StartJob(oPC, oTarget, feat);
          }
          else
          {
            NWScript.SendMessageToPC(oidSelf, "[ERREUR HRP] - Le patron utilisé n'est pas correctement initialisé. Le bug a été remonté au staff.");
            Utils.LogMessageToDMs($"Blueprint Invalid : {NWScript.GetName(oTarget)} - Used by : {NWScript.GetName(oidSelf)}");
          }
        }
        else
          NWScript.SendMessageToPC(oidSelf, "Vous devez sélectionner un patron valide.");
      }
    }
    public static Blueprint InitializeBlueprint(uint oItem)
    {
      var item = oItem;
      Blueprint blueprint;
      BlueprintType blueprintType = GetBlueprintTypeFromName(NWScript.GetName(item));

      if (CollectSystem.blueprintDictionnary.ContainsKey(blueprintType))
        return blueprint = CollectSystem.blueprintDictionnary[blueprintType];
      else
        return blueprint = new Blueprint(blueprintType);
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
          player.craftJob.Start(CraftJob.JobType.BlueprintCopy, this, player, blueprint);
          break;
        case Feat.Research:
        case Feat.Research2:
        case Feat.Research3:
        case Feat.Research4:
        case Feat.Research5:

          break;
        case Feat.Metallurgy:
        case Feat.Metallurgy2:
        case Feat.Metallurgy3:
        case Feat.Metallurgy4:
        case Feat.Metallurgy5:

          break;
      }
    }
    public void DisplayBlueprintInfo(Player player, uint oItem)
    {
      int iMineralCost = this.GetBlueprintMineralCostForPlayer(player, oItem);
      float iJobDuration = this.GetBlueprintTimeCostForPlayer(player, oItem);

      NWScript.SendMessageToPC(player.oid, $"Patron de création de l'objet artisanal : {this.type}");
      NWScript.SendMessageToPC(player.oid, $"Recherche d'efficacité matérielle niveau {NWScript.GetLocalInt(oItem, "_BLUEPRINT_MATERIAL_EFFICIENCY")}");
      NWScript.SendMessageToPC(player.oid, $"Coût initial en Tritanium : {iMineralCost}. Puis 10 % de moins par amélioration vers un matériau supérieur.");
      NWScript.SendMessageToPC(player.oid, $"Recherche d'efficacité de production niveau {NWScript.GetLocalInt(oItem, "_BLUEPRINT_TIME_EFFICIENCY")}");
      NWScript.SendMessageToPC(player.oid, $"Temps de fabrication et d'amélioration : {Utils.StripTimeSpanMilliseconds(DateTime.Now.AddSeconds(iJobDuration).Subtract(DateTime.Now))}.");
    }
    public int GetBlueprintMineralCostForPlayer(PlayerSystem.Player player, uint item)
    {
      int iSkillLevel = 1;

      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Forge)), out value))
        iSkillLevel += value;

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)this.feat)), out value))
        iSkillLevel += value;

      return this.mineralsCost - (this.mineralsCost * (iSkillLevel + NWScript.GetLocalInt(item, "_BLUEPRINT_MATERIAL_EFFICIENCY")) / 100);
    }
    public float GetBlueprintTimeCostForPlayer(PlayerSystem.Player player, uint item)
    {
      int iSkillLevel = 1;
      float fJobDuration = this.mineralsCost;

      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Forge)), out value))
        iSkillLevel += value;

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)this.feat)), out value))
        iSkillLevel += value;

      return fJobDuration - (fJobDuration * (iSkillLevel + NWScript.GetLocalInt(item, "_BLUEPRINT_TIME_EFFICIENCY")) / 100);
    }
    public string GetMaterialFromTargetItem(uint oTarget)
    {
      if (NWScript.GetTag(oTarget) == this.workshopTag)
        return "Tritanium";
      else 
        return NWScript.GetLocalString(oTarget, "_ITEM_MATERIAL");
    }
  }
}
