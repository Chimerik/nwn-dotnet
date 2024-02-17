using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ExpertiseChoiceWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<string> expertiseName = new("expertiseName");
        private readonly NuiBind<bool> expertiseChecked = new("expertiseChecked");
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<int> rowCount = new("rowCount");

        private readonly List<LearnableSkill> expertises = new();

        public ExpertiseChoiceWindow(Player player) : base(player)
        {
          windowId = "expertiseChoice";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> abilitiesTemplate = new List<NuiListTemplateCell>
          {
            new(new NuiSpacer()),
            new(new NuiButtonImage(icon) { Id = "description" }) { Width = 40 },
            new(new NuiLabel(expertiseName) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 160 },
            new(new NuiCheck("", expertiseChecked) { Tooltip = "Apprendre cette expertise", Margin = 0.0f }) { Width = 40 },
            new(new NuiSpacer())
          };

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(abilitiesTemplate, rowCount) { RowHeight = 40 } } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Valider") { Id = "validate", Width = 80 }, new NuiSpacer() } });
          
          CreateWindow();
        }
        public void CreateWindow()
        {
          expertises.Clear();

          foreach (var expertise in SkillSystem.learnableDictionary.Values.Where(l => l is LearnableSkill learnable && learnable.category == SkillSystem.Category.Expertise))
            if (!player.learnableSkills.TryGetValue(expertise.id, out LearnableSkill learnable) || learnable.currentLevel < 1)
              expertises.Add((LearnableSkill)expertise);

          if (expertises.Count < 1)
          {
            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_CHOICE").Delete();
            CloseWindow();
            return;
          }

          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiScaledWidth * 0.4f, player.guiHeight * 0.15f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f);

          window = new NuiWindow(rootColumn, "Choisissez 2 expertises")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = false,
            Transparent = true,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleExpertiseChoiceEvents;

            LoadWeaponList();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleExpertiseChoiceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "validate":

                  var checkedList = expertiseChecked.GetBindValues(player.oid, nuiToken.Token);
                  List<LearnableSkill> toLearn = new();

                  for (int i = 0; i < checkedList.Count; i++)
                    if (checkedList[i])
                      toLearn.Add(expertises[i]);

                  if (toLearn.Count > 2)
                  {
                    player.oid.SendServerMessage("Veuillez sélectionner jusqu'à 2 expertises");
                    return;
                  }

                  if (toLearn.Count == 2 || expertises.Count == toLearn.Count || expertises.Count < 1)
                  {
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_CHOICE").Delete();

                    foreach (var learnable in toLearn)
                    {
                      player.learnableSkills.TryAdd(learnable.id, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[learnable.id], player, (int)SkillSystem.Category.Class));
                      player.learnableSkills[learnable.id].LevelUp(player);
                    }

                    CloseWindow();
                  }
                  else
                    player.oid.SendServerMessage($"Veuillez sélectionner jusqu'à 2 manoeuvres avant de valider");

                  return;
              }

              int learnableId = expertises.ElementAt(nuiEvent.ArrayIndex).id;

              if (!player.windows.TryGetValue("learnableDescription", out var value)) player.windows.Add("learnableDescription", new LearnableDescriptionWindow(player, learnableId));
              else ((LearnableDescriptionWindow)value).CreateWindow(learnableId);

              break;
          }
        }
        private void LoadWeaponList()
        {
          List<string> iconList = new List<string>();
          List<string> manoeuvreNameList = new();
          List<bool> manoeuvreCheckList = new();
          List<bool> manoeuvreCheckBinding = expertiseChecked?.GetBindValues(player.oid, nuiToken.Token);
          int i = 0;

          foreach (var manoeuvre in expertises)
          {
            iconList.Add(manoeuvre.icon);
            manoeuvreNameList.Add(manoeuvre.name);
            manoeuvreCheckList.Add(manoeuvreCheckBinding is not null && manoeuvreCheckBinding[i]);
          }

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          expertiseName.SetBindValues(player.oid, nuiToken.Token, manoeuvreNameList);
          expertiseChecked.SetBindValues(player.oid, nuiToken.Token, manoeuvreCheckList);
          rowCount.SetBindValue(player.oid, nuiToken.Token, expertises.Count);
        }
      }
    }
  }
}
