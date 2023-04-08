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
      public class SimpleItemAppearanceWindow : PlayerWindow
      {
        private NwItem item { get; set; }
        private readonly NuiColumn rootColumn = new ();
        private readonly List<NuiElement> colChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> icon2 = new("icon2");
        private readonly NuiBind<string> icon3 = new("icon3");
        private readonly NuiBind<string> icon4 = new("icon4");
        private readonly NuiBind<string> icon5 = new("icon5");
        private readonly NuiBind<bool> iconVisible = new("iconVisible");
        private readonly NuiBind<bool> icon2Visible = new("icon2Visible");
        private readonly NuiBind<bool> icon3Visible = new("icon3Visible");
        private readonly NuiBind<bool> icon4Visible = new("icon4Visible");
        private readonly NuiBind<bool> icon5Visible = new("icon5Visible");
        private readonly NuiBind<int> listCount = new("listCount");

        public SimpleItemAppearanceWindow(Player player, NwItem item) : base(player)
        {
          windowId = "simpleItemAppearanceModifier";
          rootColumn.Children = colChildren;

          colChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiButton("Nom & Description") { Id = "loadItemNameEditor", Width = 150, Height = 50 } } });

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(icon) { Id = "0", Height = 100, Visible = iconVisible }) { Width = 75  });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(icon2) { Id = "1", Height = 100, Visible = icon2Visible }) { Width = 75 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(icon3) { Id = "2", Height = 100, Visible = icon3Visible }) { Width = 75 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(icon4) { Id = "3", Height = 100, Visible = icon4Visible }) { Width = 75 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(icon5) { Id = "4", Height = 100, Visible = icon5Visible }) { Width = 75 });

          colChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 100 } } });

          CreateWindow(item);
        }
        public void CreateWindow(NwItem item)
        {
          this.item = item;

          if (string.IsNullOrEmpty(item.BaseItem.DefaultIcon))
          {
            player.oid.SendServerMessage($"Il n'existe actuellement pas de modèle modifiable pour {item.Name.ColorString(ColorConstants.White)}.", ColorConstants.Red);
            return;
          }

          player.DisableItemAppearanceFeedbackMessages();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          window = new NuiWindow(rootColumn, $"Modifier l'apparence de {item.Name}")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = true,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleSimpleItemAppearanceEvents;

            LoadIcons();

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleSimpleItemAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.EventType == NuiEventType.Close)
            player.EnableItemAppearanceFeedbackMessages();

          if (nuiEvent.EventType == NuiEventType.Click)
          {
            if (!item.IsValid && (item.Possessor != player.oid.ControlledCreature || !player.IsDm()))
            {
              player.oid.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
              CloseWindow();
              return;
            }

            if(nuiEvent.EventType == NuiEventType.Click && nuiEvent.ElementId == "loadItemNameEditor")
            {
              if (!player.windows.ContainsKey("editorItemName")) player.windows.Add("editorItemName", new EditorItemName(player, item));
              else ((EditorItemName)player.windows["editorItemName"]).CreateWindow(item);
              return;
            }
            
            item.Appearance.SetSimpleModel((byte)BaseItems2da.simpleItemModels[item.BaseItem.DefaultIcon].ElementAt(5 * nuiEvent.ArrayIndex + int.Parse(nuiEvent.ElementId)));
            NwItem newItem = item.Clone(player.oid.ControlledCreature);

            for (int i = 0; i < 13; i++)
            {
              if (player.oid.ControlledCreature.GetItemInSlot((InventorySlot)i) == item)
              {
                player.oid.ControlledCreature.RunEquip(newItem, (InventorySlot)i);
                break;
              }
            }

            item.Destroy();
            item = newItem;
            
            player.oid.SendServerMessage($"Apparence sélectionnée : {item.BaseItem.ItemClass}_{item.Appearance.GetSimpleModel()}", ColorConstants.Orange);
          }
        }
        private void LoadIcons()
        {
          List<string> iconList1 = new();
          List<string> iconList2 = new();
          List<string> iconList3 = new();
          List<string> iconList4 = new();
          List<string> iconList5 = new();

          List<bool> iconVisibleList1 = new();
          List<bool> iconVisibleList2 = new();
          List<bool> iconVisibleList3 = new();
          List<bool> iconVisibleList4 = new();
          List<bool> iconVisibleList5 = new();

          int nbIcons = BaseItems2da.simpleItemModels[item.BaseItem.DefaultIcon].Count;
          int currentCount = 0;
          int nbRows = 0;

          while (currentCount < nbIcons)
          {
            try
            {
              var modelList = BaseItems2da.simpleItemModels[item.BaseItem.DefaultIcon].GetRange(currentCount, 5);

              iconList1.Add($"{item.BaseItem.DefaultIcon}_{modelList[0].ToString().PadLeft(3, '0')}");
              iconList2.Add($"{item.BaseItem.DefaultIcon}_{modelList[1].ToString().PadLeft(3, '0')}");
              iconList3.Add($"{item.BaseItem.DefaultIcon}_{modelList[2].ToString().PadLeft(3, '0')}");
              iconList4.Add($"{item.BaseItem.DefaultIcon}_{modelList[3].ToString().PadLeft(3, '0')}");
              iconList5.Add($"{item.BaseItem.DefaultIcon}_{modelList[4].ToString().PadLeft(3, '0')}");

              iconVisibleList1.Add(true);
              iconVisibleList2.Add(true);
              iconVisibleList3.Add(true);
              iconVisibleList4.Add(true);
              iconVisibleList5.Add(true);

              currentCount += 5;
              nbRows++;
            }
            catch(Exception)
            {
              var modelList = BaseItems2da.simpleItemModels[item.BaseItem.DefaultIcon].GetRange(currentCount, nbIcons - currentCount);

              if (modelList.Count > 0)
              {
                iconList1.Add($"{item.BaseItem.DefaultIcon}_{modelList[0].ToString().PadLeft(3, '0')}");
                iconVisibleList1.Add(true);
                nbRows++;
              }

              if (modelList.Count > 1)
              {
                iconList2.Add($"{item.BaseItem.DefaultIcon}_{modelList[1].ToString().PadLeft(3, '0')}");
                iconVisibleList2.Add(true);
              }
              else
              {
                iconList2.Add("invalid");
                iconVisibleList2.Add(false);
              }

              if (modelList.Count > 2)
              {
                iconList3.Add($"{item.BaseItem.DefaultIcon}_{modelList[2].ToString().PadLeft(3, '0')}");
                iconVisibleList3.Add(true);
              }
              else
              { 
                iconList3.Add("invalid");
                iconVisibleList3.Add(false);
              }

              if (modelList.Count > 3)
              {
                iconList4.Add($"{item.BaseItem.DefaultIcon}_{modelList[3].ToString().PadLeft(3, '0')}");
                iconVisibleList4.Add(true);
              }
              else
              {
                iconList4.Add("invalid");
                iconVisibleList4.Add(false);
              }

              if (modelList.Count > 4)
              {
                iconList5.Add($"{item.BaseItem.DefaultIcon}_{modelList[4].ToString().PadLeft(3, '0')}");
                iconVisibleList5.Add(true);
              }
              else
              {
                iconList5.Add("invalid");
                iconVisibleList5.Add(false);
              }

              currentCount += 5;
            }

            icon.SetBindValues(player.oid, nuiToken.Token, iconList1);
            icon2.SetBindValues(player.oid, nuiToken.Token, iconList2);
            icon3.SetBindValues(player.oid, nuiToken.Token, iconList3);
            icon4.SetBindValues(player.oid, nuiToken.Token, iconList4);
            icon5.SetBindValues(player.oid, nuiToken.Token, iconList5);
            iconVisible.SetBindValues(player.oid, nuiToken.Token, iconVisibleList1);
            icon2Visible.SetBindValues(player.oid, nuiToken.Token, iconVisibleList2);
            icon3Visible.SetBindValues(player.oid, nuiToken.Token, iconVisibleList3);
            icon4Visible.SetBindValues(player.oid, nuiToken.Token, iconVisibleList4);
            icon5Visible.SetBindValues(player.oid, nuiToken.Token, iconVisibleList5);
            listCount.SetBindValue(player.oid, nuiToken.Token, nbRows);
          }
        }
      }
    }
  }
}
