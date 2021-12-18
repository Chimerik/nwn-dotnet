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
      public class IntroWelcomeWindow : PlayerWindow
      {
        NuiGroup rootGroup { get; }
        NuiColumn rootColumn { get; }
        NuiRow textRow { get; }
        List<NuiElement> rootChidren { get; }

        public IntroWelcomeWindow(Player player) : base(player)
        {
          windowId = "introWelcome";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "learnableGroup", Border = true, Layout = rootColumn };

          textRow = new NuiRow()
          {
            Children = new List<NuiElement>() {
              new NuiText("Ce module, encore tout jeune, est en plein développement.\nNotre but est de modifier le fonctionnement habituel de NWN" +
              "à l'aide d'outils modernes de façon à rendre plus concret et roleplay chaque élément de gameplay.\n\n" +
              "Nous avons trois grands principes :\n\n" +
              " - Le Role Play prime sur tout\n" +
              " - La cohérence est fondamentale\n" +
              " - Tout acte doit avoir des conséquences\n\n" +
              "Seulement, tout cela est un énorme travail et nous avons besoin de l'aide de tout ceux intéressés par la construction d'un mon et par le concept de base !\n\n" +
              "Surtout, n'hésitez pas à nous contacter en jeu et à rejoindre notre Discord : https://discord.gg/vYVhxsnH") { Height = 130 }
            }
          };

          rootChidren.Add(textRow);
          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          window = new NuiWindow(rootGroup, "Bienvenue sur les Larmes des Erylies !")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true
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
