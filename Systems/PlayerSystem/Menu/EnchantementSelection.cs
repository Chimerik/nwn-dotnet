﻿using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class EnchantementSelectionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChidren = new();
        private readonly NuiBind<string> buttonText = new ("buttonText");
        private readonly NuiBind<int> listCount = new ("listCount");
        private NwSpell spell { get; set; }
        private NwItem itemTarget { get; set; }

        public EnchantementSelectionWindow(Player player, NwSpell spell, NwItem item) : base(player)
        {
          windowId = "enchantementSelection";
          rootColumn.Children = rootChidren;

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell> { new NuiListTemplateCell(new NuiButton(buttonText) { Id = "select", Height = 35 }) { Width = 400 } };
          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 35 });

          CreateWindow(spell, item);
        }
        public void CreateWindow(NwSpell spell, NwItem item)
        {
          this.spell = spell;
          this.itemTarget = item;
          List<string> enchantementList = new List<string>();

          foreach (ItemProperty ip in player.spellSystem.enchantementCategories[(int)spell.Id])
          {
            if (!ip.Property.ItemMap.IsItemPropertyValidForItem(item))
              continue;

            string ipName = $"{ip.Property.Name?.ToString()}";

            if (ip?.SubType?.RowIndex > -1)
              ipName += $" : {NwGameTables.ItemPropertyTable.GetRow(ip.Property.RowIndex).SubTypeTable?.GetRow(ip.SubType.RowIndex).Name?.ToString()}";

            ipName += " " + ip.CostTableValue?.Name?.ToString();
            ipName += " " + ip.Param1TableValue?.Name?.ToString();

            enchantementList.Add(ipName);
          }
          
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.4f);

          window = new NuiWindow(rootColumn, "Enchantement - Sélection")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleEnchantementSelectionEvents;

            buttonText.SetBindValues(player.oid, nuiToken.Token, enchantementList);
            listCount.SetBindValue(player.oid, nuiToken.Token, enchantementList.Count);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          } 
        }
        private void HandleEnchantementSelectionEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "select":

                  if(player.HandleEnchantementItemChecks(itemTarget, spell, player.spellSystem.enchantementCategories[(int)spell.Id][nuiEvent.ArrayIndex]))                        
                    CloseWindow();

                  break;

              }
              break;
          }
        }
      }
    }
  }
}
