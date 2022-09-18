using System;
using System.Collections.Generic;

using Anvil.API;
using Anvil.Services;

using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class RebootCountdownWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new ();
        private readonly NuiBind<string> timeLeft = new ("timeLeft");
        private ScheduledTask scheduler;

        public RebootCountdownWindow(Player player) : base(player)
        {
          windowId = "rebootCountdown";

          rootColumn.Children = new ()
          {
            new NuiRow()
            {
              Children = new ()
              {
                  new NuiLabel("Redémarrage du module dans : ") { Width = 160, Height = 35, HorizontalAlign = NuiHAlign.Center },
                  new NuiLabel(timeLeft) { Width = 20, Height = 35, HorizontalAlign = NuiHAlign.Center }
              }
            }
          };

          window = new NuiWindow(rootColumn, "")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = false,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            timeLeft.SetBindValue(player.oid, nuiToken.Token, "30");

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2 - 200, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 300, 80));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            RefreshWindowUntillClosed();
          } 
        }

        private void RefreshWindowUntillClosed()
        {
          scheduler = player.scheduler.ScheduleRepeating(() =>
          {
            if(player.oid.LoginCreature == null)
            {
              scheduler.Dispose();
              return;
            }

            player.oid.PlaySound("gui_select");

            int remainingTime = int.Parse(timeLeft.GetBindValue(player.oid, nuiToken.Token)) - 1;
            timeLeft.SetBindValue(player.oid, nuiToken.Token, remainingTime.ToString());
            
            if(remainingTime < 1)
            {
              scheduler.Dispose();
              player.oid.BootPlayer("Le serveur redémarre. Vous pourrez vous reconnecter dans une minute.");
            }

          }, TimeSpan.FromSeconds(1));
        }
      }
    }
  }
}
