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
      public class FeatDescriptionWindow : PlayerWindow
      {
        private readonly List<NuiElement> rootChidren = new();
        private readonly NuiColumn rootColumn = new();
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> name = new("name");
        private readonly NuiBind<string> description = new("description");

        public FeatDescriptionWindow(Player player, NwFeat feat) : base(player)
        {
          windowId = "featDescription";
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

          CreateWindow(feat);
        }
        public void CreateWindow(NwFeat feat)
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 500, 300);

          if (player.openedWindows.ContainsKey(windowId))
            CloseWindow();

          window = new NuiWindow(rootColumn, feat.Name.ToString().Replace("’", "'"))
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

              icon.SetBindValue(player.oid, nuiToken.Token, feat.IconResRef);
              name.SetBindValue(player.oid, nuiToken.Token, feat.Name.ToString().Replace("’", "'"));
              description.SetBindValue(player.oid, nuiToken.Token, feat.Description.ToString().Replace("’", "'"));

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
