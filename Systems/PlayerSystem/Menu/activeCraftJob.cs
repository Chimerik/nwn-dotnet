using System;
using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ActiveCraftJobWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn;
        private readonly Color white = new (255, 255, 255);
        private readonly NuiRect drawListRect = new (0, 35, 150, 60);
        private readonly NuiBind<string> icon = new ("icon");
        private readonly NuiBind<string> name = new ("name");
        public readonly NuiBind<string> timeLeft = new ("timeLeft");

        public ActiveCraftJobWindow(Player player) : base(player)
        {
          windowId = "activeCraftJob";

          rootColumn = new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow()
              {
                Children = new List<NuiElement>()
                {
                    new NuiButtonImage(icon) { Id = "item", Height = 40, Width = 40 },
                    new NuiLabel(name) { Tooltip = name, Width = 160, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
                    new NuiDrawListText(white, drawListRect, timeLeft) } },
                    new NuiButton("Annuler") { Id = "cancel", Tooltip = "En cas d'annulation, le travail en cours sera perdu. L'objet d'origine vous sera restitué.", Width = 60, Height = 40 }
                }
              }
            }
          };

          CreateWindow();
        }
        public void CreateWindow()
        {
          if (player.craftJob == null)
          {
            player.oid.SendServerMessage("Vous n'avez pas de travail artisanal en cours pour le moment. N'hésitez pas à vous rendre auprès d'un atelier afin d'en commencer un !", ColorConstants.Red);
            return;
          }

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 320, 100);

          window = new NuiWindow(rootColumn, "Travail artisanal en cours")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleActiveCraftJobEvents;
          player.oid.OnNuiEvent += HandleActiveCraftJobEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          icon.SetBindValue(player.oid, token, player.craftJob.icon);
          name.SetBindValue(player.oid, token, player.craftJob.type.ToDescription());

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          if (player.craftJob.progressLastCalculation.HasValue)
          {
            player.craftJob.remainingTime -= (DateTime.Now - player.craftJob.progressLastCalculation.Value).TotalSeconds;
            player.craftJob.progressLastCalculation = null;
          }

          timeLeft.SetBindValue(player.oid, token, player.craftJob.GetReadableJobCompletionTime());

          player.openedWindows[windowId] = token;

          HandleRealTimeJobProgression();
        }

        private void HandleActiveCraftJobEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          if (player.craftJob == null)
          {
            CloseWindow();
            return;
          }

          switch (nuiEvent.ElementId)
          {
            case "cancel":
                player.craftJob.HandleCraftJobCancellation(player);
              return;

            case "item":

              if (player.windows.ContainsKey("itemExamine"))
                ((ItemExamineWindow)player.windows["itemExamine"]).CreateWindow(NwItem.Deserialize(player.craftJob.serializedCraftedItem.ToByteArray()));
              else
                player.windows.Add("itemExamine", new ItemExamineWindow(player, NwItem.Deserialize(player.craftJob.serializedCraftedItem.ToByteArray())));
              
              return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Close:
              player.craftJob.progressLastCalculation = DateTime.Now;
              player.craftJob.HandleDelayedJobProgression(player);
              return;
          }
        }
        public async void HandleRealTimeJobProgression()
        {
          if (player.craftJob.jobProgression != null)
            player.craftJob.jobProgression.Cancel();

          await NwTask.WaitUntil(() => player.oid.LoginCreature == null || player.oid.LoginCreature.Area != null);

          if (player.oid.LoginCreature == null)
            return;

          if (player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 0)
          {
            timeLeft.SetBindValue(player.oid, token, "En pause (Hors Cité)");

            player.oid.OnServerSendArea -= player.craftJob.HandleCraftJobOnAreaChange;
            player.oid.OnServerSendArea += player.craftJob.HandleCraftJobOnAreaChange;
            return;
          }

          player.oid.OnServerSendArea -= player.craftJob.HandleCraftJobOnAreaChange;
          player.oid.OnServerSendArea += player.craftJob.HandleCraftJobOnAreaChange;
          player.oid.OnClientDisconnect -= player.craftJob.HandleCraftJobOnPlayerLeave;
          player.oid.OnClientDisconnect += player.craftJob.HandleCraftJobOnPlayerLeave;

          player.craftJob.jobProgression = player.scheduler.ScheduleRepeating(() =>
          {
            player.craftJob.remainingTime -= 1;
            timeLeft.SetBindValue(player.oid, token, player.craftJob.GetReadableJobCompletionTime());

            if (player.craftJob.remainingTime < 1)
            {
              player.craftJob.jobProgression.Dispose();
              HandleSpecificJobCompletion[player.craftJob.type].Invoke(player, true);
              player.craftJob.HandleGenericJobCompletion(player);
            }

          }, TimeSpan.FromSeconds(1));
        }
      }
    }
  }
}
