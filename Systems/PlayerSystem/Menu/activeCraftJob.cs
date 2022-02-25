using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ActiveCraftJobWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn;
        private readonly NuiColor white = new NuiColor(255, 255, 255);
        private readonly NuiRect drawListRect = new NuiRect(0, 35, 150, 60);
        private readonly NuiBind<string> icon = new NuiBind<string>("icon");
        private readonly NuiBind<string> name = new NuiBind<string>("name");
        public readonly NuiBind<string> timeLeft = new NuiBind<string>("timeLeft");

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
          if (player.newCraftJob == null)
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

          icon.SetBindValue(player.oid, token, player.newCraftJob.icon);
          name.SetBindValue(player.oid, token, player.newCraftJob.type.ToDescription());

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          if (player.newCraftJob.progressLastCalculation.HasValue)
          {
            player.newCraftJob.remainingTime -= (DateTime.Now - player.newCraftJob.progressLastCalculation.Value).TotalSeconds;
            player.newCraftJob.progressLastCalculation = null;
          }

          timeLeft.SetBindValue(player.oid, token, player.newCraftJob.GetReadableJobCompletionTime());

          player.openedWindows[windowId] = token;

          HandleRealTimeJobProgression();
        }

        private void HandleActiveCraftJobEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          if (player.newCraftJob == null)
          {
            player.oid.NuiDestroy(token);
            return;
          }

          switch (nuiEvent.ElementId)
          {
            case "cancel":
                player.newCraftJob.HandleCraftJobCancellation(player);
              return;

            case "item":

              if (player.windows.ContainsKey("itemExamine"))
                ((ItemExamineWindow)player.windows["itemExamine"]).CreateWindow(NwItem.Deserialize(player.newCraftJob.serializedCraftedItem.ToByteArray()));
              else
                player.windows.Add("itemExamine", new ItemExamineWindow(player, NwItem.Deserialize(player.newCraftJob.serializedCraftedItem.ToByteArray())));
              
              return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Close:
              player.newCraftJob.progressLastCalculation = DateTime.Now;
              player.newCraftJob.HandleDelayedJobProgression(player);
              return;
          }
        }
        public async void HandleRealTimeJobProgression()
        {
          if (player.newCraftJob.jobProgression != null)
            player.newCraftJob.jobProgression.Cancel();

          await NwTask.WaitUntil(() => player.oid.LoginCreature == null || player.oid.LoginCreature.Area != null);

          if (player.oid.LoginCreature == null)
            return;

          if (player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 0)
          {
            timeLeft.SetBindValue(player.oid, token, "En pause (Hors Cité)");

            player.oid.OnServerSendArea -= player.newCraftJob.HandleCraftJobOnAreaChange;
            player.oid.OnServerSendArea += player.newCraftJob.HandleCraftJobOnAreaChange;
            return;
          }

          player.oid.OnServerSendArea -= player.newCraftJob.HandleCraftJobOnAreaChange;
          player.oid.OnServerSendArea += player.newCraftJob.HandleCraftJobOnAreaChange;
          player.oid.OnClientDisconnect -= player.newCraftJob.HandleCraftJobOnPlayerLeave;
          player.oid.OnClientDisconnect += player.newCraftJob.HandleCraftJobOnPlayerLeave;

          player.newCraftJob.jobProgression = ModuleSystem.scheduler.ScheduleRepeating(() =>
          {
            player.newCraftJob.remainingTime -= 1;
            timeLeft.SetBindValue(player.oid, token, player.newCraftJob.GetReadableJobCompletionTime());

            if (player.newCraftJob.remainingTime < 1)
            {
              player.newCraftJob.jobProgression.Dispose();
              HandleSpecificJobCompletion[player.newCraftJob.type].Invoke(player, true);
              player.newCraftJob.HandleGenericJobCompletion(player);
            }

          }, TimeSpan.FromSeconds(1));
        }
      }
    }
  }
}
