using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class FishinMiniGame : PlayerWindow
      {
        NuiGroup strengthGroup { get; }
        NuiGroup fishGroup { get; }
        NuiGroup successGroup { get; }
        NuiColumn successCol { get; }
        NuiColumn rootColumn { get; }
        NuiColumn strengthCol { get; }
        NuiColumn fishCol { get; }
        NuiBind<float> fishingStrengthBind { get; }
        NuiBind<float> successBind { get; }
        NuiBind<Color> successColorBind { get; }
        NuiProgress fishingStrengthProgress { get; }
        NuiProgress successProgress { get; }
        private readonly NuiBind<Color> fishingStrengthColor;
        private readonly Color red = new (255, 0, 0);
        private readonly Color yellow = new (255, 255, 0);
        private readonly Color green = new (32, 255, 32);
        int fishingState { get; set; }
        float strValueChg { get; set; }
        NuiBind<NuiRect> weightPos { get; }
        NuiBind<NuiRect> fishPos { get; }
        float weightSpeed { get; set; }
        float weightAcceleration { get; set; }
        float fishSpeed { get; set; }
        //NuiDrawListPolyLine successBar { get; }

        public FishinMiniGame(Player player) : base(player)
        {
          windowId = "fishing";

          List<NuiElement> strengthChildren = new List<NuiElement>();
          strengthCol = new NuiColumn() { Children = strengthChildren };
          List<NuiElement> rootList = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootList };
          strengthGroup = new NuiGroup() { Id = "fishingStrengthGroup", Border = false, Height = 60, Padding = 0, Margin = 0, Layout = strengthCol };
          rootList.Add(strengthGroup);

          fishingState = 0;
          fishingStrengthBind = new NuiBind<float>("fishingStrength");
          fishingStrengthColor = new NuiBind<Color>("fishingStrengthColor");
          fishingStrengthProgress = new NuiProgress(fishingStrengthBind) { ForegroundColor = fishingStrengthColor, Width = 280 };
          strengthChildren.Add(fishingStrengthProgress);

          List<NuiElement> fishingChildren = new List<NuiElement>();
          fishCol = new NuiColumn() { Children = fishingChildren, Visible = false };
          fishGroup = new NuiGroup() { Id = "fishingGroup", Border = false, Padding = 0, Margin = 0, Layout = fishCol };
          rootList.Add(fishGroup);

          weightPos = new NuiBind<NuiRect>("weightPos");
          fishPos = new NuiBind<NuiRect>("fishPos");

          //successBar = new NuiDrawListPolyLine(green, true, 100, new List<float>() { 175, 460, 150, 460, 150, 420, 175, 420 });

          fishingChildren.Add(new NuiSpacer() { Id = "fishingWidget",
            DrawList = new List<NuiDrawListItem>() 
            { 
              new NuiDrawListImage("fishingrod", new NuiRect(0, 0, 0, 0)),
              new NuiDrawListImage("fishingweight", weightPos),
              new NuiDrawListImage("fish", fishPos),
            } 
          });

          List<NuiElement> successChildren = new List<NuiElement>();
          successCol = new NuiColumn() { Children = successChildren, Visible = false };
          successGroup = new NuiGroup() { Id = "successGroup", Border = false, Height = 60, Padding = 0, Margin = 0, Layout = successCol };
          rootList.Add(successGroup);

          successBind = new NuiBind<float>("success");
          successColorBind = new NuiBind<Color>("successColor");
          successProgress = new NuiProgress(successBind) { ForegroundColor = successColorBind, Width = 280 };
          successChildren.Add(successProgress);

          //fishingChildren.Add(new NuiImage("diceroll"));

          CreateWindow();
        }

        public void CreateWindow()
        {
          NuiRect windowRectangle = new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2 - 150, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 6, 300, 500);

          window = new NuiWindow(rootColumn, "")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = false,
            Transparent = true,
            Border = false,
          };

          player.oid.OnNuiEvent -= HandleFishingStrengthEvents;
          player.oid.OnNuiEvent += HandleFishingStrengthEvents;
          player.oid.OnServerSendArea -= OnAreaChangeCloseWindow;
          player.oid.OnServerSendArea += OnAreaChangeCloseWindow;

          token = player.oid.CreateNuiWindow(window, windowId);

          weightSpeed = 0;

          fishingStrengthBind.SetBindValue(player.oid, token, 0);
          fishingStrengthColor.SetBindValue(player.oid, token, red);

          successBind.SetBindValue(player.oid, token, 0.2f);

          resizable.SetBindValue(player.oid, token, true);
          weightPos.SetBindValue(player.oid, token, new NuiRect(82, 335, 0, 0));
          fishPos.SetBindValue(player.oid, token, new NuiRect(82, 425, 0, 0));
          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true); fishSpeed = 0.2f;

          fishSpeed = 0.2f;
        }
        private void HandleFishingStrengthEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          if (nuiEvent.EventType == NuiEventType.MouseDown && fishingState == 0)
            StartStrengthProgress();
          else if(nuiEvent.EventType == NuiEventType.MouseUp && fishingState == 1)
          {
            fishingState = 2;

            strengthCol.Visible = false;
            fishCol.Visible = true;
            successCol.Visible = true;

            strengthGroup.SetLayout(player.oid, token, strengthCol);
            fishGroup.SetLayout(player.oid, token, fishCol);
            successGroup.SetLayout(player.oid, token, successCol);

            weightSpeed = 0;
            weightAcceleration = 0;

            StartFishMovement();
            StartWeightMovement();
          }
          else if (nuiEvent.EventType == NuiEventType.MouseDown)
            weightAcceleration = 0.05f;
          else if (nuiEvent.EventType == NuiEventType.MouseUp)
            weightAcceleration = - 0.05f;
        }

        private async void StartStrengthProgress()
        {
          strValueChg = 0.01f;

          await Task.Run(async () =>
          {
            var spawnScheduler = player.scheduler.ScheduleRepeating(() =>
            {

              float currentValue = fishingStrengthBind.GetBindValue(player.oid, token);

              if (currentValue > 0.4)
              {
                if (fishingStrengthColor.GetBindValue(player.oid, token) != green)
                  fishingStrengthColor.SetBindValue(player.oid, token, green);

                if (currentValue > 0.99)
                  strValueChg = -0.01f;
              }
              else if (currentValue > 0.2)
              {
                if (fishingStrengthColor.GetBindValue(player.oid, token) != yellow)
                  fishingStrengthColor.SetBindValue(player.oid, token, yellow);
              }
              else
              {
                if (fishingStrengthColor.GetBindValue(player.oid, token) != red)
                  fishingStrengthColor.SetBindValue(player.oid, token, red);

                if (currentValue < 0.01)
                  strValueChg = 0.01f;
              }

              fishingStrengthBind.SetBindValue(player.oid, token, currentValue + strValueChg);
            }, TimeSpan.FromMilliseconds(1));

            fishingState = 1;

            await NwTask.WaitUntil(() => !player.oid.LoginCreature.IsValid || fishingState != 1 || !player.openedWindows.ContainsKey(windowId));
            spawnScheduler.Dispose();
          });
        }

        private async void StartWeightMovement()
        {
          await Task.Run(async () =>
          {
            var spawnScheduler = player.scheduler.ScheduleRepeating(() =>
            {

              NuiRect oldPos = weightPos.GetBindValue(player.oid, token);
              float pos = oldPos.Y - weightSpeed;

              weightSpeed += weightAcceleration;

              if (pos > 335)
              {
                if (weightSpeed > -0.16 && weightSpeed < 0.05)
                {
                  weightSpeed = 0;
                  return;
                }
                else
                  weightSpeed = weightSpeed < 0 ? -(weightSpeed * 0.65f) : weightSpeed * 0.65f;
              }
              else if (pos < 23)
              {
                weightSpeed = 0;
                return;
              }

              weightPos.SetBindValue(player.oid, token, new NuiRect(oldPos.X, pos, oldPos.Width, oldPos.Height));

            }, TimeSpan.FromMilliseconds(10));

            await NwTask.WaitUntil(() => !player.oid.LoginCreature.IsValid || !player.openedWindows.ContainsKey(windowId));
            spawnScheduler.Dispose();
          });
        }
        private async void HandleFishMove()
        {
          await Task.Run(async () =>
          {
            var spawnScheduler = player.scheduler.ScheduleRepeating(() =>
            {

              NuiRect oldPos = fishPos.GetBindValue(player.oid, token);
              float yPos = oldPos.Y - fishSpeed;

              if (yPos < 23)
                yPos = 23;
              else if (yPos > 425)
                yPos = 425;

              fishPos.SetBindValue(player.oid, token, new NuiRect(oldPos.X, yPos, oldPos.Width, oldPos.Height));
              float weightYPos = weightPos.GetBindValue(player.oid, token).Y;

              float successValue = successBind.GetBindValue(player.oid, token);

              if (successValue < 1 && yPos > weightYPos && yPos < weightYPos + 90)
              {
                successValue += 0.01f;
                successBind.SetBindValue(player.oid, token, successValue);
              }
              else if (successValue > 0)
              {
                successValue -= 0.01f;
                successBind.SetBindValue(player.oid, token, successValue);
              }

              if (successValue <= 0)
                Log.Info("PERDU !");
              else if (successValue >= 1)
                Log.Info("GAGNE !");

              if (successValue > 0.6)
              {
                if (successColorBind.GetBindValue(player.oid, token) != green)
                  successColorBind.SetBindValue(player.oid, token, green);
              }
              else if (successValue > 0.3)
              {
                if (successColorBind.GetBindValue(player.oid, token) != yellow)
                  successColorBind.SetBindValue(player.oid, token, yellow);
              }
              else
              {
                if (successColorBind.GetBindValue(player.oid, token) != red)
                  successColorBind.SetBindValue(player.oid, token, red);
              }

            }, TimeSpan.FromMilliseconds(10));

            await NwTask.WaitUntil(() => !player.oid.LoginCreature.IsValid || !player.openedWindows.ContainsKey(windowId));
            spawnScheduler.Dispose();
          });
        }
        private async void SetFishSpeed()
        {
          await Task.Run(async () =>
          {
            var spawnScheduler = player.scheduler.ScheduleRepeating(() =>
            {

              float yPos = fishPos.GetBindValue(player.oid, token).Y;
              fishSpeed = Utils.random.NextFloat();

              if (yPos < 100)
                fishSpeed = Utils.random.Next(4) < 3 ? fishSpeed : -fishSpeed;
              else if (yPos < 225)
                fishSpeed = Utils.random.Next(2) < 1 ? fishSpeed : -fishSpeed;
              else if (yPos < 335)
                fishSpeed = Utils.random.Next(4) < 3 ? -fishSpeed : fishSpeed;

              fishSpeed *= Utils.random.Next(1, 4);

            }, TimeSpan.FromMilliseconds(250));

            await NwTask.WaitUntil(() => !player.oid.LoginCreature.IsValid || !player.openedWindows.ContainsKey(windowId));
            spawnScheduler.Dispose();
          });
        }
        private void StartFishMovement()
        {
          weightPos.SetBindValue(player.oid, token, new NuiRect(82, 335, 0, 0));
          fishPos.SetBindValue(player.oid, token, new NuiRect(82, 425, 0, 0));
          SetFishSpeed();
          HandleFishMove();
        }
      }
    }
  }
}
