using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class MateriaDetectorWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn;
        private readonly NuiBind<List<NuiComboEntry>> categories = new("categories");
        private readonly List<NuiComboEntry> resourceCategories = new();

        private readonly NuiBind<int> selectedCategory = new ("selectedCategory");
        private readonly NuiBind<string> remainingTime = new ("remainingTime");
        private readonly NuiBind<string> estimatedQuantity = new ("estimatedQuantity");
        private readonly NuiBind<string> estimatedDistance = new ("estimatedDistance");
        private readonly NuiBind<string> estimatedCoordinates = new ("estimatedCoordinates");
        private int scanDuration { get; set; }
        private NwItem detector { get; set; }
        public ScheduledTask detectionProgress { get; set; }
        private string resourceTemplate = "ore_spawn_wp";
        private int resourceDetectionSkill = CustomSkill.OreDetection;
        private int resourceAccuracyDetectionSkill = CustomSkill.OreDetectionAccuracy;
        private int resourceOrientationSkill = CustomSkill.OreDetectionOrientation;
        private int resourceEstimationSkill = CustomSkill.OreDetectionEstimation;
        private int resourceFindDistanceSkill = CustomSkill.OreDetectionAdvanced;
        private int resourceSpawnChanceSkill = CustomSkill.OreDetectionMastery;

        public MateriaDetectorWindow(Player player, NwItem detector) : base(player)
        {
          windowId = "materiaDetector";

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

          resourceCategories.Clear();
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

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            nuiToken.OnNuiEvent += HandleDetectorEvents;
            player.oid.OnServerSendArea += OnAreaChangeCloseWindow;

            selectedCategory.SetBindValue(player.oid, nuiToken.Token, 0);
            selectedCategory.SetBindWatch(player.oid, nuiToken.Token, true);

            remainingTime.SetBindValue(player.oid, nuiToken.Token, "");
            estimatedQuantity.SetBindValue(player.oid, nuiToken.Token, "");
            estimatedDistance.SetBindValue(player.oid, nuiToken.Token, "");
            estimatedCoordinates.SetBindValue(player.oid, nuiToken.Token, "");

            categories.SetBindValue(player.oid, nuiToken.Token, resourceCategories);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }

        private void HandleDetectorEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "start_detection":

                  if(selectedCategory.GetBindValue(player.oid, nuiToken.Token) == 0)
                  {
                    player.oid.SendServerMessage("Impossible d'entamer une recherche sans avoir préciser la catégorie recherchée.", ColorConstants.Red);
                    return;
                  }

                  detector.GetObjectVariable<LocalVariableInt>("_REMAINING_USES").Value -= 1;
                  if (detector.GetObjectVariable<LocalVariableInt>("_REMAINING_USES").Value < 0)
                  {
                    detector.Destroy();
                    CloseWindow();
                    player.oid.SendServerMessage("Le détecteur se désagrège entre vos mains, vidé de toute substance.", ColorConstants.Red);
                    return;
                  }

                  SetDetectionTime();
                  remainingTime.SetBindValue(player.oid, nuiToken.Token, GetReadableDetectionTime());

                  if (!detectionProgress.IsCancelled)
                    CancelScanProgress();

                  detectionProgress = player.scheduler.ScheduleRepeating(HandleScanProgress, TimeSpan.FromSeconds(1));

                  return;
              }

              break;

            case NuiEventType.Watch:
              if (nuiEvent.ElementId == "selectedCategory")
                if (!detectionProgress.IsCancelled)
                  CancelScanProgress();
              break;
          }
        }

        private void HandleScanProgress()
        {
          if(player.oid.LoginCreature == null || !player.openedWindows.ContainsKey(windowId))
          {
            CancelScanProgress();
            return;
          }

          scanDuration -= 1;
          remainingTime.SetBindValue(player.oid, nuiToken.Token, GetReadableDetectionTime());

          if (scanDuration < 1)
          {
            detectionProgress.Dispose();

            SelectDetectionSkill(selectedCategory.GetBindValue(player.oid, nuiToken.Token));

            var materiaList = player.oid.LoginCreature.Area.FindObjectsOfTypeInArea<NwPlaceable>().Where(m => m.Tag == "mineable_materia" && m.GetObjectVariable<LocalVariableString>("_RESOURCE_TYPE").Value == resourceTemplate);

            if (!materiaList.Any())
            {
              remainingTime.SetBindValue(player.oid, nuiToken.Token, "Recherche - Échec");
              return;
            }

            remainingTime.SetBindValue(player.oid, nuiToken.Token, "Recherche - Terminée");

            NwPlaceable biggestMateria = materiaList.OrderByDescending(m => m.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value).FirstOrDefault();

            EstimateDistance(biggestMateria);

            if (player.learnableSkills.ContainsKey(resourceOrientationSkill))
              EstimateCoordinates(biggestMateria);
            else
              estimatedCoordinates.SetBindValue(player.oid, nuiToken.Token, "");

            Craft.Collect.System.UpdateResourceBlockInfo(biggestMateria);
            EstimateQuantity(biggestMateria);
            MateriaRevealCheck(materiaList);

            return;
          }
        }
        private void CancelScanProgress()
        {
          detectionProgress.Dispose();
          remainingTime.SetBindValue(player.oid, nuiToken.Token, "");
          estimatedQuantity.SetBindValue(player.oid, nuiToken.Token, "");
          estimatedDistance.SetBindValue(player.oid, nuiToken.Token, "");
          estimatedCoordinates.SetBindValue(player.oid, nuiToken.Token, "");
        }
        private void SelectDetectionSkill(int resourceType)
        {
          switch (resourceType)
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
        }
        private void EstimateDistance(NwPlaceable materia)
        {
          int totalSkillPoints = player.learnableSkills.ContainsKey(resourceAccuracyDetectionSkill) ? player.learnableSkills[resourceDetectionSkill].totalPoints + player.learnableSkills[resourceAccuracyDetectionSkill].totalPoints : player.learnableSkills[resourceDetectionSkill].totalPoints;
          int distance = (int)player.oid.ControlledCreature.Distance(materia);
          int previousEstimation = materia.GetObjectVariable<LocalVariableInt>($"_DISTANCE_ESTIMATE_{player.characterId}").Value;
          int newEstimation = Utils.random.Next((int)(distance * totalSkillPoints * 0.05) - 1, 2 * distance - (int)(distance * totalSkillPoints * 0.05));

          distance = distance - previousEstimation < distance - newEstimation ? previousEstimation : newEstimation;
          estimatedDistance.SetBindValue(player.oid, nuiToken.Token, $"Proximité : {distance}");

          materia.GetObjectVariable<LocalVariableInt>($"_DISTANCE_ESTIMATE_{player.characterId}").Value = distance;
        }
        private void EstimateCoordinates(NwPlaceable materia)
        {
          int skillPoints = player.learnableSkills[resourceOrientationSkill].totalPoints;
          Location previousEstimation = materia.GetObjectVariable<LocalVariableLocation>($"_LOCATION_ESTIMATE_{player.characterId}").Value;

          int randomX = Utils.random.Next((int)(materia.Position.X * skillPoints * 0.05) - 1, (int)(2 * materia.Position.X - materia.Position.X * skillPoints * 0.05));
          int randomY = Utils.random.Next((int)(materia.Position.Y * skillPoints * 0.05) - 1, (int)(2 * materia.Position.Y - materia.Position.Y * skillPoints * 0.05));
          int randomZ = Utils.random.Next((int)(materia.Position.Z * skillPoints * 0.05) - 1, (int)(2 * materia.Position.Z - materia.Position.Z * skillPoints * 0.05));
          
          int newX = materia.Position.X - previousEstimation.Position.X < materia.Position.X - randomX ? (int)previousEstimation.Position.X : randomX;
          int newY = materia.Position.Y - previousEstimation.Position.Y < materia.Position.Y - randomY ? (int)previousEstimation.Position.Y : randomY;
          int newZ = materia.Position.Z - previousEstimation.Position.Z < materia.Position.Z - randomX ? (int)previousEstimation.Position.Z : randomZ;

          estimatedCoordinates.SetBindValue(player.oid, nuiToken.Token, $"Direction : {newX}X {newY}Y {newZ}Z");

          materia.GetObjectVariable<LocalVariableLocation>($"_LOCATION_ESTIMATE_{player.characterId}").Value =  Location.Create(materia.Area, new Vector3(newX, newY, newZ), materia.Rotation);
        }
        private void EstimateQuantity(NwPlaceable materia)
        {
          int realQuantity = materia.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value;
          int skillPoints = player.learnableSkills.ContainsKey(resourceEstimationSkill) ? player.learnableSkills[resourceDetectionSkill].totalPoints + player.learnableSkills[resourceEstimationSkill].totalPoints : player.learnableSkills[resourceDetectionSkill].totalPoints;
          int previousEstimation = materia.GetObjectVariable<LocalVariableInt>($"_QUANTITY_ESTIMATE_{player.characterId}").Value;
          int newEstimate = Utils.random.Next((int)(realQuantity * skillPoints * 0.05) - 1, 2 * realQuantity - (int)(realQuantity * skillPoints * 0.05));

          newEstimate = realQuantity - previousEstimation < realQuantity - newEstimate ? previousEstimation : newEstimate;
          estimatedQuantity.SetBindValue(player.oid, nuiToken.Token, $"Masse : {newEstimate}");

          materia.GetObjectVariable<LocalVariableInt>($"_QUANTITY_ESTIMATE_{player.characterId}").Value = newEstimate;
        }
        private void MateriaRevealCheck(IEnumerable<NwPlaceable> materiaList)
        {
          int areaLevel = player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value;
          int findDistance = player.learnableSkills.ContainsKey(resourceFindDistanceSkill) ? 2 + player.learnableSkills[resourceDetectionSkill].totalPoints + player.learnableSkills[resourceFindDistanceSkill].totalPoints : 2 + player.learnableSkills[resourceDetectionSkill].totalPoints;
          int detectionChance = player.learnableSkills.ContainsKey(resourceSpawnChanceSkill) ? player.learnableSkills[resourceDetectionSkill].totalPoints + player.learnableSkills[resourceSpawnChanceSkill].totalPoints - (areaLevel - 2) * 15 : player.learnableSkills[resourceDetectionSkill].totalPoints - (areaLevel - 2) * 15;

          if (detectionChance < 1)
          {
            player.oid.SendServerMessage("Votre détecteur indique la présence de matéria dans cette zone, mais votre sensibilité n'est pas suffisament affinée pour déterminer avec précision les points d'extraction.");
            return;
          }

          foreach (NwPlaceable materia in materiaList.Where(m => m.DistanceSquared(player.oid.ControlledCreature) < findDistance * findDistance))
          {
            if (NwRandom.Roll(Utils.random, 100) < detectionChance)
            {
              player.oid.SetPersonalVisibilityOverride(materia, Anvil.Services.VisibilityMode.Visible);

              foreach (var partyMember in player.oid.PartyMembers)
                partyMember.SetPersonalVisibilityOverride(materia, Anvil.Services.VisibilityMode.Visible);
            }
          }
        }
        private void SetDetectionTime()
        {
          scanDuration = 120;

          switch(selectedCategory.GetBindValue(player.oid, nuiToken.Token))
          {
            case 1:
              scanDuration = Craft.Collect.System.GetResourceDetectionTime(player, CustomSkill.OreDetection, CustomSkill.OreDetectionSpeed);
              break;
            case 2:
              scanDuration = Craft.Collect.System.GetResourceDetectionTime(player, CustomSkill.WoodDetection, CustomSkill.WoodDetectionSpeed);
              break;
            case 3:
              scanDuration = Craft.Collect.System.GetResourceDetectionTime(player, CustomSkill.PeltDetection, CustomSkill.PeltDetectionSpeed);
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
