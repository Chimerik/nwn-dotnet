using System;
using System.Collections.Generic;
using System.ComponentModel;

using Anvil.API;
using Anvil.Services;

using NWN.Systems.Craft;

namespace NWN.Systems
{
  // TODO : créer et afficher fenêtre craft en cours + vérifier fonctionne du Dispose
  public partial class PlayerSystem
  {
    public static Dictionary<JobType, Func<Player, bool, bool>> HandleSpecificJobCompletion = new Dictionary<JobType, Func<Player, bool, bool>>
    {
      { JobType.BlueprintCopy, CompleteBlueprintCopy }
    };

    public enum JobType
    {
      [Description("Invalide")]
      Invalid = 0,
      [Description("Création_artisanale")]
      Item = 1,
      [Description("Copie_de_patron")]
      BlueprintCopy = 2,
      [Description("Recherche_en_rendement")]
      BlueprintResearchMaterialEfficiency = 3,
      [Description("Recherche_en_efficacité")]
      BlueprintResearchTimeEfficiency = 4,
      [Description("Enchantement")]
      Enchantement = 5,
      [Description("Recyclage")]
      Recycling = 6,
      [Description("Renforcement")]
      Renforcement = 7,
      [Description("Réparations")]
      Repair = 8,
      [Description("Réactivation_d'_enchantement")]
      EnchantementReactivation = 9,
      [Description("Alchimie")]
      Alchemy = 10,
    }

    public class CraftJob
    {
      public readonly JobType type;
      public readonly string icon;
      public double remainingTime { get; set; }
      public string originalSerializedItem { get; set; }
      public string serializedCraftedItem { get; set; }
      public DateTime? progressLastCalculation { get; set; }
      public CraftJob(Player player, NwItem oBlueprint, Blueprint blueprint) // Blueprint Copy
      {
        try
        {
          type = JobType.BlueprintCopy;
          icon = "menu_select";
          remainingTime = blueprint.mineralsCost * 2 * player.learnableSkills[CustomSkill.BlueprintCopy].bonusReduction;

          originalSerializedItem = oBlueprint.Serialize().ToBase64EncodedString();

          NwItem clone = oBlueprint.Clone(player.oid.LoginCreature.Location);
          clone.Name = $"Patron copié : {blueprint.name}";
          clone.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value = player.learnableSkills.ContainsKey(CustomSkill.BlueprintEfficiency) ? 10 + player.learnableSkills[CustomSkill.BlueprintEfficiency].totalPoints : 10;

          serializedCraftedItem = clone.Serialize().ToBase64EncodedString();
          oBlueprint.Destroy();
          clone.Destroy();

          HandleCraftProgression(player);
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

        HandleCraftProgression(player);
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

      public void HandleCraftProgression(Player player)
      {
        if (progressLastCalculation.HasValue)
          remainingTime -= (DateTime.Now - progressLastCalculation.Value).TotalSeconds;

        ScheduledTask jobProgression = null;
        jobProgression = ModuleSystem.scheduler.ScheduleRepeating(() =>
        {
          if (player.oid.LoginCreature == null)
          {
            progressLastCalculation = DateTime.Now;
            jobProgression.Dispose();
            return;
          }

          if(player.newCraftJob == null)
          { 
            jobProgression.Dispose();
            return;
          }

          if (player.windows.ContainsKey("activeCraftJob") && player.openedWindows.ContainsKey("activeCraftJob"))
          {
            Player.ActiveCraftJobWindow jobWindow = (Player.ActiveCraftJobWindow)player.windows["activeCraftJob"];

            if (player.location.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 0)
            {
              jobWindow.timeLeft.SetBindValue(player.oid, jobWindow.token, "Le travail artisanal ne peut pas progresser hors de la cité.");
              return;
            }

            jobWindow.timeLeft.SetBindValue(player.oid, jobWindow.token, GetReadableJobCompletionTime());
          }

          remainingTime -= 1;

          if (remainingTime < 1)
          {
            HandleGenericJobCompletion(player);
            HandleSpecificJobCompletion[type].Invoke(player, true);
            jobProgression.Dispose();
          }

        }, TimeSpan.FromSeconds(1));
      }
      private void HandleGenericJobCompletion(Player player)
      {

        if (player.openedWindows.ContainsKey("activeCraftJob"))
          player.oid.NuiDestroy(player.openedWindows["activeCraftJob"]);

        player.newCraftJob = null;

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

        player.newCraftJob = null;

        //player.oid.PlaySound("gui_level_up"); // TODO : ajouter SFX annulation
        player.oid.ExportCharacter();
      }
      public string GetReadableJobCompletionTime()
      {
        TimeSpan timespan = TimeSpan.FromSeconds(remainingTime);
        return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds).ToString();
      }
    }

    private static bool CompleteBlueprintCopy(Player player, bool completed)
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
  }
}
