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
      public class QuickLootWindow : PlayerWindow
      {
        NuiGroup rootGroup { get; }
        List<NuiElement> rowList { get; }

        public QuickLootWindow(Player player) : base(player)
        {
          windowId = "quickLoot";

          rowList = new List<NuiElement>();
          NuiColumn rootColumn = new NuiColumn() { Children = rowList };
          rootGroup = new NuiGroup() { Id = "quickLootGroup", Border = false, Padding = 0, Margin = 0, Children = new List<NuiElement> { rootColumn } };

          foreach(NwItem item in player.oid.ControlledCreature.Area.FindObjectsOfTypeInArea<NwItem>().Where(i => i.IsValid && i.DistanceSquared(player.oid.ControlledCreature) < 16 && i.GetObjectVariable<LocalVariableBool>($"{player.oid.PlayerName}_IGNORE_QUICKLOOT").HasNothing))
          {
            rowList.Add(new NuiRow() 
            {
              Children = new List<NuiElement>
              {
                Utils.Util_GetIconResref(item),
                new NuiLabel(item.Name) { HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Top, Margin = 10 },
                new NuiButton("Prendre") { Id = "take", Height = 30, Width = 60,  },
                new NuiButton("Voler") { Id = "steal", Height = 30, Width = 60 },
                new NuiButtonImage("menu_exit") { Id = "ignore", Height = 30, Width = 30, Tooltip = "Ignorer" }
              }
            });
          }
          
          CreateWindow();
        }

        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) && player.windowRectangles[windowId].Width > 0 && player.windowRectangles[windowId].Width <= player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          window = new NuiWindow(rootGroup, "")
          {
            Geometry = geometry,
            Resizable = resizable,
            Collapsed = false,
            Closable = closable,
            Transparent = true,
            Border = false,
          };

          player.oid.OnNuiEvent -= HandleQuickLootEvents;
          player.oid.OnNuiEvent += HandleQuickLootEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          resizable.SetBindValue(player.oid, token, true);
          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);
        }

        private void HandleQuickLootEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.ElementId)
          {
            case "examine":

              if (nuiEvent.EventType == NuiEventType.Click)
                nuiEvent.Player.ActionExamine(new NuiBind<uint>("item").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken).ToNwObject<NwItem>());

              break;

            case "take":

              if (nuiEvent.EventType == NuiEventType.Click)
              {
                NwItem item = new NuiBind<uint>("item").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken).ToNwObject<NwItem>();
                if (item.IsValid && item.Possessor is null)
                {
                  item.Destroy();
                  item.Clone(nuiEvent.Player.ControlledCreature);

                  foreach (NwCreature nearbyPlayer in nuiEvent.Player.ControlledCreature.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.IsPlayerControlled && p.DistanceSquared(item) < 25))
                    nearbyPlayer.ControllingPlayer.SendServerMessage($"{nuiEvent.Player.ControlledCreature.Name.ColorString(ColorConstants.White)} ramasse {item.Name.ColorString(ColorConstants.White)}.");
                }


              }

              break;

            case "ignore":

              if (nuiEvent.EventType == NuiEventType.Click)
              {
                NwItem item = new NuiBind<uint>("item").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken).ToNwObject<NwItem>();
                if (item.IsValid)
                {
                  item.GetObjectVariable<LocalVariableBool>($"{nuiEvent.Player.PlayerName}_IGNORE_QUICKLOOT").Value = true;
                }

              }

              break;
          }
        }
      }
    }
  }
}
