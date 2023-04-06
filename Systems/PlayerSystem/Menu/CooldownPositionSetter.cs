using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class CooldownPositionSetter : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        Color color;

        public CooldownPositionSetter(Player player) : base(player)
        {
          windowId = "cooldownPosition";
          rootColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiLabel("Alignement") { Height = 20, Width = 80 },
            new NuiButton("<") { Id = "align_left", Height = 20, Width = 20, Tooltip = "Déplacer vers la gauche" },
            new NuiButton(">") { Id = "align_right", Height = 20, Width = 20, Tooltip = "Déplacer vers la droite" },
          } });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiLabel("Espacement") { Height = 20, Width = 80 },
            new NuiButton("<") { Id = "spacing_left", Height = 20, Width = 20, Tooltip = "Réduire l'espacement" },
            new NuiButton(">") { Id = "spacing_right", Height = 20, Width = 20, Tooltip = "Augmenter l'espacement" },
          }
          });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(450, 600, 200, 140);

          window = new NuiWindow(rootColumn, "Alignement des cooldowns")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleCooldownPositionSetterEvents;

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            color = player.chatColors.TryGetValue(102, out byte[] colorArray) ? new(colorArray[0], colorArray[1], colorArray[2], colorArray[3])
              : ColorConstants.Red;

            for(int i = 0; i < 12; i++) 
              player.oid.PostString("99", player.cooldownPositions.xPos + i * player.cooldownPositions.spacing, 100, ScreenAnchor.TopLeft, 25, color, color, 10000 + i);
          }
        }

        private void HandleCooldownPositionSetterEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "align_left":

                  player.cooldownPositions.xPos -= 1;

                  for (int i = 0; i < 12; i++)
                    player.oid.PostString("99", player.cooldownPositions.xPos + i * player.cooldownPositions.spacing, 100, ScreenAnchor.TopLeft, 25, color, color, 10000 + i);

                  return;

                case "align_right":

                  player.cooldownPositions.xPos += 1;

                  for (int i = 0; i < 12; i++)
                    player.oid.PostString("99", player.cooldownPositions.xPos + i * player.cooldownPositions.spacing, 100, ScreenAnchor.TopLeft, 25, color, color, 10000 + i);

                  return;

                case "spacing_left":

                  player.cooldownPositions.spacing -= 1;

                  for (int i = 0; i < 12; i++)
                    player.oid.PostString("99", player.cooldownPositions.xPos + i * player.cooldownPositions.spacing, 100, ScreenAnchor.TopLeft, 25, color, color, 10000 + i);
                  
                  return;

                case "spacing_right":

                  player.cooldownPositions.spacing += 1;

                  for (int i = 0; i < 12; i++)
                    player.oid.PostString("99", player.cooldownPositions.xPos + i * player.cooldownPositions.spacing, 100, ScreenAnchor.TopLeft, 25, color, color, 10000 + i);


                  return;
              }

              return;
          }
        }
      }
    }
  }
}
