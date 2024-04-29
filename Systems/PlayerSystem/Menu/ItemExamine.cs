using System;
using System.Collections;
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
      public class ItemExamineWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();
        private readonly NuiGroup sequenceRegisterGroup;
        private readonly NuiRow sequenceRegisterRow;
        private readonly NuiBind<string> sequenceSaveButtonLabel = new("sequenceSaveButtonLabel");
        private readonly NuiBind<string> itemDescription = new("itemDescription");
        private readonly NuiBind<string> itemState = new("itemState");
        private readonly NuiBind<string> itemStateTooltip = new("itemStateTooltip");
        private readonly NuiBind<string> bpNBUseTooltip = new("bpNBUseTooltip");
        private readonly NuiBind<string> bpNBUse = new("bpNBUse");

        private readonly NuiBind<string> ipName = new("ipName");
        private readonly NuiBind<Color> ipColor = new("ipColor");
        private readonly NuiBind<int> listCount = new("listCount");

        private readonly NuiBind<string> hide = new("hide");
        private readonly NuiBind<string> hideTooltip = new("hideTooltip");
        private bool modificationAllowed { get; set; }
        private NwItem item { get; set; }

        public ItemExamineWindow(Player player, NwItem item) : base(player)
        {
          windowId = "itemExamine";

          rootColumn = new NuiColumn() { Children = rootChildren };
          rootGroup = new NuiGroup() { Id = "learnableGroup", Border = true, Layout = rootColumn };
          sequenceRegisterRow = new NuiRow();
          sequenceRegisterGroup = new NuiGroup() { Id = "sequencerGroup", Border = true, Layout = sequenceRegisterRow };

          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(ipName) { Tooltip = ipName, ForegroundColor = ipColor, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true });

          CreateWindow(item);
        }
        public void CreateWindow(NwItem item)
        {
          rootChildren.Clear();

          if (item == null || !item.IsValid)
            return;

          this.item = item;
          string originalCrafterName = item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value;
          modificationAllowed = (string.IsNullOrWhiteSpace(originalCrafterName) || originalCrafterName == player.oid.ControlledCreature.OriginalName)
            && (item.RootPossessor == player.oid.ControlledCreature || player.IsDm());

          //if(item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").HasValue && item.BaseItem.ItemType != BaseItemType.Amulet && item.BaseItem.ItemType != BaseItemType.Ring)
           // rootChildren.Add(new NuiLabel(GetItemRequirementText(item)) { Height = 30, Width = 580, Tooltip = GetItemRequirementTooltip(item), ForegroundColor = GetItemRequirementColor(item), HorizontalAlign = NuiHAlign.Left });

          NuiText descriptionWidget = new NuiText(itemDescription) { Height = 100, Width = 590 };
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { descriptionWidget } });

          if (!string.IsNullOrWhiteSpace(originalCrafterName))
            rootChildren.Add(new NuiLabel($"Artisan : {originalCrafterName}") { Height = 30, Width = 580, Tooltip = $"Il est indiqué : 'Pour toute modification sur mesure, vous adresser à {originalCrafterName}'", HorizontalAlign = NuiHAlign.Center });

          if (item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").HasValue)
          {
            List<NuiElement> enchantementSlotsRowChildren = new List<NuiElement>();
            NuiRow enchantementSlotsRow = new NuiRow() { Children = enchantementSlotsRowChildren };
            enchantementSlotsRowChildren.Add(new NuiSpacer());

            for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
            {
              bool isSlotUsed = item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasValue;
              NwSpell spell = NwSpell.FromSpellId(item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value);
              string icon = isSlotUsed ? spell.IconResRef : "empty_ench_slot";
              string id = isSlotUsed ? spell.Id.ToString() : "";
              string spellName = isSlotUsed ? spell.Name.ToString() : "Emplacement d'inscription libre";

              NuiButtonImage slotButton = new NuiButtonImage(icon) { Id = $"spell_{id}", Tooltip = spellName, Height = 40, Width = 40 };
              enchantementSlotsRowChildren.Add(slotButton);
            }

            enchantementSlotsRowChildren.Add(new NuiSpacer());
            rootChildren.Add(enchantementSlotsRow);
          }

          int materiaQuality = item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE");
          string materiaQualityIcon = GetMateriaQualityIcon(materiaQuality);
          string weight = item.Weight.ToString();
          string weightTooltip = weight;
          string windowName = item.Name;
          int nbIP = item.BaseItem.ItemType != BaseItemType.SpellScroll ? item.ItemProperties.Count() : 0;

          if (item.StackSize > 1)
          {
            weightTooltip += $"{item.BaseItem.Weight / 10} par unité";
            windowName += $" (x{item.StackSize})";
          }

          if (ItemUtils.IsWeapon(item.BaseItem) || item.BaseItem.ItemType == BaseItemType.Gloves || item.BaseItem.ItemType == BaseItemType.Bracer)
          {
            /*if((item.GetObjectVariable<LocalVariableInt>("_MIN_WEAPON_DAMAGE").HasNothing || item.GetObjectVariable<LocalVariableInt>("_MAX_WEAPON_DAMAGE").HasNothing))
            {
              if(ItemUtils.itemDamageDictionary.ContainsKey(item.BaseItem.ItemType))
              {
                int weaponGrade = item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE");
                item.GetObjectVariable<LocalVariableInt>("_MIN_WEAPON_DAMAGE").Value = ItemUtils.itemDamageDictionary[item.BaseItem.ItemType][weaponGrade, 0];
                item.GetObjectVariable<LocalVariableInt>("_MAX_WEAPON_DAMAGE").Value = ItemUtils.itemDamageDictionary[item.BaseItem.ItemType][weaponGrade, 1];
              }
              else
                Utils.LogMessageToDMs($"WARNING - WEAPON SYSTEM - ITEM TYPE {item.BaseItem.ItemType} ({(int)item.BaseItem.ItemType}) - NO BASE DAMAGE SET");
            }

            int minDamage = item.GetObjectVariable<LocalVariableInt>("_MIN_WEAPON_DAMAGE").Value;
            int maxDamage = item.GetObjectVariable<LocalVariableInt>("_MAX_WEAPON_DAMAGE").Value;
            double weaponMasteryLevel = player.GetWeaponMasteryLevel(item);
            int minSkillDamage = (int)(minDamage * weaponMasteryLevel);
            int maxSkillDamage = (int)(maxDamage * weaponMasteryLevel);

            if (minSkillDamage < 1)
              minSkillDamage = 1;

            if (maxSkillDamage < 1)
              maxSkillDamage = 1;*/

            rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() 
            {
              new NuiSpacer(),
              new NuiButtonImage("ir_powerattack") { Height = 35, Width = 35, Tooltip = "Dégats" },
              new NuiSpacer() { Width = 5 },
              new NuiLabel($"{item.BaseItem.NumDamageDice}d{item.BaseItem.DieToRoll}") { Height = 35, Width = 80, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle } ,
              new NuiSpacer(),
              new NuiButtonImage("ir_moreattacks") { Height = 35, Width = 35, Tooltip = "Critiques" },
              new NuiSpacer() { Width = 5 },
              new NuiButtonImage("ir_sell02") { Height = 35, Width = 35, Tooltip = "Type de dégâts" },
              new NuiSpacer() { Width = 5 },
              new NuiLabel(ItemUtils.DisplayDamageType(item)) { Height = 35, Width = 100, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
              new NuiSpacer(),
              new NuiButtonImage("weight") { Height = 35, Width = 35, Tooltip = "Poids" },
              new NuiSpacer() { Width = 5 },
              new NuiLabel(weight) { Tooltip = weightTooltip, Height = 35, Width = 50, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
              new NuiSpacer(),
              new NuiButtonImage(materiaQualityIcon) { Height = 35, Width = 35, Tooltip = $"Qualité de matéria : {materiaQuality}" },
              new NuiSpacer() { Width = 15 },
              new NuiSpacer(),
              new NuiButtonImage(itemState) { Id = "edit", Height = 35, Width = 35, Tooltip = itemStateTooltip },
              new NuiSpacer()
            } });

            if (nbIP > 0)
              rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 20, Height = nbIP < 10 ? nbIP * 25 : 250 } } });
          }
          else if (item.BaseItem.ItemType == BaseItemType.Armor)
          {
            var armorEntry = Armor2da.armorTable[item.BaseACValue];
           // string armorProficiency = $"{player.GetArmorProficiencyLevel(item.BaseACValue) * 10} %";

            rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() 
            {
              new NuiSpacer(),
              new NuiButtonImage("ir_guard") { Height = 35, Width = 35, Tooltip = "Armure" },
              new NuiSpacer() { Width = 5 },
             // new NuiLabel(armorProficiency) { Tooltip = $"Votre niveau d'entrainement actuel vous permet de bénéficier de {armorProficiency} des bonus de CA de cette armure.", Height = 35, Width = 50, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle } ,
              //new NuiSpacer(),
              new NuiButtonImage("ir_healme") { Height = 35, Width = 35, Tooltip = "Bonus de dextérité maximal" },
              new NuiSpacer() { Width = 5 },
              new NuiLabel($"{armorEntry.maxDex}") { Height = 35, Width = 35, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
              new NuiSpacer(),
              new NuiButtonImage("ief_slow") { Height = 35, Width = 35, Tooltip = "Pénalité d'armure" },
              new NuiSpacer() { Width = 5 },
              new NuiLabel(armorEntry.ACPenalty.ToString()) { Height = 35, Width = 35, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
              new NuiSpacer(),
              new NuiButtonImage("ief_sleep") { Height = 35, Width = 35, Tooltip = "Echec des sorts profanes" },
              new NuiSpacer() { Width = 5 },
              new NuiLabel(armorEntry.arcaneFailure.ToString()) { Height = 35, Width = 35, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
              new NuiSpacer(),
              new NuiButtonImage("weight") { Height = 35, Width = 35, Tooltip = "Poids" },
              new NuiSpacer() { Width = 5 },
              new NuiLabel(weight) { Tooltip = weightTooltip, Height = 35, Width = 50, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
              new NuiSpacer(),
              new NuiButtonImage(materiaQualityIcon) { Height = 35, Width = 35, Tooltip = $"Qualité de matéria : {materiaQuality}" },
              new NuiSpacer() { Width = 15 },
              new NuiSpacer(),
              new NuiButtonImage(itemState) { Id = "edit", Height = 35, Width = 35, Tooltip = itemStateTooltip },
              new NuiSpacer()
            } });

            if (nbIP > 0)
              rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 20, Height = nbIP < 10 ? nbIP * 25 : 250 } } });
          }
          else if (item.BaseItem.ItemType == BaseItemType.SmallShield || item.BaseItem.ItemType == BaseItemType.LargeShield || item.BaseItem.ItemType == BaseItemType.TowerShield)
          {
            //string armorProficiency = $"{player.GetShieldProficiencyLevel(item.BaseItem.ItemType) * 10} %";

            rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() 
            {
              new NuiSpacer(),
              new NuiButtonImage("ir_guard") { Height = 35, Width = 35, Tooltip = "Armure" },
              new NuiSpacer() { Width = 5 },
              //new NuiLabel(armorProficiency) { Tooltip = $"Votre niveau d'entrainement actuel vous permet de bénéficier de {armorProficiency} des bonus de CA de ce bouclier.", Height = 35, Width = 50, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle } ,
              //new NuiSpacer(),
              new NuiButtonImage("ief_slow") { Height = 35, Width = 35, Tooltip = "Pénalité d'armure" },
              new NuiSpacer() { Width = 5 },
              new NuiLabel(item.BaseItem.ArmorCheckPenalty.ToString()) { Height = 35, Width = 35, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
              new NuiSpacer(),
              new NuiButtonImage("ief_sleep") { Height = 35, Width = 35, Tooltip = "Echec des sorts profanes" },
              new NuiSpacer() { Width = 5 },
              new NuiLabel(item.BaseItem.ArcaneSpellFailure.ToString()) { Height = 35, Width = 35, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
              new NuiSpacer(),
              new NuiButtonImage("weight") { Height = 35, Width = 35, Tooltip = "Poids" },
              new NuiSpacer() { Width = 5 },
              new NuiLabel(weight) { Tooltip = weightTooltip, Height = 35, Width = 50, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
              new NuiSpacer(),
              new NuiButtonImage(materiaQualityIcon) { Height = 35, Width = 35, Tooltip = $"Qualité de matéria : {materiaQuality}" },
              new NuiSpacer() { Width = 15 },
              new NuiSpacer(),
              new NuiButtonImage(itemState) { Id = "edit", Height = 35, Width = 35, Tooltip = itemStateTooltip },
              new NuiSpacer()
            } });

            if (nbIP > 0)
              rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 20, Height = nbIP < 10 ? nbIP * 25 : 250 } } });
          }
          else
          {
            switch (item.Tag)
            {
              case "blueprint":

                int materielEfficiency = item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value;
                int mineralCost = (int)(player.GetItemMateriaCost(item) * (1 - (materielEfficiency / 100)));
                TimeSpan jobDuration = TimeSpan.FromSeconds(player.GetItemCraftTime(item, mineralCost));

                rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() 
                {
                  new NuiSpacer(),
                  new NuiButtonImage("s_mefficiency") { Id = "blueprintME", Height = 35, Width = 35, Tooltip = "Recherche en rendement matériel - Démarrer un travail de recherche" },
                  new NuiSpacer() { Width = 5 },
                  new NuiLabel(materielEfficiency.ToString()) { Height = 35, Width = 50, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer(),
                  new NuiButtonImage("brick_stack") { Height = 35, Width = 35, Tooltip = $"Coût initial en {ItemUtils.GetResourceNameFromBlueprint(item)}. Puis 10 % de moins par amélioration vers un matériau supérieur." },
                  new NuiSpacer() { Width = 5 },
                  new NuiLabel(mineralCost.ToString()) { Height = 35, Width = 50, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer(),
                  new NuiButtonImage("s_tefficiency") { Id = "blueprintTE", Height = 35, Width = 35, Tooltip = "Recherche en efficacité - Démarrer un travail de recherche" },
                  new NuiSpacer() { Width = 5 },
                  new NuiLabel(item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value.ToString()) { Height = 35, Width = 50, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer(),
                  new NuiButtonImage("clockwork") { Height = 35, Width = 35, Tooltip = "Temps de fabrication et d'amélioration" },
                  new NuiSpacer() { Width = 5 },
                  new NuiLabel($"{new TimeSpan(jobDuration.Days, jobDuration.Hours, jobDuration.Minutes, jobDuration.Seconds)}") { Height = 35, Width = 50, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer(),
                  new NuiButtonImage("gauge") { Id = "copy", Height = 35, Width = 35, Tooltip = bpNBUseTooltip },
                  new NuiSpacer() { Width = 5 },
                  new NuiLabel(bpNBUse) { Height = 35, Width = 50, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer(),
                  new NuiButtonImage("ir_examine") { Id = "edit", Height = 35, Width = 35, Tooltip = "Modifier", Visible = player.IsDm() },
                  new NuiSpacer()
                } });

                break;

              case "craft_resource":

                if (!Enum.TryParse(item.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value, out ResourceType resourceType))
                {
                  Utils.LogMessageToDMs($"ITEM EXAMINE - Impossible de parser en ressource de craft : {item.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value}");
                  return;
                }

                CraftResource resource = Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == resourceType);

                rootChildren.Add(new NuiRow() { Height = 20, Children = new List<NuiElement>() { new NuiLabel("Raffinage - Rendement détaillé :") }, Width = 590, });
                rootChildren.Add(new NuiRow() { Height = 20, Children = new List<NuiElement>() { new NuiLabel("    30 % (base fonderie Impériale)") }, Width = 590, });
                /*rootChildren.Add(new NuiRow() { Height = 20, Children = new List<NuiElement>() { new NuiLabel($"      x{(player.learnableSkills.ContainsKey(resource.reprocessingLearnable) ? 1.00 + 3 * player.learnableSkills[resource.reprocessingLearnable].totalPoints / 100 : 1.00)} (Raffinage)") }, Width = 590, });
                rootChildren.Add(new NuiRow() { Height = 20, Children = new List<NuiElement>() { new NuiLabel($"      x{(player.learnableSkills.ContainsKey(resource.reprocessingEfficiencyLearnable) ? 1.00 + 2 * player.learnableSkills[resource.reprocessingLearnable].totalPoints / 100 : 1.00)} (Raffinage Efficace)") }, Width = 590, });
                rootChildren.Add(new NuiRow() { Height = 20, Children = new List<NuiElement>() { new NuiLabel($"      x{(player.learnableSkills.ContainsKey(resource.reprocessingGradeLearnable) ? 1.00 + 2 * player.learnableSkills[resource.reprocessingGradeLearnable].totalPoints / 100 : 1.00)} (Raffinage Expert)") }, Width = 590, });
                rootChildren.Add(new NuiRow() { Height = 20, Children = new List<NuiElement>() { new NuiLabel($"      x{(player.learnableSkills.ContainsKey(CustomSkill.ConnectionsPromenade) ? 0.95 + player.learnableSkills[CustomSkill.ConnectionsPromenade].totalPoints / 100 : 0.95)} (Taxes de la Promenade)") }, Width = 590, });
                rootChildren.Add(new NuiRow() { Height = 20, Children = new List<NuiElement>() { new NuiSpacer() }, Width = 590, });
                */rootChildren.Add(new NuiRow() { Height = 20, Children = new List<NuiElement>() { new NuiLabel("Votre rendement net à la raffinerie :") }, Width = 590, });
                rootChildren.Add(new NuiRow() { Height = 20, Children = new List<NuiElement>() { new NuiLabel($"    {player.GetMateriaYieldFromResource(item.StackSize, resource)}") }, Width = 590, });
                rootChildren.Add(new NuiRow()
                {
                  Height = 35,
                  Width = 590,
                  Visible = player.IsDm(),
                  Children = new List<NuiElement>() {
                  new NuiSpacer(),
                  new NuiButtonImage("ir_examine") { Id = "edit", Height = 30, Width = 30, Tooltip = "Modifier" },
                  new NuiSpacer()
                } });

                break;

              case "sequence_register":
                CreateSequenceRegisterLayout(item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value.Split("_"));
                rootChildren.Add(sequenceRegisterGroup);
                break;

              case "skillbook":

                bool canLearn = !player.learnableSkills.ContainsKey(item.GetObjectVariable<LocalVariableInt>("_SKILL_ID").Value);
                
                rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() 
                {
                  new NuiSpacer(),
                  new NuiButtonImage("ir_wizard") { Id = "skillbook_learn", Enabled = canLearn, Tooltip = canLearn ? "Ajouter cette compétence à votre livre d'apprentissage" : "Cette compétence se trouve déjà dans votre livre d'apprentissage", Height = 35, Width = 35 },
                  new NuiSpacer(),
                  new NuiButtonImage("ir_examine") { Id = "edit", Height = 35, Width = 35, Tooltip = "Modifier" },
                  new NuiSpacer()
                } });

                break;

              case "missive": descriptionWidget.Height = 360; break;

              default:

                rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() 
                {
                  new NuiSpacer(),
                  new NuiButtonImage("weight") { Height = 35, Width = 35, Tooltip = "Poids" },
                  new NuiSpacer() { Width = 5 },
                  new NuiLabel(weight) { Tooltip = weightTooltip, Height = 35, Width = 50, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle } ,
                  new NuiSpacer(),
                  new NuiButtonImage(materiaQualityIcon) { Height = 35, Width = 35, Tooltip = $"Qualité de matéria : {materiaQuality}" },
                  new NuiSpacer() { Width = 15 },
                  new NuiSpacer(),
                  new NuiButtonImage(itemState) { Id = "edit", Height = 35, Width = 35, Tooltip = itemStateTooltip },
                  new NuiSpacer(),
                  new NuiButtonImage(hide) { Id = "hide", Tooltip = hideTooltip, Height = 35, Width = 35, Visible = item.BaseItem.ItemType == BaseItemType.Helmet || item.BaseItem.ItemType == BaseItemType.Cloak },
                  new NuiSpacer()
                } });

                if (nbIP > 0)
                  rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 20, Height = nbIP < 10 ? nbIP * 25 : 250 } } });

                break;
            }
          }

          // TODO : Si DM, bouton vers la modification d'objet spéciale ?

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? new NuiRect(player.windowRectangles[windowId].X, player.windowRectangles[windowId].Y, 620, 400) : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 620, 400);

          window = new NuiWindow(rootGroup, windowName)
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
            nuiToken.OnNuiEvent += HandleItemExamineEvents;

            sequenceSaveButtonLabel.SetBindValue(player.oid, nuiToken.Token, "Continuer la séquence");
            itemDescription.SetBindValue(player.oid, nuiToken.Token, item.Description);

            int runs = item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value;
            bpNBUse.SetBindValue(player.oid, nuiToken.Token, runs > 0 ? runs.ToString() : "");
            bpNBUseTooltip.SetBindValue(player.oid, nuiToken.Token, runs > 0 ? "Licence(s) restante(s)" : "Patron original - Faire une copie");

            hide.SetBindValue(player.oid, nuiToken.Token, item.HiddenWhenEquipped == 0 ? "ief_concealed" : "ief_fatigue");
            hideTooltip.SetBindValue(player.oid, nuiToken.Token, item.HiddenWhenEquipped == 0 ? "Cacher (Considération purement esthétique permettant de ne pas afficher cet objet lorsque le personnage le porte)" : "Afficher (considération purement esthétique)");

            LoadStateBindings();
            LoadIPBindings();

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void CloseItemWindow()
        {
          player.oid.LoginCreature.OnSpellAction -= RegisterSpellSequence;
          CloseWindow();
        }
        private void HandleItemExamineEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (item == null)
          {
            CloseItemWindow();
            return;
          }

          if (nuiEvent.EventType == NuiEventType.Close)
          {
            player.oid.LoginCreature.OnSpellAction -= RegisterSpellSequence;
            return;
          }

          if (nuiEvent.EventType == NuiEventType.Click)
          {
            if (item.RootPossessor != player.oid.ControlledCreature && !player.IsDm())
            {
              ((ItemExamineWindow)player.windows["itemExamine"]).CreateWindow(item);
              player.oid.SendServerMessage($"L'objet doit être en votre possession afin de permettre ce type d'action !", ColorConstants.Red);
              CloseItemWindow();
              return;
            }

            switch (nuiEvent.ElementId)
            {
              case "edit":
                CloseWindow();

                if (player.IsDm())
                {
                  if (!player.windows.ContainsKey("editorItem")) player.windows.Add("editorItem", new EditorItemWindow(player, item));
                  else ((EditorItemWindow)player.windows["editorItem"]).CreateWindow(item);
                }
                else if (modificationAllowed)
                  ItemUtils.OpenItemCustomizationWindow(item, player);
                else
                  player.oid.SendServerMessage("Seul l'artisan d'origine peut modifier l'apparence de cet objet !", ColorConstants.Orange);

                return;

              case "add":

                if (sequenceSaveButtonLabel.GetBindValue(player.oid, nuiToken.Token) == "Enregistrer")
                {
                  sequenceSaveButtonLabel.SetBindValue(player.oid, nuiToken.Token, "Continuer la séquence");
                  player.oid.LoginCreature.OnSpellAction -= RegisterSpellSequence;
                  player.oid.SendServerMessage("La séquence a bien été enregistrée.", ColorConstants.Orange);
                }
                else
                {
                  sequenceSaveButtonLabel.SetBindValue(player.oid, nuiToken.Token, "Enregistrer");
                  player.oid.LoginCreature.OnSpellAction -= RegisterSpellSequence;
                  player.oid.LoginCreature.OnSpellAction += RegisterSpellSequence;
                  player.oid.SendServerMessage("Les sorts que vous lancerez seront ajoutés à la séquence en cours.", ColorConstants.Orange);
                }
                return;

              case "reinit":
                item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Delete();
                CreateSequenceRegisterLayout(new List<string>());
                return;

              case "copy":

                if (!CanStartBlueprintJob(CustomSkill.BlueprintCopy))
                  return;

                player.craftJob = new CraftJob(player, item, JobType.BlueprintCopy);
                CloseWindow();

                if (!player.windows.ContainsKey("activeCraftJob")) player.windows.Add("activeCraftJob", new ActiveCraftJobWindow(player));
                else ((ActiveCraftJobWindow)player.windows["activeCraftJob"]).CreateWindow();

                break;

              case "blueprintME":

                if (!CanStartBlueprintJob(CustomSkill.BlueprintMetallurgy))
                  return;

                player.craftJob = new CraftJob(player, item, JobType.BlueprintResearchMaterialEfficiency);
                CloseWindow();

                if (!player.windows.ContainsKey("activeCraftJob")) player.windows.Add("activeCraftJob", new ActiveCraftJobWindow(player));
                else ((ActiveCraftJobWindow)player.windows["activeCraftJob"]).CreateWindow();

                return;

              case "blueprintTE":

                if (!CanStartBlueprintJob(CustomSkill.BlueprintResearch))
                  return;

                if(!player.learnableSkills.ContainsKey(CustomSkill.BlueprintResearch))
                {
                  player.oid.SendServerMessage("Il est tout d'abord nécessaire de maîtriser les bases de la recherche avant de pouvoir lancer cette étude !");
                  return;
                }

                player.craftJob = new CraftJob(player, item, JobType.BlueprintResearchTimeEfficiency);
                CloseWindow();

                if (!player.windows.ContainsKey("activeCraftJob")) player.windows.Add("activeCraftJob", new ActiveCraftJobWindow(player));
                else ((ActiveCraftJobWindow)player.windows["activeCraftJob"]).CreateWindow();

                return;

              case "skillbook_learn":

                int learnableId = item.GetObjectVariable<LocalVariableInt>("_SKILL_ID").Value;
                LearnableSkill learnable = player.ApplyLearningDiscout(new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[learnableId], player));
                player.learnableSkills.Add(learnableId, learnable);
                player.oid.SendServerMessage("Vous venez d'ajouter une nouvelle compétence à votre livre d'apprentissage !", ColorConstants.Rose);

                CloseWindow();
                item.Destroy();

                if (player.TryGetOpenedWindow("learnables", out PlayerWindow window))
                  ((LearnableWindow)window).HandleLearnableSearch();

                return;

              case "hide":

                if (item.HiddenWhenEquipped == 0)
                  item.HiddenWhenEquipped = 1;
                else
                  item.HiddenWhenEquipped = 0;

                hide.SetBindValue(player.oid, nuiToken.Token, item.HiddenWhenEquipped == 0 ? "ief_concealed" : "ief_fatigue");
                hideTooltip.SetBindValue(player.oid, nuiToken.Token, item.HiddenWhenEquipped == 0 ? "Cacher (Considération purement esthétique permettant de ne pas afficher cet objet lorsque le personnage le porte)" : "Afficher (considération purement esthétique)");

                return;
            }

            if (nuiEvent.ElementId.StartsWith("up_"))
            {
              int sequenceId = int.Parse(nuiEvent.ElementId.Split("_")[1]);
              if (sequenceId < 1)
                return;

              SequenceRegisterSwap(sequenceId, -1, item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value.Split("_"));
            }
            else if (nuiEvent.ElementId.StartsWith("down_"))
            {
              string[] spellList = item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value.Split("_");
              int sequenceId = int.Parse(nuiEvent.ElementId.Split("_")[1]);
              if (sequenceId >= spellList.Length - 1)
                return;

              SequenceRegisterSwap(sequenceId, -1, spellList);
            }
            else if (nuiEvent.ElementId.StartsWith("delete_"))
            {
              List<string> spellList = item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value.Split("_").ToList();
              int sequenceId = int.Parse(nuiEvent.ElementId.Split("_")[1]);
              spellList.RemoveAt(sequenceId);
              UpdateSequenceAndLayout(spellList);
            }
            else if (nuiEvent.ElementId.StartsWith("spell_"))
            {
              if (int.TryParse(nuiEvent.ElementId.Split("_")[1], out int spellId))
              {
                NwSpell spell = NwSpell.FromSpellId(int.Parse(nuiEvent.ElementId.Split("_")[1]));

                if (!player.windows.ContainsKey("spellDescription")) player.windows.Add("spellDescription", new SpellDescriptionWindow(player, spell));
                else ((SpellDescriptionWindow)player.windows["spellDescription"]).CreateWindow(spell);
              }
            }
          }
        }
        private void CreateSequenceRegisterLayout(IEnumerable spellList)
        {
          List<NuiElement> sequencerRegisterRowChildren = new();
          sequenceRegisterRow.Children = sequencerRegisterRowChildren;
          int i = 0;

          foreach (string spellId in spellList)
          {
            NwSpell spell = NwSpell.FromSpellId(int.Parse(spellId));
            sequencerRegisterRowChildren.Add(new NuiImage(spell.IconResRef) { Tooltip = spell.Name.ToString() });

            if (item.RootPossessor == player.oid.ControlledCreature)
            {
              sequencerRegisterRowChildren.Add(new NuiButtonImage("menu_up") { Id = $"up_{i}", Tooltip = "Déplacer vers le haut" });
              sequencerRegisterRowChildren.Add(new NuiButtonImage("menu_down") { Id = $"down_{i}", Tooltip = "Déplacer vers le bas" });
              sequencerRegisterRowChildren.Add(new NuiButtonImage("menu_exit") { Id = $"delete_{i}", Tooltip = "Supprimer" });
            }

            i++;
          }

          if (item.RootPossessor == player.oid.ControlledCreature)
          {
            sequencerRegisterRowChildren.Add(new NuiButton(sequenceSaveButtonLabel) { Id = "add", Tooltip = "Les sorts seront ajoutés à la fin de la séquence existante" });
            sequencerRegisterRowChildren.Add(new NuiButton("Effacer") { Id = "reinit", Tooltip = "La séquence actuellement enregistrée sera réinitialisée" });
          }
        }
        private void SequenceRegisterSwap(int sequenceId, int swapValue, string[] spellList)
        {
          (spellList[sequenceId + swapValue], spellList[sequenceId]) = (spellList[sequenceId], spellList[sequenceId + swapValue]);
          UpdateSequenceAndLayout(spellList);
        }
        private void UpdateSequenceAndLayout(IEnumerable spellList)
        {
          string sequence = "";

          foreach (string spell in spellList)
            sequence += $"{spell}_";

          item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value = sequence.Remove(sequence.Length - 1);

          CreateSequenceRegisterLayout(spellList);
          sequenceRegisterGroup.SetLayout(player.oid, nuiToken.Token, sequenceRegisterRow);
        }
        private void RegisterSpellSequence(OnSpellAction onSpellAction)
        {
          onSpellAction.PreventSpellCast = true;

          if (item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").HasNothing)
            item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value = ((int)onSpellAction.Spell.SpellType).ToString();
          else
            item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value += $"_{((int)onSpellAction.Spell.SpellType)}";

          CreateSequenceRegisterLayout(item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value.Split("_"));
          sequenceRegisterGroup.SetLayout(player.oid, nuiToken.Token, sequenceRegisterRow);
        }
        private string GetMateriaQualityIcon(int materiaQuality)
        {
          return materiaQuality switch
          {
            1 => "ir_level1",
            2 => "ir_level2",
            3 => "ir_level3",
            4 => "ir_level4",
            5 => "ir_level5",
            6 => "ir_level6",
            7 => "ir_level789",
            8 or 9 => "ir_cntrspell",
            _ => "ir_cantrips",
          };
        }
        private void LoadIPBindings()
        {
          if (item.BaseItem.ItemType == BaseItemType.SpellScroll) // On affiche pas les propriétés des parchemins
            return;

          List<string> ipNameList = new();
          List<Color> ipColorList = new();

          GetItemProperties(ipNameList, ipColorList);

          if (item.GetObjectVariable<LocalVariableInt>("_ARME_LIEE").HasValue)
          {
            ipNameList.Add("Arme Liée");
            ipColorList.Add(ColorConstants.Blue);
          }

          foreach (ItemProperty ip in item.ItemProperties)
          {
            string ipName = $"{ip.Property.Name?.ToString()}";

            if (ip?.SubType?.RowIndex > -1)
              ipName += $" : {NwGameTables.ItemPropertyTable.GetRow(ip.Property.RowIndex).SubTypeTable?.GetRow(ip.SubType.RowIndex).Name?.ToString()}";

            ipName += " " + ip.CostTableValue?.Name?.ToString();

            if (ip.Property.PropertyType == ItemPropertyType.OnHitProperties && ip.SubType.RowIndex == 18) // OnHit, Ability Drain
              ipName += " " + NwGameTables.ItemPropertyTable.GetRow(0)?.SubTypeTable.GetRow(ip.IntParams[5]).Name?.ToString();

            ipName += " " + ip.Param1TableValue?.Name?.ToString();
            ipName += ip.RemainingDuration > TimeSpan.Zero ? $" ({new TimeSpan(ip.RemainingDuration.Days, ip.RemainingDuration.Hours, ip.RemainingDuration.Minutes, ip.RemainingDuration.Seconds)})" : "";

            ipNameList.Add(ipName);
            ipColorList.Add(ip.DurationType == EffectDuration.Permanent ? ColorConstants.White : ColorConstants.Blue);
          }

          if(item.GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).HasValue)
          {
            ipNameList.Add("Finesse");
            ipColorList.Add(ColorConstants.White);
          }

          ipName.SetBindValues(player.oid, nuiToken.Token, ipNameList);
          ipColor.SetBindValues(player.oid, nuiToken.Token, ipColorList);
          listCount.SetBindValue(player.oid, nuiToken.Token, ipNameList.Count);
        }
        private void LoadStateBindings()
        {
          if (item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue)
          {
            int durabilityState = (int)((double)item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value / (double)item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value * 100);
            string tooltip = "Etat : ";

            if (durabilityState >= 100)
            {
              tooltip += "flambant neuf ";
              itemState.SetBindValue(player.oid, nuiToken.Token, "is_perfect");
            }
            if (durabilityState < 100 && durabilityState >= 75)
            {
              tooltip += "très bon ";
              itemState.SetBindValue(player.oid, nuiToken.Token, "is_vgood");
            }
            else if (durabilityState < 75 && durabilityState >= 50)
            {
              tooltip += "bon ";
              itemState.SetBindValue(player.oid, nuiToken.Token, "is_good");
            }
            else if (durabilityState < 50 && durabilityState >= 25)
            {
              tooltip += "usé ";
              itemState.SetBindValue(player.oid, nuiToken.Token, "is_bad");
            }
            else if (durabilityState < 25 && durabilityState >= 5)
            {
              tooltip += "abimé ";
              itemState.SetBindValue(player.oid, nuiToken.Token, "is_vbad");
            }
            else if (durabilityState < 5 && durabilityState >= 1)
            {
              tooltip += "vétuste ";
              itemState.SetBindValue(player.oid, nuiToken.Token, "is_awful");
            }
            else if (durabilityState < 1)
            {
              tooltip += "ruiné ";
              itemState.SetBindValue(player.oid, nuiToken.Token, "is_ruined");
            }

            if (item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").HasValue)
              tooltip += $"- Artisan original : {item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value} ";

            if (modificationAllowed)
              tooltip += "- Modifier";

            itemStateTooltip.SetBindValue(player.oid, nuiToken.Token, tooltip);
          }
          else
          {
            string tooltip = "Etat : Flambant neuf ";

            if (item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").HasValue)
              tooltip += $"- Artisan original : {item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value} ";

            if (modificationAllowed)
              tooltip += "- Modifier";

            itemState.SetBindValue(player.oid, nuiToken.Token, "is_perfect");
            itemStateTooltip.SetBindValue(player.oid, nuiToken.Token, tooltip);
          }
        }
        private bool CanStartBlueprintJob(int skill)
        {
          if (item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").HasValue)
          {
            player.oid.SendServerMessage("Seuls les patrons originaux peuvent faire l'objet d'une recherche ou d'une copie de licence.", ColorConstants.Red);
            return false;
          }

          if (item.RootPossessor != player.oid.ControlledCreature)
          {
            player.oid.SendServerMessage($"{item.Name} doit être en votre possession afin de pouvoir commencer un travail de copie.", ColorConstants.Red);
            return false;
          }

          if (player.craftJob != null)
          {
            player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
            return false;
          }

          if ((skill == CustomSkill.BlueprintMetallurgy && item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value > 9)
            || (skill == CustomSkill.BlueprintResearch && item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value > 9))
          {
            player.oid.SendServerMessage("Il n'est pas possible d'améliorer davantage le niveau de recherche de ce patron.", ColorConstants.Red);
            return false;
          }

          if (!player.learnableSkills.ContainsKey(skill) || player.learnableSkills[skill].totalPoints < 1)
          {
            player.oid.SendServerMessage("Il faut avoir étudié les techniques propre à ce métier avant de pouvoir se lancer dans un travail.", ColorConstants.Red);
            return false;
          }

          return true;
        }
        private void GetItemProperties(List<string> ipNames, List<Color> ipColors)
        {
          switch (item.BaseItem.ItemType)
          {
            case BaseItemType.Armor:
            case BaseItemType.Helmet:
            case BaseItemType.Cloak:
            case BaseItemType.Boots:
            case BaseItemType.Gloves:
            case BaseItemType.Bracer:
            case BaseItemType.Belt:
              ItemUtils.GetArmorProperties(item, ipNames, ipColors);
              return;

            case BaseItemType.SmallShield:
            case BaseItemType.LargeShield:
            case BaseItemType.TowerShield:
              ItemUtils.GetShieldProperties(item, ipNames, ipColors);
              return;

            case BaseItemType.Amulet:
            case BaseItemType.Ring:
              ItemUtils.GetJewelProperties(item, ipNames, ipColors);
              return;
          }

          if (item.GetObjectVariable<LocalVariableInt>("_MIN_WEAPON_DAMAGE").HasValue)
            ItemUtils.GetWeaponProperties(item, ipNames, ipColors);
        }
        /*private string GetItemRequirementTooltip(NwItem item)
        {
          int proficiencyLevel = 0;

          switch(item.BaseItem.ItemType)
          {
            case BaseItemType.Armor: proficiencyLevel = player.GetArmorProficiencyLevel(item.BaseItem.BaseAC); break;
            case BaseItemType.SmallShield:
            case BaseItemType.LargeShield:
            case BaseItemType.TowerShield: proficiencyLevel = player.GetShieldProficiencyLevel(item.BaseItem.ItemType); break;
            default: proficiencyLevel = player.GetWeaponMasteryLevel(item); break; ;
          }

          return proficiencyLevel < ItemUtils.GetItemProficiencyRequirement(item) ? "Maîtrise insuffisante, malus 70 % appliqué" : "";
        }
        private Color GetItemRequirementColor(NwItem item)
        {
          int proficiencyLevel = 0;

          switch (item.BaseItem.ItemType)
          {
            case BaseItemType.Armor: proficiencyLevel = player.GetArmorProficiencyLevel(item.BaseItem.BaseAC); break;
            case BaseItemType.SmallShield:
            case BaseItemType.LargeShield:
            case BaseItemType.TowerShield: proficiencyLevel = player.GetShieldProficiencyLevel(item.BaseItem.ItemType); break;
            default: proficiencyLevel = player.GetWeaponMasteryLevel(item); break; ;
          }

          return proficiencyLevel < ItemUtils.GetItemProficiencyRequirement(item) ? ColorConstants.Red : ColorConstants.Blue;
        }
        private string GetItemRequirementText(NwItem item)
      {
        string requirementText = "Entraînement requis : ";

        switch (item.BaseItem.ItemType)
        {
          case BaseItemType.Armor:

              switch (item.BaseItem.BaseAC)
              {
                default: requirementText += "vêtement : "; break; ;
                case 1:
                case 2:
                case 3: requirementText += "armure légère : "; break;
                case 4:
                case 5: requirementText += "armure intermédiaire : "; break;
                case 6:
                case 7: requirementText += "armure lourde : "; break;
                case 8: requirementText += "harnois : "; break;
              }

              break;

            case BaseItemType.SmallShield: requirementText += "rondache : "; break;
            case BaseItemType.LargeShield: requirementText += "écu : "; break;
            case BaseItemType.TowerShield: requirementText += "pavois : "; break;
            case BaseItemType.Shortsword:
            case BaseItemType.Longsword:
            case BaseItemType.Greatsword:
            case BaseItemType.Bastardsword:
            case BaseItemType.TwoBladedSword:
            case BaseItemType.Katana:
            case BaseItemType.Scimitar: requirementText += "épée : "; break;

            case BaseItemType.Handaxe:
            case BaseItemType.Battleaxe:
            case BaseItemType.Greataxe:
            case BaseItemType.Doubleaxe:
            case BaseItemType.DwarvenWaraxe: requirementText += "hache : "; break;

            case BaseItemType.Shortbow:
            case BaseItemType.Longbow:
            case BaseItemType.Sling:
            case BaseItemType.Shuriken:
            case BaseItemType.Dart:
            case BaseItemType.LightCrossbow:
            case BaseItemType.HeavyCrossbow: requirementText += "adresse au tir : "; break;

            case BaseItemType.Club:
            case BaseItemType.LightFlail:
            case BaseItemType.LightHammer:
            case BaseItemType.Morningstar:
            case BaseItemType.Warhammer:
            case BaseItemType.HeavyFlail:
            case BaseItemType.DireMace: requirementText += "marteau : "; break;

            case BaseItemType.ShortSpear:
            case BaseItemType.ThrowingAxe: requirementText += "armes de jet : "; break;

            case BaseItemType.Dagger:
            case BaseItemType.Kama:
            case BaseItemType.Kukri: requirementText += "dague : "; break;

            case BaseItemType.Scythe: requirementText += "faux : "; break;

          }

          return requirementText += ItemUtils.GetItemProficiencyRequirement(item).ToString();
      }*/
      }
    }
  }
}
