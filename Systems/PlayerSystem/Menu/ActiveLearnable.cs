using System;
using System.Collections.Generic;

using Anvil.API;
using Anvil.Services;

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
        private readonly NuiRect drawListRect = new(0, 35, 150, 60);
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> name = new("name");
        public readonly NuiBind<string> timeLeft = new("timeLeft");
        private readonly NuiBind<string> level = new("level");
        //bool stopPreviousSPGain { get; set; }
        Learnable learnable { get; set; }

        public ActiveLearnableWindow(Player player) : base(player)
        {
          windowId = "activeLearnable";

          rootColumn = new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow()
              {
                Children = new List<NuiElement>()
                {
                    new NuiButtonImage(icon) { Height = 40, Width = 40 },
                    new NuiLabel(name) { Tooltip = name, Width = 160, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
                    new NuiDrawListText(white, drawListRect, timeLeft) } },
                    new NuiLabel("Niveau/Max") { Width = 90, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
                    new NuiDrawListText(white, drawListRect, level) } }
                }
              }
            }
          };

          CreateWindow();
        }
        public void CreateWindow()
        {
          learnable = player.GetActiveLearnable();

          if (learnable == null)
          {
            player.oid.SendServerMessage("Vous n'avez pas d'apprentissage actif pour le moment. Veuillez utiliser le journal afin de décider de votre prochain apprentissage.", ColorConstants.Red);
            return;
          }

          if (IsOpen)
            return;

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 320, 100);

          window = new NuiWindow(rootColumn, "Apprentissage en cours")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            timeLeft.SetBindValue(player.oid, nuiToken.Token, learnable.GetReadableTimeSpanToNextLevel(player));
            icon.SetBindValue(player.oid, nuiToken.Token, learnable.icon);
            name.SetBindValue(player.oid, nuiToken.Token, learnable.name);
            level.SetBindValue(player.oid, nuiToken.Token, $"{learnable.currentLevel}/{learnable.maxLevel}");

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            //stopPreviousSPGain = true;
            //DelayStartSPGain();
          }
        }

        /*private async void RefreshWindowUntillClosed()
        {
          ScheduledTask scheduler = player.scheduler.ScheduleRepeating(() =>
          {
            learnable.acquiredPoints += player.GetSkillPointsPerSecond(learnable);
            timeLeft.SetBindValue(player.oid, nuiToken.Token, learnable.GetReadableTimeSpanToNextLevel(player));
            player.oid.ExportCharacter();
          }, TimeSpan.FromSeconds(1));

          await NwTask.WaitUntil(() => player.oid.LoginCreature == null || !IsOpen || stopPreviousSPGain || !learnable.active);
          scheduler.Dispose();
        }
        private async void DelayStartSPGain()
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          stopPreviousSPGain = false;
          RefreshWindowUntillClosed();
        }*/
      }
    }
  }
}
