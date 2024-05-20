using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class FavoredEnemySelectionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiBind<string> selectedItemTitle = new("selectedItemTitle");
        private readonly NuiBind<string> selectedItemDescription = new("selectedItemDescription");
        private readonly NuiBind<string> selectedItemIcon = new("selectedItemIcon");
        private readonly NuiBind<bool> selectedItemVisibility = new("selectedItemVisibility");
        private readonly NuiBind<bool> encouraged = new("encouraged");

        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> skillName = new("skillName");

        private IEnumerable<Learnable> currentList;
        private Learnable selectedLearnable;

        public FavoredEnemySelectionWindow(Player player, int archetypeId = 0) : base(player)
        {
          windowId = "favoredEnemySelection";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> learnableTemplate = new List<NuiListTemplateCell>
          {
            new(new NuiButtonImage(icon) { Id = "select", Tooltip = skillName, Encouraged = encouraged, Height = 40, Width = 40 }) { Width = 40 },
            new(new NuiLabel(skillName) { Width = 200, Id = "select", ForegroundColor = ColorConstants.White, Encouraged = encouraged, Tooltip = skillName, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 220 },
            new(new NuiButtonImage("select_right") { Id = "select", Tooltip = skillName, Encouraged = encouraged, Height = 40, Width = 40 }) { Width = 40 },
            new(new NuiSpacer())
          };

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiColumn() { Children = new List<NuiElement>() { new NuiList(learnableTemplate, listCount) { RowHeight = 40 } }, Width = 340 },
            new NuiSpacer(),
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() 
              {
                new NuiSpacer(),
                new NuiButtonImage(selectedItemIcon) { Height = 40, Width = 40, Visible = selectedItemVisibility },
                new NuiLabel(selectedItemTitle) { Height = 40, Width = 200, Visible = selectedItemVisibility, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                new NuiSpacer()
              } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiText(selectedItemDescription) } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Sélectionner") { Id = "validate", Height = 40, Width = player.guiScaledWidth * 0.6f - 370 }, new NuiSpacer() } }
            }, Width = player.guiScaledWidth * 0.6f - 370 }
          } });

          CreateWindow(archetypeId);
        }
        public void CreateWindow(int archetypeId = 0)
        {
          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FAVORED_ENEMY_SELECTION").Value = archetypeId;

          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiWidth * 0.2f, player.guiHeight * 0.05f, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f);
          selectedLearnable = null;

          window = new NuiWindow(rootColumn, "Choisissez un ennemi juré")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = false,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleLearnableEvents;

            selectedItemTitle.SetBindValue(player.oid, nuiToken.Token, "");
            selectedItemDescription.SetBindValue(player.oid, nuiToken.Token, "Sélectionner un ennemi juré pour afficher ses détails.\n\nAttention, le choix est définitif.");
            selectedItemIcon.SetBindValue(player.oid, nuiToken.Token, "ir_examine");
            selectedItemVisibility.SetBindValue(player.oid, nuiToken.Token, false);

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            if(archetypeId < 1)
            {
              if (player.learnableSkills.ContainsKey(CustomSkill.RangerGardienDuVoile))
                archetypeId = CustomSkill.RangerGardienDuVoile;
              if (player.learnableSkills.ContainsKey(CustomSkill.RangerBriseurDeMages))
                archetypeId = CustomSkill.RangerBriseurDeMages;
              if (player.learnableSkills.ContainsKey(CustomSkill.RangerChevalier))
                archetypeId = CustomSkill.RangerChevalier;
              if (player.learnableSkills.ContainsKey(CustomSkill.RangerSanctifie))
                archetypeId = CustomSkill.RangerSanctifie;
              if (player.learnableSkills.ContainsKey(CustomSkill.RangerChasseurDePrimes))
                archetypeId = CustomSkill.RangerChasseurDePrimes;
            }

            currentList = archetypeId switch
            {
              CustomSkill.RangerGardienDuVoile => learnableDictionary.Values.Where(l => l is LearnableSkill && l.currentLevel < 1
                && (l.id == CustomSkill.FavoredEnemyAberration || l.id == CustomSkill.FavoredEnemyOutsider || l.id == CustomSkill.FavoredEnemyFey
                || l.id == CustomSkill.FavoredEnemyElemental || l.id == CustomSkill.FavoredEnemyMagicalBeast || l.id == CustomSkill.FavoredEnemyShapechanger)).OrderBy(l => l.name),
              
              CustomSkill.RangerBriseurDeMages => learnableDictionary.Values.Where(l => l is LearnableSkill && l.currentLevel < 1
                && (l.id == CustomSkill.FavoredEnemyMagicalBeast || l.id == CustomSkill.FavoredEnemyConstruct || l.id == CustomSkill.FavoredEnemyUndead
                || l.id == CustomSkill.FavoredEnemyDragon || l.id == CustomSkill.FavoredEnemyElemental || l.id == CustomSkill.FavoredEnemyMonstrous)).OrderBy(l => l.name),
              
              CustomSkill.RangerChevalier => learnableDictionary.Values.Where(l => l is LearnableSkill && l.currentLevel < 1
                && (l.id == CustomSkill.FavoredEnemyDragon || l.id == CustomSkill.FavoredEnemyGiant || l.id == CustomSkill.FavoredEnemyBeast
                || l.id == CustomSkill.FavoredEnemyOrc || l.id == CustomSkill.FavoredEnemyVermin || l.id == CustomSkill.FavoredEnemyShapechanger)).OrderBy(l => l.name),
              
              CustomSkill.RangerSanctifie => learnableDictionary.Values.Where(l => l is LearnableSkill && l.currentLevel < 1
                && (l.id == CustomSkill.FavoredEnemyUndead || l.id == CustomSkill.FavoredEnemyGiant || l.id == CustomSkill.FavoredEnemyBeast
                || l.id == CustomSkill.FavoredEnemyMagicalBeast || l.id == CustomSkill.FavoredEnemyVermin || l.id == CustomSkill.FavoredEnemyMonstrous)).OrderBy(l => l.name),
              
              _ => learnableDictionary.Values.Where(l => l is LearnableSkill && l.currentLevel < 1
                && (l.id == CustomSkill.FavoredEnemyMonstrous || l.id == CustomSkill.FavoredEnemyGoblinoid || l.id == CustomSkill.FavoredEnemyReptilian
                || l.id == CustomSkill.FavoredEnemyOrc || l.id == CustomSkill.FavoredEnemyShapechanger || l.id == CustomSkill.FavoredEnemyVermin)).OrderBy(l => l.name),
            };

            LoadLearnableList(currentList);
          }
        }
        private void HandleLearnableEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch(nuiEvent.ElementId)
              {
                case "select":

                  selectedItemVisibility.SetBindValue(player.oid, nuiToken.Token, true);

                  if (selectedLearnable == currentList.ElementAt(nuiEvent.ArrayIndex))
                    return;

                  selectedLearnable = currentList.ElementAt(nuiEvent.ArrayIndex);
                  selectedItemTitle.SetBindValue(player.oid, nuiToken.Token, selectedLearnable.name);
                  selectedItemDescription.SetBindValue(player.oid, nuiToken.Token, selectedLearnable.description);
                  selectedItemIcon.SetBindValue(player.oid, nuiToken.Token, selectedLearnable.icon);

                  LoadLearnableList(currentList);

                  break;

                case "validate":

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FAVORED_ENEMY_SELECTION").Delete();

                  player.learnableSkills.TryAdd(selectedLearnable.id, new LearnableSkill((LearnableSkill)learnableDictionary[selectedLearnable.id], player, levelTaken: player.oid.LoginCreature.Level));
                  player.learnableSkills[selectedLearnable.id].LevelUp(player);
                  player.learnableSkills[selectedLearnable.id].source.Add(Category.Class);

                  player.oid.SendServerMessage($"Les {StringUtils.ToWhitecolor(selectedLearnable.name)} sont dorénavant vos ennemis jurés !", ColorConstants.Orange);

                  CloseWindow();

                  if (!player.windows.TryGetValue("rangerEnvironmentSelection", out var style)) player.windows.Add("rangerEnvironmentSelection", new RangerEnvironmentSelectionWindow(player));
                  else ((RangerEnvironmentSelectionWindow)style).CreateWindow();

                  break;
              }

              break;
          }
        }
        private void LoadLearnableList(IEnumerable<Learnable> filteredList)
        {
          List<string> iconList = new List<string>();
          List<string> skillNameList = new List<string>();
          List<bool> encouragedList = new();

          foreach (Learnable learnable in filteredList)
          {
            iconList.Add(learnable.icon);
            skillNameList.Add(learnable.name);
            encouragedList.Add(selectedLearnable is not null && selectedLearnable == learnable);
          }

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          skillName.SetBindValues(player.oid, nuiToken.Token, skillNameList);
          encouraged.SetBindValues(player.oid, nuiToken.Token, encouragedList);
          listCount.SetBindValue(player.oid, nuiToken.Token, filteredList.Count());
        }
      }
    }
  }
}
