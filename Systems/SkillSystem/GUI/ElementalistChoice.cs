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
      public class ElementalistChoiceWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<List<NuiComboEntry>> damages = new("damages");
        private readonly NuiBind<int> selectedDamage = new("selectedDamage");
        private readonly NuiBind<bool> enabled = new("enabled");
        private readonly List<NuiComboEntry> damageList = new() 
        { 
          new NuiComboEntry("Acide", (int)DamageType.Acid),
          new NuiComboEntry("Froid", (int)DamageType.Cold),
          new NuiComboEntry("Feu", (int)DamageType.Fire),
          new NuiComboEntry("Foudre", (int)DamageType.Electrical),
          new NuiComboEntry("Tonnerre", (int)DamageType.Sonic),
        };

        private int acquiredLevel;

        public ElementalistChoiceWindow(Player player, int level) : base(player)
        {
          windowId = "elementalistChoice";
          rootColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCombo() { Entries = damages, Selected = selectedDamage } } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Valider") { Id = "validate", Width = 80, Encouraged = enabled }, new NuiSpacer() } });
          
          CreateWindow(level);
        }
        public void CreateWindow(int level)
        {
          acquiredLevel = level;

          foreach (var damageType in player.learnableSkills[CustomSkill.Elementaliste].featOptions.Values)
            damageList.Remove(damageList.FirstOrDefault(d => d.Value == damageType[0]));

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ELEMENTALIST_CHOICE_FEAT").Value = 1;

          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.guiScaledWidth * 0.4f, player.guiHeight * 0.15f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f);

          window = new NuiWindow(rootColumn, "Don élémentaliste - Choisissez un type de dégâts")
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
            nuiToken.OnNuiEvent += HandleElementalistChoiceEvents;

            damages.SetBindValue(player.oid, nuiToken.Token, damageList);
            selectedDamage.SetBindValue(player.oid, nuiToken.Token, damageList.FirstOrDefault().Value);

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleElementalistChoiceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "validate":

                  CloseWindow();
                  player.learnableSkills[CustomSkill.Elementaliste].featOptions.Add(acquiredLevel, new int[] {selectedDamage.GetBindValue(player.oid, nuiToken.Token)} );
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ELEMENTALIST_CHOICE_FEAT").Delete();

                  return;
              }

              break;

          }
        }
      }
    }
  }
}
