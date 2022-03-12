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
        private readonly List<NuiElement> rootChidren = new List<NuiElement>();
        private readonly NuiGroup sequenceRegisterGroup;
        private readonly NuiRow sequenceRegisterRow;
        private readonly NuiBind<string> sequenceSaveButtonLabel = new NuiBind<string>("sequenceSaveButtonLabel");
        private readonly NuiBind<string> itemName = new NuiBind<string>("itemName");
        private readonly NuiBind<string> itemDescription = new NuiBind<string>("itemDescription");
        private bool modificationAllowed { get; set; }
        private bool IsInWorkshopRange { get; set; }
        private NwItem item { get; set; }
        private NwItem bestBlueprint { get; set; }

        public ItemExamineWindow(Player player, NwItem item) : base(player)
        {
          windowId = "itemExamine";

          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "learnableGroup", Border = true, Layout = rootColumn };
          sequenceRegisterRow = new NuiRow();
          sequenceRegisterGroup = new NuiGroup() { Id = "sequencerGroup", Border = true, Layout = sequenceRegisterRow };

          CreateWindow(item);
        }
        public void CreateWindow(NwItem item)
        {
          rootChidren.Clear();
          this.item = item;
          string originalCrafterName = item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value;
          modificationAllowed = (string.IsNullOrWhiteSpace(originalCrafterName) || originalCrafterName == player.oid.ControlledCreature.OriginalName)
            && (item.Possessor == player.oid.ControlledCreature || player.oid.IsDM)
            && item.Tag != "skillbook" && item.Tag != "blueprint";

          if (item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue)
            IsInWorkshopRange = IsPlayerInWorkshopRange();

          List<NuiElement> nameRowChildren = new List<NuiElement>();
          NuiRow nameRow = new NuiRow() { Children = nameRowChildren };

          nameRowChildren.Add(new NuiTextEdit("Nom de l'objet", itemName, 50, false) { Height = 30, Width = 500, Tooltip = item.Name });

          List<NuiElement> labelRowChildren = new List<NuiElement>();
          NuiRow lbelRow = new NuiRow() { Children = labelRowChildren };
          labelRowChildren.Add(new NuiLabel(itemDescription));

          List<NuiElement> descriptionRowChildren = new List<NuiElement>();
          NuiRow descriptionRow = new NuiRow() { Children = descriptionRowChildren };
          descriptionRowChildren.Add(new NuiTextEdit("Description de l'objet", itemDescription, 3000, true) { Height = 200, Width = 500 });

          if (modificationAllowed)
          {
            nameRowChildren.Add(new NuiButton("Modifier") { Id = "modifyName", Height = 30, Width = 80 });
            descriptionRowChildren.Add(new NuiButton("Modifier") { Id = "modifyDescription", Height = 30, Width = 80 });
          }

          rootChidren.Add(nameRow);
          rootChidren.Add(descriptionRow);

          if (!string.IsNullOrWhiteSpace(originalCrafterName))
          {
            rootChidren.Add(new NuiLabel($"Artisan : {originalCrafterName}") { Height = 30, Width = 500, Tooltip = $"Il est indiqué : 'Pour toute modification sur mesure, vous adresser à {originalCrafterName}'" });

            if (originalCrafterName == player.oid.ControlledCreature.OriginalName && item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value < 8)
              DrawUpgradeWidget();
          }         

          if (item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").HasValue)
          {
            List<NuiElement> enchantementSlotsRowChildren = new List<NuiElement>();
            NuiRow enchantementSlotsRow = new NuiRow() { Children = enchantementSlotsRowChildren };

            for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS"); i++)
            {
              NwSpell spell = NwSpell.FromSpellId(item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value);
              string icon = spell != null ? spell.IconResRef : "empty_materia_slot";
              string id = spell != null ? spell.Id.ToString() : "";
              string spellName = spell != null ? spell.Name : "Emplacement libre";

              // TODO : si l'enchantement est désactivé (item ruiné), écrire "Désactivé dans le tooltip" et si possible, une utiliser une icône grisée
              NuiButtonImage slotButton = new NuiButtonImage(icon) { Id = $"spell_{id}", Tooltip = spellName, Height = 40, Width = 40 };
              enchantementSlotsRowChildren.Add(slotButton);
            }

            rootChidren.Add(enchantementSlotsRow);
          }

          if (ItemUtils.IsWeapon(item.BaseItem) || item.BaseItem.ItemType == BaseItemType.Gloves || item.BaseItem.ItemType == BaseItemType.Bracer)
          {
            int damage = item.BaseItem.DieToRoll * player.GetWeaponMasteryLevel(item.BaseItem.ItemType) / 10;
            if (damage < 1)
              damage = 1;

            string damageTypeLabel = "Type de dégâts : ";

            foreach (DamageType damageType in item.BaseItem.WeaponType)
              damageTypeLabel += $"{ItemUtils.DisplayDamageType(damageType)} ";

            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Dégats (base) : {item.BaseItem.NumDamageDice}d{item.BaseItem.DieToRoll} - (effectifs) : {item.BaseItem.NumDamageDice}d{damage}") { Tooltip = "Vos dégâts effectifs peuvent être améliorés en entrainant la compétence spécifique à l'arme." } } });
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Critiques : {player.GetWeaponCritScienceLevel(item.BaseItem.ItemType) + 5} %") { Tooltip = "Vos chances de critiques peuvent être améliorées en entrainant la compétence de science du critique spécifique à l'arme" } } });
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel(damageTypeLabel) } });
          }

          if(item.BaseItem.ItemType == BaseItemType.Armor)
          {
            /*int totalAC = item.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.AcBonus)
              .OrderByDescending(i => i.CostTableValue).FirstOrDefault().CostTableValue;*/

            ArmorTable.Entry armorEntry = Armor2da.armorTable.GetDataEntry(item.BaseACValue);
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Armure (effective) : {player.GetArmorProficiencyLevel(item.BaseACValue) * 10} %") { Tooltip = "Le pourcentage s'applique par rapport à la valeur maximum du bonus d'armure pour chaque type spécifique" } } });
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Bonus de dextérité maximal : {armorEntry.maxDex}") } });
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Pénalité d'armure : {armorEntry.ACPenalty}") } });
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Echec des sorts profanes : {armorEntry.arcaneFailure} %") } });            
          }

          if (item.BaseItem.ItemType == BaseItemType.SmallShield || item.BaseItem.ItemType == BaseItemType.LargeShield || item.BaseItem.ItemType == BaseItemType.TowerShield)
          {
            /*int totalAC = item.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.AcBonus)
              .OrderByDescending(i => i.CostTableValue).FirstOrDefault().CostTableValue;*/

            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel("Armure effective : {player.GetShieldProficiencyLevel(item.BaseItem.ItemType) * 10} %") { Tooltip = "Le pourcentage s'applique par rapport à la valeur maximum du bonus d'armure pour chaque type spécifique" } } });
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Pénalité d'armure : {item.BaseItem.ArmorCheckPenalty}") } });
          }
          
          string weight = $"Poids : {item.Weight}";
          string windowName = item.Name;

          if (item.StackSize > 1)
          {
            weight += $" ({item.BaseItem.Weight / 10} par unité)";
            windowName += $" (x{item.StackSize})";
          }

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel(weight) } });

          if(item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").HasValue)
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Qualité de matéria : {item.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value}") } });

          foreach (ItemProperty ip in item.ItemProperties)
          {
            string ipName = $"{ItemPropertyDefinition2da.ipDefinitionTable.GetIPDefinitionlDataEntry(ip.PropertyType).name}";

            if (ip.SubType > -1)
              try { ipName += $" - {ItemPropertyDefinition2da.ipDefinitionTable.GetSubTypeName(ip.PropertyType, ip.SubType)}"; } catch (Exception) { }
              
            
            if (ip.CostTableValue > -1)
            {
              if(ip.PropertyType == ItemPropertyType.DamageBonus || ip.PropertyType == ItemPropertyType.DamageBonusVsAlignmentGroup || ip.PropertyType == ItemPropertyType.DamageBonusVsRacialGroup
                || ip.PropertyType == ItemPropertyType.DamageBonusVsSpecificAlignment || ip.PropertyType == ItemPropertyType.ExtraMeleeDamageType || ip.PropertyType == ItemPropertyType.ExtraRangedDamageType)
                ipName += $" : {ItemPropertyDamageCost2da.ipDamageCost.GetLabelFromIPCostTableValue(ip.CostTableValue)}";
              else
                ipName += $" : {ip.CostTableValue}";
            }

            ipName += ip.RemainingDuration != TimeSpan.Zero ? $" ({new TimeSpan(ip.RemainingDuration.Days, ip.RemainingDuration.Hours, ip.RemainingDuration.Minutes, ip.RemainingDuration.Seconds).ToString()})" : "";

            NuiColor ipColor = ip.RemainingDuration != TimeSpan.Zero ? ColorConstants.Blue : ColorConstants.White;

            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel(ipName) { Height = 10, Width = 500, ForegroundColor = ipColor, Margin = 5 } } });
          }

          if (item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue)
          {
            List<NuiElement> durabilityRowChildren = new List<NuiElement>();
            NuiRow durabilityRow = new NuiRow() { Children = durabilityRowChildren };

            int durabilityState = item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value / item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value * 100;
            string durabilityText = "Flambant neuf"; ;
            NuiColor durabilityColor = new NuiColor(32, 255, 32);

            if (durabilityState < 100 && durabilityState >= 75)
            {
              durabilityText = "Très bon état";
              durabilityColor = ColorConstants.Green;
            }
            else if (durabilityState < 75 && durabilityState >= 50)
            {
              durabilityText = "Bon état";
              durabilityColor = ColorConstants.Olive;
            }
            else if (durabilityState < 50 && durabilityState >= 25)
            {
              durabilityText = "Usé";
              durabilityColor = ColorConstants.Lime;
            }
            else if (durabilityState < 25 && durabilityState >= 5)
            {
              durabilityText = "Abimé";
              durabilityColor = ColorConstants.Orange;
            }
            else if (durabilityState < 5 && durabilityState >= 1)
            {
              durabilityText = "Vétuste";
              durabilityColor = ColorConstants.Red;
            }
            else if (durabilityState < 1)
            { 
              durabilityText = "Ruiné";
              durabilityColor = ColorConstants.Purple;
            }

            durabilityRowChildren.Add(new NuiLabel($"Etat : {durabilityText}") { Height = 20, Width = 500, ForegroundColor = durabilityColor });

            if (item.GetObjectVariable<LocalVariableInt>("_DURABILITY_NB_REPAIRS").HasValue)
            {
              NuiColor repairColor = ColorConstants.Orange;
              durabilityRowChildren.Add(new NuiLabel($"Réparé {item.GetObjectVariable<LocalVariableInt>("_DURABILITY_NB_REPAIRS").Value} fois") { Height = 30, Width = 500, ForegroundColor = repairColor });
            }

            rootChidren.Add(durabilityRow);

            if(player.learnableSkills.ContainsKey(CustomSkill.Repair))
              rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel(player.GetItemRepairDescriptionString(item)) } });
          }

          switch(item.Tag)
          {
            case "blueprint":

              int mineralCost = (int)(player.GetItemMateriaCost(item) * (1 - (item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value / 100)));
              TimeSpan jobDuration = TimeSpan.FromSeconds(player.GetItemCraftTime(item, mineralCost));

                //rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Patron de création de l'objet artisanal : {blueprint.name}") } });
                rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Recherche en rendement : {item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value}") } });
                rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Coût initial en {ItemUtils.GetResourceNameFromBlueprint(item)} : {mineralCost}") { Tooltip = "Puis 10 % de moins par amélioration vers un matériau supérieur." } } });
                rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Recherche en efficacité : {item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value}") } });
                rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Temps de fabrication et d'amélioration : {new TimeSpan(jobDuration.Days, jobDuration.Hours, jobDuration.Minutes, jobDuration.Seconds)}") } });

                int runs = item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value;

                if (runs > 0)
                  rootChidren.Add(new NuiLabel($"Utilisation(s) restante(s) : {runs}"));
                else if (item.Possessor == player.oid.ControlledCreature) // TODO : rediriger vers le menu de copie de blueprint
                {
                  string tooltip = "Lancer un travail de copie de ce patron original";
                  bool copySkill = player.learnableSkills.ContainsKey(CustomSkill.BlueprintCopy) && player.learnableSkills[CustomSkill.BlueprintCopy].totalPoints > 0;

                  if (player.craftJob != null)
                    tooltip = "Vous ne pouvez pas lancer un travail de copie de patron tant que vous avez un travail artisanal en cours.";
                  else if (!copySkill)
                    tooltip = "Il faut avoir étudié les techniques de copie de patron un minimum avant de pouvoir lancer ce travail";

                  List<NuiElement> blueprintActionRowChildren = new List<NuiElement>();
                  NuiRow blueprintActionRow = new NuiRow() { Children = blueprintActionRowChildren };
                  blueprintActionRowChildren.Add(new NuiSpacer());
                  blueprintActionRowChildren.Add(new NuiButton("Copier") { Id = "copy", Tooltip = tooltip, Enabled = player.craftJob == null && copySkill });
                  blueprintActionRowChildren.Add(new NuiSpacer());
                  blueprintActionRowChildren.Add(new NuiButton("Recherche en rendement") { Id = "blueprintME", Tooltip = "Lancer un travail de recherche en rendement sur ce patron original", Enabled = player.craftJob == null && player.learnableSkills.ContainsKey(CustomSkill.BlueprintMetallurgy) && player.learnableSkills[CustomSkill.BlueprintMetallurgy].totalPoints > 0 && item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value < 10 });
                  blueprintActionRowChildren.Add(new NuiSpacer());
                  blueprintActionRowChildren.Add(new NuiButton("Recherche en efficacité") { Id = "blueprintTE", Tooltip = "Lancer un travail recherche en efficacité sur ce patron original", Enabled = player.craftJob == null && player.learnableSkills.ContainsKey(CustomSkill.BlueprintResearch) && player.learnableSkills[CustomSkill.BlueprintResearch].totalPoints > 0 && item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value < 10 });
                  blueprintActionRowChildren.Add(new NuiSpacer());
                  rootChidren.Add(blueprintActionRow);
                }

              break;

            case "craft_resource":

              Enum.TryParse(item.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value, out ResourceType resourceType);
              CraftResource resource = Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == resourceType && r.grade == item.GetObjectVariable<LocalVariableInt>("CRAFT_GRADE").Value);

              List<NuiElement> reprocessingRowChildren = new List<NuiElement>();
              NuiRow reprocessingRow = new NuiRow() { Children = reprocessingRowChildren };
              reprocessingRowChildren.Add(new NuiText("Efficacité raffinage -30 % (base fonderie Impériale)"));
              rootChidren.Add(reprocessingRow);

              reprocessingRowChildren.Add(new NuiText($"x{(player.learnableSkills.ContainsKey(resource.reprocessingLearnable) ? 1.00 + 3 * player.learnableSkills[resource.reprocessingLearnable].totalPoints / 100 : 1.00)} (Raffinage)"));
              rootChidren.Add(reprocessingRow);

              reprocessingRowChildren.Add(new NuiText($"x{(player.learnableSkills.ContainsKey(resource.reprocessingEfficiencyLearnable) ? 1.00 + 2 * player.learnableSkills[resource.reprocessingLearnable].totalPoints / 100 : 1.00)} (Raffinage Efficace)"));
              rootChidren.Add(reprocessingRow);

              reprocessingRowChildren.Add(new NuiText($"x{(player.learnableSkills.ContainsKey(resource.reprocessingGradeLearnable) ? 1.00 + 2 * player.learnableSkills[resource.reprocessingGradeLearnable].totalPoints / 100 : 1.00)} (Raffinage Expert)"));
              rootChidren.Add(reprocessingRow);

              reprocessingRowChildren.Add(new NuiText($"x{(player.learnableSkills.ContainsKey(CustomSkill.ConnectionsPromenade) ? 0.95 + player.learnableSkills[CustomSkill.ConnectionsPromenade].totalPoints / 100 : 0.95)} (Relations Promenade)"));
              rootChidren.Add(reprocessingRow);

              break;

            case "sequence_register":
              CreateSequenceRegisterLayout(item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value.Split("_"));
              rootChidren.Add(sequenceRegisterGroup);
              break;

            case "skillbook":

              bool canLearn = !player.learnableSkills.ContainsKey(item.GetObjectVariable<LocalVariableInt>("_SKILL_ID").Value);
              rootChidren.Add(new NuiButton("Apprendre") { Id = "skillbook_learn", Enabled = canLearn, Tooltip = canLearn ? "Ajouter cette compétence à votre livre d'apprentissage" : "Cette compétence se trouve déjà dans votre livre d'apprentissage" });

              break;
          }

          List<NuiElement> actionRowChildren = new List<NuiElement>();
          NuiRow actionRow = new NuiRow() { Children = actionRowChildren };

          if(item.BaseItem.ItemType == BaseItemType.Helmet || item.BaseItem.ItemType == BaseItemType.Cloak)
          {
            actionRowChildren.Add(new NuiSpacer());
            actionRowChildren.Add(new NuiButton(item.HiddenWhenEquipped == 0 ? "Cacher" : "Afficher") { Id = "hide", Tooltip = "Considération purement esthétique permettant d'afficher ou non cet objet lorsque le personnage le porte." });
            actionRowChildren.Add(new NuiSpacer());
          }

          if (modificationAllowed) // TODO : ajouter un métier permettant de modifier n'importe quelle tenue (ex : styliste)
          {
            actionRowChildren.Add(new NuiSpacer());
            actionRowChildren.Add(new NuiButton("Modifier") { Id = "appearanceModifier", Tooltip = "Vers les modifications d'apparence" });
            actionRowChildren.Add(new NuiSpacer());
          }

          if (item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue && IsInWorkshopRange && player.craftJob == null
            && player.learnableSkills.ContainsKey(CustomSkill.Repair) && player.learnableSkills[CustomSkill.Repair].totalPoints > 0)
          {
            actionRowChildren.Add(new NuiSpacer());
            actionRowChildren.Add(new NuiButton("Réparer") { Id = "repair", Tooltip = "Vers les réparations" });
            actionRowChildren.Add(new NuiSpacer());

            if(player.learnableSkills.ContainsKey(CustomSkill.Renforcement) && player.learnableSkills[CustomSkill.Renforcement].totalPoints > 0
              && item.GetObjectVariable<LocalVariableInt>("_REINFORCEMENT_LEVEL").Value < 10)
            {
              actionRowChildren.Add(new NuiButton("Renforcer") { Id = "renforcement", Tooltip = "Permet d'augmenter la durabilité maximale de l'objet de 5 %. Cumulable 10 fois." });
              actionRowChildren.Add(new NuiSpacer());
            }

            if (player.learnableSkills.ContainsKey(CustomSkill.Recycler) && player.learnableSkills[CustomSkill.Recycler].totalPoints > 0)
            {
              actionRowChildren.Add(new NuiButton("Recycler") { Id = "recycle", Tooltip = "Permet de mettre en pièces les objets afin d'extraire une fraction de la matéria brute qu'ils contiennent." });
              actionRowChildren.Add(new NuiSpacer());
            }

            if (player.learnableSkills.ContainsKey(CustomSkill.SurchargeArcanique) && player.learnableSkills[CustomSkill.SurchargeArcanique].totalPoints > 0)
            {
              actionRowChildren.Add(new NuiButton("Surcharger") { Id = "surcharge", Tooltip = "Force l'ajout d'un emplacement d'enchantement au risque de briser l'objet." });
              actionRowChildren.Add(new NuiSpacer());
            }
          }

          rootChidren.Add(actionRow);

          // TODO : Si DM, bouton vers la modification d'objet spéciale ?

          //NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 620, 600);
          NuiRect windowRectangle = new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 620, 600);

          window = new NuiWindow(rootGroup, windowName)
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          token = player.oid.CreateNuiWindow(window, windowId);

          player.oid.OnNuiEvent -= HandleItemExamineEvents;
          player.oid.OnNuiEvent += HandleItemExamineEvents;

          sequenceSaveButtonLabel.SetBindValue(player.oid, token, "Continuer la séquence");
          itemName.SetBindValue(player.oid, token, item.Name);
          itemDescription.SetBindValue(player.oid, token, item.Description);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
        private void HandleItemExamineEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          if(item == null)
          {
            player.oid.NuiDestroy(token);
            return;
          }

          if(modificationAllowed && item.Possessor != player.oid.ControlledCreature && !player.oid.IsDM)
          {
            player.oid.NuiDestroy(token);
            ((ItemExamineWindow)player.windows["itemExamine"]).CreateWindow(item);
            return;
          }

          if(nuiEvent.EventType == NuiEventType.Close)
          {
            player.oid.LoginCreature.OnSpellAction -= RegisterSpellSequence;
            return;
          }

          if(nuiEvent.EventType == NuiEventType.Click)
          {
            switch(nuiEvent.ElementId)
            {
              case "modifyName":
                item.Name = itemName.GetBindValue(player.oid, token);
                return;

              case "modifyDescription":
                item.Description = itemDescription.GetBindValue(player.oid, token);
                break;

              case "appearanceModifier":
                player.oid.NuiDestroy(token);
                OpenItemAppearanceModificationWindow();
                return;

              case "repair":
                player.HandleRepairItemChecks(item);
                player.oid.NuiDestroy(token);
                return;

              case "add":

                if (sequenceSaveButtonLabel.GetBindValue(player.oid, token) == "Enregistrer")
                {
                  sequenceSaveButtonLabel.SetBindValue(player.oid, token, "Continuer la séquence");
                  player.oid.LoginCreature.OnSpellAction -= RegisterSpellSequence;
                  player.oid.SendServerMessage("La séquence a bien été enregistrée.", ColorConstants.Orange);
                }
                else
                {
                  sequenceSaveButtonLabel.SetBindValue(player.oid, token, "Enregistrer");
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

                if (player.craftJob != null)
                {
                  player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
                  player.oid.NuiDestroy(token);
                  return;
                }

                player.craftJob = new CraftJob(player, item, JobType.BlueprintCopy);
                player.oid.NuiDestroy(token);

                if (player.windows.ContainsKey("activeCraftJob"))
                  ((ActiveCraftJobWindow)player.windows["activeCraftJob"]).CreateWindow();
                else
                  player.windows.Add("activeCraftJob", new ActiveCraftJobWindow(player));
                break;

              case "blueprintME":

                if (player.craftJob != null)
                {
                  player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
                  player.oid.NuiDestroy(token);
                  return;
                }

                player.craftJob = new CraftJob(player, item, JobType.BlueprintResearchMaterialEfficiency);
                player.oid.NuiDestroy(token);

                if (player.windows.ContainsKey("activeCraftJob"))
                  ((ActiveCraftJobWindow)player.windows["activeCraftJob"]).CreateWindow();
                else
                  player.windows.Add("activeCraftJob", new ActiveCraftJobWindow(player));
                return;

              case "blueprintTE":

                if (player.craftJob != null)
                  player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
                else
                {
                  player.craftJob = new CraftJob(player, item, JobType.BlueprintResearchTimeEfficiency);
                  player.oid.NuiDestroy(token);

                  if (player.windows.ContainsKey("activeCraftJob"))
                    ((ActiveCraftJobWindow)player.windows["activeCraftJob"]).CreateWindow();
                  else
                    player.windows.Add("activeCraftJob", new ActiveCraftJobWindow(player));
                }

                player.oid.NuiDestroy(token);
                return;

              case "skillbook_learn":

                int learnableId = item.GetObjectVariable<LocalVariableInt>("_SKILL_ID").Value;
                player.learnableSkills.Add(learnableId, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[learnableId]));
                player.oid.SendServerMessage("Vous venez d'ajouter une nouvelle compétence à votre livre d'apprentissage !", ColorConstants.Rose);
                
                item.Destroy();
                player.oid.NuiDestroy(token);

                return;

              case "upgrade":
                player.HandleCraftItemChecks(bestBlueprint, item);
                player.oid.NuiDestroy(token);
                return;

              case "renforcement":
                if (player.craftJob != null)
                  player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
                else
                  player.craftJob = new CraftJob(player, item, JobType.Renforcement);

                player.oid.NuiDestroy(token);
                return;

              case "recycle":
                if (player.craftJob != null)
                  player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
                else
                  player.craftJob = new CraftJob(player, item, JobType.Recycling);

                player.oid.NuiDestroy(token);
                return;

              case "surcharge":

                int controlLevel = player.learnableSkills.ContainsKey(CustomSkill.SurchargeControlee) ? player.learnableSkills[CustomSkill.SurchargeControlee].totalPoints : 0;

                int dice = NwRandom.Roll(Utils.random, 100);

                if (dice <= player.learnableSkills[CustomSkill.SurchargeArcanique].totalPoints)
                {
                  item.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
                  player.oid.SendServerMessage($"En forçant à l'aide de votre puissance brute, vous parvenez à ajouter un emplacement de sort supplémentaire à votre {item.Name.ColorString(ColorConstants.White)} !", ColorConstants.Navy);
                }
                else if (dice > controlLevel)
                {
                  item.Destroy();
                  player.oid.SendServerMessage($"Vous forcez, forcez, et votre {item.Name.ColorString(ColorConstants.White)} se brise sous l'excès infligé.", ColorConstants.Purple);
                }

                player.oid.NuiDestroy(token);
                return;

              case "hide":

                if (item.HiddenWhenEquipped == 0)
                  item.HiddenWhenEquipped = 1;
                else
                  item.HiddenWhenEquipped = 0;

                return;
            }

            if(nuiEvent.ElementId.StartsWith("up_"))
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
              int spellId = int.Parse(nuiEvent.ElementId.Split("_")[1]);
              if (player.windows.ContainsKey("spellDescription"))
                ((SpellDescriptionWindow)player.windows["spellDescription"]).CreateWindow(spellId);
              else
                player.windows.Add("spellDescription", new SpellDescriptionWindow(player, spellId));
            }
          }
        }
        private void CreateSequenceRegisterLayout(IEnumerable spellList)
        {
          List<NuiElement> sequencerRegisterRowChildren = new List<NuiElement>();
          sequenceRegisterRow.Children = sequencerRegisterRowChildren;
          int i = 0;

          foreach (string spellId in spellList)
          {
            NwSpell spell = NwSpell.FromSpellId(int.Parse(spellId));
            sequencerRegisterRowChildren.Add(new NuiImage(spell.IconResRef) { Tooltip = spell.Name });

            if (item.Possessor == player.oid.ControlledCreature)
            {
              sequencerRegisterRowChildren.Add(new NuiButtonImage("menu_up") { Id = $"up_{i}", Tooltip = "Déplacer vers le haut" });
              sequencerRegisterRowChildren.Add(new NuiButtonImage("menu_down") { Id = $"down_{i}", Tooltip = "Déplacer vers le bas" });
              sequencerRegisterRowChildren.Add(new NuiButtonImage("menu_exit") { Id = $"delete_{i}", Tooltip = "Supprimer" });
            }

            i++;
          }

          if (item.Possessor == player.oid.ControlledCreature)
          {
            sequencerRegisterRowChildren.Add(new NuiButton(sequenceSaveButtonLabel) { Id = "add", Tooltip = "Les sorts seront ajoutés à la fin de la séquence existante" });
            sequencerRegisterRowChildren.Add(new NuiButton("Effacer") { Id = "reinit", Tooltip = "La séquence actuellement enregistrée sera réinitialisée" });
          }
        }
        private void SequenceRegisterSwap(int sequenceId, int swapValue, string[] spellList)
        {
          string tempSwap = spellList[sequenceId];
          spellList[sequenceId] = spellList[sequenceId + swapValue];
          spellList[sequenceId + swapValue] = tempSwap;
          UpdateSequenceAndLayout(spellList);
        }
        private void UpdateSequenceAndLayout(IEnumerable spellList)
        {
          string sequence = "";

          foreach (string spell in spellList)
            sequence += $"{spell}_";

          sequence.Remove(sequence.Length - 1);
          item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value = sequence;

          CreateSequenceRegisterLayout(spellList);
          sequenceRegisterGroup.SetLayout(player.oid, token, sequenceRegisterRow);
        }
        private void RegisterSpellSequence(OnSpellAction onSpellAction)
        {
          onSpellAction.PreventSpellCast = true;

          if (item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").HasNothing)
            item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value = ((int)onSpellAction.Spell.SpellType).ToString();
          else
            item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value += $"_{((int)onSpellAction.Spell.SpellType)}";

          CreateSequenceRegisterLayout(item.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value.Split("_"));
          sequenceRegisterGroup.SetLayout(player.oid, token, sequenceRegisterRow);
        }
        private void OpenItemAppearanceModificationWindow()
        {
          switch (item.BaseItem.ItemType)
          {
            case BaseItemType.Armor:
              if (player.windows.ContainsKey("itemAppearanceModifier"))
                ((ArmorAppearanceWindow)player.windows["itemAppearanceModifier"]).CreateWindow(item);
              else
                player.windows.Add("itemAppearanceModifier", new ArmorAppearanceWindow(player, item));
              break;
            case BaseItemType.Bastardsword:
            case BaseItemType.Battleaxe:
            case BaseItemType.Club:
            case BaseItemType.Dagger:
            case BaseItemType.DireMace:
            case BaseItemType.Doubleaxe:
            case BaseItemType.DwarvenWaraxe:
            case BaseItemType.Greataxe:
            case BaseItemType.Greatsword:
            case BaseItemType.Halberd:
            case BaseItemType.Handaxe:
            case BaseItemType.HeavyCrossbow:
            case BaseItemType.HeavyFlail:
            case BaseItemType.Kama:
            case BaseItemType.Katana:
            case BaseItemType.Kukri:
            case BaseItemType.LightCrossbow:
            case BaseItemType.LightFlail:
            case BaseItemType.LightHammer:
            case BaseItemType.LightMace:
            case BaseItemType.Longbow:
            case BaseItemType.Longsword:
            case BaseItemType.MagicStaff:
            case BaseItemType.Morningstar:
            case BaseItemType.Quarterstaff:
            case BaseItemType.Rapier:
            case BaseItemType.Scimitar:
            case BaseItemType.Scythe:
            case BaseItemType.Shortbow:
            case BaseItemType.ShortSpear:
            case BaseItemType.Shortsword:
            case BaseItemType.Sickle:
            case BaseItemType.Sling:
            case BaseItemType.ThrowingAxe:
            case BaseItemType.Trident:
            case BaseItemType.TwoBladedSword:
            case BaseItemType.Warhammer:
            case BaseItemType.Whip:
              if (player.windows.ContainsKey("weaponAppearanceModifier"))
                ((WeaponAppearanceWindow)player.windows["weaponAppearanceModifier"]).CreateWindow(item);
              else
                player.windows.Add("weaponAppearanceModifier", new WeaponAppearanceWindow(player, item));
              break;
            case BaseItemType.Amulet:
            case BaseItemType.Arrow:
            case BaseItemType.Belt:
            case BaseItemType.Bolt:
            case BaseItemType.Book:
            case BaseItemType.Boots:
            case BaseItemType.Bracer:
            case BaseItemType.Bullet:
            case BaseItemType.EnchantedPotion:
            case BaseItemType.EnchantedScroll:
            case BaseItemType.EnchantedWand:
            case BaseItemType.Gloves:
            case BaseItemType.Grenade:
            case BaseItemType.LargeShield:
            case BaseItemType.MagicRod:
            case BaseItemType.MagicWand:
            case BaseItemType.Potions:
            case BaseItemType.Ring:
            case BaseItemType.Scroll:
            case BaseItemType.Shuriken:
            case BaseItemType.SmallShield:
            case BaseItemType.SpellScroll:
            case BaseItemType.TowerShield:
            case BaseItemType.TrapKit:
              if (player.windows.ContainsKey("simpleItemAppearanceModifier"))
                ((SimpleItemAppearanceWindow)player.windows["simpleItemAppearanceModifier"]).CreateWindow(item);
              else
                player.windows.Add("simpleItemAppearanceModifier", new SimpleItemAppearanceWindow(player, item));
              break;
            case BaseItemType.Helmet:
              if (player.windows.ContainsKey("helmetAppearanceModifier"))
                ((HelmetAppearanceWindow)player.windows["helmetAppearanceModifier"]).CreateWindow(item);
              else
                player.windows.Add("helmetAppearanceModifier", new HelmetAppearanceWindow(player, item));
              break;
            case BaseItemType.Cloak:
              if (player.windows.ContainsKey("cloakAppearanceModifier"))
                ((CloakAppearanceWindow)player.windows["cloakAppearanceModifier"]).CreateWindow(item);
              else
                player.windows.Add("cloakAppearanceModifier", new CloakAppearanceWindow(player, item));
              break;
          }
        }
        private void DrawUpgradeWidget()
        {
          if (!IsInWorkshopRange)
          {
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Améliorable à proximité d'un atelier artisanal adéquat.") } });
            return;
          }

          bestBlueprint = player.oid.ControlledCreature.Inventory.Items
            .Where(i => i.Tag == "blueprint" && i.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value == (int)item.BaseItem.ItemType)
            .OrderByDescending(i => i.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value)
            .ThenByDescending(i => i.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value).FirstOrDefault();

          if(bestBlueprint == null)
          {
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel("Améliorable avec le patron adéquat.") } });
            return;
          }

          string icon = item.BaseItem.WeaponFocusFeat.IconResRef;
          int materiaCost = (int)(player.GetItemMateriaCost(bestBlueprint) * (1 - (bestBlueprint.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value / 100)));
          TimeSpan jobDuration = TimeSpan.FromSeconds(player.GetItemCraftTime(bestBlueprint, materiaCost));

          CraftResource resource = player.craftResourceStock.FirstOrDefault(r => r.type == ItemUtils.GetResourceTypeFromBlueprint(item) && r.grade == 1);
          int availableQuantity = resource != null ? resource.quantity : 0;

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>()
            {
              new NuiButtonImage(icon) { Tooltip = bestBlueprint.Name, Height = 40, Width = 40 },
              new NuiLabel($"Coût en {ItemUtils.GetResourceNameFromBlueprint(bestBlueprint)} : {materiaCost}/{availableQuantity}") { Width = 160, Tooltip =  bestBlueprint.Name,
                DrawList = new List<NuiDrawListItem>() { new NuiDrawListText(new NuiColor(255, 255, 255), new NuiRect(0, 35, 150, 60), $"Temps de fabrication : {new TimeSpan(jobDuration.Days, jobDuration.Hours, jobDuration.Minutes, jobDuration.Seconds)}") } },
              new NuiButton("Améliorer") { Id = "upgrade", Enabled = player.craftJob == null && availableQuantity >= materiaCost, Tooltip = "Améliore l'objet à son prochain niveau de qualité avec le patron et la matéria adéquate.", Height = 40, Width = 80 }
            } 
          });
        }

        private bool IsPlayerInWorkshopRange()
        {
          string workshopName = BaseItems2da.baseItemTable.GetBaseItemDataEntry(item.BaseItem.ItemType).workshop;
          NwPlaceable workshop = player.oid.ControlledCreature.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(w => w.Tag == workshopName);

          if (workshop == null || player.oid.ControlledCreature.DistanceSquared(workshop) > 25)
            return false;

          return true;
        }
      }
    }
  }
}
