using System.Collections.Generic;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class IntroMirroWindow : PlayerWindow
      {
        NuiGroup rootGroup { get; }
        NuiColumn rootColumn { get; }
        List<NuiElement> rootChidren { get; }

        public IntroMirroWindow(Player player) : base(player)
        {
          windowId = "introMirror";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          CreateWindow();
        }
        public void CreateWindow()
        {
          rootChidren.Clear();

          //potentiellement : relire le dialogue d'intro de Disco Elysium
          //portrait
          //texte
          // option : me refaire une beauté
          // option : me perdre dans le passé (niveaux de compétences de départ en fonction du background) + éventuellement quelques éléments de classe de base
          // option : me préparer à l'avenir

          NuiRow row = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButtonImage(learnable.icon) { Height = 40, Width = 40 },
              new NuiSpacer()
            }
          };

          rootChidren.Add(row);

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 500, 300);

          window = new NuiWindow(rootGroup, learnable.name)
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          token = player.oid.CreateNuiWindow(window, windowId);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
      }
    }
  }
}
