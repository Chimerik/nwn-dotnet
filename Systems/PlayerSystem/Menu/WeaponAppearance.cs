using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void CreateWeaponAppearanceWindow(NwItem item)
      {
        string windowId = "weaponAppearanceModifier";
        DisableItemAppearanceFeedbackMessages();
        NuiBind<string> title = new NuiBind<string>("title");
        NuiBind<int> topModelSelection = new NuiBind<int>("topModelSelection");
        NuiBind<int> topModelSlider = new NuiBind<int>("topModelSlider");
        NuiBind<int> middleModelSelection = new NuiBind<int>("middleModelSelection");
        NuiBind<int> middleModelSlider = new NuiBind<int>("middleModelSlider");
        NuiBind<int> bottomModelSelection = new NuiBind<int>("bottomModelSelection");
        NuiBind<int> bottomModelSlider = new NuiBind<int>("bottomModelSlider");

        List<NuiComboEntry> topModelCombo = BaseItems2da.baseItemTable.GetWeaponModelList(item.BaseItemType, ItemAppearanceWeaponModel.Top);
        List<NuiComboEntry> midModelCombo = BaseItems2da.baseItemTable.GetWeaponModelList(item.BaseItemType, ItemAppearanceWeaponModel.Middle);
        List<NuiComboEntry> botModelCombo = BaseItems2da.baseItemTable.GetWeaponModelList(item.BaseItemType, ItemAppearanceWeaponModel.Bottom);

        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) && windowRectangles[windowId].Width > 0 && windowRectangles[windowId].Width <= oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? windowRectangles[windowId] : new NuiRect(10, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        // Construct the window layout.
        NuiColumn root = new NuiColumn
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

        NuiWindow window = new NuiWindow(root, title)
        {
          Geometry = geometry,
          Resizable = true,
          Collapsed = false,
          Closable = true,
          Transparent = true,
          Border = true,
        };

        oid.OnNuiEvent -= HandleWeaponAppearanceEvents;
        oid.OnNuiEvent += HandleWeaponAppearanceEvents;

        int token = oid.CreateNuiWindow(window, windowId);

        PlayerPlugin.ApplyLoopingVisualEffectToObject(oid.ControlledCreature, oid.ControlledCreature, 173);

        title.SetBindValue(oid, token, $"Modifier l'apparence de {item.Name}");

        topModelSelection.SetBindValue(oid, token, item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Top) / 10);
        topModelSlider.SetBindValue(oid, token, topModelCombo.IndexOf(topModelCombo.FirstOrDefault(l => l.Value == item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Top) / 10)) );

        middleModelSelection.SetBindValue(oid, token, item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Middle) / 10);
        middleModelSlider.SetBindValue(oid, token, midModelCombo.IndexOf(midModelCombo.FirstOrDefault(l => l.Value == item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Middle) / 10)));

        bottomModelSelection.SetBindValue(oid, token, item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Bottom) / 10);
        bottomModelSlider.SetBindValue(oid, token, botModelCombo.IndexOf(botModelCombo.FirstOrDefault(l => l.Value == item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Bottom) / 10)));

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);

        Task waitWindowOpened = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.6));

          topModelSelection.SetBindWatch(oid, token, true);
          topModelSlider.SetBindWatch(oid, token, true);

          middleModelSelection.SetBindWatch(oid, token, true);
          middleModelSlider.SetBindWatch(oid, token, true);

          bottomModelSelection.SetBindWatch(oid, token, true);
          bottomModelSlider.SetBindWatch(oid, token, true);
        });
      }

      public void HandleWeaponModelSliderChange(int windowToken, ItemAppearanceWeaponModel model, NwItem weapon)
      {
        if (!weapon.IsValid)
          return;

        int sliderValue = 0;
        NuiBind<int> selector = null;
        int selectedValue = 0;
        int result = 0;

        switch (model)
        {
          case ItemAppearanceWeaponModel.Top:
            sliderValue = new NuiBind<int>("topModelSlider").GetBindValue(oid, windowToken);
            selector = new NuiBind<int>("topModelSelection");
            selectedValue = selector.GetBindValue(oid, windowToken);
            result = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, model).ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceWeaponModel.Middle:
            sliderValue = new NuiBind<int>("middleModelSlider").GetBindValue(oid, windowToken);
            selector = new NuiBind<int>("middleModelSelection");
            selectedValue = selector.GetBindValue(oid, windowToken);            
            result = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, model).ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceWeaponModel.Bottom:
            sliderValue = new NuiBind<int>("bottomModelSlider").GetBindValue(oid, windowToken);
            selector = new NuiBind<int>("bottomModelSelection");
            selectedValue = selector.GetBindValue(oid, windowToken);
            
            result = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, model).ElementAt(sliderValue).Value;
            break;
        }

        weapon.Appearance.SetWeaponModel(model, (byte)result);

        NwItem newItem = weapon.Clone(oid.ControlledCreature);
        weapon.Destroy();
        weapon = newItem;
        oid.ControlledCreature.RunEquip(weapon, InventorySlot.RightHand);

        oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value = weapon;

        selector.SetBindWatch(oid, windowToken, false);
        selector.SetBindValue(oid, windowToken, result);
        selector.SetBindWatch(oid, windowToken, true);
      }
      public void HandleWeaponModelSelectorChange(int windowToken, ItemAppearanceWeaponModel model, NwItem weapon)
      {
        if (!weapon.IsValid)
          return;

        int selectorValue = 0;
        NuiBind<int> slider = null;
        List<NuiComboEntry> modelComboEntries = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, model);
        int sliderValue = 0;
        int sliderResult = 0;
        BaseItemTable.Entry entry = BaseItems2da.baseItemTable.GetBaseItemDataEntry(weapon.BaseItemType);

        switch (model)
        {
          case ItemAppearanceWeaponModel.Top:
            selectorValue = new NuiBind<int>("topModelSelection").GetBindValue(oid, windowToken);
            slider = new NuiBind<int>("topModelSlider");
            sliderResult = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, model).IndexOf(modelComboEntries.FirstOrDefault(m => m.Value == selectorValue));
            sliderValue = slider.GetBindValue(oid, windowToken);
            break;
          case ItemAppearanceWeaponModel.Middle:
            selectorValue = new NuiBind<int>("middleModelSelection").GetBindValue(oid, windowToken);
            slider = new NuiBind<int>("middleModelSlider");
            sliderResult = modelComboEntries.IndexOf(modelComboEntries.FirstOrDefault(m => m.Value == selectorValue));
            sliderValue = slider.GetBindValue(oid, windowToken);
            break;
          case ItemAppearanceWeaponModel.Bottom:
            selectorValue = new NuiBind<int>("bottomModelSelection").GetBindValue(oid, windowToken);
            slider = new NuiBind<int>("bottomModelSlider");
            sliderResult = modelComboEntries.IndexOf(modelComboEntries.FirstOrDefault(m => m.Value == selectorValue));
            sliderValue = slider.GetBindValue(oid, windowToken);
            break;
        }

        weapon.Appearance.SetWeaponModel(model, (byte)selectorValue);

        NwItem newItem = weapon.Clone(oid.ControlledCreature);
        oid.ControlledCreature.RunEquip(newItem, InventorySlot.RightHand);
        weapon.Destroy();
        weapon = newItem;
        oid.ControlledCreature.RunEquip(weapon, InventorySlot.RightHand);

        oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value = weapon;

        slider.SetBindWatch(oid, windowToken, false);
        slider.SetBindValue(oid, windowToken, sliderResult);
        slider.SetBindWatch(oid, windowToken, true);
      }

      private void HandleWeaponAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "weaponAppearanceModifier" || !Players.TryGetValue(nuiEvent.Player.LoginCreature, out Player player))
          return;

        if (nuiEvent.EventType == NuiEventType.Close)
        {
          EnableItemAppearanceFeedbackMessages();
          PlayerPlugin.ApplyLoopingVisualEffectToObject(nuiEvent.Player.ControlledCreature, nuiEvent.Player.ControlledCreature, 173);
          return;
        }
        
        NwItem item = nuiEvent.Player.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value;

        if (!item.IsValid || item.Possessor != nuiEvent.Player.ControlledCreature)
        {
          nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
          nuiEvent.Player.NuiDestroy(nuiEvent.WindowToken);
          return;
        }

        if (nuiEvent.EventType == NuiEventType.Watch)
          switch (nuiEvent.ElementId)
          {
            case "topModelSlider":
              player.HandleWeaponModelSliderChange(nuiEvent.WindowToken, ItemAppearanceWeaponModel.Top, item);
              break;

            case "topModelSelection":
              player.HandleWeaponModelSelectorChange(nuiEvent.WindowToken, ItemAppearanceWeaponModel.Top, item);
              break;

            case "middleModelSlider":
              player.HandleWeaponModelSliderChange(nuiEvent.WindowToken, ItemAppearanceWeaponModel.Middle, item);
              break;

            case "middleModelSelection":
              player.HandleWeaponModelSelectorChange(nuiEvent.WindowToken, ItemAppearanceWeaponModel.Middle, item);
              break;

            case "bottomModelSlider":
              player.HandleWeaponModelSliderChange(nuiEvent.WindowToken, ItemAppearanceWeaponModel.Bottom, item);
              break;

            case "bottomModelSelection":
              player.HandleWeaponModelSelectorChange(nuiEvent.WindowToken, ItemAppearanceWeaponModel.Bottom, item);
              break;
          }
      }
    }
  }
}
