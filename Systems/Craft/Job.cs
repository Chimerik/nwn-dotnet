using System;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.Craft.Collect.System;
using NWN.API;

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
    private readonly PlayerSystem.Player player;

    public Job(int baseItemType, string material, float time, PlayerSystem.Player player, string item = "")
    {
      //this.name = name;
      this.baseItemType = baseItemType;
      ModuleSystem.Log.Info($"got base item : {baseItemType}");
      this.craftedItem = item;
      this.material = material;
      if (Config.env == Config.Env.Chim)
        this.remainingTime = 10.0f;
      else
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
        case -14:
          this.type = JobType.Enchantement;
          break;
        default:
          this.type = JobType.Item;
          break;
      }

      if (IsActive())
      {
        player.oid.ExportCharacter();
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
      Enchantement = 5,
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
      if (type == JobType.BlueprintCopy || type == JobType.BlueprintResearchMaterialEfficiency || type == JobType.BlueprintResearchTimeEfficiency) // Dans le cas d'une copie ou d'une recherche de BP
      {
        if (!IsBlueprintOriginal(blueprint))
        {
          NWScript.SendMessageToPC(player, "Il vous faut un patron original afin d'effectuer une recherche ou une copie.");
          return false;
        }
      }

      switch (type)
      {
        case JobType.BlueprintResearchTimeEfficiency:
          if (NWScript.GetLocalInt(blueprint, "_BLUEPRINT_TIME_EFFICIENCY") >= 10)
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

      if (this.isCancelled)
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
    public void Start(JobType type, Blueprint blueprint, PlayerSystem.Player player, uint oItem, uint oTarget = 0, string sMaterial = "")
    {
      switch (type)
      {
        case JobType.Item:
          StartItemCraft(blueprint, oItem, oTarget, sMaterial);
          break;
        case JobType.Enchantement:
          StartEnchantementCraft(oTarget, sMaterial);
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
      int iMineralCost = blueprint.GetBlueprintMineralCostForPlayer(player.oid, oItem.ToNwObject<NwItem>());
      float iJobDuration = blueprint.GetBlueprintTimeCostForPlayer(player.oid, oItem.ToNwObject<NwItem>());

      int materialType = 0;
      if (Enum.TryParse(material, out MineralType myMineralType))
        materialType = (int)myMineralType;
      else if (Enum.TryParse(material, out PlankType myPlankType))
        materialType = (int)myPlankType;
      else if (Enum.TryParse(material, out LeatherType myLeatherType))
        materialType = (int)myLeatherType;

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

        ItemUtils.DecreaseItemDurability(NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid));
      }
      else
        NWScript.SendMessageToPC(player.oid, $"Vous n'avez pas les ressources nécessaires pour démarrer la fabrication de cet objet artisanal.");

      player.craftJob.isCancelled = false;
    }
    public void StartBlueprintCopy(PlayerSystem.Player player, uint oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint, JobType.BlueprintCopy))
      {
        int value;
        if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.BlueprintCopy)), out value))
        {
          int timeCost = blueprint.mineralsCost * 80;
          float iJobDuration = timeCost - timeCost * (value * 5) / 100;
          player.craftJob = new Job(-11, "", iJobDuration, player, ObjectPlugin.Serialize(oBlueprint)); // - 11 = blueprint copy
        }
      }
    }
    public void StartEnchantementCraft(uint oTarget, string ipString)
    {
      Log.Info("start ench craft");

      int spellId = Int32.Parse(ipString.Remove(ipString.IndexOf("_")));
      ipString = ipString.Remove(0, ipString.IndexOf("_") + 1);
      Log.Info($"ipString : {ipString}");

      if (!int.TryParse(NWScript.Get2DAString("spells", "Wiz_Sorc", spellId), out int spellLevel))
      {
        player.oid.SendServerMessage("HRP - Le niveau de sort de votre enchantement est incorrectement configuré. Le staff a été prévenu !");
        Utils.LogMessageToDMs($"ENCHANTEMENT - {player.oid.Name} - spell level introuvable pour spellid : {spellId}");
        return;
      }

      int baseItemType = NWScript.GetBaseItemType(oTarget);
      int baseCost = 9999;

      Log.Info($"base item type job : {baseItemType}");

      if (baseItemType == NWScript.BASE_ITEM_ARMOR)
      {
        if (!int.TryParse(NWScript.Get2DAString("armor", "COST", ItemPlugin.GetBaseArmorClass(oTarget)), out baseCost))
        {
          player.oid.SendServerMessage("HRP - Le coût de base de l'object à enchanter est incorrectement configuré. Le staff a été prévenu !");
          Utils.LogMessageToDMs($"ENCHANTEMENT - {player.oid.Name} - baseCost introuvable pour baseItemType : {baseItemType}");
          return;
        }
      }
      else
      {
        if (!int.TryParse(NWScript.Get2DAString("baseitems", "BaseCost", baseItemType), out baseCost))
        {
          player.oid.SendServerMessage("HRP - Le coût de base de l'object à enchanter est incorrectement configuré. Le staff a été prévenu !");
          Utils.LogMessageToDMs($"ENCHANTEMENT - {player.oid.Name} - baseCost introuvable pour baseItemType : {baseItemType}");
          return;
        }
      }

      float iJobDuration = baseCost * spellLevel * 100;
      Log.Info($"duration : {iJobDuration}");
      player.craftJob = new Job(-14, ipString, iJobDuration, player, ObjectPlugin.Serialize(oTarget)); // -14 = JobType enchantement

      NWScript.SendMessageToPC(player.oid, $"Vous venez de démarrer l'enchantement de : {NWScript.GetName(oTarget)}");
      // TODO : afficher des effets visuels

      NWScript.DestroyObject(oTarget);
      NWScript.SendMessageToPC(player.oid, $"L'objet {NWScript.GetName(oTarget)} ne sera pas disponible jusqu'à la fin du travail d'enchantement.");

      player.craftJob.isCancelled = false;
    }
    public void StartBlueprintMaterialEfficiencyResearch(PlayerSystem.Player player, uint oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint, JobType.BlueprintResearchMaterialEfficiency))
      {
        int metallurgyLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Metallurgy)), out metallurgyLevel);

        int advancedCraftLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.AdvancedCraft)), out advancedCraftLevel);

        float iJobDuration = blueprint.mineralsCost * 100 - blueprint.mineralsCost * (metallurgyLevel * 5 + advancedCraftLevel * 3) / 100;
        player.craftJob = new Job(-12, "", iJobDuration, player, ObjectPlugin.Serialize(oBlueprint)); // - 12 = recherche ME
        NWScript.DestroyObject(oBlueprint);
        NWScript.SendMessageToPC(player.oid, $"L'objet {NWScript.GetName(oBlueprint)} ne sera pas disponible jusqu'à la fin du travail de recherche métallurgique.");
      }
    }
    public void StartBlueprintTimeEfficiencyResearch(PlayerSystem.Player player, uint oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint, JobType.BlueprintResearchTimeEfficiency))
      {
        int researchLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Research)), out researchLevel);

        int advancedCraftLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.AdvancedCraft)), out advancedCraftLevel);

        float iJobDuration = blueprint.mineralsCost * 100 - blueprint.mineralsCost * (researchLevel * 5 + advancedCraftLevel * 3) / 100;
        player.craftJob = new Job(-13, "", iJobDuration, player, ObjectPlugin.Serialize(oBlueprint)); // -13 = recherche TE
        NWScript.DestroyObject(oBlueprint);
        NWScript.SendMessageToPC(player.oid, $"L'objet {NWScript.GetName(oBlueprint)} ne sera pas disponible jusqu'à la fin du travail de recherche d'efficacité.");
      }
    }
    public void CreateCraftJournalEntry()
    {
      if (!IsActive())
        return;

      player.playerJournal.craftJobCountDown = DateTime.Now.AddSeconds(remainingTime);
      JournalEntry journalEntry = new JournalEntry();
      if(remainingTime > 0)
        journalEntry.sName = $"Travail artisanal - {Utils.StripTimeSpanMilliseconds((TimeSpan)(player.playerJournal.craftJobCountDown - DateTime.Now))}";
      else
        journalEntry.sName = $"Travail artisanal - Terminé !";

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
        case JobType.Enchantement:
          journalEntry.sText = $"Enchantement en cours : {NWScript.GetName(ObjectPlugin.Deserialize(craftedItem))}";
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
        case JobType.Enchantement:
          journalEntry.sText = $"Enchantement en pause";
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
        case JobType.Enchantement:
          journalEntry.sName = $"Enchantement terminé - {NWScript.GetName(ObjectPlugin.Deserialize(craftedItem))}";
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
