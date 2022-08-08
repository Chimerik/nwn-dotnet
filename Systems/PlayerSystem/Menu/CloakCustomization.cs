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
      public class CloakCustomizationWindow : PlayerWindow
      {
        private NwItem item { get; set; }

        private const float BUTTONSIZE = 12;
        private const float COMBOSIZE = 100;

        private readonly NuiColumn rootColumn = new() { Margin = 0.0f };
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<int> cloakSelection = new("cloakSelection");
        private readonly NuiBind<string> cloakLeather1 = new("cloakLeather1");
        private readonly NuiBind<string> cloakCloth1 = new("cloakCloth1");
        private readonly NuiBind<string> cloakMetal1 = new("cloakMetal1");
        private readonly NuiBind<string> cloakLeather2 = new("cloakLeather2");
        private readonly NuiBind<string> cloakCloth2 = new("cloakCloth2");
        private readonly NuiBind<string> cloakMetal2 = new("cloakMetal2");

        private NuiBind<string> lastClickedColorButton;
        private readonly NuiBind<List<NuiComboEntry>> cloakList = new NuiBind<List<NuiComboEntry>>("cloakList");

        private ItemAppearanceArmorColor selectedColorChannel { get; set; }

        public CloakCustomizationWindow(Player player, NwItem item) : base(player)
        {
          windowId = "cloakColorsModifier";
          rootColumn.Children = rootChildren;

          int nbButton = 0;

          for (int i = 0; i < 13; i++)
          {
            NuiRow row = new NuiRow() { Height = 15, Margin = 0.0f };
            List<NuiElement> rowChildren = new List<NuiElement>();

            for (int j = 0; j < 22; j++)
            {
              if (nbButton > 255)
                break;

              rowChildren.Add(new NuiButton("")
              {
                Id = $"{nbButton}",
                Width = 15,
                Height = 15,
                Margin = 0.0f,
                DrawList = new() { new NuiDrawListImage(Utils.paletteColorBindings[nbButton], new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } }
              });

              nbButton++;
            }

            row.Children = rowChildren;
            rootChildren.Add(row);
          }

          rootChildren.Add(new NuiRow() { Height = 20, Margin = 0.0f, Children = new List<NuiElement>() 
          {  
            new NuiSpacer(),
            new NuiLabel("Cape") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiSpacer()
          }});

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("<") { Id = "cloakDecrease", Height = 20, Width = 20, Margin = 0.0f },
            new NuiCombo(){ Entries = cloakList, Selected = cloakSelection, Height = 20, Width = 200, Margin = 0.0f },
            new NuiButton(">") { Id = "cloakIncrease", Height = 20, Width = 20, Margin = 0.0f },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("") { Id = "cloakLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuir 1", DrawList = new() { new NuiDrawListImage(cloakLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
            new NuiButton("") { Id = "cloakCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tissu 1", DrawList = new() { new NuiDrawListImage(cloakCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
            new NuiButton("") { Id = "cloakMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Métal 1", DrawList = new() { new NuiDrawListImage(cloakMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("") { Id = "cloakLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuir 2", DrawList = new() { new NuiDrawListImage(cloakLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
            new NuiButton("") { Id = "cloakCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tissu 2", DrawList = new() { new NuiDrawListImage(cloakCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
            new NuiButton("") { Id = "cloakMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Métal 2", DrawList = new() { new NuiDrawListImage(cloakMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
            new NuiSpacer()
          }});

          CreateWindow(item);
        }
        public void CreateWindow(NwItem item)
        {
          this.item = item;
          selectedColorChannel = ItemAppearanceArmorColor.Leather1;

          player.DisableItemAppearanceFeedbackMessages();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 8, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          window = new NuiWindow(rootColumn, $"Modification de {item.Name}")
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
            nuiToken.OnNuiEvent += HandleItemColorsEvents;

            player.ActivateSpotLight(player.oid.ControlledCreature);

            cloakList.SetBindValue(player.oid, nuiToken.Token, CloakModel2da.combo);

            cloakLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Leather1) + 1}");
            cloakCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Cloth1) + 1}");
            cloakMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Metal1) + 1));
            cloakLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Leather2) + 1}");
            cloakCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Cloth2) + 1}");
            cloakMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Metal2) + 1));

            cloakSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetSimpleModel());
            cloakSelection.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            for (int i = 0; i < 256; i++)
              Utils.paletteColorBindings[i].SetBindValue(player.oid, nuiToken.Token, $"leather{i + 1}");
          }
        }
        private void HandleItemColorsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.EventType == NuiEventType.Close)
          {
            player.RemoveSpotLight(player.oid.ControlledCreature);
            player.EnableItemAppearanceFeedbackMessages();
            return;
          }

          if (!item.IsValid || item.Possessor != nuiEvent.Player.ControlledCreature)
          {
            nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
            player.EnableItemAppearanceFeedbackMessages();
            CloseWindow();
            return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "cloakLeather1":
                  HandlePaletteSwap();
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = cloakLeather1;
                  return;

                case "cloakCloth1":
                  HandlePaletteSwap();
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = cloakCloth1;
                  return;

                case "cloakMetal1":
                  HandlePaletteSwap("metal");
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = cloakMetal1;
                  return;

                case "cloakLeather2":
                  HandlePaletteSwap();
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = cloakLeather2;
                  return;

                case "cloakCloth2":
                  HandlePaletteSwap();
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = cloakCloth2;
                  return;

                case "cloakMetal2":
                  HandlePaletteSwap("metal");
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = cloakMetal2;
                  return;

                case "cloakDecrease":
                  HandleArmorSelectorChange(-1);
                  return;

                case "cloakIncrease":
                  HandleArmorSelectorChange(1);
                  return; 
              }

              int resRef = int.Parse(nuiEvent.ElementId) + 1;

              if (lastClickedColorButton.Key.Contains("metal"))
                lastClickedColorButton.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(resRef));
              else
                lastClickedColorButton.SetBindValue(player.oid, nuiToken.Token, $"leather{resRef}");

              item.Appearance.SetArmorColor(selectedColorChannel, byte.Parse(nuiEvent.ElementId));

              NwItem newItem = item.Clone(nuiEvent.Player.ControlledCreature);
              nuiEvent.Player.ControlledCreature.RunEquip(newItem, InventorySlot.Cloak);
              item.Destroy();
              item = newItem;

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "cloakSelection":

                  Log.Info(cloakSelection.GetBindValue(player.oid, nuiToken.Token));

                  item.Appearance.SetSimpleModel((byte)cloakSelection.GetBindValue(player.oid, nuiToken.Token));
                  NwItem newCloak = item.Clone(nuiEvent.Player.ControlledCreature);
                  nuiEvent.Player.ControlledCreature.RunEquip(newCloak, InventorySlot.Cloak);
                  item.Destroy();
                  item = newCloak;

                  break;
              }

              break;
          }
        }
        private void LoadPaletteBindings(string paletteType = "")
        {
          if (paletteType == "metal")
            for (int i = 0; i < 56; i++)
              Utils.paletteColorBindings[i].SetBindValue(player.oid, nuiToken.Token, $"metal{i + 1}");
          else
            for (int i = 0; i < 56; i++)
              Utils.paletteColorBindings[i].SetBindValue(player.oid, nuiToken.Token, $"leather{i + 1}");
        }
        private void HandleArmorSelectorChange(int change)
        {
          cloakSelection.SetBindWatch(player.oid, nuiToken.Token, false);
          
          int newValue = CloakModel2da.combo.IndexOf(CloakModel2da.combo.FirstOrDefault(p => p.Value == cloakSelection.GetBindValue(player.oid, nuiToken.Token))) + change;

          if (newValue >= CloakModel2da.combo.Count)
            newValue = 0;

          if (newValue < 0)
            newValue = CloakModel2da.combo.Count - 1;

          cloakSelection.SetBindValue(player.oid, nuiToken.Token, CloakModel2da.combo[newValue].Value);

          item.Appearance.SetSimpleModel((byte)cloakSelection.GetBindValue(player.oid, nuiToken.Token));

          NwItem newCloak = item.Clone(player.oid.ControlledCreature);
          player.oid.ControlledCreature.RunEquip(newCloak, InventorySlot.Cloak);
          item.Destroy();
          item = newCloak;

          cloakSelection.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        private void HandlePaletteSwap(string type = "")
        {
          if (type == "metal")
          {
            if (selectedColorChannel != ItemAppearanceArmorColor.Metal1 && selectedColorChannel != ItemAppearanceArmorColor.Metal2)
              LoadPaletteBindings("metal");
          }
          else
          {
            if (selectedColorChannel == ItemAppearanceArmorColor.Metal1 || selectedColorChannel == ItemAppearanceArmorColor.Metal2)
              LoadPaletteBindings();
          }
        }
      }
    }
  }
}
