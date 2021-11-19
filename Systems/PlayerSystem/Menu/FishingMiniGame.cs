using System;
using System.Collections.Generic;
using System.Threading;
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
        NuiBind<NuiColor> successColorBind { get; }
        NuiProgress fishingStrengthProgress { get; }
        NuiProgress successProgress { get; }
        NuiBind<NuiColor> fishingStrengthColor { get; }
        NuiColor red { get; }
        NuiColor yellow { get; }
        NuiColor green { get; }
        int fishingState { get; set; }
        float strValueChg { get; set; }
        NuiBind<NuiRect> weightPos { get; }
        NuiBind<NuiRect> fishPos { get; }
        float weightSpeed { get; set; }
        float fishSpeed { get; set; }
        //NuiDrawListPolyLine successBar { get; }

        public FishinMiniGame(Player player) : base(player)
        {
          windowId = "fishing";

          red = new NuiColor(255, 0, 0);
          yellow = new NuiColor(255, 255, 0);
          green = new NuiColor(32, 255, 32);

          List<NuiElement> strengthChildren = new List<NuiElement>();
          strengthCol = new NuiColumn() { Children = strengthChildren };
          List<NuiElement> rootList = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootList };
          strengthGroup = new NuiGroup() { Id = "fishingStrengthGroup", Border = false, Height = 60, Padding = 0, Margin = 0, Layout = strengthCol };
          rootList.Add(strengthGroup);

          fishingState = 0;
          fishingStrengthBind = new NuiBind<float>("fishingStrength");
          fishingStrengthColor = new NuiBind<NuiColor>("fishingStrengthColor");
          fishingStrengthProgress = new NuiProgress(fishingStrengthBind) { ForegroundColor = fishingStrengthColor, Width = 280 };
          strengthChildren.Add(fishingStrengthProgress);

          List<NuiElement> fishingChildren = new List<NuiElement>();
          fishCol = new NuiColumn() { Children = fishingChildren };
          fishGroup = new NuiGroup() { Id = "fishingGroup", Border = false, Padding = 0, Margin = 0, Layout = fishCol, Visible = false };
          rootList.Add(fishGroup);

          weightPos = new NuiBind<NuiRect>("weightPos");
          fishPos = new NuiBind<NuiRect>("fishPos");

          //successBar = new NuiDrawListPolyLine(green, true, 100, new List<float>() { 175, 460, 150, 460, 150, 420, 175, 420 });

          fishingChildren.Add(new NuiSpacer() { Id = "fishingWidget",
            DrawList = new List<NuiDrawListItem>() 
            { 
              new NuiDrawListImage("fishingrod", new NuiRect(0, 0, 0, 0)) { },
              new NuiDrawListImage("fishingweight", weightPos),
              new NuiDrawListImage("fish", fishPos),
              //successBar
            } 
          });

          List<NuiElement> successChildren = new List<NuiElement>();
          successCol = new NuiColumn() { Children = successChildren };
          successGroup = new NuiGroup() { Id = "successGroup", Border = false, Height = 60, Padding = 0, Margin = 0, Layout = successCol, Visible = false };
          rootList.Add(successGroup);

          successBind = new NuiBind<float>("success");
          successColorBind = new NuiBind<NuiColor>("successColor");
          successProgress = new NuiProgress(successBind) { ForegroundColor = successColorBind, Width = 400 };
          successChildren.Add(successProgress);

          /*successRow = new NuiRow()
          {
            Children = new List<NuiElement> { 
              new NuiSpacer()
              {
                Id = "fishingSuccessBar",
                DrawList = new List<NuiDrawListItem>()
                {
                  successBar
                }
              } 
            }
          };
          successBarGroup = new NuiGroup() { Id = "successBarGroup", Border = false, Padding = 0, Margin = 0, Layout = successRow };

          fishingChildren.Add(successBarGroup);*/

          //fishingChildren.Add(new NuiImage("diceroll"));

          CreateWindow();
        }

        public void CreateWindow()
        {
          NuiRect windowRectangle = new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2 - 150, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 6, 350, 600);

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

          token = player.oid.CreateNuiWindow(window, windowId);

          weightSpeed = 0;

          fishingStrengthBind.SetBindValue(player.oid, token, 0);
          //fishingStrengthBind.SetBindWatch(player.oid, token, true);
          fishingStrengthColor.SetBindValue(player.oid, token, red);

          successBind.SetBindValue(player.oid, token, 0.2f);
          successBind.SetBindWatch(player.oid, token, true);

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

          if (nuiEvent.EventType == NuiEventType.Watch)
          {
            switch(nuiEvent.ElementId)
            {
              case "fishingStrength":
                Log.Info("prout");
                float strengthValue = fishingStrengthBind.GetBindValue(player.oid, token);

                Log.Info($"str value : {strengthValue}");

                if(strengthValue > 0.4)
                  fishingStrengthColor.SetBindValue(player.oid, token, green);
                else if(strengthValue > 0.2)
                  fishingStrengthColor.SetBindValue(player.oid, token, yellow);
                else
                  fishingStrengthColor.SetBindValue(player.oid, token, red);
                
                return;

              case "success":

                float successValue = successBind.GetBindValue(player.oid, token);

                if (successValue <= 0)
                  Log.Info("PERDU !");
                else if (successValue >= 1)
                  Log.Info("GAGNE !");

                if (successValue > 0.6)
                  successColorBind.SetBindValue(player.oid, token, green);
                else if (successValue > 0.3)
                  successColorBind.SetBindValue(player.oid, token, yellow);
                else
                  successColorBind.SetBindValue(player.oid, token, red);

                return;
            }
          }

          if (nuiEvent.EventType == NuiEventType.MouseDown && fishingState == 0)
          {
            strValueChg = 0.01f;
            
            HandleStrengthValueChange();
            //HandleStrengthColorChange();
            StartStrengthProgress();

            fishingState = 1;
          }
          else if(nuiEvent.EventType == NuiEventType.MouseUp && fishingState == 1)
          {
            /*fishingState = 2;

            strengthGroup.Visible = false;
            fishGroup.Visible = true;
            successGroup.Visible = true;

            weightSpeed += 1; 
            strengthGroup.SetLayout(player.oid, token, strengthCol);
            fishGroup.SetLayout(player.oid, token, fishCol);
            successGroup.SetLayout(player.oid, token, successCol);

            StartFishMovement();*/
          }
          else if (nuiEvent.EventType == NuiEventType.MouseDown && fishingState == 2)
          {
            HandleFishingWeightPush();
          }
          else if (nuiEvent.EventType == NuiEventType.MouseUp && fishingState == 2)
          {
            weightSpeed -= 1;
            HandleFishingWeightDrop();
          }
        }
        private async void HandleStrengthValueChange() // Utiliser un scheduler plutôt que de l'async pour gérer ces trucs là
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();
          
          Task fishingCancelled = NwTask.WaitUntil(() => !player.oid.IsValid || fishingState != 1 || !player.openedWindows.ContainsKey(windowId), tokenSource.Token);
          Task strValueGreaterThan1 = NwTask.WaitUntil(() => fishingStrengthBind.GetBindValue(player.oid, token) > 0.99, tokenSource.Token);
          Task strValueLowerThan0 = NwTask.WaitUntil(() => fishingStrengthBind.GetBindValue(player.oid, token) < 0.01, tokenSource.Token);
          
          await NwTask.WhenAny(fishingCancelled, strValueGreaterThan1, strValueLowerThan0);
          tokenSource.Cancel();

          if (fishingCancelled.IsCompletedSuccessfully)
            return;

          if (strValueGreaterThan1.IsCompletedSuccessfully)
            strValueChg = -0.01f;

          if (strValueLowerThan0.IsCompletedSuccessfully)
            strValueChg = 0.01f;

          HandleStrengthValueChange();
        }

        /*private async void HandleStrengthColorChange()
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task fishingCancelled = NwTask.WaitUntil(() => !player.oid.IsValid || fishingState != 1 || !player.openedWindows.ContainsKey(windowId), tokenSource.Token);
          Task strValueGreaterThan02 = NwTask.WaitUntil(() =>  fishingStrengthBind.GetBindValue(player.oid, token) >= 0.2, tokenSource.Token);

          await NwTask.WhenAny(fishingCancelled, strValueGreaterThan02);
          tokenSource.Cancel();

          if (fishingCancelled.IsCompletedSuccessfully)
            return;

          fishingStrengthColor.SetBindValue(player.oid, token, yellow);
          HandleStrengthGreenColorChange();
        }

        private async void HandleStrengthGreenColorChange()
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task fishingCancelled = NwTask.WaitUntil(() => !player.oid.IsValid || fishingState != 1 || !player.openedWindows.ContainsKey(windowId), tokenSource.Token);
          Task strValueGreaterThan04 = NwTask.WaitUntil(() => fishingStrengthBind.GetBindValue(player.oid, token) >= 0.4, tokenSource.Token);

          await NwTask.WhenAny(fishingCancelled, strValueGreaterThan04);
          tokenSource.Cancel();

          if (fishingCancelled.IsCompletedSuccessfully)
            return;

          fishingStrengthColor.SetBindValue(player.oid, token, green);

          HandleStrengthYellowColorChange();
        }

        private async void HandleStrengthYellowColorChange()
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task fishingCancelled = NwTask.WaitUntil(() => !player.oid.IsValid || fishingState != 1 || !player.openedWindows.ContainsKey(windowId), tokenSource.Token);
          Task strValueLowerThan04 = NwTask.WaitUntil(() => fishingStrengthBind.GetBindValue(player.oid, token) <= 0.4, tokenSource.Token);

          await NwTask.WhenAny(fishingCancelled, strValueLowerThan04);
          tokenSource.Cancel();

          if (fishingCancelled.IsCompletedSuccessfully)
            return;

          fishingStrengthColor.SetBindValue(player.oid, token, yellow);

          HandleStrengthRedColorChange();
        }

        private async void HandleStrengthRedColorChange()
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task fishingCancelled = NwTask.WaitUntil(() => !player.oid.IsValid || fishingState != 1 || !player.openedWindows.ContainsKey(windowId), tokenSource.Token);
          Task strValueLowerThan02 = NwTask.WaitUntil(() => fishingStrengthBind.GetBindValue(player.oid, token) <= 0.2, tokenSource.Token);

          await NwTask.WhenAny(fishingCancelled, strValueLowerThan02);
          tokenSource.Cancel();

          if (fishingCancelled.IsCompletedSuccessfully)
            return;

          fishingStrengthColor.SetBindValue(player.oid, token, red);
          HandleStrengthColorChange();
        }*/

        private async void StartStrengthProgress()
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task fishingCancelled = NwTask.WaitUntil(() => !player.oid.IsValid || fishingState != 1 || !player.openedWindows.ContainsKey(windowId), tokenSource.Token);
          Task timeElapsed = NwTask.Delay(TimeSpan.FromMilliseconds(1), tokenSource.Token);

          await NwTask.WhenAny(fishingCancelled, timeElapsed);
          tokenSource.Cancel();

          if (fishingCancelled.IsCompletedSuccessfully)
            return;

          fishingStrengthBind.SetBindValue(player.oid, token, fishingStrengthBind.GetBindValue(player.oid, token) + strValueChg);
          StartStrengthProgress();
        }
        private async void HandleFishingWeightPush()
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task fishingCancelled = NwTask.WaitUntil(() => !player.oid.IsValid || weightPos.GetBindValue(player.oid, token).Y  < 23 || fishingState != 1 || !player.openedWindows.ContainsKey(windowId), tokenSource.Token);
          Task timeElapsed = NwTask.Delay(TimeSpan.FromMilliseconds(1), tokenSource.Token);

          await NwTask.WhenAny(fishingCancelled, timeElapsed);
          tokenSource.Cancel();

          if (fishingCancelled.IsCompletedSuccessfully)
          {
            if (weightPos.GetBindValue(player.oid, token).Y < 23)
              weightSpeed = 0;
            return;
          }

          NuiRect oldPos = weightPos.GetBindValue(player.oid, token);

          float yPos = oldPos.Y - weightSpeed;
          if (yPos > 334)
          {
            yPos = 334;
            weightSpeed = 0;
          }

          NuiRect newPos = new NuiRect(oldPos.X, yPos, oldPos.Width, oldPos.Height);

          weightPos.SetBindValue(player.oid, token, newPos);

          weightSpeed += 0.05f;

          HandleFishingWeightPush();
        }
        private async void HandleFishingWeightDrop()
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task fishingCancelled = NwTask.WaitUntil(() => !player.oid.IsValid || fishingState != 0 || !player.openedWindows.ContainsKey(windowId), tokenSource.Token);
          Task bottomReached = NwTask.WaitUntil(() => weightPos.GetBindValue(player.oid, token).Y > 334, tokenSource.Token);
          Task timeElapsed = NwTask.Delay(TimeSpan.FromMilliseconds(1), tokenSource.Token);

          await NwTask.WhenAny(fishingCancelled, bottomReached, timeElapsed);
          tokenSource.Cancel();

          if (fishingCancelled.IsCompletedSuccessfully)
            return;

          if(bottomReached.IsCompletedSuccessfully)
          {
            weightSpeed = weightSpeed < 0 ? -(weightSpeed * 0.65f) : weightSpeed * 0.65f;

            if (weightSpeed < 0.1 && weightSpeed > -0.1)
              return;
          }

          NuiRect oldPos = weightPos.GetBindValue(player.oid, token);

          float yPos = oldPos.Y - weightSpeed;
          if(yPos < 23)
          {
            yPos = 23;
            weightSpeed = 0;
          }

          NuiRect newPos = new NuiRect(oldPos.X, yPos, oldPos.Width, oldPos.Height);

          weightPos.SetBindValue(player.oid, token, newPos);

          weightSpeed -= 0.05f;

          HandleFishingWeightDrop();
        }
        private async void HandleFishMove()
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task fishingCancelled = NwTask.WaitUntil(() => !player.oid.IsValid || !player.openedWindows.ContainsKey(windowId), tokenSource.Token);
          Task timeElapsed = NwTask.Delay(TimeSpan.FromMilliseconds(10), tokenSource.Token);

          await NwTask.WhenAny(fishingCancelled, timeElapsed);
          tokenSource.Cancel();

          if (fishingCancelled.IsCompletedSuccessfully)
            return;

          NuiRect oldPos = fishPos.GetBindValue(player.oid, token);

          float yPos = oldPos.Y - fishSpeed;

          if (yPos < 23)
            yPos = 23;
          else if (yPos > 425)
            yPos = 425;

          NuiRect newPos = new NuiRect(oldPos.X, yPos, oldPos.Width, oldPos.Height);
          fishPos.SetBindValue(player.oid, token, newPos);

          float weightYPos = weightPos.GetBindValue(player.oid, token).Y;
          
          if (yPos > weightYPos && yPos > weightYPos + 50)
            successBind.SetBindValue(player.oid, token, successBind.GetBindValue(player.oid, token) + 0.01f);
          else
            successBind.SetBindValue(player.oid, token, successBind.GetBindValue(player.oid, token) - 0.01f);

          HandleFishMove();
        }
        private async void SetFishSpeed()
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task fishingCancelled = NwTask.WaitUntil(() => !player.oid.IsValid || !player.openedWindows.ContainsKey(windowId), tokenSource.Token);
          Task timeElapsed = NwTask.Delay(TimeSpan.FromMilliseconds(250), tokenSource.Token);

          await NwTask.WhenAny(fishingCancelled, timeElapsed);
          tokenSource.Cancel();

          if (fishingCancelled.IsCompletedSuccessfully)
            return;

          float yPos = fishPos.GetBindValue(player.oid, token).Y;
          fishSpeed = Utils.random.NextFloat();

          if (yPos < 100)
            fishSpeed = Utils.random.Next(4) < 3 ? fishSpeed : - fishSpeed;
          else if (yPos < 225)
            fishSpeed = Utils.random.Next(2) < 1 ? fishSpeed : -fishSpeed;
          else if (yPos < 334)
            fishSpeed = Utils.random.Next(4) < 3 ? - fishSpeed : fishSpeed;

          fishSpeed *= Utils.random.Next(1, 4);

          SetFishSpeed();
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
