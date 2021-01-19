using System;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.Items.Utils;
using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.Craft.Collect.System;

namespace NWN.Systems.Craft
{
  public class Job
  {
    public JobType type;
    //public string name { get; set; }
    public int baseItemType { get; }
    public string craftedItem { get; set; }
    public float remainingTime { get; set; }
    public string material { get; set; }
    public Boolean isCancelled { get; set; }
    private readonly Player player;

    public Job(int baseItemType, string material, float time, Player player, string item = "")
    {
      //this.name = name;
      this.baseItemType = baseItemType;
      this.craftedItem = item;
      this.material = material;
      this.remainingTime = time;
      this.isCancelled = false;
      this.player = player;

      switch (baseItemType)
      {
        case -11:
          this.type = JobType.BlueprintCopy;
          break;
        case -12:
          this.type = JobType.BlueprintResearchMaterialEfficiency;
          break;
        case -13:
          this.type = JobType.BlueprintResearchTimeEfficiency;
          break;
        default:
          this.type = JobType.Item;
          break;
      }

      if (IsActive())
      {
        HandleBeforePlayerSave(player.oid);
        HandleAfterPlayerSave(player.oid);
        this.CreateCraftJournalEntry();
      }
    }
    public enum JobType
    {
      Invalid = 0,
      Item = 1,
      BlueprintCopy = 2,
      BlueprintResearchMaterialEfficiency = 3,
      BlueprintResearchTimeEfficiency = 4,
    }
    public Boolean IsActive()
    {
      if (baseItemType == -10)
        return false;
      return true;
    }
    public void ResetCancellation()
    {
      this.isCancelled = false;
    }
    public void AskCancellationConfirmation(uint player)
    {
      NWScript.SendMessageToPC(player, $"Attention, votre travail précédent n'est pas terminé. Lancer un nouveau travail signifie perdre la totalité du travail en cours !");
      NWScript.SendMessageToPC(player, $"Utilisez une seconde fois le plan pour confirmer l'annulation du travail en cours.");
      this.isCancelled = true;
      NWScript.DelayCommand(60.0f, () => this.ResetCancellation());
    }
    public Boolean CanStartJob(uint player, uint blueprint, JobType type)
    {    
      if ((int)type > 1) // Dans le cas d'une copie ou d'une recherche de BP
      {
        if (!IsBlueprintOriginal(blueprint))
        {
          NWScript.SendMessageToPC(player, "Il vous faut un patron original afin d'effectuer une recherche ou une copie.");
          return false;
        }
      }

      switch(type)
      {
        case JobType.BlueprintResearchTimeEfficiency:
          if(NWScript.GetLocalInt(blueprint, "_BLUEPRINT_TIME_EFFICIENCY") >= 10)
          {
            NWScript.SendMessageToPC(player, "Ce patron dispose déjà d'un niveau de recherche maximal.");
            return false;
          }
          break; 
        case JobType.BlueprintResearchMaterialEfficiency:
          if (NWScript.GetLocalInt(blueprint, "_BLUEPRINT_MATERIAL_EFFICIENCY") >= 10)
          {
            NWScript.SendMessageToPC(player, "Ce patron dispose déjà d'un niveau de recherche métallurgique maximal.");
            return false;
          }
          break;
      }

      if (IsActive() && !isCancelled)
      {
        AskCancellationConfirmation(player);
        return false;
      }

      if(this.isCancelled)
      {
        ObjectPlugin.AcquireItem(player, ObjectPlugin.Deserialize(this.craftedItem));
      }

      return true;
    }
    private Boolean IsBlueprintOriginal(uint oBlueprint)
    {
      if (NWScript.GetLocalInt(oBlueprint, "_BLUEPRINT_RUNS") > 0)
        return false;
      else
        return true;
    }
    public void Start(JobType type, Blueprint blueprint, Player player, uint oItem, uint oTarget = 0, string sMaterial = "")
    {
      switch(type)
      {
        case JobType.Item:
          StartItemCraft(blueprint, oItem, oTarget, sMaterial);
          break;
        case JobType.BlueprintCopy:
          StartBlueprintCopy(player, oItem, blueprint);
          break;
        case JobType.BlueprintResearchTimeEfficiency:
          StartBlueprintTimeEfficiencyResearch(player, oItem, blueprint);
          break;
        case JobType.BlueprintResearchMaterialEfficiency:
          StartBlueprintMaterialEfficiencyResearch(player, oItem, blueprint);
          break;
      }

    }
    public void StartItemCraft(Blueprint blueprint, uint oItem, uint oTarget, string sMaterial)
    {
      int iMineralCost = blueprint.GetBlueprintMineralCostForPlayer(player, oItem);
      float iJobDuration = blueprint.GetBlueprintTimeCostForPlayer(player, oItem);

      int materialType = 0;
      if (Enum.TryParse(material, out MineralType myMineralType))
        materialType = (int)myMineralType;
      else if (Enum.TryParse(material, out PlankType myPlankType))
        materialType = (int)myPlankType;

      iMineralCost -= iMineralCost * (int)materialType / 10;

      if (player.materialStock[sMaterial] >= iMineralCost)
      {
        player.craftJob = new Job(blueprint.baseItemType, sMaterial, iJobDuration, player);
        player.materialStock[sMaterial] -= iMineralCost;

        NWScript.SendMessageToPC(player.oid, $"Vous venez de démarrer la fabrication de l'objet artisanal : {blueprint.name} en {sMaterial}");
        // TODO : afficher des effets visuels sur la forge

        if (NWScript.GetTag(oTarget) == blueprint.craftedItemTag) // En cas d'amélioration d'un objet, on détruit l'original
        {
          NWScript.DestroyObject(oTarget);
          NWScript.SendMessageToPC(player.oid, $"L'objet {NWScript.GetName(oTarget)} ne sera pas disponible jusqu'à la fin du travail artisanal.");
        }

        // s'il s'agit d'une copie de blueprint, alors le nombre d'utilisation diminue de 1
        int iBlueprintRemainingRuns = NWScript.GetLocalInt(oItem, "_BLUEPRINT_RUNS");
        if (iBlueprintRemainingRuns == 1)
          NWScript.DestroyObject(oItem);
        if (iBlueprintRemainingRuns > 0)
          NWScript.SetLocalInt(oItem, "_BLUEPRINT_RUNS", iBlueprintRemainingRuns - 1);
        
        DecreaseItemDurability(NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid));
      }
      else
        NWScript.SendMessageToPC(player.oid, $"Vous n'avez pas les ressources nécessaires pour démarrer la fabrication de cet objet artisanal.");

