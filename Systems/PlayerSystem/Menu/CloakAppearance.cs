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
      public class CloakAppearanceWindow : PlayerWindow
      {
        private NwItem item { get; set; }
        private readonly NuiColumn rootColumn;
        private readonly NuiSlider slider;
        private readonly NuiBind<int> modelSelection = new ("modelSelection");
        private readonly NuiBind<int> modelSlider = new ("modelSlider");
        public CloakAppearanceWindow(Player player, NwItem item) : base(player)
        {
          windowId = "cloakAppearanceModifier";

          slider = new NuiSlider(modelSlider, 0, CloakModel2da.combo.Count - 1) { Step = 1 };

          rootColumn = new NuiColumn
          {
            Children = new List<NuiElement>
            {
              new NuiRow
              {
                Children = new List<NuiElement>
                {
                  new NuiSpacer(),
                  new NuiButton("Nom & Description") { Id = "openNameDescription", Height = 35, Width = 150 },
                  new NuiButton("Couleurs") { Id = "openColors", Height = 35, Width = 150 },
                  new NuiSpacer()
                }
              },
              new NuiRow
              {
                Children = new List<NuiElement>
                {
                  new NuiLabel("Modèle") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle },
                  new NuiCombo { Width = 150, Entries = CloakModel2da.combo, Selected = modelSelection },
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
          this.item = item;

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          slider.Width = (windowRectangle.Width - 210) * 0.96f;

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
            nuiToken.OnNuiEvent += HandleCloakAppearanceEvents;

            PlayerPlugin.ApplyLoopingVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 173);

            modelSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetSimpleModel());
            modelSlider.SetBindValue(player.oid, nuiToken.Token, CloakModel2da.combo.IndexOf(CloakModel2da.combo.FirstOrDefault(l => l.Value == item.Appearance.GetSimpleModel())));

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            modelSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            modelSlider.SetBindWatch(player.oid, nuiToken.Token, true);
          }        }
        private void HandleCloakAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.EventType == NuiEventType.Close)
          {
            PlayerPlugin.ApplyLoopingVisualEffectToObject(nuiEvent.Player.ControlledCreature, nuiEvent.Player.ControlledCreature, 173);
            player.EnableItemAppearanceFeedbackMessages();
            return;
          }

          if (!item.IsValid || item.Possessor != nuiEvent.Player.ControlledCreature)
          {
            nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
            CloseWindow();
            return;
          }

          if (nuiEvent.EventType == NuiEventType.Click && nuiEvent.ElementId == "openColors")
          {
            CloseWindow();

            if (player.windows.ContainsKey("cloakColorModifier"))
              ((CloakColorWindow)player.windows["cloakColorModifier"]).CreateWindow(item);
            else
              player.windows.Add("cloakColorModifier", new CloakColorWindow(player, item));

            return;
          }

          if (nuiEvent.EventType == NuiEventType.Watch)
            switch (nuiEvent.ElementId)
            {
              case "modelSlider":
                HandleCloakModelSliderChange();
                break;

              case "modelSelection":
                HandleCloakModelSelectorChange();
                break;
            }
        }
        public void HandleCloakModelSliderChange()
        {
          if (!item.IsValid)
            return;

          int sliderValue = modelSlider.GetBindValue(player.oid, nuiToken.Token);
          int result = CloakModel2da.combo.ElementAt(sliderValue).Value;

          item.Appearance.SetSimpleModel((byte)result);

          NwItem newItem = item.Clone(player.oid.ControlledCreature);
          player.oid.ControlledCreature.RunEquip(newItem, InventorySlot.Cloak);
          item.Destroy();
          item = newItem;

          modelSelection.SetBindWatch(player.oid, nuiToken.Token, false);
          modelSelection.SetBindValue(player.oid, nuiToken.Token, result);
          modelSelection.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        public void HandleCloakModelSelectorChange()
        {
          if (!item.IsValid)
            return;

          int selectorValue = modelSelection.GetBindValue(player.oid, nuiToken.Token);
          int sliderResult = CloakModel2da.combo.IndexOf(CloakModel2da.combo.FirstOrDefault(m => m.Value == selectorValue));

          item.Appearance.SetSimpleModel((byte)selectorValue);

          NwItem newItem = item.Clone(player.oid.ControlledCreature);
          player.oid.ControlledCreature.RunEquip(newItem, InventorySlot.Cloak);
          item.Destroy();
          item = newItem;

          modelSlider.SetBindWatch(player.oid, nuiToken.Token, false);
          modelSlider.SetBindValue(player.oid, nuiToken.Token, sliderResult);
          modelSlider.SetBindWatch(player.oid, nuiToken.Token, true);
        }
      }
    }
  }
}
