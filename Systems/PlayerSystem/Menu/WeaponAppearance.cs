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
      public class WeaponAppearanceWindow : PlayerWindow
      {
        private NwItem item { get; set; }
        private readonly NuiColumn rootColumn = new() { Margin = 0.0f };
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiBind<string> weaponName = new("weaponName");
        private readonly NuiBind<int> topModelSelection = new ("topModelSelection");
        private readonly NuiBind<int> midModelSelection = new ("midModelSelection");
        private readonly NuiBind<int> botModelSelection = new ("botModelSelection");
        private readonly NuiBind<List<NuiComboEntry>> topModelList = new("topModelList");
        private readonly NuiBind<List<NuiComboEntry>> midModelList = new("midModelList");
        private readonly NuiBind<List<NuiComboEntry>> botModelList = new("botModelList");

        public WeaponAppearanceWindow(Player player, NwItem item) : base(player)
        {
          windowId = "weaponAppearanceModifier";
          rootColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Nom & Description") { Id = "loadItemNameEditor", Width = 150, Height = 50 }, new NuiSpacer() } });

          rootChildren.Add(new NuiRow() { Height = 20, Margin = 0.0f,  Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel(weaponName) { Tooltip = weaponName, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle, Width = 200 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("<") { Id = "topDecrease", Height = 20, Width = 20, Margin = 0.0f },
            new NuiCombo(){ Entries = topModelList, Selected = topModelSelection, Height = 20, Width = 80, Margin = 0.0f },
            new NuiButton(">") { Id = "topIncrease", Height = 20, Width = 20, Margin = 0.0f },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("<") { Id = "midDecrease", Height = 20, Width = 20, Margin = 0.0f },
            new NuiCombo(){ Entries = midModelList, Selected = midModelSelection, Height = 20, Width = 80, Margin = 0.0f },
            new NuiButton(">") { Id = "midIncrease", Height = 20, Width = 20, Margin = 0.0f },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("<") { Id = "botDecrease", Height = 20, Width = 20, Margin = 0.0f },
            new NuiCombo(){ Entries = botModelList, Selected = botModelSelection, Height = 20, Width = 80, Margin = 0.0f },
            new NuiButton(">") { Id = "botIncrease", Height = 20, Width = 20, Margin = 0.0f },
            new NuiSpacer()
          } });

          CreateWindow(item);
        }
        public void CreateWindow(NwItem item)
        {
          this.item = item;
          player.DisableItemAppearanceFeedbackMessages();

          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 8, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 250, 150);

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
            nuiToken.OnNuiEvent += HandleWeaponAppearanceEvents;
            player.ActivateSpotLight(player.oid.ControlledCreature);

            weaponName.SetBindValue(player.oid, nuiToken.Token, item.Name);

            topModelSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Top));
            midModelSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Middle));
            botModelSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Bottom));

            topModelList.SetBindValue(player.oid, nuiToken.Token, ItemUtils.GetWeaponModelList(item.BaseItem.ItemType, ItemAppearanceWeaponModel.Top));
            midModelList.SetBindValue(player.oid, nuiToken.Token, ItemUtils.GetWeaponModelList(item.BaseItem.ItemType, ItemAppearanceWeaponModel.Middle));
            botModelList.SetBindValue(player.oid, nuiToken.Token, ItemUtils.GetWeaponModelList(item.BaseItem.ItemType, ItemAppearanceWeaponModel.Bottom));

            topModelSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            midModelSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            botModelSelection.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        public void HandleWeaponSelectorChange(ItemAppearanceWeaponModel model, int change)
        {
          topModelSelection.SetBindWatch(player.oid, nuiToken.Token, false);
          midModelSelection.SetBindWatch(player.oid, nuiToken.Token, false);
          botModelSelection.SetBindWatch(player.oid, nuiToken.Token, false);

          List<NuiComboEntry> entries = ItemUtils.GetWeaponModelList(item.BaseItem.ItemType, model);
          NuiBind<int> selector = topModelSelection;

          switch (model)
          {
            case ItemAppearanceWeaponModel.Middle: selector = midModelSelection; break;
            case ItemAppearanceWeaponModel.Bottom: selector = botModelSelection; break;
          }

          int newValue = entries.IndexOf(entries.FirstOrDefault(p => p.Value == selector.GetBindValue(player.oid, nuiToken.Token))) + change;

          if (newValue >= entries.Count)
            newValue = 0;

          if (newValue < 0)
            newValue = entries.Count - 1;

          selector.SetBindValue(player.oid, nuiToken.Token, entries[newValue].Value);
          item.Appearance.SetWeaponModel(model, (byte)selector.GetBindValue(player.oid, nuiToken.Token));

          NwItem newItem = item.Clone(player.oid.ControlledCreature);
          player.oid.ControlledCreature.RunEquip(newItem, InventorySlot.RightHand);
          item.Destroy();
          item = newItem;

          topModelSelection.SetBindWatch(player.oid, nuiToken.Token, true);
          midModelSelection.SetBindWatch(player.oid, nuiToken.Token, true);
          botModelSelection.SetBindWatch(player.oid, nuiToken.Token, true);
        }

        private void HandleWeaponAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.EventType == NuiEventType.Close)
          {
            player.EnableItemAppearanceFeedbackMessages();
            player.RemoveSpotLight(player.oid.ControlledCreature);
            return;
          }

          if (!item.IsValid && (item.RootPossessor != nuiEvent.Player.ControlledCreature || !player.IsDm()))
          {
            nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
            CloseWindow();
            return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "topModelSelection": HandleWeaponModelSelectorChange(ItemAppearanceWeaponModel.Top); break;
                case "midModelSelection": HandleWeaponModelSelectorChange(ItemAppearanceWeaponModel.Middle); break;
                case "botModelSelection": HandleWeaponModelSelectorChange(ItemAppearanceWeaponModel.Bottom); break;
              }

              break;

            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "topDecrease": HandleWeaponSelectorChange(ItemAppearanceWeaponModel.Top, -1); break;
                case "midDecrease": HandleWeaponSelectorChange(ItemAppearanceWeaponModel.Middle, -1); break;
                case "botDecrease": HandleWeaponSelectorChange(ItemAppearanceWeaponModel.Bottom, -1); break;
                case "topIncrease": HandleWeaponSelectorChange(ItemAppearanceWeaponModel.Top, 1); break;
                case "midIncrease": HandleWeaponSelectorChange(ItemAppearanceWeaponModel.Middle, 1); break;
                case "botIncrease": HandleWeaponSelectorChange(ItemAppearanceWeaponModel.Bottom, 1); break;
                case "loadItemNameEditor":
                  if (!player.windows.ContainsKey("editorItemName")) player.windows.Add("editorItemName", new EditorItemName(player, item));
                  else ((EditorItemName)player.windows["editorItemName"]).CreateWindow(item);
                  return;
              }

              break;
          }
        }
        private void HandleWeaponModelSelectorChange(ItemAppearanceWeaponModel model)
        {
          item.Appearance.SetWeaponModel(model, (byte)topModelSelection.GetBindValue(player.oid, nuiToken.Token));

          NwItem newItem = item.Clone(player.oid.ControlledCreature);
          player.oid.ControlledCreature.RunEquip(newItem, InventorySlot.RightHand);
          item.Destroy();
          item = newItem;
        }
      }
    }
  }
}