      player.craftJob.isCancelled = false;
    }
    public void StartBlueprintCopy(Player player, uint oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint, JobType.BlueprintCopy))
      {
        int value;
        if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.BlueprintCopy)), out value))
        {
          int timeCost = blueprint.mineralsCost * 80 / 100;
          float iJobDuration = timeCost - timeCost * (value * 5) / 100;
          player.craftJob = new Job(-11, "", iJobDuration, player, ObjectPlugin.Serialize(oBlueprint)); // - 11 = blueprint copy
        }
      }
    }
    public void StartBlueprintMaterialEfficiencyResearch(Player player, uint oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint, JobType.BlueprintResearchMaterialEfficiency))
      {
        int metallurgyLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Metallurgy)), out metallurgyLevel);

        int advancedCraftLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.AdvancedCraft)), out advancedCraftLevel);

        float iJobDuration = blueprint.mineralsCost - blueprint.mineralsCost * (metallurgyLevel * 5 + advancedCraftLevel * 3) / 100;
        player.craftJob = new Job(-12, "", iJobDuration, player, ObjectPlugin.Serialize(oBlueprint)); // - 12 = recherche ME
        NWScript.DestroyObject(oBlueprint);
        NWScript.SendMessageToPC(player.oid, $"L'objet {NWScript.GetName(oBlueprint)} ne sera pas disponible jusqu'à la fin du travail de recherche métallurgique.");
      }
    }
    public void StartBlueprintTimeEfficiencyResearch(Player player, uint oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint, JobType.BlueprintResearchTimeEfficiency))
      {
        int researchLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Research)), out researchLevel);

        int advancedCraftLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.AdvancedCraft)), out advancedCraftLevel);

        float iJobDuration = blueprint.mineralsCost - blueprint.mineralsCost * (researchLevel * 5 + advancedCraftLevel * 3) / 100;
        player.craftJob = new Job(-13, "", iJobDuration, player, ObjectPlugin.Serialize(oBlueprint)); // -13 = recherche TE
        NWScript.DestroyObject(oBlueprint);
        NWScript.SendMessageToPC(player.oid, $"L'objet {NWScript.GetName(oBlueprint)} ne sera pas disponible jusqu'à la fin du travail de recherche d'efficacité.");
      }
    }
    public void CreateCraftJournalEntry()
    {
      if (!IsActive())
        return;

      this.player.playerJournal.craftJobCountDown = DateTime.Now.AddSeconds(this.remainingTime);
      JournalEntry journalEntry = new JournalEntry();
      journalEntry.sName = $"Travail artisanal - {Utils.StripTimeSpanMilliseconds((TimeSpan)(player.playerJournal.craftJobCountDown - DateTime.Now))}";

      switch (this.type)
      {
        case JobType.BlueprintCopy:
          journalEntry.sText = $"Copie de {NWScript.GetName(ObjectPlugin.Deserialize(craftedItem))} en cours";
          break;
        case JobType.BlueprintResearchMaterialEfficiency:
          journalEntry.sText = $"Recherche métallurgique en cours : {NWScript.GetName(ObjectPlugin.Deserialize(craftedItem))}";
          break;
        case JobType.BlueprintResearchTimeEfficiency:
          journalEntry.sText = $"Recherche d'efficacité en cours : {NWScript.GetName(ObjectPlugin.Deserialize(craftedItem))}";
          break;
        default:
          journalEntry.sText = $"Fabrication en cours : {blueprintDictionnary[baseItemType].name}";
          break;
      }
      
      journalEntry.sTag = "craft_job";
      journalEntry.nPriority = 1;
      journalEntry.nQuestDisplayed = 1;
      PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);
    }
    public void CancelCraftJournalEntry()
    {
      JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(player.oid, "craft_job");

      switch (type)
      {
        case JobType.BlueprintCopy:
          journalEntry.sName = $"Travail artisanal en pause - Copie de patron";
          break;
        case JobType.BlueprintResearchMaterialEfficiency:
          journalEntry.sName = $"Travail artisanal en pause - Recherche métallurgique";
          break;
        case JobType.BlueprintResearchTimeEfficiency:
          journalEntry.sName = $"Travail artisanal en pause - Recherche en efficacité";
          break;
        default:
          journalEntry.sName = $"Travail artisanal en pause - {blueprintDictionnary[baseItemType].name}";
          break;
      }

      journalEntry.sTag = "craft_job";
      journalEntry.nQuestDisplayed = 0;
      PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);
      player.playerJournal.craftJobCountDown = null;
    }
    public void CloseCraftJournalEntry()
    {
      JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(player.oid, "craft_job");

      switch (type)
      {
        case JobType.BlueprintCopy:
          journalEntry.sName = $"Travail artisanal terminé - Copie de patron";
          break;
        case JobType.BlueprintResearchMaterialEfficiency:
          journalEntry.sName = $"Travail artisanal terminé - Recherche métallurgique";
          break;
        case JobType.BlueprintResearchTimeEfficiency:
          journalEntry.sName = $"Travail artisanal terminé - Recherche en efficacité";
          break;
        default:
          journalEntry.sName = $"Travail artisanal terminé - {blueprintDictionnary[baseItemType].name}";
          break;
      }
      
      journalEntry.sTag = "craft_job";
      journalEntry.nQuestCompleted = 1;
      journalEntry.nQuestDisplayed = 0;
      PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);
      player.playerJournal.craftJobCountDown = null;
    }
  }
}
