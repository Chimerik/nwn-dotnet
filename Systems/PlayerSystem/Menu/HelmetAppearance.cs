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
      public void CreateHelmetAppearanceWindow(NwItem item)
      {
        string windowId = "helmetAppearanceModifier";
        DisableItemAppearanceFeedbackMessages();
        NuiBind<string> title = new NuiBind<string>("title");
        NuiBind<int> modelSelection = new NuiBind<int>("modelSelection");
        NuiBind<int> modelSlider = new NuiBind<int>("modelSlider");

        List<NuiComboEntry> modelCombo = BaseItems2da.baseItemTable.helmetModelEntries;

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
                new  NuiSpacer { },
                new NuiButton("Nom & Description") { Id = "openNameDescription", Height = 35, Width = 150 },
                new NuiButton("Couleurs") { Id = "openColors", Height = 35, Width = 150 },
                new NuiSpacer { }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Modèle") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle },
                new NuiCombo
                {
                  Width = 70,
                  Entries = modelCombo,
                  Selected = modelSelection
                },
                new NuiSlider(modelSlider, 0, modelCombo.Count - 1) { Step = 1,  Width = (windowRectangle.Width - 130)  * 0.96f },
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

        oid.OnNuiEvent -= HandleHelmetAppearanceEvents;
        oid.OnNuiEvent += HandleHelmetAppearanceEvents;

        int token = oid.CreateNuiWindow(window, windowId);

        PlayerPlugin.ApplyLoopingVisualEffectToObject(oid.ControlledCreature, oid.ControlledCreature, 173);

        title.SetBindValue(oid, token, $"Modifier l'apparence de {item.Name}");

        modelSelection.SetBindValue(oid, token, item.Appearance.GetSimpleModel());
        modelSlider.SetBindValue(oid, token, modelCombo.IndexOf(modelCombo.FirstOrDefault(l => l.Value == item.Appearance.GetSimpleModel())));

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);

        Task waitWindowOpened = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.6));

          modelSelection.SetBindWatch(oid, token, true);
          modelSlider.SetBindWatch(oid, token, true);
        });
      }

      public void HandleHelmetModelSliderChange(int windowToken, NwItem helmet)
      {
        if (!helmet.IsValid)
          return;

        int sliderValue = new NuiBind<int>("modelSlider").GetBindValue(oid, windowToken);
        NuiBind<int> selector = new NuiBind<int>("modelSelection");
        int result = BaseItems2da.baseItemTable.helmetModelEntries.ElementAt(sliderValue).Value;

        helmet.Appearance.SetSimpleModel((byte)result);

        NwItem newItem = helmet.Clone(oid.ControlledCreature);
        oid.ControlledCreature.RunEquip(newItem, InventorySlot.Head);
        oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value = newItem;
        helmet.Destroy();

        selector.SetBindWatch(oid, windowToken, false);
        selector.SetBindValue(oid, windowToken, result);
        selector.SetBindWatch(oid, windowToken, true);
      }
      public void HandleHelmetModelSelectorChange(int windowToken, NwItem helmet)
      {
        if (!helmet.IsValid)
          return;

        int selectorValue = new NuiBind<int>("modelSelection").GetBindValue(oid, windowToken);
        NuiBind<int> slider = new NuiBind<int>("modelSlider");
        int sliderValue = slider.GetBindValue(oid, windowToken);
        int sliderResult = BaseItems2da.baseItemTable.helmetModelEntries.IndexOf(BaseItems2da.baseItemTable.helmetModelEntries.FirstOrDefault(m => m.Value == selectorValue));

        helmet.Appearance.SetSimpleModel((byte)selectorValue);

        NwItem newItem = helmet.Clone(oid.ControlledCreature);
        oid.ControlledCreature.RunEquip(newItem, InventorySlot.Head);
        oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value = newItem;
        helmet.Destroy();

        slider.SetBindWatch(oid, windowToken, false);
        slider.SetBindValue(oid, windowToken, sliderResult);
        slider.SetBindWatch(oid, windowToken, true);
      }

      private void HandleHelmetAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "helmetAppearanceModifier" || !Players.TryGetValue(nuiEvent.Player.LoginCreature, out Player player))
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

        switch(nuiEvent.EventType)
        {
          case NuiEventType.Click:
            switch (nuiEvent.ElementId)
            {
              case "openColors":
                nuiEvent.Player.NuiDestroy(nuiEvent.WindowToken);
                player.CreateHelmetColorsWindow(item);
                break;
              case "openNameDescription":
                player.CreateItemNameDescriptionWindow(item);
                break;
            }
            break;

          case NuiEventType.Watch:
            switch (nuiEvent.ElementId)
            {
              case "modelSlider":
                player.HandleHelmetModelSliderChange(nuiEvent.WindowToken, item);
                break;

              case "modelSelection":
                player.HandleHelmetModelSelectorChange(nuiEvent.WindowToken, item);
                break;
            }
            break;
        }
      }
    }
  }
}
