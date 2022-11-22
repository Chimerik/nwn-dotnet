using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static readonly Dictionary<JobType, Func<Player, bool, bool>> HandleSpecificJobCompletion = new()
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
      { JobType.Mining, CompleteMining },
      { JobType.WoodCutting, CompleteWoodCutting },
      { JobType.Pelting, CompletePelting },
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
      [Description("Extraction_minérale_passive")]
      Mining = 12,
      [Description("Extraction_arboricole_passive")]
      WoodCutting = 13,
      [Description("Extraction_animale_passive")]
      Pelting = 14,
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
      public DateTime? startTime { get; set; }
      //public ScheduledTask jobProgression { get; set; }
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

          try
          {
            icon = NwBaseItem.FromItemId(baseItemType).WeaponFocusFeat.IconResRef;
          }
          catch(Exception)
          {
            icon = "clockwork";
          }

          remainingTime = jobDuration;
          type = JobType.ItemCreation;

          NwItem craftedItem = NwItem.Create(BaseItems2da.baseItemTable[baseItemType].craftedItem, player.oid.LoginCreature.Location);
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
          type = JobType.ItemUpgrade;

          originalSerializedItem = upgradedItem.Serialize().ToBase64EncodedString();

          Craft.Collect.System.AddCraftedItemProperties(upgradedItem, upgradedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value + 1);
          upgradedItem.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value = player.oid.LoginCreature.OriginalName;

          DelayItemSerialization(upgradedItem);
          player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      private async void DelayItemSerialization(NwItem upgradedItem) // La suppression d'item property ne s'exécute qu'après la fin du script en cours. Je suis donc obligé de mettre un faux délai avant serialization
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0));
        serializedCraftedItem = upgradedItem.Serialize().ToBase64EncodedString();
        upgradedItem.Destroy();
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

          NwItem repairedItem = NwItem.Create(BaseItems2da.baseItemTable[(int)item.BaseItem.ItemType].craftedItem, player.oid.LoginCreature.Location);
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
          icon = spell.IconResRef;
          type = jobType;

          remainingTime = ItemUtils.GetBaseItemCost(item) * 10 * spell.InnateSpellLevel;

          if (player.learnableSkills.ContainsKey(CustomSkill.Enchanteur))
            remainingTime *= player.learnableSkills[CustomSkill.Enchanteur].bonusReduction;

          originalSerializedItem = item.Serialize().ToBase64EncodedString();

          NwItem enchantedItem = item.Clone(player.oid.LoginCreature.Location);

          enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value -= 1;
          if (enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value <= 0)
            enchantedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Delete();

          int i = 0;

          while (enchantedItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasValue && i < 20)
            i++;

          if (i > 19)
            Utils.LogMessageToDMs($"SYSTEME ENCHANTEMENT - {player.oid.LoginCreature.Name} - {spell.Name.ToString()} - {item.Name} - Impossible de trouver un slot valide libre");

          enchantedItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value = spell.Id;
          enchantementTag = AddCraftedEnchantementProperties(enchantedItem, spell, ip, player.characterId);

          item.Destroy();
          DelayItemSerialization(enchantedItem);
          player.oid.ApplyInstantVisualEffectToObject((VfxType)832, player.oid.ControlledCreature);
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"{e.Message}\n\n{e.StackTrace}");
        }
      }
      public CraftJob(Player player, ResourceType resourceType, double consumedTime, string icon) // Passive repeatable mining
      {
        try
        {
          switch(resourceType)
          {
            case ResourceType.Plank: type = JobType.WoodCutting; break;
            case ResourceType.Leather: type = JobType.Pelting; break;
            default: type = JobType.Mining; break;
          }

          remainingTime = 3600;
          startTime = DateTime.Now.AddSeconds(-consumedTime);
          this.icon = icon;

          player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
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
        startTime = serializedJob.startTime;

        if (progressLastCalculation.HasValue)
        {
          remainingTime -= (DateTime.Now - progressLastCalculation.Value).TotalSeconds;
          progressLastCalculation = null;
        }
        //HandleDelayedJobProgression(player);
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
        public DateTime? startTime { get; set; }

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
          baseJob.progressLastCalculation = DateTime.Now;
          progressLastCalculation = DateTime.Now;
          enchantementTag = baseJob.enchantementTag;
          startTime = baseJob.startTime;
        }
      }
      /*public async void HandleDelayedJobProgression(Player player)
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

        player.oid.OnServerSendArea -= HandleCraftJobOnAreaChange;
        player.oid.OnServerSendArea += HandleCraftJobOnAreaChange;
        player.oid.OnClientDisconnect -= HandleCraftJobOnPlayerLeave;
        player.oid.OnClientDisconnect += HandleCraftJobOnPlayerLeave;

        if (remainingTime > 0)
        {
          jobProgression = player.scheduler.ScheduleRepeating(() =>
          {
            jobProgression.Dispose();
            HandleSpecificJobCompletion[type].Invoke(player, true);
            HandleGenericJobCompletion(player);
          }, TimeSpan.FromSeconds(remainingTime));
        }
        else
        {
          HandleSpecificJobCompletion[type].Invoke(player, true);
          HandleGenericJobCompletion(player);
        }
      }*/
      public void HandleGenericJobCompletion(Player player)
      {
        if (player.TryGetOpenedWindow("activeCraftJob", out Player.PlayerWindow craftWindow))
          if (player.craftJob.type != JobType.Mining && player.craftJob.type != JobType.WoodCutting && player.craftJob.type != JobType.Pelting)
            craftWindow.CloseWindow();

        //if (jobProgression != null)
        //jobProgression.Dispose();

        player.craftJob = null;
        //player.oid.OnClientDisconnect -= HandleCraftJobOnPlayerLeave;

        //player.oid.ApplyInstantVisualEffectToObject((VfxType)1516, player.oid.ControlledCreature);
        player.oid.PlaySound("gui_level_up");
        player.oid.ExportCharacter();
      }
      public void HandleCraftJobCancellation(Player player)
      {
        if (type != JobType.Invalid)
          HandleSpecificJobCompletion[type].Invoke(player, false);

        if (player.TryGetOpenedWindow("activeCraftJob", out Player.PlayerWindow craftWindow))
          craftWindow.CloseWindow();

        //if (jobProgression != null)
        //jobProgression.Dispose();

        //player.oid.OnClientDisconnect -= HandleCraftJobOnPlayerLeave;

        player.craftJob = null;

        //player.oid.PlaySound("gui_level_up"); // TODO : ajouter SFX annulation
        player.oid.ExportCharacter();
      }
      public string GetReadableJobCompletionTime()
      {
        TimeSpan timespan = TimeSpan.FromSeconds(remainingTime);
        return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds).ToString();
      }

      /*public void HandleCraftJobOnPlayerLeave(OnClientDisconnect onPCDisconnect)
      {
        onPCDisconnect.Player.OnClientDisconnect -= HandleCraftJobOnPlayerLeave;
        onPCDisconnect.Player.OnServerSendArea -= HandleCraftJobOnAreaChange;

        if (jobProgression != null)
          jobProgression.Dispose();

        progressLastCalculation = DateTime.Now;
      }*/
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
          remainingTime = player.GetItemReinforcementTime(item);
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
          remainingTime = player.GetItemRecycleTime(item);

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
      if (string.IsNullOrEmpty(player.craftJob.serializedCraftedItem))
      {
        player.oid.SendServerMessage("Il semble y avoir eu une erreur. Votre travail artisanal a été annulé.");
        player.craftJob = null;
        return true;
      }

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
      if (string.IsNullOrEmpty(player.craftJob.serializedCraftedItem))
      {
        player.oid.SendServerMessage("Il semble y avoir eu une erreur. Votre travail artisanal a été annulé.");
        player.craftJob = null;
        return true;
      }

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
      if (string.IsNullOrEmpty(player.craftJob.serializedCraftedItem))
      {
        player.oid.SendServerMessage("Il semble y avoir eu une erreur. Votre travail artisanal a été annulé.");
        player.craftJob = null;
        return true;
      }

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
        double quantity = player.GetItemRecycleGain(item);

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

          oldIp.IntParams[3] += 1; // IntParams[3] = CostTableValue
          item.AddItemProperty(oldIp, EffectDuration.Permanent);

          player.oid.SendServerMessage("Votre talent d'enchanteur vous a permis d'obtenir un effet plus puissant !", ColorConstants.Orange);
        }
      }
      else // cancelled
      {
        ItemUtils.DeserializeAndAcquireItem(player.craftJob.originalSerializedItem, player.oid.LoginCreature);
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
      ItemProperty existingIP = craftedItem.ItemProperties.FirstOrDefault(i => i.DurationType == EffectDuration.Permanent && i.Property.RowIndex == ip.Property.RowIndex && i.SubType?.RowIndex == ip.SubType?.RowIndex && i.Param1TableValue?.RowIndex == ip.Param1TableValue?.RowIndex);

      if (existingIP != null)
      {
        craftedItem.RemoveItemProperty(existingIP);

        if (ip.Property.PropertyType == ItemPropertyType.DamageBonus
          || ip.Property.PropertyType == ItemPropertyType.DamageBonusVsAlignmentGroup
          || ip.Property.PropertyType == ItemPropertyType.DamageBonusVsRacialGroup
          || ip.Property.PropertyType == ItemPropertyType.DamageBonusVsSpecificAlignment)
        {
          int newRank = ItemPropertyDamageCost2da.GetRankFromCostValue(ip.IntParams[3]); // IntParams[3] = CostTableValue
          int existingRank = ItemPropertyDamageCost2da.GetRankFromCostValue(existingIP.IntParams[3]); // IntParams[3] = CostTableValue

          if (existingRank > newRank)
            newRank = existingRank + 1;
          else
            newRank += 1;

          ip.IntParams[3] = ItemPropertyDamageCost2da.GetDamageCostValueFromRank(newRank); // IntParams[3] = CostTableValue
        }
        else if (ip.Property.PropertyType == ItemPropertyType.AcBonus
          || ip.Property.PropertyType == ItemPropertyType.AcBonusVsAlignmentGroup
          || ip.Property.PropertyType == ItemPropertyType.AcBonusVsDamageType
          || ip.Property.PropertyType == ItemPropertyType.AcBonusVsRacialGroup
          || ip.Property.PropertyType == ItemPropertyType.AcBonusVsSpecificAlignment
          || ip.Property.PropertyType == ItemPropertyType.AttackBonus
          || ip.Property.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup
          || ip.Property.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup
          || ip.Property.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment)
        {
          ip.IntParams[3] += existingIP.IntParams[3];
        }
        else
        {
          if (existingIP.IntParams[3] > ip.IntParams[3])
            ip.IntParams[3] = existingIP.IntParams[3] + 1;
          else
            ip.IntParams[3] += 1;
        }
      }

      ip.Tag = $"ENCHANTEMENT_{spell.Id}_{ip.Property.PropertyType}_{ip.SubType}_{ip.CostTable}_{ip.CostTableValue}_{enchanterId}";

      return ip;
    }
    private static bool CompleteMining(Player player, bool completed)
    {
      if (completed)
      {
        double elapsedTime = (DateTime.Now - player.craftJob.startTime.Value).TotalSeconds;

        if (elapsedTime < 3600)
          elapsedTime = 3600;

        int nbCycles = (int)Math.Truncate(elapsedTime / 3600);
        double alreadyConsumedTime = elapsedTime - (nbCycles * 3600);
        double miningYield = nbCycles * 900;
        miningYield = player.learnableSkills.ContainsKey(CustomSkill.MineralExtraction) ? miningYield * player.learnableSkills[CustomSkill.MineralExtraction].bonusMultiplier : miningYield;
        miningYield = player.learnableSkills.ContainsKey(CustomSkill.MineralExtractionYield) ? miningYield * player.learnableSkills[CustomSkill.MineralExtractionYield].bonusMultiplier : miningYield;

        CraftResource resourceStock = player.craftResourceStock.FirstOrDefault(r => r.type == ResourceType.Ore && r.grade == 1);

        if (resourceStock != null)
          resourceStock.quantity += (int)miningYield;
        else
        {
          CraftResource resource = Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == ResourceType.Ore && r.grade == 1);
          player.craftResourceStock.Add(new CraftResource(resource, (int)miningYield));
        }

        player.RescheduleRepeatableJob(ResourceType.Ore, elapsedTime, player.craftJob.icon);
        player.oid.SendServerMessage($"La fin de votre job d'extraction passive de matéria minérale vous rapporte : {(int)miningYield} unités de matéria !", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        player.oid.SendServerMessage($"Vous venez d'annuler votre job de récolte passive de matéria minérale.", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteWoodCutting(Player player, bool completed)
    {
      if (completed)
      {
        double elapsedTime = (DateTime.Now - player.craftJob.startTime.Value).TotalSeconds;

        if (elapsedTime < 3600)
          elapsedTime = 3600;

        int nbCycles = (int)Math.Truncate(elapsedTime / 3600);
        double alreadyConsumedTime = elapsedTime - (nbCycles * 3600);
        double miningYield = nbCycles * 900;
        miningYield = player.learnableSkills.ContainsKey(CustomSkill.WoodExtraction) ? miningYield * player.learnableSkills[CustomSkill.WoodExtraction].bonusMultiplier : miningYield;
        miningYield = player.learnableSkills.ContainsKey(CustomSkill.WoodExtractionYield) ? miningYield * player.learnableSkills[CustomSkill.WoodExtractionYield].bonusMultiplier : miningYield;

        CraftResource resourceStock = player.craftResourceStock.FirstOrDefault(r => r.type == ResourceType.Wood && r.grade == 1);

        if (resourceStock != null)
          resourceStock.quantity += (int)miningYield;
        else
        {
          CraftResource resource = Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == ResourceType.Wood && r.grade == 1);
          player.craftResourceStock.Add(new CraftResource(resource, (int)miningYield));
        }

        player.RescheduleRepeatableJob(ResourceType.Ore, alreadyConsumedTime, player.craftJob.icon);
        player.oid.SendServerMessage($"La fin de votre job d'extraction passive de matéria arboricole vous rapporte : {(int)miningYield} unités de matéria, directement envoyées à l'entrepôt !", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        player.oid.SendServerMessage($"Vous venez d'annuler votre job de récolte passive de matéria arboricole.", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompletePelting(Player player, bool completed)
    {
      if (completed)
      {
        double elapsedTime = (DateTime.Now - player.craftJob.startTime.Value).TotalSeconds;

        if (elapsedTime < 3600)
          elapsedTime = 3600;

        int nbCycles = (int)Math.Truncate(elapsedTime / 3600);
        double alreadyConsumedTime = elapsedTime - (nbCycles * 3600);
        double miningYield = nbCycles * 900;
        miningYield = player.learnableSkills.ContainsKey(CustomSkill.PeltExtraction) ? miningYield * player.learnableSkills[CustomSkill.PeltExtraction].bonusMultiplier : miningYield;
        miningYield = player.learnableSkills.ContainsKey(CustomSkill.PeltExtractionYield) ? miningYield * player.learnableSkills[CustomSkill.PeltExtractionYield].bonusMultiplier : miningYield;

        CraftResource resourceStock = player.craftResourceStock.FirstOrDefault(r => r.type == ResourceType.Wood && r.grade == 1);

        if (resourceStock != null)
          resourceStock.quantity += (int)miningYield;
        else
        {
          CraftResource resource = Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == ResourceType.Wood && r.grade == 1);
          player.craftResourceStock.Add(new CraftResource(resource, (int)miningYield));
        }

        player.RescheduleRepeatableJob(ResourceType.Ore, alreadyConsumedTime, player.craftJob.icon);
        player.oid.SendServerMessage($"La fin de votre job d'extraction passive de matéria animale vous rapporte : {(int)miningYield} unités de matéria, directement envoyées à l'entrepôt !", ColorConstants.Orange);
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1501, player.oid.ControlledCreature);
      }
      else // cancelled
      {
        player.oid.SendServerMessage($"Vous venez d'annuler votre job de récolte passive de matéria animale, directement envoyées à l'entrepôt !", ColorConstants.Orange);
      }

      return true;
    }
  }
}
