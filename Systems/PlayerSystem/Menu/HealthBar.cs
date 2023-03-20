using System.Threading;
using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;
using System.Threading.Tasks;
using System;

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

        private readonly NuiProgress healthBar;

        public HealthBarWindow(Player player) : base(player)
        {
          windowId = "healthBar";

          healthBar = new NuiProgress(health) { Width = 485, Height = 35, ForegroundColor = ColorConstants.Red, DrawList = new List<NuiDrawListItem>() {
              new NuiDrawListText(white, drawListRect, readableHealth) 
          } };

          root = new NuiRow() { Children = new List<NuiElement>() };

          CreateWindow();
        }
        public void CreateWindow()
        {
          bool closableBind = IsOpen && !closable.GetBindValue(player.oid, nuiToken.Token);
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? new NuiRect(player.windowRectangles[windowId].X, player.windowRectangles[windowId].Y, player.windowRectangles[windowId].Width, 45) : new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2 - 250, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 495, 45);

          root.Children.Clear();
          healthBar.Width = windowRectangle.Width - 7;
          root.Children.Add(healthBar);

          window = new NuiWindow(root, "")
          {
            Geometry = geometry,
            Resizable = resizable,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = false,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            int maxHP = player.oid.LoginCreature.LevelInfo[0].HitDie + ((player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2);

            readableHealth.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.HP.ToString());
            health.SetBindValue(player.oid, nuiToken.Token, (float)((double)player.oid.LoginCreature.HP / (double)maxHP));
            drawListRect.SetBindValue(player.oid, nuiToken.Token, new((float)(healthBar.Width.Value / 1.3), 15, 151, 20));

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            closable.SetBindValue(player.oid, nuiToken.Token, closableBind);
            resizable.SetBindValue(player.oid, nuiToken.Token, closableBind);

            IsOpen = true;
            UpdateCurrentHealth();
          }
        }
        private async void UpdateCurrentHealth()
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          int previousHP = player.oid.LoginCreature.HP;

          Task windowClosed = NwTask.WaitUntil(() => !player.oid.IsValid || !IsOpen, tokenSource.Token);
          Task healthChanged = NwTask.WaitUntil(() => !player.oid.IsValid || previousHP != player.oid.LoginCreature.HP, tokenSource.Token);

          await NwTask.WhenAny(windowClosed, healthChanged);
          tokenSource.Cancel();

          if (windowClosed.IsCompletedSuccessfully)
            return;

          int maxHP = player.oid.LoginCreature.LevelInfo[0].HitDie + ((player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2);
          string currentHP = player.oid.LoginCreature.HP.ToString() + " ";

          if (player.oid.LoginCreature.HP < maxHP && player.endurance.regenerableHP > 0)
            for (int i = 0; i < player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_CURRENT_PASSIVE_REGEN").Value / 2; i++)
              currentHP += ">";

          readableHealth.SetBindValue(player.oid, nuiToken.Token, currentHP);
          health.SetBindValue(player.oid, nuiToken.Token, (float)((double)player.oid.LoginCreature.HP / (double)maxHP));

          UpdateCurrentHealth();
        }
      }
    }
  }
}
