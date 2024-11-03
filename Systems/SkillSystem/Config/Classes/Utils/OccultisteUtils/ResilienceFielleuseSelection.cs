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
      public class ResilienceFielleuseSelectionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> skillName = new("skillName");

        private List<DamageType> currentList = new();

        public ResilienceFielleuseSelectionWindow(Player player) : base(player)
        {
          windowId = "resilienceFielleuseSelection";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> learnableTemplate = new List<NuiListTemplateCell> { new(new NuiButtonImage(icon) { Id = "select", Tooltip = skillName, Height = 40, Width = 40 }) { Width = 40 } };

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() {
            new NuiSpacer(),
            new NuiColumn() { Children = new List<NuiElement>() { new NuiList(learnableTemplate, listCount) { RowHeight = 40 } }, Width = 340 },
            new NuiSpacer() } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.guiWidth * 0.2f, player.guiHeight * 0.05f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.6f);

          window = new NuiWindow(rootColumn, "Résilience Fielleuse - Choisissez un type de dégât")
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
            nuiToken.OnNuiEvent += HandleResistanceFielleuseEvents;

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.6f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            LoadResistanceList();
          }
        }
        private void HandleResistanceFielleuseEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "select":

                  player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ResilienceFielleuse(player.oid.LoginCreature, currentList[nuiEvent.ArrayIndex]));
                  CloseWindow();

                  break;
              }

              break;
          }
        }
        private void LoadResistanceList()
        {
          List<string> iconList = new List<string>();
          List<string> nameList = new List<string>();
          currentList.Clear();

          foreach (var entry in DamageType2da.damageTypeTable)
          {
            if (entry.damageType != DamageType.Magical)
            {
              iconList.Add(entry.resistanceIcon);
              nameList.Add(entry.nameRef.ToString());
              currentList.Add(entry.damageType);
            }
          }

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          skillName.SetBindValues(player.oid, nuiToken.Token, nameList);
          listCount.SetBindValue(player.oid, nuiToken.Token, currentList.Count());
        }
      }
    }
  }
}
