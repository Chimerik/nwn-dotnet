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
        NuiColumn rootColumn { get; }
        NuiColumn groupCol { get; }
        List<NuiElement> rowList { get; }
        Dictionary<int, NwItem> itemList { get; }

        public QuickLootWindow(Player player) : base(player)
        {
          windowId = "quickLoot";

          itemList = new Dictionary<int, NwItem>();

          rowList = new List<NuiElement>();
          groupCol = new NuiColumn() { Children = rowList };
          List<NuiElement> rootList = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootList };
          rootGroup = new NuiGroup() { Id = "quickLootGroup", Border = false, Padding = 0, Margin = 0, Layout = groupCol };
          rootList.Add(rootGroup);

          int i = 0;

          foreach(NwItem item in player.oid.ControlledCreature.Area.FindObjectsOfTypeInArea<NwItem>().Where(i => i.IsValid && i.DistanceSquared(player.oid.ControlledCreature) < 16 && i.GetObjectVariable<LocalVariableBool>($"{player.oid.PlayerName}_IGNORE_QUICKLOOT").HasNothing))
          {
            itemList.Add(i, item);

            rowList.Add(new NuiRow() 
            {
              Children = new List<NuiElement>
              {
                Utils.Util_GetIconResref(item, i),
                new NuiLabel("") { Id = $"examine_{i}", Width = 160, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Top, DrawList = DrawItemName(item.Name) },
                new NuiButton("Prendre") { Id = $"take_{i}", Height = 30, Width = 60 },
                new NuiButton("Voler") { Id = $"steal_{i}", Height = 30, Width = 60 },
                new NuiButtonImage("menu_exit") { Id = $"ignore_{i}", Height = 30, Width = 30, Tooltip = "Ignorer" }
              }
            });

            i++;
          }
          
          CreateWindow();
        }

        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) && player.windowRectangles[windowId].Width > 0 && player.windowRectangles[windowId].Width <= player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          window = new NuiWindow(rootColumn, "Rapide Butin")
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

          if (nuiEvent.ElementId.StartsWith("examine_") && nuiEvent.EventType == NuiEventType.MouseDown)
          {
            int itemId = int.Parse(nuiEvent.ElementId.Substring(nuiEvent.ElementId.IndexOf('_') + 1));
            nuiEvent.Player.ActionExamine(itemList[itemId]);
          }
          else if (nuiEvent.ElementId.StartsWith("take_") && nuiEvent.EventType == NuiEventType.Click)
          {
            int itemId = int.Parse(nuiEvent.ElementId.Substring(nuiEvent.ElementId.IndexOf('_') + 1));

            NwItem item = itemList[itemId];
            if (item.IsValid && item.Possessor is null)
            {
              item.Destroy();
              item.Clone(nuiEvent.Player.ControlledCreature);

              foreach (NwCreature nearbyPlayer in nuiEvent.Player.ControlledCreature.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.IsPlayerControlled && p.DistanceSquared(item) < 100))
                nearbyPlayer.ControllingPlayer.SendServerMessage($"{nuiEvent.Player.ControlledCreature.Name.ColorString(ColorConstants.White)} ramasse {item.Name.ColorString(ColorConstants.White)}.");
            }
          }
          else if (nuiEvent.ElementId.StartsWith("steal_") && nuiEvent.EventType == NuiEventType.Click)
          {
            int itemId = int.Parse(nuiEvent.ElementId.Substring(nuiEvent.ElementId.IndexOf('_') + 1));

            NwItem item = itemList[itemId];
            if (item.IsValid && item.Possessor is null)
            {
              item.Destroy();
              item.Clone(nuiEvent.Player.ControlledCreature);

              int stealerRoll = nuiEvent.Player.ControlledCreature.GetSkillRank(Skill.PickPocket) + NwRandom.Roll(Utils.random, 20);

              foreach (NwCreature nearbyPlayer in nuiEvent.Player.ControlledCreature.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.IsPlayerControlled && p.DistanceSquared(item) < 100))
              {
                int watcherRoll = nearbyPlayer.GetSkillRank(Skill.Spot) + NwRandom.Roll(Utils.random, 20);
                if(stealerRoll < watcherRoll)
                  nearbyPlayer.ControllingPlayer.SendServerMessage($"{nuiEvent.Player.ControlledCreature.Name.ColorString(ColorConstants.White)} ramasse {item.Name.ColorString(ColorConstants.White)}.");
              }
            }
          }
          else if (nuiEvent.ElementId.StartsWith("ignore_") && nuiEvent.EventType == NuiEventType.Click)
          {
            int itemId = int.Parse(nuiEvent.ElementId.Substring(nuiEvent.ElementId.IndexOf('_') + 1));

            NwItem item = itemList[itemId];
            if (item.IsValid)
              item.GetObjectVariable<LocalVariableBool>($"{nuiEvent.Player.PlayerName}_IGNORE_QUICKLOOT").Value = true;
          }
        }
        public void UpdateWindow()
        {
          rowList.Clear();
          itemList.Clear();

          int i = 0;

          foreach (NwItem item in player.oid.ControlledCreature.Area.FindObjectsOfTypeInArea<NwItem>().Where(it => it.IsValid && it.DistanceSquared(player.oid.ControlledCreature) < 16 && it.GetObjectVariable<LocalVariableBool>($"{player.oid.PlayerName}_IGNORE_QUICKLOOT").HasNothing))
          {
            itemList.Add(i, item);

            rowList.Add(new NuiRow()
            {
              Children = new List<NuiElement>
              {
                Utils.Util_GetIconResref(item, i),
                new NuiLabel("") { Id = $"examine_{i}", Width = 160, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Top, DrawList = DrawItemName(item.Name) },
                new NuiButton("Prendre") { Id = $"take_{i}", Height = 30, Width = 60 },
                new NuiButton("Voler") { Id = $"steal_{i}", Height = 30, Width = 60 },
                new NuiButtonImage("menu_exit") { Id = $"ignore_{i}", Height = 30, Width = 30, Tooltip = "Ignorer" }
              }
            });

            i++;
          }

          rootGroup.SetLayout(player.oid, token, groupCol);
        }

        private List<NuiDrawListItem> DrawItemName(string itemName)
        {
          NuiProperty<NuiColor> color = new NuiColor(255, 255, 255);
          List<NuiDrawListItem> textBreakerDrawList = new List<NuiDrawListItem>();
          int nbCharPerLine = 20;
          int i = 0;
          string currentLine;

          do
          {
            currentLine = itemName.Length > nbCharPerLine ? itemName.Substring(0, nbCharPerLine) : itemName;
            int breakPosition = currentLine.Length;

            if (itemName.Length > nbCharPerLine)
            {
              breakPosition = currentLine.Contains(" ") ? currentLine.LastIndexOf(" ") : currentLine.Length;
              currentLine = currentLine.Substring(0, breakPosition);
            }

            textBreakerDrawList.Add(new NuiDrawListText(color, new NuiRect(0, 5 + i * 20, 160, 20), currentLine) { Fill = true });

            itemName = itemName.Remove(0, breakPosition);

            i++;
          } while (itemName.Length > 1);

          return textBreakerDrawList;
        }
      }
    }
  }
}
