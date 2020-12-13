using System;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.CollectSystem;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public class CraftJob
  {
    public JobType type;
    //public string name { get; set; }
    public int baseItemType { get; }
    public string craftedItem { get; set; }
    public float remainingTime { get; set; }
    public string material { get; set; }
    public Boolean isActive { get; set; }
    public Boolean isCancelled { get; set; }
    private readonly Player player;

    public CraftJob(int baseItemType, string material, float time, Player player, string item = "")
    {
      //this.name = name;
      this.baseItemType = baseItemType;
      this.craftedItem = item;
      this.material = material;
      this.remainingTime = time;
      this.isCancelled = false;
      this.player = player;
      this.isActive = true;

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
        case -10: // Valeur par défaut pour job inactif
          this.isActive = false;
          break;
        default:
          this.type = JobType.Item;
          break;
      }

      if(this.isActive)
        this.CreateCraftJournalEntry();
    }
    public enum JobType
    {
      Invalid = 0,
      Item = 1,
      BlueprintCopy = 2,
      BlueprintResearchMaterialEfficiency = 3,
      BlueprintResearchTimeEfficiency = 4,
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
    public Boolean CanStartJob(uint player, uint blueprint)
    {
      if (this.baseItemType < -10) // Dans le cas d'une copie une recherche de BP
      {
        if (!IsBlueprintOriginal(blueprint))
        {
          NWScript.SendMessageToPC(player, "Il vous faut un patron original afin d'effectuer une recherche ou une copie.");
          return false;
        }
      }

      if (this.isActive && !this.isCancelled)
      {
        this.AskCancellationConfirmation(player);
        return false;
      }

      return true;
    }
    private Boolean IsBlueprintOriginal(uint oBlueprint)
    {
      if (Convert.ToBoolean(NWScript.GetLocalInt(oBlueprint, "_BLUEPRINT_RUNS")))
        return false;
      else
        return true;
    }
    public void Start(JobType type, Blueprint blueprint, Player player, uint oItem, uint oTarget = 0, string sMaterial = "", MineralType mineralType = MineralType.Invalid)
    {
      switch(type)
      {
        case JobType.Item:
          this.StartItemCraft(blueprint, oItem, oTarget, sMaterial, mineralType);
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
    public void StartItemCraft(Blueprint blueprint, uint oItem, uint oTarget, string sMaterial, MineralType mineralType)
    {
      int iMineralCost = blueprint.GetBlueprintMineralCostForPlayer(player, oItem);
      float iJobDuration = blueprint.GetBlueprintTimeCostForPlayer(player, oItem);
      iMineralCost -= iMineralCost * (int)mineralType / 10;

      if (player.materialStock[sMaterial] >= iMineralCost)
      {
        player.craftJob = new CraftJob(blueprint.baseItemType, sMaterial, iJobDuration, player);
        player.materialStock[sMaterial] -= iMineralCost;

        NWScript.SendMessageToPC(player.oid, $"Vous venez de démarrer la fabrication de l'objet artisanal : {blueprint.name} en {sMaterial}");
        // TODO : afficher des effets visuels sur la forge

        if (NWScript.GetTag(oTarget) == blueprint.craftedItemTag) // En cas d'amélioration d'un objet, on détruit l'original
          NWScript.DestroyObject(oTarget);

        // s'il s'agit d'une copie de blueprint, alors le nombre d'utilisation diminue de 1
        int iBlueprintRemainingRuns = NWScript.GetLocalInt(oItem, "_BLUEPRINT_RUNS");
        if (iBlueprintRemainingRuns == 1)
          NWScript.DestroyObject(oItem);
        else
          NWScript.SetLocalInt(oItem, "_BLUEPRINT_RUNS", iBlueprintRemainingRuns - 1);

        ItemSystem.DecreaseItemDurability(NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid));
      }
      else
        NWScript.SendMessageToPC(player.oid, $"Vous n'avez pas les ressources nécessaires pour démarrer la fabrication de cet objet artisanal.");

      player.craftJob.isCancelled = false;
    }
    public void StartBlueprintCopy(Player player, uint oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint))
      {
        int value;
        if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.BlueprintCopy)), out value))
        {
          int timeCost = blueprint.mineralsCost * 80 / 100;
          float iJobDuration = timeCost - timeCost * (value * 5) / 100;
          player.craftJob = new CraftJob(-11, "", iJobDuration, player, ObjectPlugin.Serialize(oBlueprint)); // - 11 = blueprint copy
        }
      }
    }
    public void StartBlueprintMaterialEfficiencyResearch(Player player, uint oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint))
      {
        int metallurgyLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Metallurgy)), out metallurgyLevel);

        int advancedCraftLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.AdvancedCraft)), out advancedCraftLevel);

        float iJobDuration = blueprint.mineralsCost - blueprint.mineralsCost * (metallurgyLevel * 5 + advancedCraftLevel * 3) / 100;
        NWScript.SetLocalInt(oBlueprint, "_BLUEPRINT_MATERIAL_EFFICIENCY", NWScript.GetLocalInt(oBlueprint, "_BLUEPRINT_MATERIAL_EFFICIENCY") + 1);
        player.craftJob = new CraftJob(-12, "", iJobDuration, player, ObjectPlugin.Serialize(oBlueprint)); // - 12 = recherche ME
        NWScript.DestroyObject(oBlueprint);
      }
    }
    public void StartBlueprintTimeEfficiencyResearch(Player player, uint oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint))
      {
        int researchLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Research)), out researchLevel);

        int advancedCraftLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.AdvancedCraft)), out advancedCraftLevel);

        float iJobDuration = blueprint.mineralsCost - blueprint.mineralsCost * (researchLevel * 5 + advancedCraftLevel * 3) / 100;
        NWScript.SetLocalInt(oBlueprint, "_BLUEPRINT_TIME_EFFICIENCY", NWScript.GetLocalInt(oBlueprint, "_BLUEPRINT_TIME_EFFICIENCY") + 1);
        player.craftJob = new CraftJob(-13, "", iJobDuration, player, ObjectPlugin.Serialize(oBlueprint)); // -13 = recherche TE
        NWScript.DestroyObject(oBlueprint);
      }
    }
    public void CreateCraftJournalEntry()
    {
      this.player.playerJournal.craftJobCountDown = DateTime.Now.AddSeconds(this.remainingTime);
      JournalEntry journalEntry = new JournalEntry();
      journalEntry.sName = $"Travail artisanal - {Utils.StripTimeSpanMilliseconds((TimeSpan)(player.playerJournal.craftJobCountDown - DateTime.Now))}";
      switch(this.type)
      {
        case JobType.BlueprintCopy:
          journalEntry.sText = "Copie de patron en cours";
          break;
        case JobType.BlueprintResearchMaterialEfficiency:
          journalEntry.sText = "Recherche métallurgique en cours";
          break;
        case JobType.BlueprintResearchTimeEfficiency:
          journalEntry.sText = "Recherche d'efficacité en cours";
          break;
        default:
          journalEntry.sText = $"Fabrication en cours : {blueprintDictionnary[this.baseItemType].name}";
          break;
      }
      
      journalEntry.sTag = "craft_job";
      journalEntry.nPriority = 1;
      journalEntry.nQuestDisplayed = 1;
      PlayerPlugin.AddCustomJournalEntry(this.player.oid, journalEntry);
    }
    public void CancelCraftJournalEntry()
    {
      JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(player.oid, "craft_job");
      journalEntry.sName = $"Travail artisanal mis en pause - {CollectSystem.blueprintDictionnary[this.baseItemType].name}";
      journalEntry.sTag = "craft_job";
      journalEntry.nQuestDisplayed = 0;
      PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);
      player.playerJournal.craftJobCountDown = null;
    }
    public void CloseCraftJournalEntry()
    {
      JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(player.oid, "craft_job");
      journalEntry.sName = $"Travail artisanal terminé - {CollectSystem.blueprintDictionnary[this.baseItemType].name}";
      journalEntry.sTag = "craft_job";
      journalEntry.nQuestCompleted = 1;
      journalEntry.nQuestDisplayed = 0;
      PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);
      player.playerJournal.craftJobCountDown = null;
    }
  }
}
