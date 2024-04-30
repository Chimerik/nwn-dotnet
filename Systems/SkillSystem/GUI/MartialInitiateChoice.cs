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
      public class MartialInitiateChoiceWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<List<NuiComboEntry>> styles = new("styles");
        private readonly NuiBind<int> selectedStyle = new("selectedStyle");
        private readonly NuiBind<bool> enabled = new("enabled");
        private readonly List<NuiComboEntry> styleList = new();

        private int acquiredLevel;
        private bool gainedFromChampion;
        public MartialInitiateChoiceWindow(Player player, int level, bool gainedFromChampion = false) : base(player)
        {
          windowId = "martialInitiateChoice";
          rootColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiCombo() { Entries = styles, Selected = selectedStyle, Width = 320 }, new NuiSpacer() } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Valider") { Id = "validate", Width = 160, Encouraged = enabled }, new NuiSpacer() } });
          
          CreateWindow(level, gainedFromChampion);
        }
        public void CreateWindow(int level, bool gainedFromChampion = false)
        {
          this.gainedFromChampion = gainedFromChampion;
          acquiredLevel = level;

          foreach (var style in SkillSystem.learnableDictionary.Values.Where(s => s is LearnableSkill skill && skill.category == SkillSystem.Category.FightingStyle
            && !player.learnableSkills.ContainsKey(s.id)))
              styleList.Add(new NuiComboEntry(style.name, style.id));

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MARTIAL_INITIATE_CHOICE_FEAT").Value = gainedFromChampion ? 1 : 2;

          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiScaledWidth * 0.4f, player.guiHeight * 0.15f, player.guiScaledWidth * 0.3f, player.guiScaledHeight * 0.20f);

          window = new NuiWindow(rootColumn, "Veuillez choisir un style de combat")
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
            nuiToken.OnNuiEvent += HandleFightingStyleChoiceEvents;

            styles.SetBindValue(player.oid, nuiToken.Token, styleList);
            selectedStyle.SetBindValue(player.oid, nuiToken.Token, styleList.FirstOrDefault().Value);

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleFightingStyleChoiceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "validate":
                  ModuleSystem.Log.Info("1");
                  int selection = selectedStyle.GetBindValue(player.oid, nuiToken.Token);

                  if (!gainedFromChampion)
                  {
                    ModuleSystem.Log.Info("2");
                    player.learnableSkills.TryAdd(CustomSkill.MartialInitiate, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.MartialInitiate], player));
                    ModuleSystem.Log.Info("3");

                    if (player.learnableSkills[CustomSkill.MartialInitiate].featOptions is null)
                      player.learnableSkills[CustomSkill.MartialInitiate].featOptions = new();

                    player.learnableSkills[CustomSkill.MartialInitiate].featOptions.Add(acquiredLevel, new int[] { selection });
                  }
                  ModuleSystem.Log.Info("4");
                  player.learnableSkills.Add(selection, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[selection], player));
                  ModuleSystem.Log.Info("5");
                  player.learnableSkills[selection].LevelUp(player);
                  ModuleSystem.Log.Info("6");
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MARTIAL_INITIATE_CHOICE_FEAT").Delete();

                  CloseWindow();

                return;
              }

            break;

          }
        }
      }
    }
  }
}
