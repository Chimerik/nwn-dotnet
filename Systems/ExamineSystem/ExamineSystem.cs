using NLog;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ExamineSystem))]
  public class ExamineSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static void OnExamineBefore(OnExamineObject onExamine)
    {
      Log.Info($"{onExamine.ExaminedBy.LoginCreature.Name} examining {onExamine.ExaminedObject.Name} - Tag : {onExamine.ExaminedObject.Tag}");

      if (!PlayerSystem.Players.TryGetValue(onExamine.ExaminedBy.LoginCreature, out PlayerSystem.Player player))
        return;

      switch (onExamine.ExaminedObject.Tag)
      {
        case "mineable_materia":

          if (player.windows.ContainsKey("materiaExamine"))
            ((PlayerSystem.Player.MateriaExamineWindow)player.windows["materiaExamine"]).CreateWindow((NwPlaceable)onExamine.ExaminedObject);
          else
            player.windows.Add("materiaExamine", new PlayerSystem.Player.MateriaExamineWindow(player, (NwPlaceable)onExamine.ExaminedObject));

          break;
        case "blueprint":
          int baseItemType = onExamine.ExaminedObject.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;
          if (Craft.Collect.System.blueprintDictionnary.ContainsKey(baseItemType))
            onExamine.ExaminedObject.Description = Craft.Collect.System.blueprintDictionnary[baseItemType].DisplayBlueprintInfo(onExamine.ExaminedBy, (NwItem)onExamine.ExaminedObject);
          else
          {
            onExamine.ExaminedBy.SendServerMessage("[ERREUR HRP] - Le patron utilisé n'est pas correctement initialisé. Le bug a été remonté au staff.");
            Utils.LogMessageToDMs($"Blueprint Invalid : {onExamine.ExaminedObject.Name} - Base Item Type : {baseItemType} - Examined by : {onExamine.ExaminedBy.LoginCreature.Name}");
          }
          break;
        case "ore":
          string reprocessingData = $"{onExamine.ExaminedObject.Name} : Efficacité raffinage -30 % (base fonderie Amirauté)";
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

          if (Enum.TryParse(onExamine.ExaminedObject.Name, out Craft.Collect.Config.OreType myOreType) 
            && Craft.Collect.Config.oresDictionnary.TryGetValue(myOreType, out Craft.Collect.Config.Ore processedOre))
          {
            if (player.learntCustomFeats.ContainsKey(processedOre.feat))
            {
              skillLevel = 2 * SkillSystem.GetCustomFeatLevelFromSkillPoints(processedOre.feat, player.learntCustomFeats[processedOre.feat]);
              reprocessingData += $"\n x1.{skillLevel} (Raffinage spécialisé {onExamine.ExaminedObject.Name})";
            }
          }

          if (player.learntCustomFeats.ContainsKey(CustomFeats.Connections))
          {
            skillLevel = 1 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Connections, player.learntCustomFeats[CustomFeats.Connections]);
            reprocessingData += $"\n x{1.00 - skillLevel / 100} (Relations Amirauté)";
          }

          onExamine.ExaminedObject.Description = reprocessingData;
          break;
        case "wood":
          string reprocessingString = $"{onExamine.ExaminedObject.Name} : Efficacité raffinage -30 % (base scierie Amirauté)";
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

          if (Enum.TryParse(onExamine.ExaminedObject.Name, out Craft.Collect.Config.WoodType myWoodType) && Craft.Collect.Config.woodDictionnary.TryGetValue(myWoodType, out Craft.Collect.Config.Wood processedWood))
          {
            if (player.learntCustomFeats.ContainsKey(processedWood.feat))
            {
              featLevel = 2 * SkillSystem.GetCustomFeatLevelFromSkillPoints(processedWood.feat, player.learntCustomFeats[processedWood.feat]);
              reprocessingString += $"\n x1.{featLevel} (Sciage spécialisé {onExamine.ExaminedObject.Name})";
            }
          }

          if (player.learntCustomFeats.ContainsKey(CustomFeats.Connections))
          {
            featLevel = 1 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Connections, player.learntCustomFeats[CustomFeats.Connections]);
            reprocessingString += $"\n x{1.00 - featLevel / 100} (Relations Amirauté)";
          }

          onExamine.ExaminedObject.Description = reprocessingString;
          break;
        case "refinery":
          string descriptionBrut = "Stock actuel de minerai brut : \n\n\n";
          foreach (var entry in Craft.Collect.Config.oresDictionnary)
            if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
              descriptionBrut += $"* {entry.Value.name}: {playerStock}\n\n";

          onExamine.ExaminedObject.Description = descriptionBrut;
          break;
        case "forge":
          string descriptionRefined = "Stock actuel de minerai raffiné : \n\n\n";
          foreach (var entry in Craft.Collect.Config.mineralDictionnary)
            if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
              descriptionRefined += $"* {entry.Value.name}: {playerStock}\n\n";

          onExamine.ExaminedObject.Description = descriptionRefined;
          break;
        case "decoupe":
          string descriptionWood = "Stock actuel de bois brut : \n\n\n";
          foreach (var entry in Craft.Collect.Config.woodDictionnary)
            if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
              descriptionWood += $"* {entry.Value.name}: {playerStock}\n\n";

          onExamine.ExaminedObject.Description = descriptionWood;
          break;
        case "scierie":
          string descriptionPlank = "Stock actuel de planches de bois raffinées : \n\n\n";
          foreach (var entry in Craft.Collect.Config.plankDictionnary)
            if (player.materialStock.TryGetValue(entry.Key.ToString(), out int playerStock))
              descriptionPlank += $"* {entry.Key.ToDescription()}: {playerStock}\n\n";

          onExamine.ExaminedObject.Description = descriptionPlank;
          break;
        case "tannerie_peau":
          string descriptionPelt = "Stock actuel de peaux brutes : \n\n\n";
          foreach (var entry in Craft.Collect.Config.peltDictionnary)
            if (player.materialStock.TryGetValue(entry.Key.ToString(), out int playerStock))
              descriptionPelt += $"* {entry.Key.ToDescription()}: {playerStock}\n\n";

          onExamine.ExaminedObject.Description = descriptionPelt;
          break;

        case "tannerie":
          string descriptionLeather = "Stock actuel de cuir tanné : \n\n\n";
          foreach (var entry in Craft.Collect.Config.leatherDictionnary)
            if (player.materialStock.TryGetValue(entry.Key.ToString(), out int playerStock))
              descriptionLeather += $"* {entry.Key.ToDescription()}: {playerStock}\n\n";

          onExamine.ExaminedObject.Description = descriptionLeather;
          break;

        case "sequence_register":

          string descriptionSequence = "Cet outil vous permet d'enregistrer une séquence de sorts dont l'incantation s'enchaînera lorsque vous l'utiliserez sur une cible, vous permettant d'économiser de nombreux clics ! \n\n";

          if (onExamine.ExaminedObject.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").HasNothing)
            return;

          descriptionSequence += "Liste des sorts enregistrés :\n\n";

          string[] spellList = onExamine.ExaminedObject.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value.Split("_");

          foreach (string spellId in spellList)
            onExamine.ExaminedObject.Description += $"- {NwSpell.FromSpellId(int.Parse(spellId)).Name}\n";
          
          onExamine.ExaminedObject.Description = descriptionSequence;

          return;
      }

      if (onExamine.ExaminedObject is NwItem oItem)
      {
        onExamine.ExaminedObject.GetObjectVariable<LocalVariableString>("_TEMP_DESC").Value = onExamine.ExaminedObject.Description;

        if(onExamine.ExaminedObject.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue)
          onExamine.ExaminedObject.Description += $"\n\n{ItemUtils.GetItemDurabilityState(oItem)}";

        if (oItem.GetObjectVariable<LocalVariableInt>("_REPAIR_DONE").HasValue)
          onExamine.ExaminedObject.Description += $"\nRéparé - en attente de ré-enchantement.";

        if (onExamine.ExaminedObject.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").HasValue)
          onExamine.ExaminedObject.Description += $"\n\nEmplacement(s) d'enchantement : [{onExamine.ExaminedObject.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value.ToString().ColorString(new Color(32, 255, 32))}] ".ColorString(ColorConstants.Orange);

        foreach (ItemProperty ip in oItem.ItemProperties.Where(ip => ip.Tag.Contains("INACTIVE")))
        {
          string[] split = ip.Tag.Split("_");
          Spell spell = (Spell)int.Parse(split[1]);
          string spellName = NwSpell.FromSpellType(spell).Name; 
          oItem.Description += $"\n{spellName} inactif.".ColorString(ColorConstants.Red);
        }

        Task waitExamineEnd = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
          onExamine.ExaminedObject.Description = onExamine.ExaminedObject.GetObjectVariable<LocalVariableString>("_TEMP_DESC").Value;
        });
      }
    }
  }
}
