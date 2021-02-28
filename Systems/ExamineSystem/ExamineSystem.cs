using NLog;
using NWN.API;
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
      nwnxEventService.Subscribe<ExamineEvents.OnExamineObjectBefore>(OnExamineBefore);
    }
    private void OnExamineBefore(ExamineEvents.OnExamineObjectBefore onExamine)
    {
      Log.Info($"{onExamine.Examiner.Name} examining {onExamine.Examinee.Name}");

      if (!PlayerSystem.Players.TryGetValue(onExamine.Examiner, out PlayerSystem.Player player))
        return;

      switch (onExamine.Examinee.Tag)
      {
        case "mineable_rock":
          int oreAmount = onExamine.Examinee.GetLocalVariable<int>("_ORE_AMOUNT").Value;

          if (onExamine.Examiner.IsDM || onExamine.Examiner.IsDMPossessed || onExamine.Examiner.IsPlayerDM)
          {
            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(onExamine.Examiner, (int)Feat.Geology)), out int geologySkillLevel))
              onExamine.Examinee.Description = $"Minerai disponible : {NWN.Utils.random.Next(oreAmount * geologySkillLevel * 20 / 100, 2 * oreAmount - geologySkillLevel * 20 / 100)}";
            else
              onExamine.Examinee.Description = $"Minerai disponible estimé : {NWN.Utils.random.Next(0, 2 * oreAmount)}";
          }
          else
            onExamine.Examinee.Description = $"Minerai disponible : {oreAmount}";

          break;
        case "mineable_tree":
          int woodAmount = onExamine.Examinee.GetLocalVariable<int>("_ORE_AMOUNT").Value;
          if (onExamine.Examiner.IsDM || onExamine.Examiner.IsDMPossessed || onExamine.Examiner.IsPlayerDM)
          {
            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(onExamine.Examiner, (int)Feat.WoodExpertise)), out int woodExpertiseSkillLevel))
              onExamine.Examinee.Description = $"Bois disponible : {NWN.Utils.random.Next(woodAmount * woodExpertiseSkillLevel * 20 / 100, 2 * woodAmount - woodExpertiseSkillLevel * 20 / 100)}";
            else
              onExamine.Examinee.Description = $"Bois disponible estimé : {NWN.Utils.random.Next(0, 2 * woodAmount)}";
          }
          else
            onExamine.Examinee.Description = $"Bois disponible : {woodAmount}";

          break;
        case "mineable_animal":
          int peltAmount = onExamine.Examinee.GetLocalVariable<int>("_ORE_AMOUNT").Value;
          if (onExamine.Examiner.IsDM || onExamine.Examiner.IsDMPossessed || onExamine.Examiner.IsPlayerDM)
          {
            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(onExamine.Examinee, (int)Feat.AnimalExpertise)), out int animalExpertiseSkillLevel))
              onExamine.Examinee.Description = $"Peau disponible : {NWN.Utils.random.Next(peltAmount * animalExpertiseSkillLevel * 20 / 100, 2 * peltAmount - animalExpertiseSkillLevel * 20 / 100)}";
            else
              onExamine.Examinee.Description = $"Peau disponible estimé : {NWN.Utils.random.Next(0, 2 * peltAmount)}";
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
            NWN.Utils.LogMessageToDMs($"Blueprint Invalid : {onExamine.Examinee.Name} - Base Item Type : {baseItemType} - Examined by : {onExamine.Examiner.Name}");
          }
          break;
        case "ore":
          string reprocessingData = $"{onExamine.Examinee.Name} : Efficacité raffinage -30 % (base fonderie Amirauté)";

          int value;
          if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(onExamine.Examiner, (int)Feat.Reprocessing)), out value))
            reprocessingData += $"\n x1.{3 * value} (Raffinage)";

          if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(onExamine.Examiner, (int)Feat.ReprocessingEfficiency)), out value))
            reprocessingData += $"\n x1.{2 * value} (Raffinage efficace)";

          if (Enum.TryParse(onExamine.Examinee.Name, out Craft.Collect.Config.OreType myOreType) && Craft.Collect.Config.oresDictionnary.TryGetValue(myOreType, out Craft.Collect.Config.Ore processedOre))
            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(onExamine.Examiner, (int)processedOre.feat)), out value))
              reprocessingData += $"\n x1.{2 * value} (Raffinage {onExamine.Examinee.Name})";

          float connectionsLevel;
          if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(onExamine.Examiner, (int)Feat.Connections)), out connectionsLevel))
            reprocessingData += $"\n x{1.00 - connectionsLevel / 100} (Raffinage {onExamine.Examinee.Name})";

          onExamine.Examinee.Description = reprocessingData;
          break;
        case "wood":
          string reprocessingString = $"{onExamine.Examinee.Name} : Efficacité raffinage -30 % (base scierie Amirauté)";

          int bonus;
          if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(onExamine.Examiner, (int)Feat.WoodReprocessing)), out bonus))
            reprocessingString += $"\n x1.{3 * bonus} (Raffinage)";

          if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(onExamine.Examiner, (int)Feat.WoodReprocessingEfficiency)), out bonus))
            reprocessingString += $"\n x1.{2 * bonus} (Raffinage efficace)";

          if (Enum.TryParse(onExamine.Examinee.Name, out Craft.Collect.Config.WoodType myWoodType) && Craft.Collect.Config.woodDictionnary.TryGetValue(myWoodType, out Craft.Collect.Config.Wood processedWood))
            if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(onExamine.Examiner, (int)processedWood.feat)), out bonus))
              reprocessingString += $"\n x1.{2 * bonus} (Raffinage {onExamine.Examinee.Name})";

          float connectionsBonus;
          if (float.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(onExamine.Examiner, (int)Feat.Connections)), out connectionsBonus))
            reprocessingString += $"\n x{1.00 - connectionsBonus / 100} (Raffinage {onExamine.Examinee.Name})";

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
            if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
              descriptionPlank += $"* {entry.Key.ToDescription()}: {playerStock}\n\n";

          onExamine.Examinee.Description = descriptionPlank;
          break;
        case "tannerie_peau":
          string descriptionPelt = "Stock actuel de peaux brut : \n\n\n";
          foreach (var entry in Craft.Collect.Config.peltDictionnary)
            if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
              descriptionPelt += $"* {entry.Key.ToDescription()}: {playerStock}\n\n";

          onExamine.Examinee.Description = descriptionPelt;
          break;

        case "tannerie":
          string descriptionLeather = "Stock actuel de planches de cuir tanné : \n\n\n";
          foreach (var entry in Craft.Collect.Config.leatherDictionnary)
            if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
              descriptionLeather += $"* {entry.Key.ToDescription()}: {playerStock}\n\n";

          onExamine.Examinee.Description = descriptionLeather;
          break;
      }

      if (onExamine.Examinee is NwItem && onExamine.Examinee.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").HasValue)
        onExamine.Examiner.SendServerMessage($"{onExamine.Examinee.Name} : {onExamine.Examinee.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Value} emplacement(s) d'enchantement disponible(s)", Color.CYAN);
    }
  }
}
