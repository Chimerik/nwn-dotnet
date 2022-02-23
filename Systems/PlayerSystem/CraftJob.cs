using System;
using System.Collections.Generic;

using Anvil.API;
using Anvil.Services;

using NWN.Systems.Craft;

namespace NWN.Systems

{
  // TODO : créer et afficher fenêtre craft en cours + vérifier fonctionne du Dispose
  public partial class PlayerSystem
  {
    public static Dictionary<JobType, Func<Player, bool>> HandleSpecificJobCompletion = new Dictionary<JobType, Func<Player, bool>>
    {
      { JobType.BlueprintCopy, CompleteBlueprintCopy }
    };

    public enum JobType
    {
      Invalid = 0,
      Item = 1,
      BlueprintCopy = 2,
      BlueprintResearchMaterialEfficiency = 3,
      BlueprintResearchTimeEfficiency = 4,
      Enchantement = 5,
      Recycling = 6,
      Renforcement = 7,
      Repair = 8,
      EnchantementReactivation = 9,
      Alchemy = 10,
    }

    public class CraftJob
    {
      readonly JobType type;
      readonly string icon;
      public double remainingTime { get; set; }
      public string serializedCraftedItem { get; set; }
      public DateTime? progressLastCalculation { get; set; }
      public CraftJob(Player player, NwItem oBlueprint, Blueprint blueprint) // Blueprint Copy
      {
        try
        {
          type = JobType.BlueprintCopy;
          icon = "menu_select";
          remainingTime = blueprint.mineralsCost * 200 * player.learnableSkills[CustomSkill.BlueprintCopy].bonusReduction;

          NwItem clone = oBlueprint.Clone(player.oid.LoginCreature.Location);
          clone.Name = $"Copie de {oBlueprint.Name}";
          clone.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value = player.learnableSkills.ContainsKey(CustomSkill.BlueprintEfficiency) ? 10 + player.learnableSkills[CustomSkill.BlueprintEfficiency].totalPoints : 10;

          serializedCraftedItem = clone.Serialize().ToBase64EncodedString();
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
        icon = serializedJob.icon;
        remainingTime = serializedJob.remainingTime;
        serializedCraftedItem = serializedJob.serializedItem;
        progressLastCalculation = serializedJob.progressLastCalculation;

        HandleCraftProgression(player);
      }

      public class SerializableCraftJob
      {
        public double remainingTime { get; set; }
        public string serializedItem { get; set; }
        public string icon { get; set; }
        public DateTime? progressLastCalculation { get; set; }

        public SerializableCraftJob()
        {

        }
        public SerializableCraftJob(CraftJob baseJob)
        {
          icon = baseJob.icon;
          remainingTime = baseJob.remainingTime;
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
          if (player.oid == null)
          {
            progressLastCalculation = DateTime.Now;
            jobProgression.Dispose();
            return;
          }

          if (player.location.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 0)
            return;

          remainingTime -= 1;

          if (remainingTime < 1)
          {
            HandleSpecificJobCompletion[type].Invoke(player);
            HandleGenericJobCompletion(player);
            jobProgression.Dispose();
          }

        }, TimeSpan.FromSeconds(1));
      }
      private void HandleGenericJobCompletion(Player player)
      {
        player.newCraftJob = null;

        /*if (player.openedWindows.ContainsKey("activeLearnable"))
        player.oid.NuiDestroy(player.openedWindows["activeLearnable"]);

        if (player.openedWindows.ContainsKey("learnables") && player.windows.ContainsKey("learnables"))
        {
          PlayerSystem.Player.LearnableWindow window = (PlayerSystem.Player.LearnableWindow)player.windows["learnables"];
          window.LoadLearnableList(window.currentList);
        }*/

        player.oid.ApplyInstantVisualEffectToObject((VfxType)1516, player.oid.ControlledCreature);
        player.oid.PlaySound("gui_level_up");
        player.oid.ExportCharacter();
      }
    }

    private static bool CompleteBlueprintCopy(Player player)
    {
      NwItem bpCopy = ItemUtils.DeserializeAndAcquireItem(player.newCraftJob.serializedCraftedItem, player.oid.LoginCreature);
      player.oid.SendServerMessage($"Vous venez de terminer le travail artisanal : {bpCopy.Name.ColorString(ColorConstants.White)}", ColorConstants.Orange);
      return true;
    }
  }
}
