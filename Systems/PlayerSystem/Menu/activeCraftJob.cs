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
                    new NuiButtonImage(icon) { Height = 40, Width = 40 },
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
            player.oid.SendServerMessage("Vous n'avez pas d'apprentissage actif pour le moment. Veuillez utiliser le journal afin de décider de votre prochain apprentissage.", ColorConstants.Red);
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

          timeLeft.SetBindValue(player.oid, token, player.newCraftJob.GetReadableJobCompletionTime());
          icon.SetBindValue(player.oid, token, player.newCraftJob.icon);
          name.SetBindValue(player.oid, token, player.newCraftJob.type.ToDescription());

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }

        private void HandleActiveCraftJobEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.ElementId)
          {
            case "cancel":
              if (player.newCraftJob != null)
                player.newCraftJob.HandleCraftJobCancellation(player);
              return;
          }
        }
      }
    }
  }
}
