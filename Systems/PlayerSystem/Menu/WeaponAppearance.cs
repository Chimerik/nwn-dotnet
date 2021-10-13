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
        NuiBind<string> title = new NuiBind<string>("title");
        NuiBind<int> topModelSelection = new NuiBind<int>("topModelSelection");
        NuiBind<int> topModelSlider = new NuiBind<int>("topModelSlider");
        NuiBind<int> middleModelSelection = new NuiBind<int>("middleModelSelection");
        NuiBind<int> middleModelSlider = new NuiBind<int>("middleModelSlider");
        NuiBind<int> bottomModelSelection = new NuiBind<int>("bottomModelSelection");
        NuiBind<int> bottomModelSlider = new NuiBind<int>("bottomModelSlider");
        NuiBind<int> topColorSelection = new NuiBind<int>("topColorSelection");
        NuiBind<int> topColorSlider = new NuiBind<int>("topColorSlider");
        NuiBind<int> middleColorSelection = new NuiBind<int>("middleColorSelection");
        NuiBind<int> middleColorSlider = new NuiBind<int>("middleColorSlider");
        NuiBind<int> bottomColorSelection = new NuiBind<int>("bottomColorSelection");
        NuiBind<int> bottomColorSlider = new NuiBind<int>("bottomColorSlider");

        NuiBind<List<NuiComboEntry>> topColorComboBind = new NuiBind<List<NuiComboEntry>>("topColorComboBind");
        NuiBind<List<NuiComboEntry>> midColorComboBind = new NuiBind<List<NuiComboEntry>>("midColorComboBind");
        NuiBind<List<NuiComboEntry>> botColorComboBind = new NuiBind<List<NuiComboEntry>>("botColorComboBind");

        List<NuiComboEntry> topModelCombo = BaseItems2da.baseItemTable.GetWeaponModelList(item.BaseItemType, "top");
        List<NuiComboEntry> topColorCombo = BaseItems2da.baseItemTable.GetWeaponColorList(item.BaseItemType, topModelCombo.FirstOrDefault().Value, "top");
        List<NuiComboEntry> midModelCombo = BaseItems2da.baseItemTable.GetWeaponModelList(item.BaseItemType, "mid");
        List<NuiComboEntry> midColorCombo = BaseItems2da.baseItemTable.GetWeaponColorList(item.BaseItemType, topModelCombo.FirstOrDefault().Value, "mid");
        List<NuiComboEntry> botModelCombo = BaseItems2da.baseItemTable.GetWeaponModelList(item.BaseItemType, "bot");
        List<NuiComboEntry> botColorCombo = BaseItems2da.baseItemTable.GetWeaponColorList(item.BaseItemType, topModelCombo.FirstOrDefault().Value, "bot");

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
                new NuiSpacer { Width = 75 },
                new NuiLabel("Modèle") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle },
                new NuiSpacer { Width = (windowRectangle.Width - 200)  * 0.48f },
                new NuiLabel("Couleur") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle }
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
                new NuiSlider(topModelSlider, 0, topModelCombo.Count - 1) { Step = 1,  Width = (windowRectangle.Width - 200)  * 0.48f },
                new NuiCombo
                {
                  Width = 70,
                  Entries = topColorComboBind,
                  Selected = topModelSelection
                },
                new NuiSlider(topColorSlider, 0, topColorCombo.Count - 1) { Step = 1,  Width = (windowRectangle.Width - 200)  * 0.48f }
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
                new NuiSlider(middleModelSlider, 0, midModelCombo.Count - 1) { Step = 1,  Width = (windowRectangle.Width - 200)  * 0.48f },
                new NuiCombo
                {
                  Width = 70,
                  Entries = midColorComboBind,
                  Selected = middleColorSelection
                },
                new NuiSlider(middleColorSlider, 0, midColorCombo.Count - 1) { Step = 1,  Width = (windowRectangle.Width - 200)  * 0.48f }
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
                new NuiSlider(bottomModelSlider, 0, botModelCombo.Count - 1) { Step = 1,  Width = (windowRectangle.Width - 200)  * 0.48f },
                new NuiCombo
                {
                  Width = 70,
                  Entries = botColorComboBind,
                  Selected = bottomColorSelection
                },
                new NuiSlider(bottomColorSlider, 0, midColorCombo.Count - 1) { Step = 1,  Width = (windowRectangle.Width - 200)  * 0.48f }
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

        Log.Info($"model top : {item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Top)}");
        Log.Info($"color top : {topColorCombo.IndexOf(topColorCombo.FirstOrDefault(l => l.Value == item.Appearance.GetWeaponColor(ItemAppearanceWeaponColor.Top)))}");
        Log.Info($"weapon color top : {item.Appearance.GetWeaponColor(ItemAppearanceWeaponColor.Top)}");



        topModelSelection.SetBindValue(oid, token, item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Top) / 10);
        topModelSlider.SetBindValue(oid, token, topColorCombo.IndexOf(topColorCombo.FirstOrDefault(l => l.Value == item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Top))) / 10);

        topColorSelection.SetBindValue(oid, token, item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Top) % 10);
        topColorSlider.SetBindValue(oid, token, topColorCombo.IndexOf(topColorCombo.FirstOrDefault(l => l.Value == item.Appearance.GetWeaponColor(ItemAppearanceWeaponColor.Top))) % 10);
        topColorComboBind.SetBindValue(oid, token, topColorCombo);

        middleModelSelection.SetBindValue(oid, token, item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Middle) / 10);
        middleModelSlider.SetBindValue(oid, token, topColorCombo.IndexOf(topColorCombo.FirstOrDefault(l => l.Value == item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Middle))) / 10);

        middleColorSelection.SetBindValue(oid, token, item.Appearance.GetWeaponColor(ItemAppearanceWeaponColor.Middle));
        middleColorSlider.SetBindValue(oid, token, topColorCombo.IndexOf(topColorCombo.FirstOrDefault(l => l.Value == item.Appearance.GetWeaponColor(ItemAppearanceWeaponColor.Middle))) % 10);
        midColorComboBind.SetBindValue(oid, token, midColorCombo);

        bottomModelSelection.SetBindValue(oid, token, item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Bottom) / 10);
        bottomModelSlider.SetBindValue(oid, token, topColorCombo.IndexOf(topColorCombo.FirstOrDefault(l => l.Value == item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Bottom))) / 10);

        bottomColorSelection.SetBindValue(oid, token, item.Appearance.GetWeaponColor(ItemAppearanceWeaponColor.Bottom) % 10);
        bottomColorSlider.SetBindValue(oid, token, topColorCombo.IndexOf(topColorCombo.FirstOrDefault(l => l.Value == item.Appearance.GetWeaponColor(ItemAppearanceWeaponColor.Bottom))) % 10);
        botColorComboBind.SetBindValue(oid, token, botColorCombo);

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);

        Task waitWindowOpened = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.6));

          topModelSelection.SetBindWatch(oid, token, true);
          topModelSlider.SetBindWatch(oid, token, true);

          topColorSelection.SetBindWatch(oid, token, true);
          topColorSlider.SetBindWatch(oid, token, true);

          middleModelSelection.SetBindWatch(oid, token, true);
          middleColorSelection.SetBindWatch(oid, token, true);

          middleModelSlider.SetBindWatch(oid, token, true);
          middleColorSlider.SetBindWatch(oid, token, true);

          bottomModelSelection.SetBindWatch(oid, token, true);
          bottomColorSelection.SetBindWatch(oid, token, true);

          bottomModelSlider.SetBindWatch(oid, token, true);
          bottomColorSlider.SetBindWatch(oid, token, true);
        });
      }

      public void HandleWeaponModelSliderChange(int windowToken, ItemAppearanceWeaponModel model, NwItem weapon)
      {
        if (!weapon.IsValid)
          return;

        int sliderValue = 0;
        NuiBind<int> selector = null;
        NuiBind<List<NuiComboEntry>> colorComboEntries = null;
        NuiBind<int> colorSelector = null;
        NuiBind<int> colorSlider = null;
        int selectedValue = 0;
        int result = 0;
        int colorResult = 0;
        string part = "top";

        switch (model)
        {
          case ItemAppearanceWeaponModel.Top:
            sliderValue = new NuiBind<int>("topModelSlider").GetBindValue(oid, windowToken);
            selector = new NuiBind<int>("topModelSelection");
            selectedValue = selector.GetBindValue(oid, windowToken);

            colorComboEntries = new NuiBind<List<NuiComboEntry>>("topColorComboBind");
            colorSelector = new NuiBind<int>("topColorSelection");
            colorSlider = new NuiBind<int>("topColorSlider");
            
            result = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, "top").ElementAt(sliderValue).Value;
            colorResult = BaseItems2da.baseItemTable.GetWeaponColorList(weapon.BaseItemType, result, "top").FirstOrDefault().Value;
            break;
          case ItemAppearanceWeaponModel.Middle:
            sliderValue = new NuiBind<int>("middleModelSlider").GetBindValue(oid, windowToken);
            selector = new NuiBind<int>("middleModelSelection");
            selectedValue = selector.GetBindValue(oid, windowToken);
            part = "mid";

            colorComboEntries = new NuiBind<List<NuiComboEntry>>("midColorComboBind");
            colorSelector = new NuiBind<int>("middleColorSelection");
            colorSlider = new NuiBind<int>("middleColorSlider");
            
            result = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, "mid").ElementAt(sliderValue).Value;
            colorResult = BaseItems2da.baseItemTable.GetWeaponColorList(weapon.BaseItemType, result, "mid").FirstOrDefault().Value;
            break;
          case ItemAppearanceWeaponModel.Bottom:
            sliderValue = new NuiBind<int>("bottomModelSlider").GetBindValue(oid, windowToken);
            selector = new NuiBind<int>("bottomModelSelection");
            selectedValue = selector.GetBindValue(oid, windowToken);
            part = "bot";
            colorComboEntries = new NuiBind<List<NuiComboEntry>>("botColorComboBind");
            colorSelector = new NuiBind<int>("bottomColorSelection");
            colorSlider = new NuiBind<int>("bottomColorSlider");
            
            result = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, "bot").ElementAt(sliderValue).Value;
            colorResult = BaseItems2da.baseItemTable.GetWeaponColorList(weapon.BaseItemType, result, "bot").FirstOrDefault().Value;
            break;
        }

        weapon.Appearance.SetWeaponModel(model, byte.Parse($"{result}{colorSelector.GetBindValue(oid, windowToken)}"));

        oid.ControlledCreature.RunUnequip(weapon);
        NwItem newItem = weapon.Clone(oid.ControlledCreature);
        weapon.Destroy();
        weapon = newItem;

        oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value = weapon;

        Task waitUnequip = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          oid.ControlledCreature.RunEquip(weapon, InventorySlot.RightHand);
        });

        selector.SetBindWatch(oid, windowToken, false);
        selector.SetBindValue(oid, windowToken, result);

        colorComboEntries.SetBindValue(oid, windowToken, BaseItems2da.baseItemTable.GetWeaponColorList(weapon.BaseItemType, result, part));

        colorSelector.SetBindWatch(oid, windowToken, false);
        colorSelector.SetBindValue(oid, windowToken, colorResult);

        colorSlider.SetBindWatch(oid, windowToken, false);
        colorSlider.SetBindValue(oid, windowToken, 0);

        selector.SetBindWatch(oid, windowToken, true);
        colorSelector.SetBindWatch(oid, windowToken, true);
        colorSlider.SetBindWatch(oid, windowToken, true);
      }
      public void HandleWeaponModelSelectorChange(int windowToken, ItemAppearanceWeaponModel model, NwItem weapon)
      {
        if (!weapon.IsValid)
          return;

        int selectorValue = 0;
        NuiBind<int> slider = null;
        NuiBind<List<NuiComboEntry>> colorComboEntries = null;
        NuiBind<int> colorSelector = null;
        NuiBind<int> colorSlider = null;
        int sliderValue = 0;
        int colorResult = 0;
        int sliderResult = 0;
        string part = "top";

        switch (model)
        {
          case ItemAppearanceWeaponModel.Top:
            selectorValue = new NuiBind<int>("topModelSelection").GetBindValue(oid, windowToken);
            slider = new NuiBind<int>("topModelSlider");
            sliderResult = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, "top").ElementAt(selectorValue).Value;
            sliderValue = slider.GetBindValue(oid, windowToken);

            colorComboEntries = new NuiBind<List<NuiComboEntry>>("topColorComboBind");
            colorSelector = new NuiBind<int>("topColorSelection");
            colorSlider = new NuiBind<int>("topColorSlider");
            colorResult = BaseItems2da.baseItemTable.GetWeaponColorList(weapon.BaseItemType, selectorValue, "top").FirstOrDefault().Value;
            break;
          case ItemAppearanceWeaponModel.Middle:
            selectorValue = new NuiBind<int>("middleModelSelection").GetBindValue(oid, windowToken);
            slider = new NuiBind<int>("middleModelSlider");
            sliderResult = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, "mid").ElementAt(selectorValue).Value;
            part = "mid";

            colorComboEntries = new NuiBind<List<NuiComboEntry>>("midColorComboBind");
            colorSelector = new NuiBind<int>("middleColorSelection");
            colorSlider = new NuiBind<int>("middleColorSlider");
            sliderValue = slider.GetBindValue(oid, windowToken);
            colorResult = BaseItems2da.baseItemTable.GetWeaponColorList(weapon.BaseItemType, selectorValue, "mid").FirstOrDefault().Value;
            break;
          case ItemAppearanceWeaponModel.Bottom:
            selectorValue = new NuiBind<int>("bottomModelSelection").GetBindValue(oid, windowToken);
            slider = new NuiBind<int>("bottomModelSlider");
            sliderResult = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, "bot").ElementAt(selectorValue).Value;
            part = "bot";

            colorComboEntries = new NuiBind<List<NuiComboEntry>>("botColorComboBind");
            colorSelector = new NuiBind<int>("middleColorSelection");
            colorSlider = new NuiBind<int>("middleColorSlider");
            sliderValue = slider.GetBindValue(oid, windowToken);
            colorResult = BaseItems2da.baseItemTable.GetWeaponColorList(weapon.BaseItemType, selectorValue, "bot").FirstOrDefault().Value;
            break;
        }

        Log.Info($"model : {selectorValue}{colorSelector.GetBindValue(oid, windowToken)}");
        weapon.Appearance.SetWeaponModel(model, byte.Parse($"{selectorValue}{colorSelector.GetBindValue(oid, windowToken)}"));
        oid.ControlledCreature.RunUnequip(weapon);
        NwItem newItem = weapon.Clone(oid.ControlledCreature);
        oid.ControlledCreature.RunEquip(newItem, InventorySlot.RightHand);
        weapon.Destroy();
        weapon = newItem;
        oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value = weapon;

        Task waitUnequip = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          oid.ControlledCreature.RunEquip(weapon, InventorySlot.RightHand);
        });

        slider.SetBindWatch(oid, windowToken, false);
        slider.SetBindValue(oid, windowToken, sliderResult);

        colorComboEntries.SetBindValue(oid, windowToken, BaseItems2da.baseItemTable.GetWeaponColorList(weapon.BaseItemType, selectorValue, part));

        colorSelector.SetBindWatch(oid, windowToken, false);
        colorSelector.SetBindValue(oid, windowToken, colorResult);

        colorSlider.SetBindWatch(oid, windowToken, false);
        colorSlider.SetBindValue(oid, windowToken, 0);

        slider.SetBindWatch(oid, windowToken, true);
        colorSelector.SetBindWatch(oid, windowToken, true);
        colorSlider.SetBindWatch(oid, windowToken, true);
      }

      public void HandleWeaponColorSliderChange(int windowToken, ItemAppearanceWeaponModel model, NwItem weapon)
      {
        if (!weapon.IsValid)
          return;

        int sliderValue = 0;
        NuiBind<int> selector = null;
        int selectedValue = 0;
        int selectedModel = 0;
        int result = 0;

        switch (model)
        {
          case ItemAppearanceWeaponModel.Top:
            sliderValue = new NuiBind<int>("topColorSlider").GetBindValue(oid, windowToken);
            selector = new NuiBind<int>("topColorSelection");
            selectedValue = selector.GetBindValue(oid, windowToken);
            selectedModel = new NuiBind<int>("topModelSelection").GetBindValue(oid, windowToken);
            result = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, "top").ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceWeaponModel.Middle:
            sliderValue = new NuiBind<int>("middleColorSlider").GetBindValue(oid, windowToken);
            selector = new NuiBind<int>("middleColorSelection");
            selectedValue = selector.GetBindValue(oid, windowToken);
            selectedModel = new NuiBind<int>("middleModelSelection").GetBindValue(oid, windowToken);
            result = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, "mid").ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceWeaponModel.Bottom:
            sliderValue = new NuiBind<int>("bottomColorSlider").GetBindValue(oid, windowToken);
            selector = new NuiBind<int>("bottomColorSelection");
            selectedValue = selector.GetBindValue(oid, windowToken);
            selectedModel = new NuiBind<int>("bottomModelSelection").GetBindValue(oid, windowToken);
            result = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, "bot").ElementAt(sliderValue).Value;
            break;
        }

        weapon.Appearance.SetWeaponModel(model, byte.Parse($"{selectedModel}{result}"));

        NwItem newItem = weapon.Clone(oid.ControlledCreature);
        oid.ControlledCreature.RunEquip(newItem, InventorySlot.RightHand);
        weapon.Destroy();
        weapon = newItem;
        oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value = weapon;

        Task waitUnequip = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          oid.ControlledCreature.RunEquip(weapon, InventorySlot.RightHand);
        });

        selector.SetBindWatch(oid, windowToken, false);
        selector.SetBindValue(oid, windowToken, result);
        selector.SetBindWatch(oid, windowToken, true);
      }
      public void HandleWeaponColorSelectorChange(int windowToken, ItemAppearanceWeaponModel model, NwItem weapon)
      {
        if (!weapon.IsValid)
          return;

        int selectorValue = 0;
        NuiBind<int> slider = null;
        int sliderValue = 0;
        int sliderResult = 0;
        int selectedModel = 0;

        switch (model)
        {
          case ItemAppearanceWeaponModel.Top:
            selectorValue = new NuiBind<int>("topModelSelection").GetBindValue(oid, windowToken);
            slider = new NuiBind<int>("topModelSlider");
            sliderResult = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, "top").ElementAt(selectorValue).Value;
            selectedModel = new NuiBind<int>("topModelSelection").GetBindValue(oid, windowToken);
            sliderValue = slider.GetBindValue(oid, windowToken);
            break;
          case ItemAppearanceWeaponModel.Middle:
            selectorValue = new NuiBind<int>("middleModelSelection").GetBindValue(oid, windowToken);
            slider = new NuiBind<int>("middleModelSlider");
            sliderResult = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, "mid").ElementAt(selectorValue).Value;
            selectedModel = new NuiBind<int>("middleModelSelection").GetBindValue(oid, windowToken);
            sliderValue = slider.GetBindValue(oid, windowToken);
            break;
          case ItemAppearanceWeaponModel.Bottom:
            selectorValue = new NuiBind<int>("bottomModelSelection").GetBindValue(oid, windowToken);
            slider = new NuiBind<int>("bottomModelSlider");
            sliderResult = BaseItems2da.baseItemTable.GetWeaponModelList(weapon.BaseItemType, "bot").ElementAt(selectorValue).Value;
            selectedModel = new NuiBind<int>("bottomModelSelection").GetBindValue(oid, windowToken);
            sliderValue = slider.GetBindValue(oid, windowToken);
            break;
        }

        weapon.Appearance.SetWeaponModel(model, byte.Parse($"{selectedModel}{selectorValue}"));
        NwItem newItem = weapon.Clone(oid.ControlledCreature);
        oid.ControlledCreature.RunEquip(newItem, InventorySlot.RightHand);
        weapon.Destroy();
        weapon = newItem;
        oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value = weapon;

        Task waitUnequip = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          oid.ControlledCreature.RunEquip(weapon, InventorySlot.RightHand);
        });

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

            case "topColorSlider":
              player.HandleWeaponColorSliderChange(nuiEvent.WindowToken, ItemAppearanceWeaponModel.Top, item);
              break;

            case "topColorSelection":
              player.HandleWeaponColorSelectorChange(nuiEvent.WindowToken, ItemAppearanceWeaponModel.Top, item);
              break;

            case "middleColorSlider":
              player.HandleWeaponColorSliderChange(nuiEvent.WindowToken, ItemAppearanceWeaponModel.Middle, item);
              break;

            case "middleColorSelection":
              player.HandleWeaponColorSelectorChange(nuiEvent.WindowToken, ItemAppearanceWeaponModel.Middle, item);
              break;

            case "bottomColorSlider":
              player.HandleWeaponColorSliderChange(nuiEvent.WindowToken, ItemAppearanceWeaponModel.Bottom, item);
              break;

            case "bottomColorSelection":
              player.HandleWeaponColorSelectorChange(nuiEvent.WindowToken, ItemAppearanceWeaponModel.Bottom, item);
              break;
          }
      }
    }
  }
}
