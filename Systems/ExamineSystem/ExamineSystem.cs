using NLog;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWNX.API.Events;
using NWNX.Services;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ExamineSystem))]
  public class ExamineSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public ExamineSystem(NWNXEventService nwnxEventService)
    {
      
    }
    public static void OnExamineBefore(ExamineEvents.OnExamineObjectBefore onExamine)
    {
      Log.Info($"{onExamine.Examiner.Name} examining {onExamine.Examinee.Name} - Tag : {onExamine.Examinee.Tag}");

      if (!PlayerSystem.Players.TryGetValue(onExamine.Examiner, out PlayerSystem.Player player))
        return;

      switch (onExamine.Examinee.Tag)
      {
        case "mineable_rock":
          int oreAmount = onExamine.Examinee.GetLocalVariable<int>("_ORE_AMOUNT").Value;

          if (onExamine.Examiner.IsDM || onExamine.Examiner.IsDMPossessed || onExamine.Examiner.IsPlayerDM)
          {
            int geologySkillLevel = 0;
            if (player.learntCustomFeats.ContainsKey(CustomFeats.Geology))
            {
              geologySkillLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Geology, player.learntCustomFeats[CustomFeats.Geology]);
              onExamine.Examinee.Description = $"Minerai disponible estimé : {Utils.random.Next(oreAmount * geologySkillLevel * 20 / 100, 2 * oreAmount - geologySkillLevel * 20 / 100)}";
            }
            else
              onExamine.Examinee.Description = $"Minerai disponible estimé : {Utils.random.Next(0, 2 * oreAmount)}";
          }
          else
            onExamine.Examinee.Description = $"Minerai disponible : {oreAmount}";

          break;
        case "mineable_tree":
          int woodAmount = onExamine.Examinee.GetLocalVariable<int>("_ORE_AMOUNT").Value;
          if (onExamine.Examiner.IsDM || onExamine.Examiner.IsDMPossessed || onExamine.Examiner.IsPlayerDM)
          {
            int woodExpertiseSkillLevel = 0;
            if (player.learntCustomFeats.ContainsKey(CustomFeats.WoodExpertise))
            {
              woodExpertiseSkillLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.WoodExpertise, player.learntCustomFeats[CustomFeats.WoodExpertise]);
              onExamine.Examinee.Description = $"Minerai disponible estimé : {Utils.random.Next(woodAmount * woodExpertiseSkillLevel * 20 / 100, 2 * woodAmount - woodExpertiseSkillLevel * 20 / 100)}";
            }
            else
              onExamine.Examinee.Description = $"Bois disponible estimé : {Utils.random.Next(0, 2 * woodAmount)}";
          }
          else
            onExamine.Examinee.Description = $"Bois disponible : {woodAmount}";

          break;
        case "mineable_animal":
          int peltAmount = onExamine.Examinee.GetLocalVariable<int>("_ORE_AMOUNT").Value;
          if (onExamine.Examiner.IsDM || onExamine.Examiner.IsDMPossessed || onExamine.Examiner.IsPlayerDM)
          {
            int animalExpertiseSkillLevel = 0;
            if (player.learntCustomFeats.ContainsKey(CustomFeats.AnimalExpertise))
            {
              animalExpertiseSkillLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.AnimalExpertise, player.learntCustomFeats[CustomFeats.AnimalExpertise]);
              onExamine.Examinee.Description = $"Minerai disponible estimé : {Utils.random.Next(peltAmount * animalExpertiseSkillLevel * 20 / 100, 2 * peltAmount - animalExpertiseSkillLevel * 20 / 100)}";
            }
            else
              onExamine.Examinee.Description = $"Peau disponible estimé : {Utils.random.Next(0, 2 * peltAmount)}";
          }
          else
            onExamine.Examinee.Description = $"Peau disponible : {peltAmount}";

          break;
        case "blueprint":
          int baseItemType = onExamine.Examinee.GetLocalVariable<int>("_BASE_ITEM_TYPE").Value;

          if (Craft.Collect.System.blueprintDictionnary.ContainsKey(baseItemType))
            onExamine.Examinee.Description = Craft.Collect.System.blueprintDictionnary[baseItemType].DisplayBlueprintInfo(onExamine.Examiner, (NwItem)onExamine.Examinee);
          else
          {
            onExamine.Examiner.SendServerMessage("[ERREUR HRP] - Le patron utilisé n'est pas correctement initialisé. Le bug a été remonté au staff.");
            Utils.LogMessageToDMs($"Blueprint Invalid : {onExamine.Examinee.Name} - Base Item Type : {baseItemType} - Examined by : {onExamine.Examiner.Name}");
          }
          break;
        case "ore":
          string reprocessingData = $"{onExamine.Examinee.Name} : Efficacité raffinage -30 % (base fonderie Amirauté)";
          int skillLevel = 0;

          if (player.learntCustomFeats.ContainsKey(CustomFeats.Reprocessing))
          {
            skillLevel = 3 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Reprocessing, player.learntCustomFeats[CustomFeats.Reprocessing]);
            reprocessingData += $"\n x1.{skillLevel} (Raffinage)";
          }

          if (player.learntCustomFeats.ContainsKey(CustomFeats.ReprocessingEfficiency))
          {
            skillLevel = 2 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ReprocessingEfficiency, player.learntCustomFeats[CustomFeats.ReprocessingEfficiency]);
            reprocessingData += $"\n x1.{skillLevel} (Raffinage Efficace)";
          }

          if (Enum.TryParse(onExamine.Examinee.Name, out Craft.Collect.Config.OreType myOreType) && Craft.Collect.Config.oresDictionnary.TryGetValue(myOreType, out Craft.Collect.Config.Ore processedOre))
          {
            if (player.learntCustomFeats.ContainsKey(processedOre.feat))
            {
              skillLevel = 2 * SkillSystem.GetCustomFeatLevelFromSkillPoints(processedOre.feat, player.learntCustomFeats[processedOre.feat]);
              reprocessingData += $"\n x1.{skillLevel} (Raffinage spécialisé {onExamine.Examinee.Name})";
            }
          }

          if (player.learntCustomFeats.ContainsKey(CustomFeats.Connections))
          {
            skillLevel = 1 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Connections, player.learntCustomFeats[CustomFeats.Connections]);
            reprocessingData += $"\n x{1.00 - skillLevel / 100} (Relations Amirauté)";
          }

          onExamine.Examinee.Description = reprocessingData;
          break;
        case "wood":
          string reprocessingString = $"{onExamine.Examinee.Name} : Efficacité raffinage -30 % (base scierie Amirauté)";
          int featLevel = 0;

          if (player.learntCustomFeats.ContainsKey(CustomFeats.WoodReprocessing))
          {
            featLevel = 3 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.WoodReprocessing, player.learntCustomFeats[CustomFeats.WoodReprocessing]);
            reprocessingString += $"\n x1.{featLevel} (Sciage)";
          }

          if (player.learntCustomFeats.ContainsKey(CustomFeats.WoodReprocessingEfficiency))
          {
            featLevel = 2 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.WoodReprocessingEfficiency, player.learntCustomFeats[CustomFeats.WoodReprocessingEfficiency]);
            reprocessingString += $"\n x1.{featLevel} (Sciage Efficace)";
          }

          if (Enum.TryParse(onExamine.Examinee.Name, out Craft.Collect.Config.WoodType myWoodType) && Craft.Collect.Config.woodDictionnary.TryGetValue(myWoodType, out Craft.Collect.Config.Wood processedWood))
          {
            if (player.learntCustomFeats.ContainsKey(processedWood.feat))
            {
              featLevel = 2 * SkillSystem.GetCustomFeatLevelFromSkillPoints(processedWood.feat, player.learntCustomFeats[processedWood.feat]);
              reprocessingString += $"\n x1.{featLevel} (Sciage spécialisé {onExamine.Examinee.Name})";
            }
          }

          if (player.learntCustomFeats.ContainsKey(CustomFeats.Connections))
          {
            featLevel = 1 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Connections, player.learntCustomFeats[CustomFeats.Connections]);
            reprocessingString += $"\n x{1.00 - featLevel / 100} (Relations Amirauté)";
          }

          onExamine.Examinee.Description = reprocessingString;
          break;
        case "refinery":
          string descriptionBrut = "Stock actuel de minerai brut : \n\n\n";
          foreach (var entry in Craft.Collect.Config.oresDictionnary)
            if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
              descriptionBrut += $"* {entry.Value.name}: {playerStock}\n\n";

          onExamine.Examinee.Description = descriptionBrut;
          break;
        case "forge":
          string descriptionRefined = "Stock actuel de minerai raffiné : \n\n\n";
          foreach (var entry in Craft.Collect.Config.mineralDictionnary)
            if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
              descriptionRefined += $"* {entry.Value.name}: {playerStock}\n\n";

          onExamine.Examinee.Description = descriptionRefined;
          break;
        case "decoupe":
          string descriptionWood = "Stock actuel de bois brut : \n\n\n";
          foreach (var entry in Craft.Collect.Config.woodDictionnary)
            if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
              descriptionWood += $"* {entry.Value.name}: {playerStock}\n\n";

          onExamine.Examinee.Description = descriptionWood;
          break;
        case "scierie":
          string descriptionPlank = "Stock actuel de planches de bois raffinées : \n\n\n";
          foreach (var entry in Craft.Collect.Config.plankDictionnary)
            if (player.materialStock.TryGetValue(entry.Key.ToString(), out int playerStock))
              descriptionPlank += $"* {entry.Key.ToDescription()}: {playerStock}\n\n";

          onExamine.Examinee.Description = descriptionPlank;
          break;
        case "tannerie_peau":
          string descriptionPelt = "Stock actuel de peaux brutes : \n\n\n";
          foreach (var entry in Craft.Collect.Config.peltDictionnary)
            if (player.materialStock.TryGetValue(entry.Key.ToString(), out int playerStock))
              descriptionPelt += $"* {entry.Key.ToDescription()}: {playerStock}\n\n";

          onExamine.Examinee.Description = descriptionPelt;
          break;

        case "tannerie":
          string descriptionLeather = "Stock actuel de cuir tanné : \n\n\n";
          foreach (var entry in Craft.Collect.Config.leatherDictionnary)
            if (player.materialStock.TryGetValue(entry.Key.ToString(), out int playerStock))
              descriptionLeather += $"* {entry.Key.ToDescription()}: {playerStock}\n\n";

          onExamine.Examinee.Description = descriptionLeather;
          break;
      }

      if (onExamine.Examinee is NwItem)
      {
        onExamine.Examinee.GetLocalVariable<string>("_TEMP_DESC").Value = onExamine.Examinee.Description;
        
        if (onExamine.Examinee.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").HasValue)
          onExamine.Examinee.Description += $"\n\nEmplacement(s) d'enchantement : [{onExamine.Examinee.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Value.ToString().ColorString(Color.ORANGE)}] ".ColorString(Color.ORANGE);

        onExamine.Examinee.Description += $"\n\n {ItemUtils.GetItemDurabilityState((NwItem)onExamine.Examinee)}";
      }
    }
    public static void OnExamineAfter(ExamineEvents.OnExamineObjectAfter onExamine)
    {
      if (onExamine.Examinee is NwItem)
      {
        onExamine.Examinee.Description = onExamine.Examinee.GetLocalVariable<string>("_TEMP_DESC").Value;
        onExamine.Examinee.GetLocalVariable<string>("_TEMP_DESC").Delete();
      }
    }
  }
}
