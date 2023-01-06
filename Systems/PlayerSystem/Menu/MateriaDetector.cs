using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

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
        private readonly List<NuiComboEntry> resourceCategories = new() { new NuiComboEntry("Dépot minéral", 1), new NuiComboEntry("Dépot végétal", 2), new NuiComboEntry("Peaux", 3) };

        private readonly NuiBind<int> selectedCategory = new ("selectedCategory");
        private readonly NuiBind<int> selectedMode = new("selectedCategory");
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
        private int timeLeft { get; set; }
        private NwItem detector { get; set; }
        public ScheduledTask scanProgress { get; set; }
        private string resourceTemplate = "mineable_rock";
        private int resourceDetectionSkill = CustomSkill.OreDetection;
        private int resourceRangeDetectionSkill = CustomSkill.OreDetectionRange;
        private int resourceDurabilitySkill = CustomSkill.OreDetectionSafe;
        private int resourceEstimationSkill = CustomSkill.OreDetectionEstimation;
        private int resourceAdvancedSkill = CustomSkill.OreDetectionAdvanced;
        private int resourceMasterSkill = CustomSkill.OreDetectionMastery;

        public MateriaDetectorWindow(Player player, NwItem detector) : base(player)
        {
          windowId = "materiaDetector";

          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(materiaType) { Tooltip = "Type de matéria", VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { Width = 150 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(areaDistance) { Tooltip = "Rayon de détection", VerticalAlign = NuiVAlign.Middle }) { Width = 150 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(meterDistance) { Tooltip = "Distance", VerticalAlign = NuiVAlign.Middle }) { Width = 150 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(estimatedQuantity) { Tooltip = "Quantité estimée", VerticalAlign = NuiVAlign.Middle }) { Width = 150 });

          rootColumn = new NuiColumn() { Children = new List<NuiElement>()
          {
            new NuiRow() { Children = new List<NuiElement>() 
            { 
              new NuiSpacer(),
              new NuiCombo() { Entries = resourceCategories, Selected = selectedCategory, Height = 35, Width = 100 },
              new NuiCombo() { Entries = new List<NuiComboEntry>() { new NuiComboEntry("Mode Passif", 0), new NuiComboEntry("Mode Actif", 1) }, Selected = selectedMode, Height = 35, Width = 100 },
              new NuiButton("Recherche") { Id = "start_detection", Tooltip = "Démarrer la recherche de matéria à proximité", Height = 35, Width = 80 },
              new NuiSpacer()
            } },
            new NuiRow() { Children = new List<NuiElement>()
            {
              new NuiProgress(progress) { Width = 460, Height = 35, DrawList = new List<NuiDrawListItem>() {
                  new NuiDrawListText(white, drawListRect, readableRemainingTime) } }
            } },
            new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } }
          } };
          
          CreateWindow(detector);
        }
        public void CreateWindow(NwItem detector)
        {
          this.detector = detector;
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? new NuiRect(player.windowRectangles[windowId].X, player.windowRectangles[windowId].Y, 500, 160) : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 500, 160);

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

            selectedCategory.SetBindValue(player.oid, nuiToken.Token, resourceCategories.FirstOrDefault().Value);
            selectedCategory.SetBindWatch(player.oid, nuiToken.Token, true);
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

            case NuiEventType.Watch:

              if (nuiEvent.ElementId == "selectedCategory" && !scanProgress.IsCancelled)
                  CancelScanProgress();

              break;
          }
        }
        private void StartScan()
        {
          SetDetectionTime();
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

              SelectDetectionSkill(selectedCategory.GetBindValue(player.oid, nuiToken.Token));

              if (selectedMode.GetBindValue(player.oid, nuiToken.Token) > 0)
                HandleActiveScan();
              else
                HandlePassiveScan();
            }
          }
          catch (Exception e) 
          {
            Utils.LogMessageToDMs($"MATERIA SCANNING - {e.Message}\n\n{e.StackTrace}");
          }
        }
        private void HandlePassiveScan()
        {
          int detectorRange = player.learnableSkills.ContainsKey(resourceRangeDetectionSkill) ? player.learnableSkills[resourceRangeDetectionSkill].totalPoints : 0 ;
          Dictionary<NwArea, int> scannedAreas = new();

          AddDoorAreaToScanRange(player.oid.LoginCreature.Area, scannedAreas, detectorRange, 0);
          AddTransitionAreaToScanRange(player.oid.LoginCreature.Area, scannedAreas, detectorRange, 0);

          scannedAreas.Remove(player.oid.LoginCreature.Area);
          IEnumerable<NwPlaceable> materiaList = player.oid.LoginCreature.Area.FindObjectsOfTypeInArea<NwPlaceable>().Where(m => m.Tag == "mineable_materia" && m.ResRef == resourceTemplate && m.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value > 5000);

          foreach(var area in scannedAreas.Keys)
            materiaList = materiaList.Concat(area.FindObjectsOfTypeInArea<NwPlaceable>().Where(m => m.Tag == "mineable_materia" && m.ResRef == resourceTemplate && m.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value > 5000));

          if (!materiaList.Any())
          {
            readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, "Recherche Passive : Aucun dépôt");
            //progress.SetBindValue(player.oid, nuiToken.Token, 0);
            return;
          }

          materiaList = materiaList.OrderBy(m => m.DistanceSquared(player.oid.LoginCreature));

          readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, "Recherche Passive : Terminée");

          List<string> typeList = new List<string>();
          List<string> areaDistanceList = new List<string>();
          List<string> distanceList = new List<string>();
          List<string> quantityList = new List<string>(); 

          foreach (var materia in materiaList)
          {
            typeList.Add((player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value - 1).ToString());
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
          var materiaList = area.FindObjectsOfTypeInArea<NwPlaceable>().Where(m => m.Tag == "mineable_materia" && m.ResRef == resourceTemplate).OrderBy(m => m.DistanceSquared(player.oid.LoginCreature));

          if (!materiaList.Any()) // S'il n'y a aucune matéria dans la zone, on vérifie si un bloc peut spawn dans la zone
          {
            readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, "Recherche Active - Échec");

            switch (resourceTemplate)
            {
              case "mineable_rock":

                if (area.GetObjectVariable<LocalVariableInt>("_CAVE").HasNothing)
                  return;
                  
                break;
              case "mineable_tree":

                if (area.GetObjectVariable<LocalVariableInt>("_FOREST").HasNothing)
                  return;

                break;

              case "mineable_animal":

                if (area.GetObjectVariable<LocalVariableInt>("_WATER").HasNothing)
                  return;
                
                break;
            } // Si un bloc peut spawn dans la zone, alors on vérifie l'enchantement du détecteur et la compétence de l'utilisateur pour en faire spawn un

            MateriaRevealCheck();
          }

          readableRemainingTime.SetBindValue(player.oid, nuiToken.Token, "Recherche - Terminée");

          List<string> typeList = new List<string>();
          List<string> areaDistanceList = new List<string>();
          List<string> distanceList = new List<string>();
          List<string> quantityList = new List<string>();

          foreach (var materia in materiaList)
          {
            typeList.Add((player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value - 1).ToString());
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
        private void SelectDetectionSkill(int resourceType)
        {
          switch (resourceType)
          {
            case 2:
              resourceTemplate = "mineable_tree";
              resourceDetectionSkill = CustomSkill.WoodDetection;
              resourceRangeDetectionSkill = CustomSkill.WoodDetectionRange;
              resourceDurabilitySkill = CustomSkill.WoodDetectionSafe;
              resourceEstimationSkill = CustomSkill.WoodDetectionEstimation;
              resourceAdvancedSkill = CustomSkill.WoodDetectionAdvanced;
              resourceMasterSkill = CustomSkill.WoodDetectionMastery;
              break;
            case 3:
              resourceTemplate = "mineable_animal";
              resourceDetectionSkill = CustomSkill.PeltDetection;
              resourceRangeDetectionSkill = CustomSkill.OreDetectionRange;
              resourceDurabilitySkill = CustomSkill.PeltDetectionSafe;
              resourceEstimationSkill = CustomSkill.PeltDetectionEstimation;
              resourceAdvancedSkill = CustomSkill.PeltDetectionAdvanced;
              resourceMasterSkill = CustomSkill.PeltDetectionMastery;
              break;
          }
        }
        private int EstimateQuantity(NwPlaceable materia)
        {
          int realQuantity = materia.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value;
          int skillPoints = player.learnableSkills.ContainsKey(resourceDetectionSkill) ? player.learnableSkills[CustomSkill.MateriaScanning].totalPoints + player.learnableSkills[resourceDetectionSkill].totalPoints : player.learnableSkills[CustomSkill.MateriaScanning].totalPoints;
          skillPoints += player.learnableSkills.ContainsKey(resourceEstimationSkill) ? player.learnableSkills[resourceEstimationSkill].totalPoints : 0;
          skillPoints += player.learnableSkills.ContainsKey(resourceAdvancedSkill) ? player.learnableSkills[resourceAdvancedSkill].totalPoints : 0;
          skillPoints += player.learnableSkills.ContainsKey(resourceMasterSkill) ? player.learnableSkills[resourceMasterSkill].totalPoints : 0;
          skillPoints += detector.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_DETECTOR_QUALITY_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value);

          return Utils.random.Next((int)(realQuantity * skillPoints * 0.05) - 1, 2 * realQuantity - (int)(realQuantity * skillPoints * 0.05));
        }
        private void MateriaRevealCheck()
        {
          NwArea area = player.oid.LoginCreature.Area;
          int areaLevel = area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value;
          int skillPoints = player.learnableSkills.ContainsKey(resourceDetectionSkill) ? player.learnableSkills[CustomSkill.MateriaScanning].totalPoints + player.learnableSkills[resourceDetectionSkill].totalPoints : player.learnableSkills[CustomSkill.MateriaScanning].totalPoints;
          skillPoints += player.learnableSkills.ContainsKey(resourceAdvancedSkill) ? player.learnableSkills[resourceAdvancedSkill].totalPoints : 0;
          skillPoints += player.learnableSkills.ContainsKey(resourceMasterSkill) ? player.learnableSkills[resourceMasterSkill].totalPoints : 0;
          skillPoints += detector.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_YIELD_QUALITY_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value);
          
          if(NwRandom.Roll(Utils.random, 100) < skillPoints)
          {
            int areaWidth = NWScript.GetAreaSize((int)AreaSizeDimension.Width, area); 
            int areaHeigth =  NWScript.GetAreaSize((int)AreaSizeDimension.Height, area);
            float radius = areaWidth > areaHeigth ? areaWidth * 7 : areaHeigth * 7;
            Vector3 randomPosition = new Vector3(Utils.random.Next(areaWidth * 8), Utils.random.Next(areaHeigth * 8), 10);

            // TODO : risque de boucle infinie ou de freezer du serveur ? Prévoir une sortie de boucle après plus de X tentatives + voir pour limiter le nombre tentatives toutes les X frames du serveur, comme pour les coffres persos
            while (AreaPlugin.GetPathExists(player.oid.LoginCreature.Area, player.oid.LoginCreature.Position, randomPosition, areaWidth * areaHeigth) < 1)
              randomPosition = new Vector3(Utils.random.Next(areaWidth * 8), Utils.random.Next(areaHeigth * 8), 10);

            NwPlaceable newResourceBlock = NwPlaceable.Create(resourceTemplate, Location.Create(area, new Vector3(randomPosition.X, randomPosition.Y, Location.Create(area, randomPosition, Utils.random.Next(360)).GroundHeight), Utils.random.Next(360)));
            newResourceBlock.Tag = "mineable_materia";
            newResourceBlock.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = 2 * NwRandom.Roll(Utils.random, 100 - (area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value * 10)) * ((100 + skillPoints) / 100);
          }
        }
        private void SetDetectionTime()
        {
          switch(selectedCategory.GetBindValue(player.oid, nuiToken.Token))
          {
            case 1: GetResourceDetectionTime(player, CustomSkill.OreDetection, CustomSkill.OreDetectionSpeed, CustomSkill.OreDetectionAdvanced, CustomSkill.OreDetectionMastery); break;
            case 2: GetResourceDetectionTime(player, CustomSkill.WoodDetection, CustomSkill.WoodDetectionSpeed, CustomSkill.WoodDetectionAdvanced, CustomSkill.WoodDetectionMastery); break;
            case 3: GetResourceDetectionTime(player, CustomSkill.PeltDetection, CustomSkill.PeltDetectionSpeed, CustomSkill.PeltDetectionAdvanced, CustomSkill.PeltDetectionMastery); break;
          }
        }
        public void GetResourceDetectionTime(Player player, int detectionSkill, int speedSkill, int advancedSkill, int masterySkill)
        {
          int scanDuration = 120;
          scanDuration -= scanDuration * (int)(player.learnableSkills[CustomSkill.MateriaScanning].totalPoints * 0.05);
          scanDuration -= player.learnableSkills.ContainsKey(detectionSkill) ? scanDuration * (int)(player.learnableSkills[detectionSkill].totalPoints * 0.05) : 0;
          scanDuration -= player.learnableSkills.ContainsKey(speedSkill) ? scanDuration * (int)(player.learnableSkills[speedSkill].totalPoints * 0.05) : 0;
          scanDuration -= player.learnableSkills.ContainsKey(advancedSkill) ? scanDuration * (int)(player.learnableSkills[advancedSkill].totalPoints * 0.05) : 0;
          scanDuration -= player.learnableSkills.ContainsKey(masterySkill) ? scanDuration * (int)(player.learnableSkills[masterySkill].totalPoints * 0.05) : 0;
          scanDuration -= scanDuration * (detector.LocalVariables.Where(l => l.Name.StartsWith($"ENCHANTEMENT_CUSTOM_DETECTOR_SPEED_") && !l.Name.Contains("_DURABILITY")).Sum(l => ((LocalVariableInt)l).Value) / 100);
          
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
