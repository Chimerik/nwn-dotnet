using System.Collections.Generic;

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
            player.oid.NuiDestroy(player.openedWindows[windowId]);

          window = new NuiWindow(rootColumn, feat.Name.ToString().Replace("’", "'"))
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          token = player.oid.CreateNuiWindow(window, windowId);

          icon.SetBindValue(player.oid, token, feat.IconResRef);
          name.SetBindValue(player.oid, token, feat.Name.ToString().Replace("’", "'"));
          description.SetBindValue(player.oid, token, feat.Description.ToString().Replace("’", "'"));

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
      }
    }
  }
}
