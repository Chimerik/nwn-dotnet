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
      public class WeaponAppearanceWindow : PlayerWindow
      {
        private NwItem item { get; set; }
        private readonly NuiBind<int> topModelSelection = new ("topModelSelection");
        private readonly NuiBind<int> topModelSlider = new ("topModelSlider");
        private readonly NuiBind<int> middleModelSelection = new ("middleModelSelection");
        private readonly NuiBind<int> middleModelSlider = new ("middleModelSlider");
        private readonly NuiBind<int> bottomModelSelection = new ("bottomModelSelection");
        private readonly NuiBind<int> bottomModelSlider = new ("bottomModelSlider");

        public WeaponAppearanceWindow(Player player, NwItem item) : base(player)
        {
          windowId = "weaponAppearanceModifier";

          CreateWindow(item);
        }
        public void CreateWindow(NwItem item)
        {
          this.item = item;
          player.DisableItemAppearanceFeedbackMessages();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          List<NuiComboEntry> topModelCombo = BaseItems2da.GetWeaponModelList(item.BaseItem.ItemType, ItemAppearanceWeaponModel.Top);
          List<NuiComboEntry> midModelCombo = BaseItems2da.GetWeaponModelList(item.BaseItem.ItemType, ItemAppearanceWeaponModel.Middle);
          List<NuiComboEntry> botModelCombo = BaseItems2da.GetWeaponModelList(item.BaseItem.ItemType, ItemAppearanceWeaponModel.Bottom);

          NuiColumn rootColumn = new NuiColumn
          {
            Children = new List<NuiElement>
            {
              new NuiRow
              {
                Children = new List<NuiElement>
                {
                  new NuiSpacer(),
                  new NuiButton("Nom & Description") { Id = "openNameDescription", Height = 35, Width = 150 },
                  new NuiSpacer()
                }
              },
              new NuiRow
              {
                Children = new List<NuiElement>
                {
                  new NuiLabel("Haut") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle },
                  new NuiCombo
                  {
                    Width = 70,
                    Entries = topModelCombo,
                    Selected = topModelSelection
                  },
                  new NuiSlider(topModelSlider, 0, topModelCombo.Count - 1) { Step = 1,  Width = (windowRectangle.Width - 130)  * 0.96f },
                }
              },
              new NuiRow
              {
                Children = new List<NuiElement>
                {
                  new NuiLabel("Milieu") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle },
                  new NuiCombo
                  {
                    Width = 70,
                    Entries = midModelCombo,
                    Selected = middleModelSelection
                  },
                  new NuiSlider(middleModelSlider, 0, midModelCombo.Count - 1) { Step = 1,  Width = (windowRectangle.Width - 130)  * 0.96f },
                }
              },
              new NuiRow
              {
                Children = new List<NuiElement>
                {
                  new NuiLabel("Bas") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle },
                  new NuiCombo
                  {
                    Width = 70,
                    Entries = botModelCombo,
                    Selected = bottomModelSelection
                  },
                  new NuiSlider(bottomModelSlider, 0, botModelCombo.Count - 1) { Step = 1,  Width = (windowRectangle.Width - 130)  * 0.96f },
                }
              },
            }
          };

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

            topModelSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Top) / 10);
            topModelSlider.SetBindValue(player.oid, nuiToken.Token, topModelCombo.IndexOf(topModelCombo.FirstOrDefault(l => l.Value == item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Top) / 10)));

            middleModelSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Middle) / 10);
            middleModelSlider.SetBindValue(player.oid, nuiToken.Token, midModelCombo.IndexOf(midModelCombo.FirstOrDefault(l => l.Value == item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Middle) / 10)));

            bottomModelSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Bottom) / 10);
            bottomModelSlider.SetBindValue(player.oid, nuiToken.Token, botModelCombo.IndexOf(botModelCombo.FirstOrDefault(l => l.Value == item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Bottom) / 10)));

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            topModelSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            topModelSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            middleModelSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            middleModelSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            bottomModelSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            bottomModelSlider.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        public void HandleWeaponModelSliderChange(ItemAppearanceWeaponModel model)
        {
          if (!item.IsValid)
            return;

          int sliderValue;
          NuiBind<int> selector = null;
          int result = 0;

          switch (model)
          {
            case ItemAppearanceWeaponModel.Top:
              sliderValue = topModelSlider.GetBindValue(player.oid, nuiToken.Token);
              result = BaseItems2da.GetWeaponModelList(item.BaseItem.ItemType, model).ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceWeaponModel.Middle:
              sliderValue = middleModelSlider.GetBindValue(player.oid, nuiToken.Token);
              result = BaseItems2da.GetWeaponModelList(item.BaseItem.ItemType, model).ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceWeaponModel.Bottom:
              sliderValue = bottomModelSlider.GetBindValue(player.oid, nuiToken.Token);

              result = BaseItems2da.GetWeaponModelList(item.BaseItem.ItemType, model).ElementAt(sliderValue).Value;
              break;
          }

          item.Appearance.SetWeaponModel(model, (byte)result);

          NwItem newItem = item.Clone(player.oid.ControlledCreature);
          item.Destroy();
          item = newItem;
          player.oid.ControlledCreature.RunEquip(item, InventorySlot.RightHand);

          selector.SetBindWatch(player.oid, nuiToken.Token, false);
          selector.SetBindValue(player.oid, nuiToken.Token, result);
          selector.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        public void HandleWeaponModelSelectorChange(ItemAppearanceWeaponModel model)
        {
          if (!item.IsValid)
            return;

          int selectorValue = 0;
          NuiBind<int> slider = null;
          List<NuiComboEntry> modelComboEntries = BaseItems2da.GetWeaponModelList(item.BaseItem.ItemType, model);
          int sliderValue = 0;
          int sliderResult = 0;

          switch (model)
          {
            case ItemAppearanceWeaponModel.Top:
              selectorValue = topModelSelection.GetBindValue(player.oid, nuiToken.Token);
              slider = new ("topModelSlider");
              sliderResult = BaseItems2da.GetWeaponModelList(item.BaseItem.ItemType, model).IndexOf(modelComboEntries.FirstOrDefault(m => m.Value == selectorValue));
              sliderValue = slider.GetBindValue(player.oid, nuiToken.Token);
              break;
            case ItemAppearanceWeaponModel.Middle:
              selectorValue = middleModelSelection.GetBindValue(player.oid, nuiToken.Token);
              slider = new ("middleModelSlider");
              sliderResult = modelComboEntries.IndexOf(modelComboEntries.FirstOrDefault(m => m.Value == selectorValue));
              sliderValue = slider.GetBindValue(player.oid, nuiToken.Token);
              break;
            case ItemAppearanceWeaponModel.Bottom:
              selectorValue = bottomModelSelection.GetBindValue(player.oid, nuiToken.Token);
              slider = new ("bottomModelSlider");
              sliderResult = modelComboEntries.IndexOf(modelComboEntries.FirstOrDefault(m => m.Value == selectorValue));
              sliderValue = slider.GetBindValue(player.oid, nuiToken.Token);
              break;
          }

          item.Appearance.SetWeaponModel(model, (byte)selectorValue);

          NwItem newItem = item.Clone(player.oid.ControlledCreature);
          player.oid.ControlledCreature.RunEquip(newItem, InventorySlot.RightHand);
          item.Destroy();
          item = newItem;

          slider.SetBindWatch(player.oid, nuiToken.Token, false);
          slider.SetBindValue(player.oid, nuiToken.Token, sliderResult);
          slider.SetBindWatch(player.oid, nuiToken.Token, true);
        }

        private void HandleWeaponAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.EventType == NuiEventType.Close)
          {
            player.EnableItemAppearanceFeedbackMessages();
            player.RemoveSpotLight(player.oid.ControlledCreature);
            return;
          }

          if (!item.IsValid || item.Possessor != nuiEvent.Player.ControlledCreature)
          {
            nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
            CloseWindow();
            return;
          }

          if (nuiEvent.EventType == NuiEventType.Watch)
            switch (nuiEvent.ElementId)
            {
              case "topModelSlider":
                HandleWeaponModelSliderChange(ItemAppearanceWeaponModel.Top);
                break;

              case "topModelSelection":
                HandleWeaponModelSelectorChange(ItemAppearanceWeaponModel.Top);
                break;

              case "middleModelSlider":
                HandleWeaponModelSliderChange(ItemAppearanceWeaponModel.Middle);
                break;

              case "middleModelSelection":
                HandleWeaponModelSelectorChange(ItemAppearanceWeaponModel.Middle);
                break;

              case "bottomModelSlider":
                HandleWeaponModelSliderChange(ItemAppearanceWeaponModel.Bottom);
                break;

              case "bottomModelSelection":
                HandleWeaponModelSelectorChange(ItemAppearanceWeaponModel.Bottom);
                break;
            }
        }
      }
    }
  }
}
