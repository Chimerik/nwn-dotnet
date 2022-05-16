using System.Collections.Generic;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class AreaDescriptionWindow : PlayerWindow
      {
        NuiGroup rootGroup { get; }
        NuiColumn rootColumn { get; }
        List<NuiElement> rootChidren { get; }

        public AreaDescriptionWindow(Player player, NwArea area) : base(player)
        {
          windowId = "areaDescription";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "areaGroup", Border = true, Layout = rootColumn };

          CreateWindow(area);
        }
        public async void CreateWindow(NwArea area)
        {
          rootChidren.Clear();

          string areaDescription = await StringUtils.DownloadGoogleDocFromName(area.Name);
          await NwTask.SwitchToMainThread();

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiRow() { Children = new List<NuiElement>() { new NuiText(areaDescription) } } } });

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 500, 300);

          window = new NuiWindow(rootGroup, area.Name)
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

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
      }
    }
  }
}
