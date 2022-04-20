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
      public class EnchantementSelectionWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChidren;
        private readonly NuiBind<string> buttonText = new ("buttonText");
        private readonly NuiBind<int> listCount = new ("listCount");
        private NwSpell spell { get; set; }
        private NwItem itemTarget { get; set; }

        public EnchantementSelectionWindow(Player player, NwSpell spell, NwItem item) : base(player)
        {
          windowId = "enchantementSelection";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell> { new NuiListTemplateCell(new NuiButton(buttonText) { Id = "select", Height = 35 }) { Width = 400 } };
          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 75 });

          CreateWindow(spell, item);
        }
        public void CreateWindow(NwSpell spell, NwItem item)
        {
          this.spell = spell;
          this.itemTarget = item;
          List<string> enchantementList = new List<string>();

          foreach (ItemProperty ip in player.spellSystem.enchantementCategories[(int)spell.Id])
            enchantementList.Add($"{ItemPropertyDefinition2da.ipDefinitionTable[(int)ip.PropertyType].name} - " +
              $"{ItemPropertyDefinition2da.GetSubTypeName(ip.PropertyType, ip.SubType)}");

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.4f);

          window = new NuiWindow(rootGroup, "Enchantement - Sélection")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleEnchantementSelectionEvents;
          player.oid.OnNuiEvent += HandleEnchantementSelectionEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          buttonText.SetBindValues(player.oid, token, enchantementList);
          listCount.SetBindValue(player.oid, token, enchantementList.Count);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
        private void HandleEnchantementSelectionEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "select":

                  HandleEnchantementChecks(player.spellSystem.enchantementCategories[(int)spell.Id][nuiEvent.ArrayIndex]);
                  CloseWindow();

                  break;

              }
              break;
          }
        }
        private void HandleEnchantementChecks(ItemProperty ip)
        {
          switch (ip.PropertyType)
          {
            case ItemPropertyType.AcBonus:
            case ItemPropertyType.AcBonusVsAlignmentGroup:
            case ItemPropertyType.AcBonusVsDamageType:
            case ItemPropertyType.AcBonusVsRacialGroup:
            case ItemPropertyType.AcBonusVsSpecificAlignment:

              if (itemTarget.BaseItem.ItemType != BaseItemType.Armor
                && itemTarget.BaseItem.ItemType != BaseItemType.Helmet
                && itemTarget.BaseItem.ItemType != BaseItemType.Cloak
                && itemTarget.BaseItem.ItemType != BaseItemType.Boots
                && itemTarget.BaseItem.ItemType != BaseItemType.Gloves
                && itemTarget.BaseItem.ItemType != BaseItemType.Bracer
                && itemTarget.BaseItem.ItemType != BaseItemType.LargeShield
                && itemTarget.BaseItem.ItemType != BaseItemType.TowerShield
                && itemTarget.BaseItem.ItemType != BaseItemType.SmallShield
                )

                player.oid.SendServerMessage("Ce type d'enchantement ne peut-être utilisé que sur une armure, un bouclier, un casque, une cape, des bottes ou des gants", ColorConstants.Red);

              return;

            case ItemPropertyType.AttackBonus:
            case ItemPropertyType.AttackBonusVsAlignmentGroup:
            case ItemPropertyType.AttackBonusVsRacialGroup:
            case ItemPropertyType.AttackBonusVsSpecificAlignment:
            case ItemPropertyType.EnhancementBonus:
            case ItemPropertyType.EnhancementBonusVsAlignmentGroup:
            case ItemPropertyType.EnhancementBonusVsRacialGroup:
            case ItemPropertyType.EnhancementBonusVsSpecificAlignment:

              ItemUtils.ItemCategory itemCategory = ItemUtils.GetItemCategory(itemTarget.BaseItem.ItemType);

              if (itemCategory != ItemUtils.ItemCategory.OneHandedMeleeWeapon
                && itemCategory != ItemUtils.ItemCategory.TwoHandedMeleeWeapon
                && itemCategory != ItemUtils.ItemCategory.RangedWeapon
                && itemTarget.BaseItem.ItemType != BaseItemType.Gloves)

                player.oid.SendServerMessage("Ce type d'enchantement ne peut-être utilisé que sur une arme.", ColorConstants.Red);

              return;
          }

          player.craftJob = new CraftJob(player, itemTarget, spell, ip, JobType.Enchantement);
        }
      }
    }
  }
}
