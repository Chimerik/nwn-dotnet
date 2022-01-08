using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class MateriaDetectorWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn;
        private readonly NuiBind<List<NuiComboEntry>> categories = new NuiBind<List<NuiComboEntry>>("categories");
        private readonly List<NuiComboEntry> resourceCategories;

        private readonly NuiBind<int> selectedCategory = new NuiBind<int>("selectedCategory");
        private readonly NuiBind<string> remainingTime = new NuiBind<string>("remainingTime");
        private readonly NuiBind<string> estimatedQuantity = new NuiBind<string>("estimatedQuantity");
        private readonly NuiBind<string> estimatedDistance = new NuiBind<string>("estimatedDistance");
        private readonly NuiBind<string> estimatedCoordinates = new NuiBind<string>("estimatedCoordinates");

        private bool cancelPreviousDetection { get; set; }
        private int scanDuration { get; set; }
        private NwItem detector { get; set; }

        public MateriaDetectorWindow(Player player, NwItem detector) : base(player)
        {
          windowId = "materiaDetector";
          cancelPreviousDetection = false;

          rootColumn = new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() 
              { 
                new NuiCombo() { Entries = categories, Selected = selectedCategory },
                new NuiButton("Recherche") { Id = "start_detection", Tooltip = "Démarrer la recherche de matéria à proximité" } }
              },
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiText(remainingTime) { Tooltip = "Temps restant avant la fin de la recherche" },
                new NuiText(estimatedQuantity) { Tooltip = "Quantité estimée de la plus volumineuse masse" },
                new NuiText(estimatedDistance) { Tooltip = "Distance estimée de la plus volumineuse masse" },
                new NuiText(estimatedCoordinates) { Tooltip = "Coordonnées estimées de la plus volumineuse masse" }
              } }
            }
          };

          CreateWindow(detector);
        }
        public void CreateWindow(NwItem detector)
        {
          this.detector = detector;
          cancelPreviousDetection = true;

          resourceCategories.Add(new NuiComboEntry("", 0));

          if (player.learnableSkills.ContainsKey(CustomSkill.OreDetection))
            resourceCategories.Add(new NuiComboEntry("Minerai", 1));

          if (player.learnableSkills.ContainsKey(CustomSkill.WoodDetection))
            resourceCategories.Add(new NuiComboEntry("Bois", 2));

          if (player.learnableSkills.ContainsKey(CustomSkill.PeltDetection))
            resourceCategories.Add(new NuiComboEntry("Peaux", 3));

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          window = new NuiWindow(rootColumn, "Détecteur de matéria")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleDetectorEvents;
          player.oid.OnNuiEvent += HandleDetectorEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          selectedCategory.SetBindValue(player.oid, token, 0);
          selectedCategory.SetBindWatch(player.oid, token, true);

          remainingTime.SetBindValue(player.oid, token, "");
          estimatedQuantity.SetBindValue(player.oid, token, "");
          estimatedDistance.SetBindValue(player.oid, token, "");
          estimatedCoordinates.SetBindValue(player.oid, token, "");
          
          categories.SetBindValue(player.oid, token, resourceCategories);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }

        private void HandleDetectorEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "start_detection":

                  if(selectedCategory.GetBindValue(player.oid, token) == 0)
                  {
                    player.oid.SendServerMessage("Impossible d'entamer une recherche sans avoir préciser la catégorie recherchée.", ColorConstants.Red);
                    return;
                  }

                  detector.GetObjectVariable<LocalVariableInt>("_REMAINING_USES").Value -= 1;
                  if (detector.GetObjectVariable<LocalVariableInt>("_REMAINING_USES").Value < 0)
                  {
                    detector.Destroy();
                    player.oid.NuiDestroy(token);
                    player.oid.SendServerMessage("Le détecteur reste interte entre vos mains, vidé de toute substance.", ColorConstants.Red);
                    return;
                  }

                  SetDetectionTime();
                  remainingTime.SetBindValue(player.oid, token, GetReadableDetectionTime());
                  cancelPreviousDetection = true;
                  HandleScanProgress();

                  return;
              }

              break;

            case NuiEventType.Watch:
              if (nuiEvent.ElementId == "selectedCategory")
                cancelPreviousDetection = true;
              break;
          }
        }

        private async void HandleScanProgress()
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();
          Task awaitCancellation = NwTask.WaitUntil(() => player.oid.LoginCreature == null || !player.openedWindows.ContainsKey(windowId) || cancelPreviousDetection == true, tokenSource.Token);
          Task awaitOneSecond = NwTask.Delay(TimeSpan.FromSeconds(1), tokenSource.Token);

          await NwTask.WhenAny(awaitCancellation, awaitOneSecond);
          tokenSource.Cancel();

          if (awaitCancellation.IsCompletedSuccessfully)
          {
            cancelPreviousDetection = false;
            remainingTime.SetBindValue(player.oid, token, "");
            estimatedQuantity.SetBindValue(player.oid, token, "");
            estimatedDistance.SetBindValue(player.oid, token, "");
            estimatedCoordinates.SetBindValue(player.oid, token, "");
            return;
          }

          scanDuration -= 1;
          remainingTime.SetBindValue(player.oid, token, GetReadableDetectionTime());

          if (scanDuration < 1)
          {
            string resourceTemplate = "ore_spawn_wp";
            int resourceDetectionSkill = CustomSkill.OreDetection;
            int resourceAccuracyDetectionSkill = CustomSkill.OreDetectionAccuracy;
            int resourceOrientationSkill = CustomSkill.OreDetectionOrientation;
            int resourceEstimationSkill = CustomSkill.OreDetectionEstimation;
            int resourceFindDistanceSkill = CustomSkill.OreDetectionAdvanced;
            int resourceSpawnChanceSkill = CustomSkill.OreDetectionMastery;

            switch (selectedCategory.GetBindValue(player.oid, token))
            {
              case 2:
                resourceTemplate = "wood_spawn_wp";
                resourceDetectionSkill = CustomSkill.WoodDetection;
                resourceAccuracyDetectionSkill = CustomSkill.WoodDetectionAccuracy;
                resourceOrientationSkill = CustomSkill.WoodDetectionOrientation;
                resourceEstimationSkill = CustomSkill.WoodDetectionEstimation;
                resourceFindDistanceSkill = CustomSkill.WoodDetectionAdvanced;
                resourceSpawnChanceSkill = CustomSkill.WoodDetectionMastery;
                break;
              case 3:
                resourceTemplate = "animal_spawn_wp";
                resourceDetectionSkill = CustomSkill.PeltDetection;
                resourceAccuracyDetectionSkill = CustomSkill.PeltDetectionAccuracy;
                resourceOrientationSkill = CustomSkill.PeltDetectionOrientation;
                resourceEstimationSkill = CustomSkill.PeltDetectionEstimation;
                resourceFindDistanceSkill = CustomSkill.PeltDetectionAdvanced;
                resourceSpawnChanceSkill = CustomSkill.PeltDetectionMastery;
                break;
            }

            var materiaList = player.oid.LoginCreature.Area.FindObjectsOfTypeInArea<NwPlaceable>().Where(m => m.Tag == "mineable_materia" && m.GetObjectVariable<LocalVariableString>("_RESOURCE_TYPE").Value == resourceTemplate);

            if (materiaList.Count() < 1)
            {
              remainingTime.SetBindValue(player.oid, token, "Recherche - Échec");
              return;
            }

            remainingTime.SetBindValue(player.oid, token, "Recherche - Terminée");

            NwPlaceable biggestMateria = materiaList.OrderByDescending(m => m.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value).FirstOrDefault();

            int distance = (int)player.oid.ControlledCreature.Distance(biggestMateria);
            int totalSkillPoints = player.learnableSkills.ContainsKey(resourceAccuracyDetectionSkill) ? player.learnableSkills[resourceDetectionSkill].totalPoints + player.learnableSkills[resourceAccuracyDetectionSkill].totalPoints : player.learnableSkills[resourceDetectionSkill].totalPoints;
            estimatedDistance.SetBindValue(player.oid, token, $"Proximité : {Utils.random.Next((int)(distance * totalSkillPoints * 0.05) - 1, 2 * distance - (int)(distance * totalSkillPoints * 0.05))}");

            if (player.learnableSkills.ContainsKey(resourceOrientationSkill))
            {
              Vector3 position = biggestMateria.Position;
              totalSkillPoints = player.learnableSkills[resourceOrientationSkill].totalPoints;
              int randomX = Utils.random.Next((int)(position.X * totalSkillPoints * 0.05) - 1, (int)(2 * position.X - position.X * totalSkillPoints * 0.05));
              int randomY = Utils.random.Next((int)(position.Y * totalSkillPoints * 0.05) - 1, (int)(2 * position.Y - position.Y * totalSkillPoints * 0.05));
              int randomZ = Utils.random.Next((int)(position.Z * totalSkillPoints * 0.05) - 1, (int)(2 * position.Z - position.Z * totalSkillPoints * 0.05));
              estimatedCoordinates.SetBindValue(player.oid, token, $"Direction : {randomX}X {randomY}Y {randomZ}Z");
            }
            else
              estimatedCoordinates.SetBindValue(player.oid, token, "");

            UpdateResourceBlockInfo(biggestMateria);

            totalSkillPoints = player.learnableSkills.ContainsKey(resourceEstimationSkill) ? player.learnableSkills[resourceDetectionSkill].totalPoints + player.learnableSkills[resourceEstimationSkill].totalPoints : player.learnableSkills[resourceDetectionSkill].totalPoints;
            int availableQuantity = biggestMateria.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value;
            estimatedQuantity.SetBindValue(player.oid, token, $"Masse : {Utils.random.Next((int)(availableQuantity * totalSkillPoints * 0.05) - 1, 2 * availableQuantity - (int)(availableQuantity * totalSkillPoints * 0.05))}");

            int areaLevel = biggestMateria.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value;
            int findDistance = player.learnableSkills.ContainsKey(resourceFindDistanceSkill) ? 2 + player.learnableSkills[resourceDetectionSkill].totalPoints + player.learnableSkills[resourceFindDistanceSkill].totalPoints : 2 + player.learnableSkills[resourceDetectionSkill].totalPoints;
            int detectionChance = player.learnableSkills.ContainsKey(resourceSpawnChanceSkill) ? player.learnableSkills[resourceDetectionSkill].totalPoints + player.learnableSkills[resourceSpawnChanceSkill].totalPoints - (areaLevel - 2) * 15  : player.learnableSkills[resourceDetectionSkill].totalPoints - (areaLevel - 2) * 15;

            if (detectionChance < 1)
            {
              player.oid.SendServerMessage("Votre détecteur indique la présence de matéria dans cette zone, mais votre sensibilité n'est pas suffisament affinée pour déterminer avec précision les points d'extraction.");
              return;
            }

            foreach (NwPlaceable materia in materiaList.Where(m => m.DistanceSquared(player.oid.ControlledCreature) < findDistance * findDistance))
            {
              if(NwRandom.Roll(Utils.random, 100) < detectionChance)
              {
                VisibilityPlugin.SetVisibilityOverride(player.oid.LoginCreature, materia, VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);

                foreach(var partyMember in player.oid.PartyMembers)
                  VisibilityPlugin.SetVisibilityOverride(partyMember.LoginCreature, materia, VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
              }
            }

            return;
          }

          HandleScanProgress();
        }
        private async void UpdateResourceBlockInfo(NwPlaceable resourceBlock)
        {
          if (resourceBlock.GetObjectVariable<DateTimeLocalVariable>("_LAST_CHECK").HasNothing)
            resourceBlock.GetObjectVariable<DateTimeLocalVariable>("_LAST_CHECK").Value = DateTime.Now.AddDays(-3);

          double totalSeconds = (DateTime.Now - resourceBlock.GetObjectVariable<DateTimeLocalVariable>("_LAST_CHECK").Value).TotalSeconds;
          double materiaGrowth = totalSeconds / (5 * resourceBlock.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value);
          resourceBlock.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value += (int)materiaGrowth;
          resourceBlock.GetObjectVariable<DateTimeLocalVariable>("_LAST_CHECK").Value = DateTime.Now;

          string resourceId = resourceBlock.GetObjectVariable<LocalVariableInt>("id").Value.ToString();
          string areaTag = resourceBlock.Area.Tag;
          string resourceType = resourceBlock.GetObjectVariable<LocalVariableString>("_RESOURCE_TYPE").Value;
          string resourceQuantity = resourceBlock.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value.ToString();

          await SqLiteUtils.InsertQueryAsync("areaResourceStock",
            new List<string[]>() { new string[] { "id", resourceId }, new string[] { "areaTag", areaTag }, new string[] { "type", resourceType }, new string[] { "quantity", resourceQuantity }, new string[] { "lastChecked", DateTime.Now.ToString() } },
            new List<string>() { "id", "areaTag", "type" },
            new List<string[]>() { new string[] { "quantity" }, new string[] { "lastChecked" } });
        }
        private void SetDetectionTime()
        {
          scanDuration = 120;

          switch(selectedCategory.GetBindValue(player.oid, token))
          {
            case 1:
              scanDuration -= scanDuration * (int)(player.learnableSkills[CustomSkill.OreDetection].totalPoints * 0.05);
              scanDuration -= player.learnableSkills.ContainsKey(CustomSkill.OreDetectionSpeed) ? scanDuration * (int)(player.learnableSkills[CustomSkill.OreDetectionSpeed].totalPoints * 0.05) : 0;
              break;
            case 2:
              scanDuration -= scanDuration * (int)(player.learnableSkills[CustomSkill.WoodDetection].totalPoints * 0.05);
              scanDuration -= player.learnableSkills.ContainsKey(CustomSkill.WoodDetectionSpeed) ? scanDuration * (int)(player.learnableSkills[CustomSkill.WoodDetectionSpeed].totalPoints * 0.05) : 0;
              break;
            case 3:
              scanDuration -= scanDuration * (int)(player.learnableSkills[CustomSkill.PeltDetection].totalPoints * 0.05);
              scanDuration -= player.learnableSkills.ContainsKey(CustomSkill.PeltDetectionSpeed) ? scanDuration * (int)(player.learnableSkills[CustomSkill.PeltDetectionSpeed].totalPoints * 0.05) : 0;
              break;
          }
        }
        private string GetReadableDetectionTime()
        {
          return new TimeSpan(TimeSpan.FromSeconds(scanDuration).Hours, TimeSpan.FromSeconds(scanDuration).Minutes, TimeSpan.FromSeconds(scanDuration).Seconds).ToString();
        }
      }
    }
  }
}
