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
      { JobType.BlueprintCopy, CompleteBlueprintCopy },
      { JobType.BlueprintResearchMaterialEfficiency, CompleteBlueprintMaterialResearch },
      { JobType.BlueprintResearchTimeEfficiency, CompleteBlueprintMaterialResearch }
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
          craftedItem.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value = player.oid.LoginCreature.Name;

          int artisanExceptionnelLevel = player.learnableSkills.ContainsKey(CustomSkill.ArtisanExceptionnel) ? player.learnableSkills[CustomSkill.ArtisanExceptionnel].totalPoints : 0;

          if (NwRandom.Roll(Utils.random, 100) <= artisanExceptionnelLevel)
          {
            craftedItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
            player.oid.SendServerMessage("Votre talent d'artisan vous a permis de créer un objet exceptionnel disposant d'un emplacement d'enchantement supplémentaire !", ColorConstants.Navy);
          }

          int artisanAppliqueLevel = player.learnableSkills.ContainsKey(CustomSkill.ArtisanApplique) ? player.learnableSkills[CustomSkill.ArtisanApplique].totalPoints : 0;

          if (NwRandom.Roll(Utils.random, 100) <= artisanAppliqueLevel * 3)
          {
            craftedItem.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value *= (1 + 20/100);
            player.oid.SendServerMessage("En travaillant de manière particulièrement appliquée, vous parvenez à fabriquer un objet plus résistant !", ColorConstants.Navy);
          }

          serializedCraftedItem = craftedItem.Serialize().ToBase64EncodedString();
          craftedItem.Destroy();
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
          }          
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

        player.oid.OnServerSendArea -= player.newCraftJob.HandleCraftJobOnAreaChange;
        player.oid.OnServerSendArea += player.newCraftJob.HandleCraftJobOnAreaChange;
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
          player.oid.NuiDestroy(player.openedWindows["activeCraftJob"]);

        if (jobProgression != null)
          jobProgression.Dispose();

        player.newCraftJob = null;
        player.oid.OnClientDisconnect -= HandleCraftJobOnPlayerLeave;
        player.oid.OnServerSendArea -= HandleCraftJobOnAreaChange;

        player.oid.ApplyInstantVisualEffectToObject((VfxType)1516, player.oid.ControlledCreature);
        player.oid.PlaySound("gui_level_up");
        player.oid.ExportCharacter();
      }
      public void HandleCraftJobCancellation(Player player)
      {
        if(type != JobType.Invalid)
          HandleSpecificJobCompletion[type].Invoke(player, false);

        if (player.openedWindows.ContainsKey("activeCraftJob"))
          player.oid.NuiDestroy(player.openedWindows["activeCraftJob"]);

        if (jobProgression != null)
          jobProgression.Dispose();

        player.oid.OnClientDisconnect -= HandleCraftJobOnPlayerLeave;
        player.oid.OnServerSendArea -= HandleCraftJobOnAreaChange;

        player.newCraftJob = null;

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
      }
    }

    private static bool CompleteBlueprintCopy(Player player, bool completed)
    {
      if (completed)
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.newCraftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
        ItemUtils.DeserializeAndAcquireItem(player.newCraftJob.originalSerializedItem, player.oid.LoginCreature);
      }
      else // cancelled
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.newCraftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteBlueprintMaterialResearch(Player player, bool completed)
    {
      if (completed)
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.newCraftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }
      else // cancelled
      {
        NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.newCraftJob.originalSerializedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez d'annuler le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }

      return true;
    }
    private static bool CompleteItemCreation(Player player, bool completed)
    {
      if (completed)
      {
        NwItem item = ItemUtils.DeserializeAndAcquireItem(player.newCraftJob.serializedCraftedItem, player.oid.LoginCreature);
        player.oid.SendServerMessage($"Vous venez de terminer la création de : {item.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      }
      else // cancelled
      {
        player.oid.SendServerMessage($"Vous venez d'annuler la création d'un objet artisanal.", ColorConstants.Orange);
      }

      return true;
    }
  }
}
