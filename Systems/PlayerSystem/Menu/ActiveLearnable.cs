using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly NuiColor white = new NuiColor(255, 255, 255);
        private readonly NuiRect drawListRect = new NuiRect(0, 35, 150, 60);
        private readonly NuiBind<string> icon = new NuiBind<string>("icon");
        private readonly NuiBind<string> name = new NuiBind<string>("name");
        private readonly NuiBind<string> timeLeft = new NuiBind<string>("timeLeft");
        private readonly NuiBind<int> currentLevel = new NuiBind<int>("currentLevel");
        private readonly NuiBind<int> maxLevel = new NuiBind<int>("maxLevel");
        bool stopPreviousSPGain { get; set; }
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
                    new NuiDrawListText(white, drawListRect, $"{currentLevel}/{maxLevel}") } }
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

          token = player.oid.CreateNuiWindow(window, windowId);

          timeLeft.SetBindValue(player.oid, token, learnable.GetReadableTimeSpanToNextLevel(player));
          icon.SetBindValue(player.oid, token, learnable.icon);
          name.SetBindValue(player.oid, token, learnable.name);
          currentLevel.SetBindValue(player.oid, token, learnable.currentLevel);
          maxLevel.SetBindValue(player.oid, token, learnable.maxLevel);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;

          stopPreviousSPGain = true;
          DelayStartSPGain();
        }

        private async void RefreshWindowUntillClosed()
        {
          var scheduler = ModuleSystem.scheduler.ScheduleRepeating(() =>
          {
            learnable.acquiredPoints += player.GetSkillPointsPerSecond(learnable);
            timeLeft.SetBindValue(player.oid, token, learnable.GetReadableTimeSpanToNextLevel(player));
            player.oid.ExportCharacter();
          }, TimeSpan.FromSeconds(1));

          await NwTask.WaitUntil(() => player.oid.LoginCreature == null || !player.openedWindows.ContainsKey(windowId) || stopPreviousSPGain || !learnable.active);
          scheduler.Dispose();
        }
        private async void DelayStartSPGain()
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          stopPreviousSPGain = false;
          RefreshWindowUntillClosed();
        }
      }
    }
  }
}
