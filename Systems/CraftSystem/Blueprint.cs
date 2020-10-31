using System;
using System.Collections.Generic;
using System.Text;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.CollectSystem;

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
      
      return null;
    }
    public void StartCopyJob(PlayerSystem.Player player, uint oBlueprint)
    {
      if(Convert.ToBoolean(NWScript.GetLocalInt(oBlueprint, "_BLUEPRINT_RUNS")))
      {
        NWScript.SendMessageToPC(player.oid, "Il vous faut un patron original afin d'effectuer une copie.");
        return;
      }

      if (player.currentCraftJobRemainingTime < 600.0f)
      {
        NWScript.SendMessageToPC(player.oid, $"Un job est déjà en cours. Impossible d'annuler un travail en cours si près de la fin !");
        return;
      }

      if (player.currentCraftJob != "" && !player.craftCancellationConfirmation)
      {
        NWScript.SendMessageToPC(player.oid, $"Attention, votre travail sur l'objet {player.currentCraftJob} n'est pas terminé. Lancer un nouveau travail signifie perdre la totalité du travail en cours !");
        NWScript.SendMessageToPC(player.oid, $"Utilisez une seconde fois le plan pour confirmer l'annulation du travail en cours.");
        player.craftCancellationConfirmation = true;
        NWScript.DelayCommand(60.0f, () => player.ResetCancellationConfirmation());
        return;
      }

      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.BlueprintCopy)), out value))
      {
        player.currentCraftJob = "blueprint";
        player.currentCraftObject = NWScript.ObjectToString(oBlueprint);
        player.currentCraftJobRemainingTime = this.mineralsCost / 4 - this.mineralsCost * value / 100;
        player.currentCraftJobMaterial = "";
      }
    }
  }
}
