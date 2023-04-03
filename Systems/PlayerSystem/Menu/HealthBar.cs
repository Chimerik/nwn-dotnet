using System.Threading;
using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;
using System.Threading.Tasks;
using System.Linq;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class HealthBarWindow : PlayerWindow
      {
        private readonly NuiRow root;
        private readonly NuiBind<float> health = new("health");
        private readonly NuiBind<string> readableHealth = new("readableHealth");
        private readonly Color white = new(255, 255, 255);
        private readonly NuiBind<NuiRect> drawListRect = new("drawListRect");
        private readonly NuiBind<Color> color = new("color");
        private readonly Color bleedingColor = new(215, 121, 101);
        private readonly NuiProgress healthBar;

        public HealthBarWindow(Player player) : base(player)
        {
          windowId = "healthBar";

          healthBar = new NuiProgress(health) { Width = 485, Height = 35, ForegroundColor = color, DrawList = new List<NuiDrawListItem>() {
              new NuiDrawListText(white, drawListRect, readableHealth) 
          } };

          root = new NuiRow() { Children = new List<NuiElement>() };

          CreateWindow();
        }
        public void CreateWindow()
        {
          bool closableBind = IsOpen && !closable.GetBindValue(player.oid, nuiToken.Token);
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2 - 250, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 495, 60);

          root.Children.Clear();
          healthBar.Width = windowRectangle.Width - 15;
          root.Children.Add(healthBar);

          window = new NuiWindow(root, "")
          {
            Geometry = geometry,
            Resizable = resizable,
            Collapsed = false,
            Closable = closable,
            Transparent = true,
            Border = false,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            color.SetBindValue(player.oid, nuiToken.Token, ColorConstants.Red);
            readableHealth.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.HP.ToString());
            health.SetBindValue(player.oid, nuiToken.Token, (float)((double)player.oid.LoginCreature.HP / (double)player.MaxHP));
            drawListRect.SetBindValue(player.oid, nuiToken.Token, new((float)(healthBar.Width.Value / StringUtils.GetDrawListTextPositionScaledToUI(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale))), healthBar.Height.Value / 3, 151, 20));
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
            
            closable.SetBindValue(player.oid, nuiToken.Token, closableBind);
            resizable.SetBindValue(player.oid, nuiToken.Token, closableBind);

            IsOpen = true;
            player.oid.OnClientLeave -= SetWindowClosed;
            player.oid.OnClientLeave += SetWindowClosed;
            UpdateCurrentHealth();
          }
        }
        private async void UpdateCurrentHealth()
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          int previousHP = player.oid.LoginCreature.HP;
          int previousRegen = player.healthRegen;

          Task windowClosed = NwTask.WaitUntil(() => !player.oid.IsValid || !IsOpen, tokenSource.Token);
          Task healthChanged = NwTask.WaitUntil(() => !player.oid.IsValid || previousHP != player.oid.LoginCreature.HP || previousRegen != player.healthRegen, tokenSource.Token);

          await NwTask.WhenAny(windowClosed, healthChanged);
          tokenSource.Cancel();

          if (windowClosed.IsCompletedSuccessfully)
            return;

          string currentHP = "";
          string pip = "";

          if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == "CUSTOM_EFFECT_BLEEDING"))
            color.SetBindValue(player.oid, nuiToken.Token, bleedingColor);
          else
            color.SetBindValue(player.oid, nuiToken.Token, ColorConstants.Red);

          if (player.healthRegen > 0)
          {
            for (int i = 0; i < player.healthRegen / 2; i++)
              pip += ">";

            currentHP = player.oid.LoginCreature.HP.ToString() + " " + pip;
          }
          else if (player.healthRegen < 0)
          {
            for (int i = 0; i > player.healthRegen / 2; i--)
              pip += "<";

            currentHP = pip + " " + player.oid.LoginCreature.HP.ToString();
          }
          else
            currentHP = player.oid.LoginCreature.HP.ToString();
 
          readableHealth.SetBindValue(player.oid, nuiToken.Token, currentHP);
          health.SetBindValue(player.oid, nuiToken.Token, (float)((double)player.oid.LoginCreature.HP / (double)player.MaxHP));

          UpdateCurrentHealth();
        }
        private void SetWindowClosed(ModuleEvents.OnClientLeave onLeave)
        {
          IsOpen = false;
        }
      }
    }
  }
}
