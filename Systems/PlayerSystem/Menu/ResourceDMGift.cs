﻿using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ResourceDMGiftWindow : PlayerWindow
      {
        private readonly NuiColumn rootRow = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();

        private readonly NuiBind<string> myResourceNames = new ("myResourceNames");
        private readonly NuiBind<int> myListCount = new ("myListCount");
        private readonly NuiBind<string> myResourceIcon = new ("myResourceIcon");
        private readonly NuiBind<string> myQuantity = new ("myQuantity");

        Player targetPlayer;

        public ResourceDMGiftWindow(Player player, Player targetPlayer) : base(player)
        {
          windowId = "resourceDMGift";

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(myResourceIcon) { Tooltip = myResourceNames, Height = 35 }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(myResourceNames) { VerticalAlign = NuiVAlign.Middle }));
          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("", myQuantity, 10, false)) { Width = 100 });

          rootRow.Children = rootChildren;
          rootChildren.Add(new NuiRow() { Height = 350, Children = new List<NuiElement>() { new NuiList(rowTemplate, myListCount) { RowHeight = 35 } } });

          rootChildren.Add(new NuiRow()
          {
            Height = 35,
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Valider") { Id = "send", Tooltip = "Valider le don de ressources.", Width = 80 },
              new NuiSpacer()
            }
          });

          CreateWindow(targetPlayer);
        }
        public void CreateWindow(Player target)
        {
          this.targetPlayer = target;

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 350, 550);

          window = new NuiWindow(rootRow, $"Don de ressources : {target.oid.LoginCreature.Name}")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };


          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleResourceGiftEvents;
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }

            

          LoadResourceList();
        }

        private void HandleResourceGiftEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "send":

                  if(targetPlayer.oid.LoginCreature == null)
                  {
                    CloseWindow();
                    player.oid.SendServerMessage("La cible du don n'est plus valide.", ColorConstants.Red);
                    return;
                  }

                  var quantities = myQuantity.GetBindValues(player.oid, nuiToken.Token);

                  for (int i = 0; i < Craft.Collect.System.craftResourceArray.Length; i++)
                  {
                    if(int.TryParse(quantities[i], out int quantity) && quantity > 0)
                    {
                      CraftResource resource = Craft.Collect.System.craftResourceArray[i];
                      CraftResource myResource = targetPlayer.craftResourceStock.FirstOrDefault(r => r.type == resource.type && r.grade == resource.grade);

                      if (myResource != null)
                        myResource.quantity += resource.quantity;
                      else
                        targetPlayer.craftResourceStock.Add(new CraftResource(resource, quantity));
                    }
                  }

                  player.oid.SendServerMessage($"Don de ressource à {targetPlayer.oid.LoginCreature.Name.ColorString(ColorConstants.White)} terminé avec succès !", new Color(32, 255, 32));
                  targetPlayer.oid.SendServerMessage($"{player.oid.LoginCreature.Name.ColorString(ColorConstants.White)} vient de vous faire don de ressources.", new Color(32, 255, 32));

                  CloseWindow();
                  targetPlayer.oid.ExportCharacter();

                  break;
              }

              break;
          }
        }
        private void LoadResourceList()
        {
          List<string> resourceNameList = new();
          List<string> resourceIconList = new();
          List<string> resourceQuantityList = new();

          foreach (CraftResource resource in Craft.Collect.System.craftResourceArray)
          {
            resourceNameList.Add(resource.name);
            resourceIconList.Add(resource.iconString);
            resourceQuantityList.Add("0");
          }

          myResourceNames.SetBindValues(player.oid, nuiToken.Token, resourceNameList);
          myResourceIcon.SetBindValues(player.oid, nuiToken.Token, resourceIconList);
          myQuantity.SetBindValues(player.oid, nuiToken.Token, resourceQuantityList);
          myListCount.SetBindValue(player.oid, nuiToken.Token, resourceNameList.Count);
        }
      }
    }
  }
}
