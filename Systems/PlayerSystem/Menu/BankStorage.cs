using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class BankStorageWindow : PlayerWindow
      {
        NuiColumn rootColumn { get; }
        private readonly NuiBind<string> gold = new NuiBind<string>("gold");
        private readonly NuiBind<string> search = new NuiBind<string>("search");
        private readonly NuiBind<string> itemNames = new NuiBind<string>("itemNames");
        private readonly NuiBind<int> listCount = new NuiBind<int>("listCount");
        private readonly NuiBind<string> topIcon = new NuiBind<string>("topIcon");
        private readonly NuiBind<string> midIcon = new NuiBind<string>("midIcon");
        private readonly NuiBind<string> botIcon = new NuiBind<string>("botIcon");
        private readonly NuiBind<bool> enabled = new NuiBind<bool>("enabled");
        private List<NwItem> itemList = new List<NwItem>();

        public BankStorageWindow(Player player) : base(player)
        {
          windowId = "bankStorage";

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButton("Examiner") { Id = "examiner", Height = 35 } ) { Width = 80 },
            new NuiListTemplateCell(new NuiSpacer()
            {
              Height = 125, Id = "item",
              DrawList = new List<NuiDrawListItem>()
              {
                new NuiDrawListImage(topIcon, new NuiRect(0, 0, 25, 25)),
                new NuiDrawListImage(midIcon, new NuiRect(0, 0, 25, 25)) { Enabled = enabled,  },
                new NuiDrawListImage(botIcon, new NuiRect(0, 0, 25, 25)) { Enabled = enabled }

              }
            }) { Width = 45 },
            new NuiListTemplateCell(new NuiLabel(itemNames) { Id = "takeItem", VerticalAlign = NuiVAlign.Middle } )
          };

          rootColumn = new NuiColumn() 
          { 
            Children = new List<NuiElement>()
            {
              new NuiRow()
              {
                Height = 35,
                Children = new List<NuiElement>()
                {
                  new NuiLabel("Pièces d'or : ") { Width = 120, VerticalAlign = NuiVAlign.Middle },
                  new NuiLabel(gold) { Width = 120, VerticalAlign = NuiVAlign.Middle },
                  new NuiButton("Dépôt") { Id = "goldDeposit", Width = 80 },
                  new NuiButton("Retrait") { Id = "goldWithdraw", Width = 80 }
                }
              },
              new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 410 } } },
              new NuiList(rowTemplate, listCount) { RowHeight = 75 },
              new NuiRow()
              {
                Height = 35,
                Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiButton("Activer mode dépôt") { Id = "itemDeposit", Width = 160 },
                  new NuiSpacer()
                }
              },
            }
          };

          CreateWindow();
        }

        public void CreateWindow()
        {
          NuiRect windowRectangle = /*player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] :*/ new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          window = new NuiWindow(rootColumn, $"Coffre privé de {player.oid.LoginCreature.Name}")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleBankStorageEvents;
          player.oid.OnNuiEvent += HandleBankStorageEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          List<string> itemNameList = new List<string>();
          List<string> topIconList = new List<string>();
          List<string> midIconList = new List<string>();
          List<string> botIconList = new List<string>();
          List<bool> enabledList = new List<bool>();

          foreach (var item in player.oid.LoginCreature.Inventory.Items)
          {
            itemList.Add(item);
            itemNameList.Add(item.BaseItem.IsStackable ? $"{item.Name} (x{item.StackSize})" : item.Name);
            string[] tempArray = Utils.GetIconResref(item);
            topIconList.Add(tempArray[0]);
            midIconList.Add(tempArray[1]);
            botIconList.Add(tempArray[2]);
            enabledList.Add(!string.IsNullOrEmpty(tempArray[1]));
          }

          gold.SetBindValue(player.oid, token, player.bankGold.ToString());
          search.SetBindValue(player.oid, token, "");

          itemNames.SetBindValues(player.oid, token, itemNameList);
          listCount.SetBindValue(player.oid, token, itemNameList.Count);

          topIcon.SetBindValues(player.oid, token, topIconList);
          midIcon.SetBindValues(player.oid, token, midIconList);
          botIcon.SetBindValues(player.oid, token, botIconList);
          enabled.SetBindValues(player.oid, token, enabledList);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);
        }

        private async void HandleBankStorageEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;
          
          switch(nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch(nuiEvent.ElementId)
              {
                case "goldDeposit":

                  if (player.windows.ContainsKey("playerInput"))
                    ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Déposer combien d'or ?", DepositGold, player.oid.LoginCreature.Gold.ToString());
                  else
                    player.windows.Add("playerInput", new PlayerInputWindow(player, "Déposer combien d'or ?", DepositGold, player.oid.LoginCreature.Gold.ToString()));

                  break;
                case "goldWithdraw":

                  if (player.windows.ContainsKey("playerInput"))
                    ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Déposer combien d'or ?", WithdrawGold, player.bankGold.ToString());
                  else
                    player.windows.Add("playerInput", new PlayerInputWindow(player, "Déposer combien d'or ?", WithdrawGold, player.bankGold.ToString()));


                  break;
              }

            break;

            case NuiEventType.MouseDown:

              switch(nuiEvent.ElementId)
              {
                case "examiner":
                  NwItem tempItem = itemList.ElementAt(nuiEvent.ArrayIndex).Clone(player.oid.ControlledCreature.Location, "_TODESTROY", false);

                  await NwTask.Delay(TimeSpan.FromSeconds(0.2));
                  await player.oid.ActionExamine(tempItem);
                  tempItem.Destroy(0.1f);
                  break;

                case "takeItem":
                  player.oid.ControlledCreature.AcquireItem(itemList.ElementAt(nuiEvent.ArrayIndex));
                  RemoveItemFromList(nuiEvent.ArrayIndex);
                  break;
              }

              break;
          }
        }
        private bool DepositGold(string inputValue)
        {
          if(!uint.TryParse(inputValue, out uint inputGold) || inputGold > player.oid.LoginCreature.Gold)
          {
            player.oid.SendServerMessage("Vous ne disposez pas d'autant d'or.", ColorConstants.Red);
            return true;
          }

          player.oid.LoginCreature.Gold -= inputGold;
          player.bankGold += (int)inputGold;
          gold.SetBindValue(player.oid, token, player.bankGold.ToString());

          return true;
        }
        private bool WithdrawGold(string inputValue)
        {
          if (!int.TryParse(inputValue, out int inputGold) || inputGold > player.bankGold)
          {
            player.oid.SendServerMessage("Vous ne disposez pas d'autant d'or.", ColorConstants.Red);
            return true;
          }

          player.bankGold -= inputGold;
          player.oid.LoginCreature.Gold += (uint)inputGold;
          gold.SetBindValue(player.oid, token, player.bankGold.ToString());

          return true;
        }
        private void RemoveItemFromList(int index)
        {
          List<string> tempList = itemNames.GetBindValues(player.oid, token);
          tempList.RemoveAt(index);
          itemNames.SetBindValues(player.oid, token, tempList);

          tempList = topIcon.GetBindValues(player.oid, token);
          tempList.RemoveAt(index);
          topIcon.SetBindValues(player.oid, token, tempList);

          tempList = midIcon.GetBindValues(player.oid, token);
          tempList.RemoveAt(index);
          midIcon.SetBindValues(player.oid, token, tempList);

          tempList = botIcon.GetBindValues(player.oid, token);
          tempList.RemoveAt(index);
          botIcon.SetBindValues(player.oid, token, tempList);

          List<bool> tempEnableList = enabled.GetBindValues(player.oid, token);
          tempEnableList.RemoveAt(index);
          enabled.SetBindValues(player.oid, token, tempEnableList);

          listCount.SetBindValue(player.oid, token, listCount.GetBindValue(player.oid, token) - 1);
        }
      }
    }
  }
}
