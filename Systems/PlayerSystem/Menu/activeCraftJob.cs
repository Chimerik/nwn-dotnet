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
        private readonly Color white = new(255, 255, 255);
        private readonly NuiBind<NuiRect> drawListRect = new("drawListRect");
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> name = new("name");
        public readonly NuiBind<string> timeLeft = new("timeLeft");

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

          if (IsOpen)
            return;

          NuiRect windowRectangle;

          if (player.windowRectangles.ContainsKey(windowId))
          {
            NuiRect playerRect = player.windowRectangles[windowId];
            windowRectangle = new(playerRect.X, playerRect.Y, 320, 100);
          }
          else
            windowRectangle = new(0, 200, 320, 100);

          window = new NuiWindow(rootColumn, "Travail artisanal en cours")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = collasped,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleActiveCraftJobEvents;

            drawListRect.SetBindValue(player.oid, nuiToken.Token, Utils.GetDrawListTextScaleFromPlayerUI(player));
            icon.SetBindValue(player.oid, nuiToken.Token, player.craftJob.icon);
            name.SetBindValue(player.oid, nuiToken.Token, player.craftJob.type.ToDescription());

            collasped.SetBindValue(player.oid, nuiToken.Token, false);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            if (player.craftJob.progressLastCalculation.HasValue)
            {
              player.craftJob.remainingTime -= (DateTime.Now - player.craftJob.progressLastCalculation.Value).TotalSeconds;
              player.craftJob.progressLastCalculation = null;
            }

            if (player.oid.LoginCreature.Area == null || player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 0)
              timeLeft.SetBindValue(player.oid, nuiToken.Token, "En pause (Hors Cité)");
            else
              timeLeft.SetBindValue(player.oid, nuiToken.Token, player.craftJob.GetReadableJobCompletionTime());

            //player.oid.OnServerSendArea -= player.craftJob.HandleCraftJobOnAreaChange;
            //player.oid.OnServerSendArea += player.craftJob.HandleCraftJobOnAreaChange;
            //player.oid.OnClientDisconnect -= player.craftJob.HandleCraftJobOnPlayerLeave;
            //player.oid.OnClientDisconnect += player.craftJob.HandleCraftJobOnPlayerLeave;

            //HandleRealTimeJobProgression();
          }
        }

        private void HandleActiveCraftJobEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
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

              if (!string.IsNullOrEmpty(player.craftJob.serializedCraftedItem))
              {
                NwItem item = NwItem.Deserialize(player.craftJob.serializedCraftedItem.ToByteArray());

                if (!player.windows.ContainsKey("itemExamine")) player.windows.Add("itemExamine", new ItemExamineWindow(player, item));
                else ((ItemExamineWindow)player.windows["itemExamine"]).CreateWindow(item);

                ItemUtils.ScheduleItemForDestruction(item, 300);
              }

              return;
          }

          /*switch (nuiEvent.EventType)
          {
            case NuiEventType.Close:
              player.craftJob.progressLastCalculation = DateTime.Now;
              player.craftJob.HandleDelayedJobProgression(player);
              return;
          }*/
        }
        /*public async void HandleRealTimeJobProgression()
        {
          if (player.craftJob.jobProgression != null)
            player.craftJob.jobProgression.Dispose();

          await NwTask.WaitUntil(() => player.oid.LoginCreature == null || player.oid.LoginCreature.Area != null);

          if (player.oid.LoginCreature == null)
            return;

          if (player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 0)
          {
            timeLeft.SetBindValue(player.oid, nuiToken.Token, "En pause (Hors Cité)");

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
            timeLeft.SetBindValue(player.oid, nuiToken.Token, player.craftJob.GetReadableJobCompletionTime());

            if (player.craftJob.remainingTime < 1)
            {
              player.craftJob.jobProgression.Dispose();
              HandleSpecificJobCompletion[player.craftJob.type].Invoke(player, true);
              player.craftJob.HandleGenericJobCompletion(player);
            }

          }, TimeSpan.FromSeconds(1));
        }*/
      }
    }
  }
}
