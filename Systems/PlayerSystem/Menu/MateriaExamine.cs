using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class MateriaExamineWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn;
        private readonly NuiBind<string> characterNames = new NuiBind<string>("characterNames");
        private readonly NuiBind<string> quantityEstimates = new NuiBind<string>("quantityEstimates");
        private readonly NuiBind<int> listCount = new NuiBind<int>("listCount");
        private int resourceDetectionSkill = CustomSkill.OreDetection;
        private int resourceEstimationSkill = CustomSkill.OreDetectionEstimation;

        public MateriaExamineWindow(Player player, NwPlaceable materia) : base(player)
        {
          windowId = "materiaExamine";

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiText(characterNames) { Tooltip = characterNames, Height = 35 }) { VariableSize = true },
            new NuiListTemplateCell(new NuiText(quantityEstimates) { Tooltip = quantityEstimates, Height = 35 }) { Width =  50 }
          };

          rootColumn = new NuiColumn() 
          { 
            Children = new List<NuiElement>() 
            {
              new NuiText("Un phénomène mystérieux provoque l'agglomération de Substance à\n certains matériaux bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être récoltée,\npuis raffinée avant de pouvoir être utilisée par un artisan.") { Height = 100 },
              new NuiList(rowTemplate, listCount) { RowHeight = 35 } 
            }
          };

          CreateWindow(materia);
        }
        public void CreateWindow(NwPlaceable materia)
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);
          
          window = new NuiWindow(rootColumn, "Matéria : Estimation de quantité disponible")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          token = player.oid.CreateNuiWindow(window, windowId);

          Craft.Collect.System.UpdateResourceBlockInfo(materia);

          SelectDetectionSkill(materia.GetObjectVariable<LocalVariableString>("_RESOURCE_TYPE").Value);
          int realQuantity = materia.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value;

          if (player.learnableSkills.ContainsKey(resourceEstimationSkill))
          {
            int skillPoints = player.learnableSkills.ContainsKey(resourceEstimationSkill) ? player.learnableSkills[resourceDetectionSkill].totalPoints + player.learnableSkills[resourceEstimationSkill].totalPoints : player.learnableSkills[resourceDetectionSkill].totalPoints;
            int previousEstimation = materia.GetObjectVariable<LocalVariableInt>($"_QUANTITY_ESTIMATE_{player.characterId}").Value;
            int newEstimate = Utils.random.Next((int)(realQuantity * skillPoints * 0.05) - 1, 2 * realQuantity - (int)(realQuantity * skillPoints * 0.05));

            newEstimate = realQuantity - previousEstimation < realQuantity - newEstimate ? previousEstimation : newEstimate;
            materia.GetObjectVariable<LocalVariableInt>($"_QUANTITY_ESTIMATE_{player.characterId}").Value = newEstimate;
          }

          var localEstimates = materia.LocalVariables.Where(v => v.Name.StartsWith("_QUANTITY_ESTIMATE_"));
          List<string> characterList = new List<string>();
          List<string> estimateList = new List<string>();

          foreach(var localVar in localEstimates)
          {
            int charId = int.Parse(localVar.Name.Replace("_QUANTITY_ESTIMATE_", ""));
            Player prospector = Players.Values.FirstOrDefault(p => p.characterId == charId);

            if(prospector != null && prospector.pcState != PcState.Offline && player.oid.PartyMembers.Any(p => p == prospector.oid))
            {
              characterList.Add(prospector.oid.LoginCreature.Name);
              estimateList.Add(((LocalVariableInt)localVar).Value.ToString());
            }
          }

          characterNames.SetBindValues(player.oid, token, characterList);
          quantityEstimates.SetBindValues(player.oid, token, estimateList);
          listCount.SetBindValue(player.oid, token, characterList.Count);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
        private void SelectDetectionSkill(string resourceType)
        {
          switch (resourceType)
          {
            case "wood_spawn_wp":
              resourceDetectionSkill = CustomSkill.WoodDetection;
              resourceEstimationSkill = CustomSkill.WoodDetectionEstimation;
              break;
            case "animal_spawn_wp":
              resourceDetectionSkill = CustomSkill.PeltDetection;
              resourceEstimationSkill = CustomSkill.PeltDetectionEstimation;
              break;
          }
        }
      }
    }
  }
}
