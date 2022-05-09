using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class SpellDescriptionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChidren = new();
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> name = new("name");
        private readonly NuiBind<string> description = new("description");

        public SpellDescriptionWindow(Player player, NwSpell spell) : base(player)
        {
          windowId = "spellDescription";

          rootColumn.Children = rootChidren;

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButtonImage(icon) { Tooltip = name, Height = 35, Width = 35 },
              new NuiSpacer()
            }
          });

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiText(description) } });

          CreateWindow(spell);
        }
        public void CreateWindow(NwSpell spell)
        {
          if (player.openedWindows.ContainsKey(windowId))
            CloseWindow();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 500, 300);

          window = new NuiWindow(rootColumn, spell.Name.ToString())
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          Task wait = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromMilliseconds(10));

            if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
            {
              nuiToken = tempToken;

              icon.SetBindValue(player.oid, nuiToken.Token, spell.IconResRef);
              name.SetBindValue(player.oid, nuiToken.Token, spell.Name.ToString().Replace("’", "'"));
              description.SetBindValue(player.oid, nuiToken.Token, spell.Description.ToString().Replace("’", "'"));

              geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
              geometry.SetBindWatch(player.oid, nuiToken.Token, true);

              player.openedWindows[windowId] = nuiToken.Token;
            }
            else
              player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
          });
        }
      }
    }
  }
}
