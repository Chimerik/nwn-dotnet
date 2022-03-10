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
      public class MainMenuWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChidren;

        public MainMenuWindow(Player player) : base(player)
        {
          windowId = "mainMenu";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          CreateWindow();
        }
        public void CreateWindow()
        {
          rootChidren.Clear();

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() 
          { 
            new NuiButton(player.oid.ControlledCreature.ActiveEffects.Any(e => e.EffectType == EffectType.CutsceneGhost) ? "Désactiver Mode Toucher" : "Activer Mode Toucher") 
            { Id = "touch", Tooltip = "Permet d'éviter les collisions entre personnages (non utilisable en combat)" } } 
          });

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() 
          { 
            new NuiButton(player.oid.ControlledCreature.GetObjectVariable<PersistentVariableInt>("_ALWAYS_WALK").HasNothing ? "Activer Mode Marche" : "Désactiver Mode Marche") 
            { Id = "walk", Tooltip = "Permet d'avoir l'air moins ridicule en ville." } } 
          });

          NwItem oHelmet = player.oid.ControlledCreature.GetItemInSlot(InventorySlot.Head);

          if (oHelmet != null)
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() 
            { 
              new NuiButton(oHelmet.HiddenWhenEquipped == 1 ? "Afficher casque" : "Ne pas afficher le casque") 
              { Id = "helm", Tooltip = "Considération purement esthétique afin de laisser le visage apparent." } } 
            });

          NwItem oCloak = player.oid.ControlledCreature.GetItemInSlot(InventorySlot.Cloak);

          if (oCloak != null)
            rootChidren.Add(new NuiRow()
            {
              Children = new List<NuiElement>()
            {
              new NuiButton(oCloak.HiddenWhenEquipped == 1 ? "Afficher cape" : "Ne pas afficher la cape")
              { Id = "cloak", Tooltip = "Considération purement esthétique." } }
            });

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.4f);

          window = new NuiWindow(rootGroup, "Menu principal")
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

                  HandleEnchantementChecks(SpellSystem.enchantementCategories[(int)spell.Id][nuiEvent.ArrayIndex]);
                  player.oid.NuiDestroy(token);

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
