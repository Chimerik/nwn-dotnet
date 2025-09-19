using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
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
        private readonly List<NuiListTemplateCell> rowTemplate = new();

        private readonly NuiBind<int> selectedMode = new("selectedMode");
        private readonly NuiBind<float> progress = new("progress");
        private readonly NuiBind<string> readableRemainingTime = new ("readableRemainingTime");
        private readonly NuiBind<string> materiaType = new ("materiaType");
        private readonly NuiBind<string> areaDistance = new("areaDistance");
        private readonly NuiBind<string> meterDistance = new ("meterDistance");
        private readonly NuiBind<string> estimatedQuantity = new ("estimatedQuantity");
        private readonly Color white = new(255, 255, 255);
        private readonly NuiBind<NuiRect> drawListRect = new("drawListRect");
        private readonly NuiBind<int> listCount = new("listCount");
        private double scanDuration { get; set; }
        private double timeLeft { get; set; }
        private NwItem detector { get; set; }
        public ScheduledTask scanProgress { get; set; }
        private readonly string resourceTemplate = "mineable_rock";

        public MateriaDetectorWindow(Player player) : base(player)
        {
          windowId = "materiaDetector";

          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(materiaType) { Tooltip = "Type de matéria", VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { Width = 60 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(areaDistance) { Tooltip = "Rayon de détection", VerticalAlign = NuiVAlign.Middle }) { Width = 60 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(meterDistance) { Tooltip = "Distance", VerticalAlign = NuiVAlign.Middle }) { Width = 185 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(estimatedQuantity) { Tooltip = "Quantité estimée", VerticalAlign = NuiVAlign.Middle }) { Width = 185 });

          rootColumn = new NuiColumn() { Children = new List<NuiElement>()
          {
            new NuiRow() { Children = new List<NuiElement>() 
            { 
              new NuiSpacer(),
              new NuiCombo() { Entries = new List<NuiComboEntry>() { new ("Mode Passif", 0), new("Mode Actif", 1) }, Selected = selectedMode, Height = 35, Width = 160 },
              new NuiButton("Recherche") { Id = "start_detection", Tooltip = "Démarrer la recherche de matéria à proximité", Height = 35, Width = 160 },
              new NuiSpacer()
            } },
            new NuiRow() { Children = new List<NuiElement>()
            {
              new NuiProgress(progress) { Width = 485, Height = 35, DrawList = new List<NuiDrawListItem>() {
                  new NuiDrawListText(white, drawListRect, readableRemainingTime) } }
            } },
            new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } }
          } };

          CreateWindow();
        }
        public void CreateWindow()
        {
          detector = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? new NuiRect(value.X, value.Y, 500, 400) : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 500, 400);

          window = new NuiWindow(rootColumn, "Détecteur de matéria")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {

            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleDetectorEvents;
            player.oid.OnClientLeave += OnLeaveCancelDetection;

            selectedMode.SetBindValue(player.oid, nuiToken.Token, 0);

            drawListRect.SetBindValue(player.oid, nuiToken.Token, new(300, 15, 151, 20));
            progress.SetBindValue(player.oid, nuiToken.Token, 0);

            readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, "");

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            listCount.SetBindValue(player.oid, nuiToken.Token, 0);
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

                  if (scanProgress != null && !scanProgress.IsCancelled)
                    CancelScanProgress();

                  StartScan();

                  return;
              }

              break;
          }
        }
        private void StartScan()
        {
          GetResourceDetectionTime();
          progress.SetBindValue(player.oid, nuiToken.Token, 0);
          readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, GetReadableDetectionTime());
          scanProgress = player.scheduler.ScheduleRepeating(HandleScanProgress, TimeSpan.FromSeconds(1));
        }

        private void HandleScanProgress()
        {
          try
          {
            if (player.oid.LoginCreature == null || !IsOpen)
            {
              CancelScanProgress();
              return;
            }

            timeLeft -= 1;
            readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, GetReadableDetectionTime());
            progress.SetBindValue(player.oid, nuiToken.Token, (float)((scanDuration - timeLeft) / scanDuration));

            if (timeLeft < 1)
            {
              scanProgress.Dispose();

              if (selectedMode.GetBindValue(player.oid, nuiToken.Token) > 0)
                HandleActiveScan();
              else
                HandlePassiveScan();

              ItemUtils.HandleCraftToolDurability(player, detector, CustomInscription.MateriaDetectionDurability, CustomSkill.InfluxDetectionSafe);
            }
          }
          catch (Exception e) 
          {
            Utils.LogMessageToDMs($"MATERIA SCANNING - {e.Message}\n\n{e.StackTrace}");
          }
        }
        private void HandlePassiveScan()
        {
          int detectorRange = player.learnableSkills.TryGetValue(CustomSkill.InfluxDetectionRange, out var value) ? value.totalPoints : 0 ;
          Dictionary<NwArea, int> scannedAreas = new();

          AddDoorAreaToScanRange(player.oid.LoginCreature.Area, scannedAreas, detectorRange, 0);
          AddTransitionAreaToScanRange(player.oid.LoginCreature.Area, scannedAreas, detectorRange, 0);

          scannedAreas.Remove(player.oid.LoginCreature.Area);
          IEnumerable<NwPlaceable> materiaList = player.oid.LoginCreature.Area.FindObjectsOfTypeInArea<NwPlaceable>().Where(m => m.Tag == "mineable_materia" && m.ResRef == resourceTemplate && m.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value > 5000);

          foreach(var area in scannedAreas.Keys)
            materiaList = materiaList.Concat(area.FindObjectsOfTypeInArea<NwPlaceable>().Where(m => m.Tag == "mineable_materia" && m.ResRef == resourceTemplate && m.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value > 5000));

          if (!materiaList.Any())
          {
            readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, "Recherche - Échec");
            //progress.SetBindValue(player.oid, nuiToken.Token, 0);
            return;
          }

          materiaList = materiaList.OrderBy(m => m.DistanceSquared(player.oid.LoginCreature));

          readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, "Recherche - Terminée");

          List<string> typeList = new List<string>();
          List<string> areaDistanceList = new List<string>();
          List<string> distanceList = new List<string>();
          List<string> quantityList = new List<string>(); 

          foreach (var materia in materiaList)
          {
            typeList.Add(materia.GetObjectVariable<LocalVariableInt>("_GRADE").Value.ToString());
            areaDistanceList.Add(scannedAreas.ContainsKey(materia.Area) ? scannedAreas[materia.Area].ToString() : "0");
            distanceList.Add(materia.Area == player.oid.LoginCreature.Area ? ((int)materia.Distance(player.oid.LoginCreature)).ToString() : "-");
            quantityList.Add(EstimateQuantity(materia).ToString());
          }

          materiaType.SetBindValues(player.oid, nuiToken.Token, typeList);
          areaDistance.SetBindValues(player.oid, nuiToken.Token, areaDistanceList);
          meterDistance.SetBindValues(player.oid, nuiToken.Token, distanceList);
          estimatedQuantity.SetBindValues(player.oid, nuiToken.Token, quantityList);
          listCount.SetBindValue(player.oid, nuiToken.Token, materiaList.Count());
        }

        private void AddDoorAreaToScanRange(NwArea area, Dictionary<NwArea, int> scannedAreas, int detectorRange, int distance)
        {
          if (detectorRange > distance)
            return;

          foreach (var door in area.FindObjectsOfTypeInArea<NwDoor>())
            if (door.TransitionTarget != null && door.TransitionTarget.Area != null)
              if (scannedAreas.TryAdd(door.TransitionTarget.Area, distance + 1))
                AddDoorAreaToScanRange(door.TransitionTarget.Area, scannedAreas, detectorRange, distance + 1);
        }

        private void AddTransitionAreaToScanRange(NwArea area, Dictionary<NwArea, int> scannedAreas, int detectorRange, int distance)
        {
          if (detectorRange > distance)
            return;

          foreach (var transition in area.FindObjectsOfTypeInArea<NwTrigger>())
            if (transition.TransitionTarget != null && transition.TransitionTarget.Area != null)
              if (scannedAreas.TryAdd(transition.TransitionTarget.Area, distance + 1))
                AddDoorAreaToScanRange(transition.TransitionTarget.Area, scannedAreas, detectorRange, distance + 1);
        }

        private void HandleActiveScan()
        {
          NwArea area = player.oid.LoginCreature.Area;
          var materiaList = area.FindObjectsOfTypeInArea<NwPlaceable>().Where(m => m.Tag == "mineable_materia").OrderBy(m => m.DistanceSquared(player.oid.LoginCreature));

          if (!materiaList.Any()) // S'il n'y a aucune matéria dans la zone, on vérifie si un bloc peut spawn
          {
            readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, "Recherche - Échec");

            if (area.GetObjectVariable<LocalVariableInt>("_CAVE").HasNothing && area.Tag != "Alphazone01")
              return; // Si un bloc peut spawn dans la zone, alors on vérifie l'enchantement du détecteur et la compétence de l'utilisateur pour en faire spawn un

            MateriaRevealCheck();
          }

          readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, "Recherche - Terminée");

          List<string> typeList = new List<string>();
          List<string> areaDistanceList = new List<string>();
          List<string> distanceList = new List<string>();
          List<string> quantityList = new List<string>();

          foreach (var materia in materiaList)
          {
            typeList.Add(materia.GetObjectVariable<LocalVariableInt>("_GRADE").Value.ToString());
            areaDistanceList.Add("-");
            distanceList.Add(((int)materia.Distance(player.oid.LoginCreature)).ToString());
            quantityList.Add(EstimateQuantity(materia).ToString());
          }

          materiaType.SetBindValues(player.oid, nuiToken.Token, typeList);
          areaDistance.SetBindValues(player.oid, nuiToken.Token, areaDistanceList);
          meterDistance.SetBindValues(player.oid, nuiToken.Token, distanceList);
          estimatedQuantity.SetBindValues(player.oid, nuiToken.Token, quantityList);
          listCount.SetBindValue(player.oid, nuiToken.Token, materiaList.Count());
        }

        private void OnLeaveCancelDetection(ModuleEvents.OnClientLeave onLeave)
        {
          if (scanProgress != null)
            CancelScanProgress();
        }

        private void CancelScanProgress()
        {
          scanProgress.Dispose();
          readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, "");
          progress.SetBindValue(player.oid, nuiToken.Token, 0);
        }

        private int EstimateQuantity(NwPlaceable materia)
        {
          int realQuantity = materia.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value;
          int skillPoints = player.learnableSkills.TryGetValue(CustomSkill.InfluxDetection, out var skill) ? skill.totalPoints : 0;
          skillPoints += player.learnableSkills.TryGetValue(CustomSkill.InfluxDetectionEstimation, out skill) ? skill.totalPoints : 0;
          skillPoints += player.learnableSkills.TryGetValue(CustomSkill.InfluxDetectionAdvanced, out skill) ? skill.totalPoints : 0;
          skillPoints += player.learnableSkills.TryGetValue(CustomSkill.InfluxDetectionMastery, out skill) ? skill.totalPoints : 0;

          if (detector is not null)
          {
            for (int i = 0; i < detector.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
            {
              if (detector.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
                continue;

              switch (detector.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
              {
                case CustomInscription.MateriaDetectionAccuracyMinor: skillPoints += 2; break;
                case CustomInscription.MateriaDetectionAccuracy: skillPoints += 4; break;
                case CustomInscription.MateriaDetectionAccuracyMajor: skillPoints += 6; break;
                case CustomInscription.MateriaDetectionAccuracySupreme: skillPoints += 8; break;
              }
            }
          }

          return Utils.random.Next((int)(realQuantity * skillPoints * 0.05) - 1, 2 * realQuantity - (int)(realQuantity * skillPoints * 0.05));
        }

        private async void MateriaRevealCheck()
        {
          NwArea area = player.oid.LoginCreature.Area;
          int areaLevel = area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value;
          int skillPoints = player.learnableSkills.TryGetValue(CustomSkill.InfluxDetection, out var value) ? value.totalPoints : 0;
          skillPoints += player.learnableSkills.TryGetValue(CustomSkill.InfluxDetectionAdvanced, out value) ? value.totalPoints : 0;
          skillPoints += player.learnableSkills.TryGetValue(CustomSkill.InfluxDetectionMastery, out value) ? value.totalPoints : 0;

          if (detector is not null)
          {
            for (int i = 0; i < detector.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
            {
              if (detector.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
                continue;

              switch (detector.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
              {
                case CustomInscription.MateriaDetectionReliabilityMinor: skillPoints += 2; break;
                case CustomInscription.MateriaDetectionReliability: skillPoints += 4; break;
                case CustomInscription.MateriaDetectionReliabilityMajor: skillPoints += 6; break;
                case CustomInscription.MateriaDetectionReliabilitySupreme: skillPoints += 8; break;
              }
            }
          }

          if(NwRandom.Roll(Utils.random, 100) < skillPoints)
          {
            Location randomLocation;

            if (area.Tag != "Alphazone01")
            {
              randomLocation = await Utils.GetRandomLocationInArea(area);
              await NwTask.SwitchToMainThread();

              if (randomLocation is null)
              {
                player.oid.SendServerMessage("HRP - La détection active de matéria a échoué en raison d'un problème technique. Le staff a été prévenu", ColorConstants.Red);
                return;
              }
            }
            else
            {
              randomLocation = Location.Create(area, CreaturePlugin.ComputeSafeLocation(player.oid.ControlledCreature, player.oid.ControlledCreature.Position, 6, 1), player.oid.ControlledCreature.Rotation);
            }

            NwPlaceable newResourceBlock = NwPlaceable.Create(resourceTemplate, randomLocation, false, "mineable_materia");
            int grade = Utils.GetSpawnedMateriaGrade(areaLevel);

            if (grade < 8)
            {
              skillPoints = player.learnableSkills.TryGetValue(CustomSkill.InfluxDetection, out value) ? value.totalPoints : 0;
              skillPoints += player.learnableSkills.TryGetValue(CustomSkill.InfluxDetectionAccuracy, out value) ? value.totalPoints : 0;
              skillPoints += player.learnableSkills.TryGetValue(CustomSkill.InfluxDetectionAdvanced, out value) ? value.totalPoints : 0;
              skillPoints += player.learnableSkills.TryGetValue(CustomSkill.InfluxDetectionMastery, out value) ? value.totalPoints : 0;

              if (detector is not null)
              {
                for (int i = 0; i < detector.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
                {
                  if (detector.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
                    continue;

                  if (detector.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == CustomInscription.MateriaDetectionQuality)
                    skillPoints += 6;

                  switch (detector.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
                  {
                    case CustomInscription.MateriaDetectionQualityMinor: skillPoints += 2; break;
                    case CustomInscription.MateriaDetectionQuality: skillPoints += 4; break;
                    case CustomInscription.MateriaDetectionQualityMajor: skillPoints += 6; break;
                    case CustomInscription.MateriaDetectionQualitySupreme: skillPoints += 8; break;
                  }
                }
              }

              if (NwRandom.Roll(Utils.random, 100) < skillPoints)
                grade++;
            }

            newResourceBlock.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = (int)(Utils.random.NextDouble(Config.minSmallMateriaSpawnYield, Config.maxSmallMateriaSpawnYield) * (1.00 + (areaLevel - grade) * Config.baseMateriaGrowthMultiplier) * ((100 + skillPoints) / 100));
            newResourceBlock.GetObjectVariable<LocalVariableInt>("_GRADE").Value = grade;
            Utils.SetResourceBlockData(newResourceBlock);
          }
        }

        public void GetResourceDetectionTime()
        {
          scanDuration = Config.env == Config.Env.Prod ? Config.scanBaseDuration : 10;
          scanDuration *= 1 - player.learnableSkills[CustomSkill.InfluxDetection].totalPoints * 0.05;
          scanDuration *= player.learnableSkills.TryGetValue(CustomSkill.InfluxDetectionSpeed, out var value) ? 1 - value.totalPoints * 0.05 : 1;
          scanDuration *= player.learnableSkills.TryGetValue(CustomSkill.InfluxDetectionAdvanced, out value) ? 1 - value.totalPoints * 0.05 : 1;
          scanDuration *= player.learnableSkills.TryGetValue(CustomSkill.InfluxDetectionMastery, out value) ? 1 - value.totalPoints * 0.05 : 1;

          if (detector is not null)
          {
            for (int i = 0; i < detector.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
            {
              if (detector.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
                continue;

              switch (detector.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
              {
                case CustomInscription.MateriaDetectionSpeedMinor: scanDuration *= 0.98; break;
                case CustomInscription.MateriaDetectionSpeed: scanDuration *= 0.96; break;
                case CustomInscription.MateriaDetectionSpeedMajor: scanDuration *= 0.94; break;
                case CustomInscription.MateriaDetectionSpeedSupreme: scanDuration *= 0.92; break;
              }
            }
          }

          timeLeft = scanDuration;
        }
        private string GetReadableDetectionTime()
        {
          return new TimeSpan(TimeSpan.FromSeconds(timeLeft).Hours, TimeSpan.FromSeconds(timeLeft).Minutes, TimeSpan.FromSeconds(timeLeft).Seconds).ToString();
        }
      }
    }
  }
}
