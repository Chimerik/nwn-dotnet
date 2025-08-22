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
        private readonly NuiBind<string> search = new("search");
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> blueprintNames = new("blueprintName");
        private readonly NuiBind<string> blueprintTEs = new("blueprintTEs");
        private readonly NuiBind<string> blueprintMEs = new("blueprintMEs");
        private readonly NuiBind<bool> enable = new("enable");
        private readonly Color white = new(255, 255, 255);
        private readonly NuiBind<NuiRect> drawListRect = new("drawListRect");
        private string workshopTag;
        private IEnumerable<NwItem> blueprintList;
        private IEnumerable<NwItem> filteredList;

        private Tab currentTab;
        private enum Tab
        {
          Craft,
          Upgrade,
          Repair,
          Reinforce,
          Recycle,
          Surcharge
        }

        NwItem tool;

        public WorkshopWindow(Player player, string placeableTag, NwItem tool) : base(player)
        {
          windowId = "craftWorkshop";

          List<NuiListTemplateCell> blueprintTemplate = new List<NuiListTemplateCell>
          {
            new(new NuiButtonImage(icon) {Id = "startCraft", Tooltip = blueprintNames, Enabled = enable, Height = 40 }) { Width = 40 },
            new(new NuiLabel(blueprintMEs)
            {
              Tooltip = blueprintNames,
              DrawList = new List<NuiDrawListItem>() { new NuiDrawListText(white, drawListRect, blueprintTEs) }
            }) { VariableSize = true },
          };

          rootColumn.Children = rootChidren;

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ife_opportunist") { Id = "craftList", Tooltip = "Création de nouveaux objets", Height = 35, Width = 35 },
            new NuiButtonImage("upgrade") { Id = "upgradeList", Tooltip = "Amélioration d'objets existants", Height = 35, Width = 35 },
            new NuiButtonImage("ife_layon") { Id = "repairList", Tooltip = "Réparer un objet abimé", Height = 35, Width = 35 },
            new NuiButtonImage("reinforce") { Id = "reinforceList", Tooltip = "Renforcer - Augmente la durabilité d'un objet", Height = 35, Width = 35 },
            new NuiButtonImage("ir_unequip") { Id = "recycleList", Tooltip = "Recycler", Height = 35, Width = 35 },
            new NuiButtonImage("overdrive") { Id = "overchargeList", Tooltip = "Surcharge arcanique - Augmente le nombre d'emplacements au risque de détruire l'objet", Height = 35, Width = 35 },
            new NuiButtonImage("beam") { Id = "extraction", Tooltip = "Lancer un job d'extraction passif", Height = 35, Width = 35 },
            new NuiSpacer()
          }
          });

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 410 } } });
          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(blueprintTemplate, listCount) { RowHeight = 40 } } });

          CreateWindow(placeableTag, tool);
        }

        public void CreateWindow(string placeableTag, NwItem tool)
        {
          workshopTag = placeableTag;
          this.tool = tool;

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, 400);

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

            drawListRect.SetBindValue(player.oid, nuiToken.Token, Utils.GetDrawListTextScaleFromPlayerUI(player));

            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            blueprintList = player.oid.ControlledCreature.Inventory.Items.Where(i => i.Tag == "blueprint" && i.GetObjectVariable<LocalVariableString>("_CRAFT_WORKSHOP").Value == workshopTag);
            filteredList = blueprintList;
            LoadBlueprintList(filteredList);

            currentTab = Tab.Craft;
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

                case "recycleList":

                  blueprintList = player.oid.ControlledCreature.Inventory.Items.Where(i => !i.PlotFlag && !i.CursedFlag);
                  filteredList = blueprintList;
                  LoadRecyclableItemList(filteredList);
                  search.SetBindValue(player.oid, nuiToken.Token, "");
                  currentTab = Tab.Recycle;

                  break;

                case "repairList":

                  blueprintList = player.oid.ControlledCreature.Inventory.Items.Where(i => !i.PlotFlag && !i.CursedFlag && i.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue && i.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value < i.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value);
                  filteredList = blueprintList;
                  LoadRepairableItemList(filteredList);
                  search.SetBindValue(player.oid, nuiToken.Token, "");
                  currentTab = Tab.Repair;

                  break;

                case "reinforceList":

                  blueprintList = player.oid.ControlledCreature.Inventory.Items.Where(i => !i.PlotFlag && !i.CursedFlag && i.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue && i.GetObjectVariable<LocalVariableInt>("_REINFORCEMENT_LEVEL").Value < 10);
                  filteredList = blueprintList;
                  LoadReinforcableItemList(filteredList);
                  search.SetBindValue(player.oid, nuiToken.Token, "");
                  currentTab = Tab.Reinforce;

                  break;

                case "overchargeList":

                  blueprintList = player.oid.ControlledCreature.Inventory.Items.Where(i => !i.PlotFlag && !i.CursedFlag && i.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").HasValue);
                  filteredList = blueprintList;
                  LoadOverchargableItemList(filteredList);
                  search.SetBindValue(player.oid, nuiToken.Token, "");
                  currentTab = Tab.Surcharge;

                  break;

                case "startCraft":

                  NwItem item = filteredList.ElementAt(nuiEvent.ArrayIndex);

                  switch (currentTab)
                  {
                    case Tab.Craft:
                      player.HandleCraftItemChecks(item, tool);
                      break;
                    case Tab.Upgrade:
                      player.HandleCraftItemChecks(item.GetObjectVariable<LocalVariableObject<NwItem>>("_BEST_BLUEPRINT").Value, tool, item);
                      break;
                    case Tab.Recycle:

                      if (player.craftJob != null) player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
                      else if(tool is null || tool.RootPossessor != player.oid.LoginCreature)
                      {
                        player.oid.SendServerMessage("L'outil que vous utilisez n'est plus valide. Veuillez en utiliser un autre.", ColorConstants.Red);
                      }
                      else
                      {
                        for (int i = 0; i < tool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
                        {
                          if (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
                            continue;

                          int inscriptionId = tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}");

                          if (inscriptionId >= CustomInscription.MateriaProductionDurabilityMinor && inscriptionId <= CustomInscription.MateriaProductionQualitySupreme)
                            break;

                          player.oid.SendServerMessage("L'outil utilisé pour votre travail ne dispose plus d'inscription permettant la manipulation de matéria, pensez à faire appliquer de nouvelles inscriptions !", ColorConstants.Red);
                          return;
                        }

                        player.craftJob = new(player, item, tool, JobType.Recycling);
                        ItemUtils.HandleCraftToolDurability(player, tool, CustomInscription.MateriaProductionDurability, CustomSkill.ArtisanPrudent);
                      }
                      
                      break;
                    case Tab.Repair:
                      player.HandleRepairItemChecks(item, tool);
                      break;
                    case Tab.Reinforce:

                      if (player.craftJob != null) player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
                      else if (tool is null || tool.RootPossessor != player.oid.LoginCreature)
                        player.oid.SendServerMessage("L'outil que vous utilisez actuellement n'est plus valide. Veuillez en utiliser un autre.", ColorConstants.Red);
                      else
                      {
                        for (int i = 0; i < tool.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
                        {
                          if (tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
                            continue;

                          int inscriptionId = tool.GetObjectVariable<LocalVariableInt>($"SLOT{i}");

                          if (inscriptionId >= CustomInscription.MateriaProductionDurabilityMinor && inscriptionId <= CustomInscription.MateriaProductionQualitySupreme)
                            break;

                          player.oid.SendServerMessage("L'outil utilisé pour votre travail ne dispose plus d'inscription permettant la manipulation de matéria, pensez à faire appliquer de nouvelles inscriptions !", ColorConstants.Red);
                          return;
                        }

                        player.craftJob = new(player, item, tool, JobType.Renforcement);
                        ItemUtils.HandleCraftToolDurability(player, tool, CustomInscription.MateriaProductionDurability, CustomSkill.ArtisanPrudent);
                      }

                      break;
                    case Tab.Surcharge:

                      int successChange = player.learnableSkills.ContainsKey(CustomSkill.CalligraphieSurcharge) ? player.learnableSkills[CustomSkill.CalligraphieSurcharge].totalPoints : 0;
                      int controlLevel = player.learnableSkills.ContainsKey(CustomSkill.CalligraphieSurchargeControlee) ? player.learnableSkills[CustomSkill.CalligraphieSurchargeControlee].totalPoints : 0;

                      int dice = NwRandom.Roll(Utils.random, 100);

                      if (dice <= 2 * successChange)
                      {
                        item.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
                        player.oid.SendServerMessage($"En forçant à l'aide de votre puissance brute, vous parvenez à ajouter un emplacement de sort supplémentaire à votre {StringUtils.ToWhitecolor(item.Name)} !", ColorConstants.Cyan);
                      }
                      else if (dice > 10 * controlLevel)
                      {
                        item.Destroy();
                        player.oid.SendServerMessage($"Vous forcez, forcez, et votre {StringUtils.ToWhitecolor(item.Name)} se brise sous l'excès infligé.", ColorConstants.Purple);
                      }

                      break;
                  }

                  CloseWindow();

                  break;

                case "extraction":

                  player.HandlePassiveJobChecks(workshopTag);
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
                    case Tab.Recycle:
                      LoadRecyclableItemList(filteredList);
                      break; 
                    case Tab.Repair:
                      LoadRepairableItemList(filteredList);
                      break;
                    case Tab.Reinforce:
                      LoadReinforcableItemList(filteredList);
                      break;
                    case Tab.Surcharge:
                      LoadOverchargableItemList(filteredList);
                      break;
                  }

                  break;
              }
              break;
          }
        }
        private void LoadBlueprintList(IEnumerable<NwItem> blueprints)
        {
          List<string> blueprintNamesList = new();
          List<string> iconList = new();
          List<string> blueprintTEsList = new();
          List<string> blueprintMEsList = new();
          List<bool> enabledList = new();

          foreach (NwItem item in blueprints)
          {
            int materiaCost = (int)(player.GetItemMateriaCost(item, tool) * (1 - (item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value / 100)));
            TimeSpan jobDuration = TimeSpan.FromSeconds(player.GetItemCraftTime(item, materiaCost, tool));

            CraftResource resource = player.craftResourceStock.FirstOrDefault(r => r.type == ResourceType.InfluxRaffine);
            int availableQuantity = resource != null ? resource.quantity : 0;

            blueprintNamesList.Add(item.Name + " - Commencer la fabrication");
            iconList.Add("ife_opportunist");
            blueprintMEsList.Add($"Coût en influx raffiné : {availableQuantity}/{materiaCost}");
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
            iconList.Add("upgrade");

            if (bestBlueprint != null)
            {
              int materiaCost = (int)(player.GetItemMateriaCost(bestBlueprint, tool, grade + 1) * (1 - (bestBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value / 100)));
              TimeSpan jobDuration = TimeSpan.FromSeconds(player.GetItemCraftTime(bestBlueprint, materiaCost, tool));

              CraftResource resource = player.craftResourceStock.FirstOrDefault(r => r.type == ResourceType.InfluxRaffine);
              int availableQuantity = resource != null ? resource.quantity : 0;

              blueprintMEsList.Add($"{item.Name} - Coût en influx raffiné : {availableQuantity}/{materiaCost}");
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
        private void LoadRepairableItemList(IEnumerable<NwItem> items)
        {
          List<string> itemNamesList = new();
          List<string> iconList = new();
          List<string> blueprintTEsList = new();
          List<string> blueprintMEsList = new();
          List<bool> enabledList = new();

          foreach (NwItem item in items)
          {
            int grade = item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").HasValue ? item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value : 1;
            int materiaCost = (int)player.GetItemRepairMateriaCost(item, tool);

            CraftResource resource = player.craftResourceStock.FirstOrDefault(r => r.type == ResourceType.InfluxRaffine && r.quantity >= materiaCost);
            int availableQuantity = resource != null ? resource.quantity : 0;

            itemNamesList.Add($"{item.Name} - Qualité {grade} - Réparé {item.GetObjectVariable<LocalVariableInt>("_DURABILITY_NB_REPAIRS").Value} fois");
            iconList.Add("ife_layon");

            TimeSpan jobDuration = TimeSpan.FromSeconds(player.GetItemRepairTime(item, materiaCost, tool));

            blueprintMEsList.Add($"{item.Name} - Coût en influx raffiné {grade} : {availableQuantity}/{materiaCost}");
            blueprintTEsList.Add($"Temps de réparation : {new TimeSpan(jobDuration.Days, jobDuration.Hours, jobDuration.Minutes, jobDuration.Seconds)}");
            enabledList.Add(player.learnableSkills.ContainsKey(CustomSkill.Repair) && player.craftJob == null && availableQuantity >= materiaCost);
          }

          blueprintNames.SetBindValues(player.oid, nuiToken.Token, itemNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, itemNamesList.Count);

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          blueprintMEs.SetBindValues(player.oid, nuiToken.Token, blueprintMEsList);
          blueprintTEs.SetBindValues(player.oid, nuiToken.Token, blueprintTEsList);
          enable.SetBindValues(player.oid, nuiToken.Token, enabledList);
        }
        private void LoadRecyclableItemList(IEnumerable<NwItem> items)
        {
          List<string> itemNamesList = new();
          List<string> iconList = new();
          List<string> blueprintTEsList = new();
          List<string> blueprintMEsList = new();
          List<bool> enabledList = new();

          foreach (NwItem item in items)
          {

            itemNamesList.Add($"{item.Name} - Recycler");
            iconList.Add("ir_unequip");

            TimeSpan jobDuration = TimeSpan.FromSeconds(player.GetItemRecycleTime(item, tool));

            blueprintMEsList.Add($"{item.Name} - Récupérable {(int)player.GetItemRecycleGain(item)}");
            blueprintTEsList.Add($"Temps de recyclage : {new TimeSpan(jobDuration.Days, jobDuration.Hours, jobDuration.Minutes, jobDuration.Seconds)}");
            enabledList.Add(player.learnableSkills.ContainsKey(CustomSkill.Recycler) && player.craftJob == null);
          }

          blueprintNames.SetBindValues(player.oid, nuiToken.Token, itemNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, itemNamesList.Count);

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          blueprintMEs.SetBindValues(player.oid, nuiToken.Token, blueprintMEsList);
          blueprintTEs.SetBindValues(player.oid, nuiToken.Token, blueprintTEsList);
          enable.SetBindValues(player.oid, nuiToken.Token, enabledList);
        }
        private void LoadReinforcableItemList(IEnumerable<NwItem> items)
        {
          List<string> itemNamesList = new();
          List<string> iconList = new();
          List<string> blueprintTEsList = new();
          List<string> blueprintMEsList = new();
          List<bool> enabledList = new();

          foreach (NwItem item in items)
          {
            if(item.BaseItem.EquipmentSlots == EquipmentSlots.None)
            {
              item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Delete();
              item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Delete();
              continue;
            }

            itemNamesList.Add($"{item.Name} - Renforcer");
            iconList.Add("reinforce");

            TimeSpan jobDuration = TimeSpan.FromSeconds(player.GetItemReinforcementTime(item, tool));

            blueprintMEsList.Add($"{item.Name} - Durabilité +5 %");
            blueprintTEsList.Add($"Temps de renforcement : {new TimeSpan(jobDuration.Days, jobDuration.Hours, jobDuration.Minutes, jobDuration.Seconds)}");
            enabledList.Add(player.learnableSkills.ContainsKey(CustomSkill.Renforcement) && player.craftJob == null);
          }

          blueprintNames.SetBindValues(player.oid, nuiToken.Token, itemNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, itemNamesList.Count);

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          blueprintMEs.SetBindValues(player.oid, nuiToken.Token, blueprintMEsList);
          blueprintTEs.SetBindValues(player.oid, nuiToken.Token, blueprintTEsList);
          enable.SetBindValues(player.oid, nuiToken.Token, enabledList);
        }
        private void LoadOverchargableItemList(IEnumerable<NwItem> items)
        {
          List<string> itemNamesList = new();
          List<string> iconList = new();
          List<string> blueprintTEsList = new();
          List<string> blueprintMEsList = new();
          List<bool> enabledList = new();

          foreach (NwItem item in items)
          {
            int successChance = player.learnableSkills.ContainsKey(CustomSkill.CalligraphieSurcharge) ? player.learnableSkills[CustomSkill.CalligraphieSurcharge].totalPoints : 0;
            int controlLevel = player.learnableSkills.ContainsKey(CustomSkill.CalligraphieSurchargeControlee) ? player.learnableSkills[CustomSkill.CalligraphieSurchargeControlee].totalPoints : 0;

            itemNamesList.Add($"{item.Name} - Surcharger");
            iconList.Add("overdrive");

            blueprintMEsList.Add($"{item.Name} - Emplacement +1");
            blueprintTEsList.Add($"Réussite : {successChance} % - Perte {100 - (10 * controlLevel + 2 * successChance)} %");
            enabledList.Add(player.learnableSkills.ContainsKey(CustomSkill.CalligraphieSurcharge));
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
