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
        private readonly NuiColumn rootColumn;
        private readonly NuiGroup rootGroup;
        private readonly List<NuiElement> rootChidren = new List<NuiElement>();
        private readonly NuiBind<string> search = new NuiBind<string>("search");
        private readonly NuiBind<int> listCount = new NuiBind<int>("listCount");
        private readonly NuiBind<string> icon = new NuiBind<string>("icon"); // TODO : Utiliser l'icone d'un don correspondant à l'arme ?
        private readonly NuiBind<string> blueprintNames = new NuiBind<string>("blueprintName");
        private readonly NuiBind<string> blueprintTEs = new NuiBind<string>("blueprintTEs");
        private readonly NuiBind<string> blueprintMEs = new NuiBind<string>("blueprintMEs");
        private readonly NuiBind<bool> enable = new NuiBind<bool>("enable");
        private readonly NuiColor white = new NuiColor(255, 255, 255);
        private readonly NuiRect drawListRect = new NuiRect(0, 35, 150, 60);
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
              Width = 160, Tooltip = blueprintNames,
              DrawList = new List<NuiDrawListItem>() { new NuiDrawListText(white, drawListRect, blueprintTEs) }
            }) { Width = 160 },
            new NuiListTemplateCell(new NuiButton("Produire") { Id = "startCraft", Enabled = enable, Tooltip = "Entame une nouvelle production artisanale. Nécessite d'avoir au moins un niveau d'entrainement dans le métier artisanal correspondant.", Height = 40, Width = 90 }) { Width = 90 }
          };

          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "forgeGroup", Border = true, Layout = rootColumn };

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

          player.oid.OnNuiEvent -= HandleWorkshopEvents;
          player.oid.OnNuiEvent += HandleWorkshopEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          search.SetBindValue(player.oid, token, "");
          search.SetBindWatch(player.oid, token, true);
          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          blueprintList = player.oid.ControlledCreature.Inventory.Items.Where(i => i.Tag == "blueprint" && i.GetObjectVariable<LocalVariableString>("_CRAFT_WORKSHOP").Value == workshopTag);
          filteredList = blueprintList;
          LoadBlueprintList(filteredList);
        }

        private void HandleWorkshopEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "startCraft":
                  NwItem blueprint = filteredList.ElementAt(nuiEvent.ArrayIndex);

                  player.HandleCraftItemChecks(blueprint);
                  player.oid.NuiDestroy(token);
                  
                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "search":

                  string currentSearch = search.GetBindValue(player.oid, token).ToLower();
                  filteredList = blueprintList.AsEnumerable();

                  if (!string.IsNullOrEmpty(currentSearch))
                    filteredList = filteredList.Where(s => s.Name.ToLower().Contains(currentSearch));

                  LoadBlueprintList(filteredList);

                  break;
              }
              break;
          }
        }
        private void LoadBlueprintList(IEnumerable<NwItem> blueprints)
        {
          List<string> blueprintNamesList = new List<string>();
          List<string> iconList = new List<string>();
          List<string> blueprintTEsList = new List<string>();
          List<string> blueprintMEsList = new List<string>();
          List<bool> enabledList = new List<bool>();

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
            enabledList.Add(player.learnableSkills.ContainsKey(player.GetJobLearnableFromWorkshop(workshopTag)) && player.newCraftJob == null && availableQuantity >= materiaCost);
          }

          blueprintNames.SetBindValues(player.oid, token, blueprintNamesList);
          listCount.SetBindValue(player.oid, token, blueprintNamesList.Count);

          icon.SetBindValues(player.oid, token, iconList);
          blueprintMEs.SetBindValues(player.oid, token, blueprintMEsList);
          blueprintTEs.SetBindValues(player.oid, token, blueprintTEsList);
          enable.SetBindValues(player.oid, token, enabledList);
        }
      }
    }
  }
}
