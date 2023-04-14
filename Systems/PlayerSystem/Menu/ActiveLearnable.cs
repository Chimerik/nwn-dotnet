using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ActiveLearnableWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn;
        private readonly Color white = new(255, 255, 255);
        private readonly NuiBind<NuiRect> drawListRect = new("drawListRect");
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> name = new("name");
        public readonly NuiBind<string> timeLeft = new("timeLeft");
        public readonly NuiBind<string> level = new("level");
        private Learnable learnable { get; set; }
        private Player target { get; set; }

        public ActiveLearnableWindow(Player player, Player target = null) : base(player)
        {
          windowId = "activeLearnable";

          rootColumn = new NuiColumn() { Children = new List<NuiElement>() { new NuiRow() { Children = new List<NuiElement>()
          {
              new NuiButtonImage(icon) { Height = 40, Width = 40 },
              new NuiLabel(name) { Tooltip = name, Width = 160, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
              new NuiDrawListText(white, drawListRect, timeLeft) } },
              new NuiLabel("Niveau/Max") { Width = 90, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
              new NuiDrawListText(white, drawListRect, level) } }
          } } } };

          CreateWindow(target);
        }
        public void CreateWindow(Player playerTarget = null)
        {
          target = playerTarget != null ? playerTarget : player;
          learnable = target.GetActiveLearnable();

          if (learnable == null)
          {
            player.oid.SendServerMessage("Vous n'avez pas d'apprentissage actif pour le moment. Veuillez utiliser le journal afin de décider de votre prochain apprentissage.", ColorConstants.Red);
            return;
          }

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 320, 100);

          window = new NuiWindow(rootColumn, "Apprentissage en cours")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = collapsed,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            drawListRect.SetBindValue(player.oid, nuiToken.Token, Utils.GetDrawListTextScaleFromPlayerUI(player));
            timeLeft.SetBindValue(player.oid, nuiToken.Token, learnable.GetReadableTimeSpanToNextLevel(target));
            icon.SetBindValue(player.oid, nuiToken.Token, learnable.icon);
            name.SetBindValue(player.oid, nuiToken.Token, learnable.name);
            level.SetBindValue(player.oid, nuiToken.Token, $"{learnable.currentLevel}/{learnable.maxLevel}");

            collapsed.SetBindValue(player.oid, nuiToken.Token, false);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
      }
    }
  }
}
