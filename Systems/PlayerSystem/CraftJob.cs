using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static Dictionary<JobType, Func<Player, bool, bool>> HandleSpecificJobCompletion = new Dictionary<JobType, Func<Player, bool, bool>>
    {
      { JobType.ItemCreation, CompleteItemCreation },
      { JobType.ItemUpgrade, CompleteItemUpgrade },
      { JobType.Renforcement, CompleteItemReinforcement },
      { JobType.Recycling, CompleteItemRecycling },
      { JobType.Repair, CompleteItemRepair },
      { JobType.BlueprintCopy, CompleteBlueprintCopy },
      { JobType.BlueprintResearchMaterialEfficiency, CompleteBlueprintMaterialResearch },
      { JobType.BlueprintResearchTimeEfficiency, CompleteBlueprintTimeResearch },
      { JobType.Enchantement, CompleteItemEnchantement },
    };

  public enum JobType
    {
      [Description("Invalide")]
      Invalid = 0,
      [Description("Création_artisanale")]
      ItemCreation = 1,
      [Description("Amélioration_artisanale")]
      ItemUpgrade = 2,
      [Description("Copie_de_patron")]
      BlueprintCopy = 3,
      [Description("Recherche_en_rendement")]
      BlueprintResearchMaterialEfficiency = 4,
      [Description("Recherche_en_efficacité")]
      BlueprintResearchTimeEfficiency = 5,
      [Description("Enchantement")]
      Enchantement = 6,
      [Description("Recyclage")]
      Recycling = 7,
      [Description("Renforcement")]
      Renforcement = 8,
      [Description("Réparations")]
      Repair = 9,
      [Description("Réactivation_d'_enchantement")]
      EnchantementReactivation = 10,
      [Description("Alchimie")]
      Alchemy = 11,
    }

    public class CraftJob
    {
      public readonly JobType type;
      public readonly string icon;
      public double remainingTime { get; set; }
      public string originalSerializedItem { get; set; }
      public string serializedCraftedItem { get; set; }
      public string enchantementTag { get; set; }
      public DateTime? progressLastCalculation { get; set; }
      public ScheduledTask jobProgression { get; set; }
      public CraftJob(Player player, NwItem oBlueprint, double jobDuration) // Item Craft
      {
        try
        {
          // TODO : gérer la durabilité de l'outil de craft

          // s'il s'agit d'une copie de blueprint, alors le nombre d'utilisation diminue de 1
          if (oBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").HasValue)
          {
            oBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value -= 1;

            if (oBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value < 1)
              oBlueprint.Destroy();
          }

          int baseItemType = oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;
          icon = NwBaseItem.FromItemId(baseItemType).WeaponFocusFeat.IconResRef;
          remainingTime = jobDuration;
          type = JobType.ItemCreation;

          NwItem craftedItem = NwItem.Create(BaseItems2da.baseItemTable.GetBaseItemDataEntry((BaseItemType)baseItemType).craftedItem, player.oid.LoginCreature.Location);
          craftedItem.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;

          Craft.Collect.System.AddCraftedItemProperties(craftedItem, 1);
          craftedItem.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value = player.oid.LoginCreature.OriginalName;

          serializedCraftedItem = craftedItem.Serialize().ToBase64EncodedString();
          craftedItem.Destroy();

          player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      public CraftJob(Player player, NwItem oBlueprint, double jobDuration, NwItem upgradedItem) // Item Upgrade
      {
        try
        {
          // TODO : gérer la durabilité de l'outil de craft

          // s'il s'agit d'une copie de blueprint, alors le nombre d'utilisation diminue de 1
          if (oBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").HasValue)
          {
            oBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value -= 1;

            if (oBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value < 1)
              oBlueprint.Destroy();
          }

          icon = upgradedItem.BaseItem.WeaponFocusFeat.IconResRef;
          remainingTime = jobDuration;
          type = JobType.ItemCreation;

          originalSerializedItem = upgradedItem.Serialize().ToBase64EncodedString();

          Craft.Collect.System.AddCraftedItemProperties(upgradedItem, upgradedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value + 1);
          upgradedItem.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value = player.oid.LoginCreature.OriginalName;

          serializedCraftedItem = upgradedItem.Serialize().ToBase64EncodedString();
          upgradedItem.Destroy();

          player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      public CraftJob(Player player, NwItem oBlueprint, JobType type) // Blueprint Copy
      {
        try
        {
          this.type = type;
          

          switch (type)
          {
            case JobType.BlueprintCopy:
              icon = NwBaseItem.FromItemId(oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value).WeaponFocusFeat.IconResRef;
              StartBlueprintCopy(player, oBlueprint);
              break;
            case JobType.BlueprintResearchMaterialEfficiency:
              icon = NwBaseItem.FromItemId(oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value).WeaponSpecializationFeat.IconResRef;
              StartBlueprintMaterialResearch(player, oBlueprint);
              break;
            case JobType.BlueprintResearchTimeEfficiency:
              icon = NwBaseItem.FromItemId(oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value).EpicWeaponFocusFeat.IconResRef;
              StartBlueprintTimeResearch(player, oBlueprint);
              break;
            case JobType.Renforcement:
              icon = NwBaseItem.FromItemId(oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value).EpicWeaponFocusFeat.IconResRef;
              StartRenforcement(player, oBlueprint);
              break;
            case JobType.Recycling:
              icon = NwBaseItem.FromItemId(oBlueprint.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value).EpicWeaponFocusFeat.IconResRef;
              StartRecycling(player, oBlueprint);
              break;
          }          
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      public CraftJob(Player player, NwItem item, double jobDuration, JobType jobType) // Repair
      {
        try
        {
          // TODO : gérer la durabilité de l'outil de craft

          icon = item.BaseItem.WeaponFocusFeat.IconResRef;
          remainingTime = jobDuration;
          type = jobType;

          originalSerializedItem = item.Serialize().ToBase64EncodedString();

          NwItem repairedItem = NwItem.Create(BaseItems2da.baseItemTable.GetBaseItemDataEntry(item.BaseItem.ItemType).craftedItem, player.oid.LoginCreature.Location);
          repairedItem.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;

          if (player.learnableSkills.ContainsKey(CustomSkill.RepairCareful))
            repairedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value = (int)(repairedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value * (0.95 + player.learnableSkills[CustomSkill.RepairCareful].totalPoints / 100));

          repairedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = repairedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value;
          repairedItem.GetObjectVariable<LocalVariableInt>("_DURABILITY_NB_REPAIRS").Value += 1;

          serializedCraftedItem = repairedItem.Serialize().ToBase64EncodedString();

          item.Destroy();
          repairedItem.Destroy();

          player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      public CraftJob(Player player, NwItem item, NwSpell spell, ItemProperty ip, JobType jobType) // Enchantement
      {
        try
        {
          icon = item.BaseItem.WeaponFocusFeat.IconResRef;
          type = jobType;

          remainingTime = ItemUtils.GetBaseItemCost(item) * 10 * spell.InnateSpellLevel;

          if (player.learnableSkills.ContainsKey(CustomSkill.Enchanteur))
            remainingTime *= player.learnableSkills[CustomSkill.Enchanteur].bonusReduction;

          originalSerializedItem = item.Serialize().ToBase64EncodedString();

          NwItem enchantedItem = NwItem.Create(BaseItems2da.baseItemTable.GetBaseItemDataEntry(item.BaseItem.ItemType).craftedItem, player.oid.LoginCreature.Location);
          enchantedItem.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;

          enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value -= 1;
          if (enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value <= 0)
            enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Delete();

          enchantementTag = AddCraftedEnchantementProperties(enchantedItem, spell, ip, player.characterId);

          serializedCraftedItem = enchantedItem.Serialize().ToBase64EncodedString();

          item.Destroy();
          enchantedItem.Destroy();

          player.oid.ApplyInstantVisualEffectToObject((VfxType)832, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      public CraftJob(SerializableCraftJob serializedJob, Player player)
      {
        type = serializedJob.type;
        icon = serializedJob.icon;
        remainingTime = serializedJob.remainingTime;
        originalSerializedItem = serializedJob.originalSerializedItem;
        serializedCraftedItem = serializedJob.serializedItem;
        enchantementTag = serializedJob.enchantementTag;
        progressLastCalculation = serializedJob.progressLastCalculation;

        if (progressLastCalculation.HasValue)
        {
          remainingTime -= (DateTime.Now - progressLastCalculation.Value).TotalSeconds;
          progressLastCalculation = null;
        }

        HandleDelayedJobProgression(player);
      }

      public class SerializableCraftJob
      {
        public JobType type;
        public double remainingTime { get; set; }
        public string originalSerializedItem { get; set; }
        public string serializedItem { get; set; }
        public string enchantementTag { get; set; }
        public string icon { get; set; }
        public DateTime? progressLastCalculation { get; set; }

        public SerializableCraftJob()
        {

        }
        public SerializableCraftJob(CraftJob baseJob)
        {
          type = baseJob.type;
          icon = baseJob.icon;
          remainingTime = baseJob.remainingTime;
          originalSerializedItem = baseJob.originalSerializedItem;
          serializedItem = baseJob.serializedCraftedItem;
          progressLastCalculation = baseJob.progressLastCalculation;
          enchantementTag = baseJob.enchantementTag;
        }
      }
      public async void HandleDelayedJobProgression(Player player)
      {
        if (jobProgression != null)
          jobProgression.Dispose();

        await NwTask.WaitUntil(() => player.oid.LoginCreature == null || player.oid.LoginCreature.Area != null);

        if (player.oid.LoginCreature == null)
          return;

        if (player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 0)
        {
          player.oid.OnServerSendArea -= HandleCraftJobOnAreaChange;
          player.oid.OnServerSendArea += HandleCraftJobOnAreaChange;
          return;
        }

        player.oid.OnServerSendArea -= player.craftJob.HandleCraftJobOnAreaChange;
        player.oid.OnServerSendArea += player.craftJob.HandleCraftJobOnAreaChange;
        player.oid.OnClientDisconnect -= HandleCraftJobOnPlayerLeave;
        player.oid.OnClientDisconnect += HandleCraftJobOnPlayerLeave;

        jobProgression = ModuleSystem.scheduler.ScheduleRepeating(() =>
        {
          jobProgression.Dispose();
          HandleSpecificJobCompletion[type].Invoke(player, true);
          HandleGenericJobCompletion(player);
        }, TimeSpan.FromSeconds(remainingTime));
      }
      public void HandleGenericJobCompletion(Player player)
      {
        if (player.openedWindows.ContainsKey("activeCraftJob"))
          player.windows["activeCraftJob"].CloseWindow();

        if (jobProgression != null)
          jobProgression.Dispose();

        player.craftJob = null;
        player.oid.OnClientDisconnect -= HandleCraftJobOnPlayerLeave;
        player.oid.OnServerSendArea -= HandleCraftJobOnAreaChange;

        //player.oid.ApplyInstantVisualEffectToObject((VfxType)1516, player.oid.ControlledCreature);
        player.oid.PlaySound("gui_level_up");
        player.oid.ExportCharacter();
      }
      public void HandleCraftJobCancellation(Player player)
      {
        if(type != JobType.Invalid)
          HandleSpecificJobCompletion[type].Invoke(player, false);

        if (player.openedWindows.ContainsKey("activeCraftJob"))
          player.windows["activeCraftJob"].CloseWindow();

        if (jobProgression != null)
          jobProgression.Dispose();

        player.oid.OnClientDisconnect -= HandleCraftJobOnPlayerLeave;
        player.oid.OnServerSendArea -= HandleCraftJobOnAreaChange;

        player.craftJob = null;

        //player.oid.PlaySound("gui_level_up"); // TODO : ajouter SFX annulation
        player.oid.ExportCharacter();
      }
      public string GetReadableJobCompletionTime()
      {
        TimeSpan timespan = TimeSpan.FromSeconds(remainingTime);
        return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds).ToString();
      }
      
      public void HandleCraftJobOnPlayerLeave(OnClientDisconnect onPCDisconnect)
      {
        onPCDisconnect.Player.OnClientDisconnect -= HandleCraftJobOnPlayerLeave;
        onPCDisconnect.Player.OnServerSendArea -= HandleCraftJobOnAreaChange;

        if (jobProgression != null)
          jobProgression.Dispose();

        progressLastCalculation = DateTime.Now;
      }
      public void HandleCraftJobOnAreaChange(OnServerSendArea onArea)
      {
         if (!Players.TryGetValue(onArea.Player.LoginCreature, out Player player))
          return;

        if (onArea.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 0 && !jobProgression.IsCancelled)
        {
          jobProgression.Dispose();
          player.oid.OnClientDisconnect -= HandleCraftJobOnPlayerLeave;

          if (player.windows.ContainsKey("activeCraftJob") && player.openedWindows.ContainsKey("activeCraftJob"))
          {
            Player.ActiveCraftJobWindow jobWindow = (Player.ActiveCraftJobWindow)player.windows["activeCraftJob"];
            jobWindow.timeLeft.SetBindValue(player.oid, jobWindow.token, "En pause (Hors Cité)");
          }

          return;
        }

        if (onArea.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value == 0 && jobProgression.IsCancelled)
        {
          if (player.windows.ContainsKey("activeCraftJob") && player.openedWindows.ContainsKey("activeCraftJob"))
            ((Player.ActiveCraftJobWindow)player.windows["activeCraftJob"]).HandleRealTimeJobProgression();
          else
            HandleDelayedJobProgression(player);
        }
      }
      private void StartBlueprintCopy(Player player, NwItem oBlueprint)
      {
        remainingTime = player.GetItemMateriaCost(oBlueprint) * 2 * player.learnableSkills[CustomSkill.BlueprintCopy].bonusReduction;

        originalSerializedItem = oBlueprint.Serialize().ToBase64EncodedString();

        NwItem clone = oBlueprint.Clone(player.oid.LoginCreature.Location);
        clone.Name = oBlueprint.Name.Replace("original", "copié");
        clone.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value = player.learnableSkills.ContainsKey(CustomSkill.BlueprintEfficiency) ? 10 + player.learnableSkills[CustomSkill.BlueprintEfficiency].totalPoints : 10;

        serializedCraftedItem = clone.Serialize().ToBase64EncodedString();
        oBlueprint.Destroy();
        clone.Destroy();

        player.oid.ApplyInstantVisualEffectToObject((VfxType)631, player.oid.ControlledCreature);
      }
      private void StartBlueprintMaterialResearch(Player player, NwItem oBlueprint)
      {
        remainingTime = player.GetItemMateriaCost(oBlueprint) * 2 * (1 - (player.learnableSkills[CustomSkill.BlueprintMetallurgy].totalPoints * 5 / 100));

        if (player.learnableSkills.ContainsKey(CustomSkill.AdvancedCraft))
          remainingTime *= (1 - (player.learnableSkills[CustomSkill.AdvancedCraft].totalPoints * 3 / 100));

        originalSerializedItem = oBlueprint.Serialize().ToBase64EncodedString();

        NwItem clone = oBlueprint.Clone(player.oid.LoginCreature.Location);
        clone.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value += 1;

        serializedCraftedItem = clone.Serialize().ToBase64EncodedString();
        oBlueprint.Destroy();
        clone.Destroy();

        player.oid.ApplyInstantVisualEffectToObject((VfxType)792, player.oid.ControlledCreature);
      }
      private void StartBlueprintTimeResearch(Player player, NwItem oBlueprint)
      {
        remainingTime = player.GetItemMateriaCost(oBlueprint) * 2 * (1 - (player.learnableSkills[CustomSkill.BlueprintResearch].totalPoints * 5 / 100));

        if (player.learnableSkills.ContainsKey(CustomSkill.AdvancedCraft))
          remainingTime *= (1 - (player.learnableSkills[CustomSkill.AdvancedCraft].totalPoints * 3 / 100));

        originalSerializedItem = oBlueprint.Serialize().ToBase64EncodedString();

        NwItem clone = oBlueprint.Clone(player.oid.LoginCreature.Location);
        clone.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value += 1;

        serializedCraftedItem = clone.Serialize().ToBase64EncodedString();
        oBlueprint.Destroy();
        clone.Destroy();

        player.oid.ApplyInstantVisualEffectToObject((VfxType)792, player.oid.ControlledCreature);
      }
      private void StartRenforcement(Player player, NwItem item)
      {
        try
        {
          remainingTime = ItemUtils.GetBaseItemCost(item) * 100 * (1 - (player.learnableSkills[CustomSkill.Renforcement].totalPoints * 5 / 100));
          originalSerializedItem = item.Serialize().ToBase64EncodedString();

          item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value += item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value * 5 / 100;
          item.GetObjectVariable<LocalVariableInt>("_REINFORCEMENT_LEVEL").Value += 1;

          serializedCraftedItem = item.Serialize().ToBase64EncodedString();
          item.Destroy();

          player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      private void StartRecycling(Player player, NwItem item)
      {
        try
        {
          remainingTime = ItemUtils.GetBaseItemCost(item) * 125 * player.learnableSkills[CustomSkill.Recycler].bonusReduction;

          if (player.learnableSkills.ContainsKey(CustomSkill.RecyclerFast))
            remainingTime *= player.learnableSkills[CustomSkill.RecyclerFast].bonusReduction;

          originalSerializedItem = item.Serialize().ToBase64EncodedString();
          item.Destroy();

          player.oid.ApplyInstantVisualEffectToObject((VfxType)818, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
    }
    private static bool CompleteBlueprintCopy(Player player, bool completed)
    {
      if (completed)
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)631, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteBlueprintMaterialResearch(Player player, bool completed)
    {
      if (completed)
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)792, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteBlueprintTimeResearch(Player player, bool completed)
    {
      if (completed)
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)792, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteItemCreation(Player player, bool completed)
    {
      if (completed)
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer la création de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);

        int artisanExceptionnelLevel = player.learnableSkills.ContainsKey(CustomSkill.ArtisanExceptionnel) ? player.learnableSkills[CustomSkill.ArtisanExceptionnel].totalPoints : 0;

        if (NwRandom.Roll(Utils.random, 100) <= artisanExceptionnelLevel)
        {
          item.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
          player.oid.SendServerMessage("Votre talent d'artisan vous a permis de créer un objet exceptionnel disposant d'un emplacement d'enchantement supplémentaire !", ColorConstants.Navy);
        }

        int artisanAppliqueLevel = player.learnableSkills.ContainsKey(CustomSkill.ArtisanApplique) ? player.learnableSkills[CustomSkill.ArtisanApplique].totalPoints : 0;

        if (NwRandom.Roll(Utils.random, 100) <= artisanAppliqueLevel * 3)
        {
          int artisanFocusLevel = player.learnableSkills.ContainsKey(CustomSkill.ArtisanFocus) ? 5 * (player.learnableSkills[CustomSkill.ArtisanFocus].totalPoints + 1) : 5;

          item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value *= (1 + artisanFocusLevel / 100);
          player.oid.SendServerMessage("En travaillant de manière particulièrement appliquée, vous parvenez à fabriquer un objet plus résistant !", ColorConstants.Navy);
        }
      }
      else // cancelled
      {
        player.oid.SendServerMessage($"Vous venez d'annuler la création d'un objet artisanal.", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteItemUpgrade(Player player, bool completed)
    {
      if (completed)
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer l'amélioration de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);

        int artisanExceptionnelLevel = player.learnableSkills.ContainsKey(CustomSkill.ArtisanExceptionnel) ? player.learnableSkills[CustomSkill.ArtisanExceptionnel].totalPoints : 0;

        if (NwRandom.Roll(Utils.random, 100) <= artisanExceptionnelLevel)
        {
          item.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
          player.oid.SendServerMessage("Votre talent d'artisan vous a permis de créer un objet exceptionnel disposant d'un emplacement d'enchantement supplémentaire !", ColorConstants.Navy);
        }

        int artisanAppliqueLevel = player.learnableSkills.ContainsKey(CustomSkill.ArtisanApplique) ? player.learnableSkills[CustomSkill.ArtisanApplique].totalPoints : 0;

        if (NwRandom.Roll(Utils.random, 100) <= artisanAppliqueLevel * 3)
        {
          item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value *= (1 + 20 / 100);
          player.oid.SendServerMessage("En travaillant de manière particulièrement appliquée, vous parvenez à fabriquer un objet plus résistant !", ColorConstants.Navy);
        }
      }
      else // cancelled
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'anuller l'amélioration de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteItemReinforcement(Player player, bool completed)
    {
      if (completed)
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer le renforcement de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler le renforcement de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteItemRecycling(Player player, bool completed)
    {
      if (completed)
      {
        NwItem item = NwItem.Deserialize(player.craftJob.originalSerializedItem.ToByteArray());

        ResourceType resourceType = ItemUtils.GetResourceTypeFromItem(item);
        int grade = item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").HasValue ? item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value : 1;
        double quantity = item.BaseItem.BaseCost * 125 * player.learnableSkills[CustomSkill.Recycler].bonusMultiplier;

        if (player.learnableSkills.ContainsKey(CustomSkill.RecyclerExpert))
          quantity *= player.learnableSkills[CustomSkill.RecyclerExpert].bonusMultiplier;

        CraftResource resource = player.craftResourceStock.FirstOrDefault(r => r.type == resourceType && r.grade == grade);

        if (resource != null)
          resource.quantity += (int)quantity;
        else
          player.craftResourceStock.Add(new CraftResource(Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == resourceType && r.grade == grade), (int)quantity));

        player.oid.SendServerMessage($"Le recyclage de : {item.Name.ColorString(ColorConstants.White)} vous rapporte {quantity.ToString().ColorString(ColorConstants.White)} unités de matéria de qualité {((int)grade).ToString().ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)818, player.oid.ControlledCreature);

        item.Destroy();
      }
      else // cancelled
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler le recyclage de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteItemRepair(Player player, bool completed)
    {
      if (completed)
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer la réparation de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler la réparation de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteItemEnchantement(Player player, bool completed)
    {
      if (completed)
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.craftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer l'enchantement de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1055, player.oid.ControlledCreature);

        int enchanteurChanceuxLevel = player.learnableSkills.ContainsKey(CustomSkill.EnchanteurChanceux) ? player.learnableSkills[CustomSkill.EnchanteurChanceux].totalPoints : 0;

        if (NwRandom.Roll(Utils.random, 100) <= enchanteurChanceuxLevel)
        {
          item.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
          player.oid.SendServerMessage("Votre talent d'enchanteur vous a permit de ne pas consommer d'emplacement !", ColorConstants.Orange);
        }

        int enchanteurExpertLevel = player.learnableSkills.ContainsKey(CustomSkill.EnchanteurExpert) ? player.learnableSkills[CustomSkill.EnchanteurExpert].totalPoints : 0;

        if (NwRandom.Roll(Utils.random, 100) <= enchanteurExpertLevel * 2)
        {
          ItemProperty oldIp = item.ItemProperties.FirstOrDefault(ip => ip.Tag == player.craftJob.enchantementTag);
          item.RemoveItemProperty(oldIp);

          oldIp.CostTableValue += 1;
          item.AddItemProperty(oldIp, EffectDuration.Permanent);

          player.oid.SendServerMessage("Votre talent d'enchanteur vous a permis d'obtenir un effet plus puissant !", ColorConstants.Orange);
        }
      }
      else // cancelled
      {
        player.oid.SendServerMessage($"Vous venez d'annuler la création d'un objet artisanal.", ColorConstants.Orange);
      }

      return true;
    }
    private static string AddCraftedEnchantementProperties(NwItem craftedItem, NwSpell spell, ItemProperty ip, int enchanterId)
    {
      ItemProperty newIp = GetCraftEnchantementProperties(craftedItem, spell, ip, enchanterId);
      craftedItem.AddItemProperty(newIp, EffectDuration.Permanent);
      return newIp.Tag;
    }
    private static ItemProperty GetCraftEnchantementProperties(NwItem craftedItem, NwSpell spell, ItemProperty ip, int enchanterId)
    {
      ItemProperty existingIP = craftedItem.ItemProperties.FirstOrDefault(i => i.DurationType == EffectDuration.Permanent && i.PropertyType == ip.PropertyType && i.SubType == ip.SubType && i.Param1Table == ip.Param1Table);

      if (existingIP != null)
      {
        craftedItem.RemoveItemProperty(existingIP);

        if (ip.PropertyType == ItemPropertyType.DamageBonus
          || ip.PropertyType == ItemPropertyType.DamageBonusVsAlignmentGroup
          || ip.PropertyType == ItemPropertyType.DamageBonusVsRacialGroup
          || ip.PropertyType == ItemPropertyType.DamageBonusVsSpecificAlignment)
        {
          int newRank = ItemPropertyDamageCost2da.ipDamageCost.GetRankFromCostValue(ip.CostTableValue);
          int existingRank = ItemPropertyDamageCost2da.ipDamageCost.GetRankFromCostValue(existingIP.CostTableValue);

          if (existingRank > newRank)
            newRank = existingRank + 1;
          else
            newRank += 1;

          ip.CostTableValue = ItemPropertyDamageCost2da.ipDamageCost.GetDamageCostValueFromRank(newRank);
        }
        else
        {
          if (existingIP.CostTableValue > ip.CostTableValue)
            ip.CostTableValue = existingIP.CostTableValue + 1;
          else
            ip.CostTableValue += 1;
        }
      }

      ip.Tag = $"ENCHANTEMENT_{spell.Id}_{ip.PropertyType}_{ip.SubType}_{ip.CostTable}_{ip.CostTableValue}_{enchanterId}";

      return ip;
    }
  }
}
