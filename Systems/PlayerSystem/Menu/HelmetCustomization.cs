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
      public class HelmetCustomizationWindow : PlayerWindow
      {
        private NwItem item { get; set; }

        private readonly NuiColumn rootColumn = new() { Margin = 0.0f };
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<string> name = new("name");
        private readonly NuiBind<int> helmetSelection = new("helmetSelection");
        private readonly NuiBind<string> helmetLeather1 = new("helmetLeather1");
        private readonly NuiBind<string> helmetCloth1 = new("helmetCloth1");
        private readonly NuiBind<string> helmetMetal1 = new("helmetMetal1");
        private readonly NuiBind<string> helmetLeather2 = new("helmetLeather2");
        private readonly NuiBind<string> helmetCloth2 = new("helmetCloth2");
        private readonly NuiBind<string> helmetMetal2 = new("helmetMetal2");

        private NuiBind<string> lastClickedColorButton;

        private ItemAppearanceArmorColor selectedColorChannel { get; set; }

        public HelmetCustomizationWindow(Player player, NwItem item) : base(player)
        {
          windowId = "helmetColorsModifier";
          rootColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiButton("Nom & Description") { Id = "loadItemNameEditor", Width = 150, Height = 50 } } });

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

          rootChildren.Add(new NuiRow()
          {
            Height = 20,
            Margin = 0.0f,
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Casque") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiSpacer()
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("<") { Id = "helmetDecrease", Height = 20, Width = 20, Margin = 0.0f },
              new NuiCombo(){ Entries = BaseItems2da.helmetModelEntries, Selected = helmetSelection, Height = 20, Width = 100, Margin = 0.0f },
              new NuiButton(">") { Id = "helmetIncrease", Height = 20, Width = 20, Margin = 0.0f },
              new NuiSpacer()
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Margin = 0.0f,
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("") { Id = "helmetLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuir 1", DrawList = new() { new NuiDrawListImage(helmetLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
              new NuiButton("") { Id = "helmetCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tissu 1", DrawList = new() { new NuiDrawListImage(helmetCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
              new NuiButton("") { Id = "helmetMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Métal 1", DrawList = new() { new NuiDrawListImage(helmetMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
              new NuiSpacer()
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Margin = 0.0f,
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("") { Id = "helmetLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuir 2", DrawList = new() { new NuiDrawListImage(helmetLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
              new NuiButton("") { Id = "helmetCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tissu 2", DrawList = new() { new NuiDrawListImage(helmetCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
              new NuiButton("") { Id = "helmetMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Métal 2", DrawList = new() { new NuiDrawListImage(helmetMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
              new NuiSpacer()
            } 
          });

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

            helmetLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Leather1) + 1}");
            helmetCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Cloth1) + 1}");
            helmetMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Metal1) + 1));
            helmetLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Leather2) + 1}");
            helmetCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Cloth2) + 1}");
            helmetMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Metal2) + 1));

            helmetSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetSimpleModel());
            helmetSelection.SetBindWatch(player.oid, nuiToken.Token, true);

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

          if (!item.IsValid && (item.RootPossessor != nuiEvent.Player.ControlledCreature || !player.IsDm()))
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
                case "helmetLeather1":
                  HandlePaletteSwap();
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = helmetLeather1;
                  return;

                case "helmetCloth1":
                  HandlePaletteSwap();
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = helmetCloth1;
                  return;

                case "helmetMetal1":
                  HandlePaletteSwap("metal");
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = helmetMetal1;
                  return;

                case "helmetLeather2":
                  HandlePaletteSwap();
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = helmetLeather2;
                  return;

                case "helmetCloth2":
                  HandlePaletteSwap();
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = helmetCloth2;
                  return;

                case "helmetMetal2":
                  HandlePaletteSwap("metal");
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = helmetMetal2;
                  return;

                case "helmetDecrease":
                  HandleArmorSelectorChange(-1);
                  return;

                case "helmetIncrease":
                  HandleArmorSelectorChange(1);
                  return;

                case "loadItemNameEditor":
                  if (!player.windows.ContainsKey("editorItemName")) player.windows.Add("editorItemName", new EditorItemName(player, item));
                  else ((EditorItemName)player.windows["editorItemName"]).CreateWindow(item);
                  return;
              }

              int resRef = int.Parse(nuiEvent.ElementId) + 1;

              if (lastClickedColorButton.Key.Contains("metal"))
                lastClickedColorButton.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(resRef));
              else
                lastClickedColorButton.SetBindValue(player.oid, nuiToken.Token, $"leather{resRef}");

              item.Appearance.SetArmorColor(selectedColorChannel, byte.Parse(nuiEvent.ElementId));

              NwItem newItem = item.Clone(nuiEvent.Player.ControlledCreature);
              nuiEvent.Player.ControlledCreature.RunEquip(newItem, InventorySlot.Head);
              item.Destroy();
              item = newItem;

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "helmetSelection":

                  item.Appearance.SetSimpleModel((byte)helmetSelection.GetBindValue(player.oid, nuiToken.Token));
                  NwItem newhelmet = item.Clone(nuiEvent.Player.ControlledCreature);
                  nuiEvent.Player.ControlledCreature.RunEquip(newhelmet, InventorySlot.Head);
                  item.Destroy();
                  item = newhelmet;

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
          helmetSelection.SetBindWatch(player.oid, nuiToken.Token, false);

          int newValue = BaseItems2da.helmetModelEntries.IndexOf(BaseItems2da.helmetModelEntries.FirstOrDefault(p => p.Value == helmetSelection.GetBindValue(player.oid, nuiToken.Token))) + change;

          if (newValue >= BaseItems2da.helmetModelEntries.Count)
            newValue = 0;

          if (newValue < 0)
            newValue = BaseItems2da.helmetModelEntries.Count - 1;

          helmetSelection.SetBindValue(player.oid, nuiToken.Token, BaseItems2da.helmetModelEntries[newValue].Value);

          item.Appearance.SetSimpleModel((byte)helmetSelection.GetBindValue(player.oid, nuiToken.Token));

          NwItem newhelmet = item.Clone(player.oid.ControlledCreature);
          player.oid.ControlledCreature.RunEquip(newhelmet, InventorySlot.Head);
          item.Destroy();
          item = newhelmet;

          helmetSelection.SetBindWatch(player.oid, nuiToken.Token, true);
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
