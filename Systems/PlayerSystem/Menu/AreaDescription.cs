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
        private readonly NuiGroup rootGroup = new() { Id = "areaGroup", Border = true };
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChidren = new();

        public AreaDescriptionWindow(Player player, NwArea area) : base(player)
        {
          windowId = "areaDescription";

          rootColumn.Children = rootChidren;
          rootGroup.Layout = rootColumn;

          CreateWindow(area);
        }
        public void CreateWindow(NwArea area)
        {
          rootChidren.Clear();

          if (AreaSystem.areaDescriptions.TryGetValue(area.Name, out string areaDescription))
          {
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
          //else
           // player.oid.SendServerMessage("Erreur - Aucune description configurée pour cette zone", ColorConstants.Red);
        }
      }
    }
  }
}
