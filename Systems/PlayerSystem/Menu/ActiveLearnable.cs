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
        NuiGroup rootGroup { get; }
        NuiColumn rootColumn { get; }
        List<NuiElement> rootChidren { get; }
        NuiColor white { get; }
        NuiRect drawListRect { get; }
        Learnable learnable { get; set; }
        NuiBind<string> timeLeft { get; }
        bool stopPreviousSPGain { get; set; }

        public ActiveLearnableWindow(Player player, int learnableId = -1) : base(player)
        {
          windowId = "activeLearnable";

          white = new NuiColor(255, 255, 255);
          drawListRect = new NuiRect(0, 35, 150, 60);

          timeLeft = new NuiBind<string>("timeLeft");

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "learnableGroup", Border = true, Layout = rootColumn };

          learnable = GetActiveLearnable(learnableId);

          if (learnable == null)
          {
            player.oid.SendServerMessage("Vous n'avez pas d'apprentissage actif pour le moment. Veuillez utiliser le journal afin de décider de votre prochain apprentissage.", ColorConstants.Red);
            return;
          }

          LearnableActivation();

          CreateWindow();
        }
        public void CreateWindow(int learnableId = -1)
        {
          if(learnableId > -1)
            learnable = GetActiveLearnable(learnableId);

          if (learnable == null)
          {
            player.oid.SendServerMessage("Vous n'avez pas d'apprentissage actif pour le moment. Veuillez utiliser le journal afin de décider de votre prochain apprentissage.", ColorConstants.Red);
            return;
          }

          rootChidren.Clear();

          NuiRow row = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
                new NuiButtonImage(learnable.icon) { Height = 40, Width = 40 },
                new NuiLabel(learnable.name) { Tooltip = learnable.name, Width = 160, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
                new NuiDrawListText(white, drawListRect, timeLeft) } },
                new NuiLabel("Niveau/Max") { Width = 90, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
                new NuiDrawListText(white, drawListRect, $"{learnable.currentLevel}/{learnable.maxLevel}") } }
            }
          };

          rootChidren.Add(row);

          LearnableActivation();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 320, 110);

          window = new NuiWindow(rootGroup, "Apprentissage en cours")
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

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;

          stopPreviousSPGain = true;
          DelayStartSPGain();
        }

        private Learnable GetActiveLearnable(int learnableId)
        {
          if (learnableId > -1)
          {
            if (learnableId < 10000)
              return learnable = player.learnableSpells[learnableId];
            else
              return learnable = player.learnableSkills[learnableId];
          }
          else
          {
            if (player.learnableSpells.Any(l => l.Value.active))
              return player.learnableSpells.FirstOrDefault(l => l.Value.active).Value;
            else if (player.learnableSkills.Any(l => l.Value.active))
              return player.learnableSkills.FirstOrDefault(l => l.Value.active).Value;
            else
              return null;
          }
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

        private void LearnableActivation()
        {
          if (player.learnableSpells.Any(l => l.Value.active))
            player.learnableSpells.FirstOrDefault(l => l.Value.active).Value.active = false;
          else if (player.learnableSkills.Any(l => l.Value.active))
            player.learnableSkills.FirstOrDefault(l => l.Value.active).Value.active = false;

          learnable.active = true;
          learnable.spLastCalculation = DateTime.Now;
          learnable.AwaitPlayerStateChangeToCalculateSPGain(player);
        }
      }
    }
  }
}
