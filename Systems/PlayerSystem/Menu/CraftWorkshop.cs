using System;
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
      public class WorkshopWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChidren = new();
        private readonly NuiBind<string> search = new ("search");
        private readonly NuiBind<int> listCount = new ("listCount");
        private readonly NuiBind<string> icon = new ("icon"); // TODO : Utiliser l'icone d'un don correspondant à l'arme ?
        private readonly NuiBind<string> blueprintNames = new ("blueprintName");
        private readonly NuiBind<string> blueprintTEs = new ("blueprintTEs");
        private readonly NuiBind<string> blueprintMEs = new ("blueprintMEs");
        private readonly NuiBind<bool> enable = new ("enable");
        private readonly Color white = new(255, 255, 255);
        private readonly NuiRect drawListRect = new(0, 35, 150, 60);
        private string workshopTag;
        private IEnumerable<NwItem> blueprintList;
        private IEnumerable<NwItem> filteredList;

        public WorkshopWindow(Player player, string placeableTag) : base(player)
        {
          windowId = "craftWorkshop";

          List<NuiListTemplateCell> blueprintTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButtonImage(icon) { Tooltip = blueprintNames, Height = 40, Width = 40 }) { Width = 40 },
            new NuiListTemplateCell(new NuiLabel(blueprintMEs)
            {
              Tooltip = blueprintNames,
              DrawList = new List<NuiDrawListItem>() { new NuiDrawListText(white, drawListRect, blueprintTEs) }
            }) { Width = 160 },
            new NuiListTemplateCell(new NuiButton("Produire") { Id = "startCraft", Enabled = enable, Tooltip = "Entame une nouvelle production artisanale. Nécessite d'avoir au moins un niveau d'entrainement dans le métier artisanal correspondant.", Height = 40, Width = 90 }) { Width = 90 }
          };

          rootColumn.Children = rootChidren;

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 410 } } });
          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(blueprintTemplate, listCount) { RowHeight = 45 } } });

          CreateWindow(placeableTag);
        }

        public void CreateWindow(string placeableTag)
        {
          workshopTag = placeableTag;

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          window = new NuiWindow(rootColumn, "Production artisanale")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleWorkshopEvents;
            player.oid.OnServerSendArea += OnAreaChangeCloseWindow;

            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            blueprintList = player.oid.ControlledCreature.Inventory.Items.Where(i => i.Tag == "blueprint" && i.GetObjectVariable<LocalVariableString>("_CRAFT_WORKSHOP").Value == workshopTag);
            filteredList = blueprintList;
            LoadBlueprintList(filteredList);
          }

            
        }

        private void HandleWorkshopEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "craftList":

                  blueprintList = player.oid.ControlledCreature.Inventory.Items.Where(i => i.Tag == "blueprint" && i.GetObjectVariable<LocalVariableString>("_CRAFT_WORKSHOP").Value == workshopTag);
                  filteredList = blueprintList;
                  LoadBlueprintList(filteredList);
                  search.SetBindValue(player.oid, nuiToken.Token, "");
                  currentTab = Tab.Craft;

                  break;

                case "upgradeList":

                  blueprintList = player.oid.ControlledCreature.Inventory.Items.Where(i => i.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value == player.oid.ControlledCreature.OriginalName 
                    && i.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value < 8 && BaseItems2da.baseItemTable[(int)i.BaseItem.ItemType].workshop == workshopTag); 
                  
                  filteredList = blueprintList;
                  LoadUpgradableItemList(filteredList.ToList());
                  search.SetBindValue(player.oid, nuiToken.Token, "");
                  currentTab = Tab.Upgrade;

                  break;

                case "startCraft":
                  NwItem blueprint = filteredList.ElementAt(nuiEvent.ArrayIndex);

                  player.HandleCraftItemChecks(blueprint);
                  CloseWindow();
                  
                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "search":

                  string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
                  filteredList = string.IsNullOrEmpty(currentSearch) ? blueprintList.AsEnumerable() : filteredList.Where(s => s.Name.ToLower().Contains(currentSearch));

                  switch (currentTab)
                  {
                    case Tab.Craft:
                      LoadBlueprintList(filteredList);
                      break;
                    case Tab.Upgrade:
                      LoadUpgradableItemList(filteredList.ToList());
                      break;
                  }
                  

                  break;
              }
              break;
          }
        }
        private void LoadBlueprintList(IEnumerable<NwItem> blueprints)
        {
          List<string> blueprintNamesList = new ();
          List<string> iconList = new ();
          List<string> blueprintTEsList = new ();
          List<string> blueprintMEsList = new ();
          List<bool> enabledList = new ();

          foreach (NwItem item in blueprints)
          {
            int materiaCost = (int)(player.GetItemMateriaCost(item) * (1 - (item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value / 100)));
            TimeSpan jobDuration = TimeSpan.FromSeconds(player.GetItemCraftTime(item, materiaCost));

            CraftResource resource = player.craftResourceStock.FirstOrDefault(r => r.type == ItemUtils.GetResourceTypeFromBlueprint(item) && r.grade == 1);
            int availableQuantity = resource != null ? resource.quantity : 0;

            blueprintNamesList.Add(item.Name);
            iconList.Add(NwBaseItem.FromItemId(item.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value).WeaponFocusFeat.IconResRef);
            blueprintMEsList.Add($"Coût en {ItemUtils.GetResourceNameFromBlueprint(item)} : {materiaCost}/{availableQuantity}");
            blueprintTEsList.Add($"Temps de fabrication : {new TimeSpan(jobDuration.Days, jobDuration.Hours, jobDuration.Minutes, jobDuration.Seconds)}");
            enabledList.Add(player.learnableSkills.ContainsKey(player.GetJobLearnableFromWorkshop(workshopTag)) && player.craftJob == null && availableQuantity >= materiaCost);
          }

          blueprintNames.SetBindValues(player.oid, nuiToken.Token, blueprintNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, blueprintNamesList.Count);

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          blueprintMEs.SetBindValues(player.oid, nuiToken.Token, blueprintMEsList);
          blueprintTEs.SetBindValues(player.oid, nuiToken.Token, blueprintTEsList);
          enable.SetBindValues(player.oid, nuiToken.Token, enabledList);
        }
        private void LoadUpgradableItemList(List<NwItem> items)
        {
          List<string> itemNamesList = new();
          List<string> iconList = new();
          List<string> blueprintTEsList = new();
          List<string> blueprintMEsList = new();
          List<bool> enabledList = new();

          foreach (NwItem item in items)
          {
            NwItem bestBlueprint = player.oid.ControlledCreature.Inventory.Items
             .Where(i => i.Tag == "blueprint" && i.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value == (int)item.BaseItem.ItemType)
             .OrderByDescending(i => i.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value)
             .ThenByDescending(i => i.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value).FirstOrDefault();

            item.GetObjectVariable<LocalVariableObject<NwItem>>("_BEST_BLUEPRINT").Value = bestBlueprint;

            int grade = item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value;
            itemNamesList.Add($"{item.Name} - Qualité {grade} - Améliorer");
            iconList.Add(item.BaseItem.WeaponFocusFeat.IconResRef);

            if (bestBlueprint != null)
            {
              int materiaCost = (int)(player.GetItemMateriaCost(bestBlueprint, grade + 1) * (1 - (bestBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value / 100)));
              TimeSpan jobDuration = TimeSpan.FromSeconds(player.GetItemCraftTime(bestBlueprint, materiaCost));

              CraftResource resource = player.craftResourceStock.FirstOrDefault(r => r.type == ItemUtils.GetResourceTypeFromBlueprint(bestBlueprint) && r.grade == grade + 1);
              int availableQuantity = resource != null ? resource.quantity : 0;

              blueprintMEsList.Add($"{item.Name} - Coût en {ItemUtils.GetResourceNameFromBlueprint(bestBlueprint)} : {availableQuantity}/{materiaCost}");
              blueprintTEsList.Add($"Temps de fabrication : {new TimeSpan(jobDuration.Days, jobDuration.Hours, jobDuration.Minutes, jobDuration.Seconds)}");
              enabledList.Add(player.learnableSkills.ContainsKey(player.GetJobLearnableFromWorkshop(workshopTag)) && player.craftJob == null && availableQuantity >= materiaCost);
            }
            else
            {
              blueprintMEsList.Add("Amélioration possible");
              blueprintTEsList.Add("Avec le patron adéquat");
              enabledList.Add(false);
            }
          }

          blueprintNames.SetBindValues(player.oid, nuiToken.Token, itemNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, itemNamesList.Count);

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          blueprintMEs.SetBindValues(player.oid, nuiToken.Token, blueprintMEsList);
          blueprintTEs.SetBindValues(player.oid, nuiToken.Token, blueprintTEsList);
          enable.SetBindValues(player.oid, nuiToken.Token, enabledList);
        }
      }
    }
  }
}
