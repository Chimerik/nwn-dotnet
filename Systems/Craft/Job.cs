using System;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
using NWNX.API;
using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.Craft.Collect.System;

namespace NWN.Systems.Craft
{
  public class Job
  {
    public JobType type;
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
        case -15:
          this.type = JobType.Recycling;
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
      Recycling = 6,
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
    public void AskCancellationConfirmation(NwPlayer player)
    {
      player.SendServerMessage($"Attention, votre travail précédent n'est pas terminé. Lancer un nouveau travail signifie perdre la totalité du travail en cours !", Color.MAGENTA);
      player.SendServerMessage($"Utilisez une seconde fois le plan pour confirmer l'annulation du travail en cours.", Color.SILVER);
      this.isCancelled = true;

      Task waitSpellUsed = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(60));
        ResetCancellation();
      });
    }
    public Boolean CanStartJob(NwPlayer player, NwItem blueprint, JobType type)
    {
      if (blueprint != null)
      {
        if (type == JobType.BlueprintCopy || type == JobType.BlueprintResearchMaterialEfficiency || type == JobType.BlueprintResearchTimeEfficiency) // Dans le cas d'une copie ou d'une recherche de BP
        {
          if (!IsBlueprintOriginal(blueprint))
          {
            player.SendServerMessage("Il vous faut un patron original afin d'effectuer une recherche ou une copie.", Color.ORANGE);
            return false;
          }
        }

        switch (type)
        {
          case JobType.BlueprintResearchTimeEfficiency:
            if (blueprint.GetLocalVariable<int>("_BLUEPRINT_TIME_EFFICIENCY").Value >= 10)
            {
              player.SendServerMessage("Ce patron dispose déjà d'un niveau de recherche maximal.", Color.ORANGE);
              return false;
            }
            break;
          case JobType.BlueprintResearchMaterialEfficiency:
            if (blueprint.GetLocalVariable<int>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value >= 10)
            {
              player.SendServerMessage("Ce patron dispose déjà d'un niveau de recherche métallurgique maximal.", Color.ORANGE);
              return false;
            }
            break;
        }
      }

      if (IsActive() && !isCancelled)
      {
        AskCancellationConfirmation(player);
        return false;
      }

      if (this.isCancelled)
      {
        player.AcquireItem(NwGameObject.Deserialize<NwItem>(craftedItem));
      }

      return true;
    }
    private Boolean IsBlueprintOriginal(NwItem oBlueprint)
    {
      if (oBlueprint.GetLocalVariable<int>("_BLUEPRINT_RUNS").Value > 0)
        return false;
      else
        return true;
    }
    public void Start(JobType type, Blueprint blueprint, PlayerSystem.Player player, NwItem oItem, NwGameObject oTarget = null, string sMaterial = "")
    {
      switch (type)
      {
        case JobType.Item:
          StartItemCraft(blueprint, oItem, oTarget, sMaterial);
          break;
        case JobType.Enchantement:
          StartEnchantementCraft(oTarget, sMaterial);
          break;
        case JobType.Recycling:
          StartRecycleCraft(oTarget, sMaterial);
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
    public void StartItemCraft(Blueprint blueprint, NwItem oItem, NwGameObject oTarget, string sMaterial)
    {
      int iMineralCost = blueprint.GetBlueprintMineralCostForPlayer(player.oid, oItem);
      float iJobDuration = blueprint.GetBlueprintTimeCostForPlayer(player.oid, oItem);

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

        player.oid.SendServerMessage($"Vous venez de démarrer la fabrication de l'objet artisanal : {blueprint.name.ColorString(Color.WHITE)} en {sMaterial.ColorString(Color.WHITE)}", Color.GREEN);
        // TODO : afficher des effets visuels sur la forge

        if (oTarget.Tag == blueprint.craftedItemTag) // En cas d'amélioration d'un objet, on détruit l'original
        {
          oTarget.Destroy();
          player.oid.SendServerMessage($"L'objet {oTarget.Name.ColorString(Color.WHITE)} ne sera pas disponible jusqu'à la fin du travail artisanal.", Color.ORANGE);
        }

        // s'il s'agit d'une copie de blueprint, alors le nombre d'utilisation diminue de 1
        int iBlueprintRemainingRuns = oItem.GetLocalVariable<int>("_BLUEPRINT_RUNS").Value;
        if (iBlueprintRemainingRuns == 1)
          oItem.Destroy();
        if (iBlueprintRemainingRuns > 0)
          oItem.GetLocalVariable<int>("_BLUEPRINT_RUNS").Value -= 1;

        ItemUtils.DecreaseItemDurability(player.oid.GetItemInSlot(InventorySlot.RightHand));
      }
      else
        player.oid.SendServerMessage("Vous n'avez pas les ressources nécessaires pour démarrer la fabrication de cet objet artisanal.", Color.RED);

      player.craftJob.isCancelled = false;
    }
    public void StartBlueprintCopy(PlayerSystem.Player player, NwItem oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint, JobType.BlueprintCopy))
      {
        int value;
        if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.BlueprintCopy)), out value))
        {
          int timeCost = blueprint.mineralsCost * 80;
          float iJobDuration = timeCost - timeCost * (value * 5) / 100;
          player.craftJob = new Job(-11, "", iJobDuration, player, oBlueprint.Serialize()); // - 11 = blueprint copy
        }
      }
    }
    private void StartEnchantementCraft(NwGameObject oTarget, string ipString)
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
      int baseCost = ItemUtils.GetBaseItemCost((NwItem)oTarget);

      Log.Info($"base item type job : {baseItemType}");

      float iJobDuration = baseCost * spellLevel * 100;
      Log.Info($"duration : {iJobDuration}");
      player.craftJob = new Job(-14, ipString, iJobDuration, player, oTarget.Serialize()); // -14 = JobType enchantement

      player.oid.SendServerMessage($"Vous venez de démarrer l'enchantement de : {NWScript.GetName(oTarget).ColorString(Color.WHITE)}", Color.GREEN);
      // TODO : afficher des effets visuels

      oTarget.Destroy();
      player.oid.SendServerMessage($"L'objet {oTarget.Name.ColorString(Color.WHITE)} ne sera pas disponible jusqu'à la fin du travail d'enchantement.", Color.ORANGE);

      player.craftJob.isCancelled = false;
    }
    private void StartRecycleCraft(NwGameObject oTarget, string material)
    {
      NwItem item = (NwItem)oTarget;

      Log.Info($"{player.oid.Name} starts recycling {item.Name} - material : {material}");

      int baseItemType = (int)item.BaseItemType;
      int baseCost = ItemUtils.GetBaseItemCost(item);

      Log.Info($"base item type job : {baseItemType}");

      float iJobDuration = baseCost * 25;
      Log.Info($"duration : {iJobDuration}");

      item.GetLocalVariable<int>("_BASE_COST").Value = baseCost;
      player.craftJob = new Job(-15, material, iJobDuration, player, item.Serialize()); // -15 = JobType recyclage

      player.oid.SendServerMessage($"Vous venez de démarrer l'enchantement de : {item.Name.ColorString(Color.WHITE)}", Color.ORANGE);
      // TODO : afficher des effets visuels

      item.Destroy();
      player.oid.SendServerMessage($"L'objet {item.Name.ColorString(Color.WHITE)} ne sera pas disponible jusqu'à la fin du travail d'enchantement.", Color.RED);

      player.craftJob.isCancelled = false;
    }
    public void StartBlueprintMaterialEfficiencyResearch(PlayerSystem.Player player, NwItem oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint, JobType.BlueprintResearchMaterialEfficiency))
      {
        int metallurgyLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Metallurgy)), out metallurgyLevel);

        int advancedCraftLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.AdvancedCraft)), out advancedCraftLevel);

        float iJobDuration = blueprint.mineralsCost * 100 - blueprint.mineralsCost * (metallurgyLevel * 5 + advancedCraftLevel * 3) / 100;
        player.craftJob = new Job(-12, "", iJobDuration, player, ObjectPlugin.Serialize(oBlueprint)); // - 12 = recherche ME
        oBlueprint.Destroy();
        player.oid.SendServerMessage($"L'objet {oBlueprint.Name.ColorString(Color.WHITE)} ne sera pas disponible jusqu'à la fin du travail de recherche métallurgique.", Color.ORANGE);
      }
    }
    public void StartBlueprintTimeEfficiencyResearch(PlayerSystem.Player player, NwItem oBlueprint, Blueprint blueprint)
    {
      if (player.craftJob.CanStartJob(player.oid, oBlueprint, JobType.BlueprintResearchTimeEfficiency))
      {
        int researchLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.Research)), out researchLevel);

        int advancedCraftLevel = 0;
        int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.AdvancedCraft)), out advancedCraftLevel);

        float iJobDuration = blueprint.mineralsCost * 100 - blueprint.mineralsCost * (researchLevel * 5 + advancedCraftLevel * 3) / 100;
        player.craftJob = new Job(-13, "", iJobDuration, player, ObjectPlugin.Serialize(oBlueprint)); // -13 = recherche TE
        oBlueprint.Destroy();
        player.oid.SendServerMessage($"L'objet {oBlueprint.Name.ColorString(Color.WHITE)} ne sera pas disponible jusqu'à la fin du travail de recherche d'efficacité.", Color.ORANGE);
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
          journalEntry.sText = $"Copie de {NwItem.Deserialize<NwItem>(craftedItem).Name.ColorString(Color.ORANGE)} en cours";
          break;
        case JobType.BlueprintResearchMaterialEfficiency:
          journalEntry.sText = $"Recherche métallurgique en cours : {NwItem.Deserialize<NwItem>(craftedItem).Name.ColorString(Color.ORANGE)}";
          break;
        case JobType.BlueprintResearchTimeEfficiency:
          journalEntry.sText = $"Recherche d'efficacité en cours : {NwItem.Deserialize<NwItem>(craftedItem).Name.ColorString(Color.ORANGE)}";
          break;
        case JobType.Enchantement:
          journalEntry.sText = $"Enchantement en cours : {NwItem.Deserialize<NwItem>(craftedItem).Name.ColorString(Color.ORANGE)}";
          break;
        case JobType.Recycling:
          journalEntry.sText = $"Recyclage en cours : {NwItem.Deserialize<NwItem>(craftedItem).Name.ColorString(Color.ORANGE)}";
          break;
        default:
          journalEntry.sText = $"Fabrication en cours : {blueprintDictionnary[baseItemType].name.ColorString(Color.ORANGE)}";
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
        case JobType.Recycling:
          journalEntry.sText = $"Recyclage en pause";
          break;
        default:
          journalEntry.sName = $"Travail artisanal en pause - {blueprintDictionnary[baseItemType].name.ColorString(Color.ORANGE)}";
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
          journalEntry.sName = $"Enchantement terminé - {NwItem.Deserialize<NwItem>(craftedItem).Name.ColorString(Color.ORANGE)}";
          break;
        case JobType.Recycling:
          journalEntry.sName = $"Recyclage terminé - {NwItem.Deserialize<NwItem>(craftedItem).Name.ColorString(Color.ORANGE)}";
          break;
        default:
          journalEntry.sName = $"Travail artisanal terminé - {blueprintDictionnary[baseItemType].name.ColorString(Color.ORANGE)}";
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
