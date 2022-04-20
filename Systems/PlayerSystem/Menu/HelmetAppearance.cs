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
      public class HelmetAppearanceWindow : PlayerWindow
      {
        NwItem item { get; set; }
        private readonly NuiColumn rootColumn;
        private readonly NuiSlider slider;
        private readonly NuiBind<int> modelSelection = new NuiBind<int>("modelSelection");
        private readonly NuiBind<int> modelSlider = new NuiBind<int>("modelSlider");
        public HelmetAppearanceWindow(Player player, NwItem item) : base(player)
        {
          windowId = "helmetAppearanceModifier";

          slider = new NuiSlider(modelSlider, 0, BaseItems2da.helmetModelEntries.Count - 1) { Step = 1 };

          rootColumn = new NuiColumn
          {
            Children = new List<NuiElement>
            {
              new NuiRow
              {
                Children = new List<NuiElement>
                {
                  new  NuiSpacer(),
                  new NuiButton("Couleurs") { Id = "openColors", Height = 35, Width = 150 },
                  new NuiSpacer()
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
                    Entries = BaseItems2da.helmetModelEntries,
                    Selected = modelSelection
                  },
                  slider
                }
              },
            }
          };

          CreateWindow(item);
        }

        public void CreateWindow(NwItem item)
        {
          player.DisableItemAppearanceFeedbackMessages();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);
          slider.Width = (windowRectangle.Width - 130) * 0.96f;

          window = new NuiWindow(rootColumn, $"Modifier l'apparence de {item.Name}")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = true,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleHelmetAppearanceEvents;
          player.oid.OnNuiEvent += HandleHelmetAppearanceEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          player.ActivateSpotLight();

          modelSelection.SetBindValue(player.oid, token, item.Appearance.GetSimpleModel());
          modelSlider.SetBindValue(player.oid, token, BaseItems2da.helmetModelEntries.IndexOf(BaseItems2da.helmetModelEntries.FirstOrDefault(l => l.Value == item.Appearance.GetSimpleModel())));

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          /*Task waitWindowOpened = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.6));
          */
            modelSelection.SetBindWatch(player.oid, token, true);
            modelSlider.SetBindWatch(player.oid, token, true);
          //});
        }
        public void HandleHelmetModelSliderChange()
        {
          if (!item.IsValid)
            return;

          int sliderValue = modelSlider.GetBindValue(player.oid, token);
          int result = BaseItems2da.helmetModelEntries.ElementAt(sliderValue).Value;

          item.Appearance.SetSimpleModel((byte)result);

          NwItem newItem = item.Clone(player.oid.ControlledCreature);
          player.oid.ControlledCreature.RunEquip(newItem, InventorySlot.Head);
          item.Destroy();
          item = newItem;

          modelSelection.SetBindWatch(player.oid, token, false);
          modelSelection.SetBindValue(player.oid, token, result);
          modelSelection.SetBindWatch(player.oid, token, true);
        }
        public void HandleHelmetModelSelectorChange()
        {
          if (!item.IsValid)
            return;

          int selectorValue = modelSelection.GetBindValue(player.oid, token);
          int sliderValue = modelSlider.GetBindValue(player.oid, token);
          int sliderResult = BaseItems2da.helmetModelEntries.IndexOf(BaseItems2da.helmetModelEntries.FirstOrDefault(m => m.Value == selectorValue));

          item.Appearance.SetSimpleModel((byte)selectorValue);

          NwItem newItem = item.Clone(player.oid.ControlledCreature);
          player.oid.ControlledCreature.RunEquip(newItem, InventorySlot.Head);
          item.Destroy();
          item = newItem;

          modelSlider.SetBindWatch(player.oid, token, false);
          modelSlider.SetBindValue(player.oid, token, sliderResult);
          modelSlider.SetBindWatch(player.oid, token, true);
        }

        private void HandleHelmetAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "helmetAppearanceModifier")
            return;

          if (nuiEvent.EventType == NuiEventType.Close)
          {
            player.EnableItemAppearanceFeedbackMessages();
            player.RemoveSpotLight();
            return;
          }

          if (!item.IsValid || item.Possessor != nuiEvent.Player.ControlledCreature)
          {
            nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
            CloseWindow();
            return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:
              switch (nuiEvent.ElementId)
              {
                case "openColors":
                  CloseWindow();

                  if (player.windows.ContainsKey("helmetColorModifier"))
                    ((HelmetColorWindow)player.windows["helmetColorModifier"]).CreateWindow(item);
                  else
                    player.windows.Add("helmetColorModifier", new HelmetColorWindow(player, item));

                  break;
              }
              break;

            case NuiEventType.Watch:
              switch (nuiEvent.ElementId)
              {
                case "modelSlider":
                  HandleHelmetModelSliderChange();
                  break;

                case "modelSelection":
                  HandleHelmetModelSelectorChange();
                  break;
              }
              break;
          }
        }
      }
    }
  }
}
