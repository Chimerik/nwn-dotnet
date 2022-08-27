﻿using System;
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
        private readonly List<NuiElement> rootChidren = new ();
        private readonly NuiGroup sequenceRegisterGroup;
        private readonly NuiRow sequenceRegisterRow;
        private readonly NuiBind<string> sequenceSaveButtonLabel = new ("sequenceSaveButtonLabel");
        private readonly NuiBind<string> itemName = new ("itemName");
        private readonly NuiBind<string> itemDescription = new ("itemDescription");
        private bool modificationAllowed { get; set; }
        private NwItem item { get; set; }

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

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiText(itemDescription) { Height = 100, Width = 590 } } });

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
              string spellName = isSlotUsed ? spell.Name.ToString() : "Emplacement d'enchantement libre";

              // TODO : si l'enchantement est désactivé (item ruiné), écrire "Désactivé dans le tooltip" et si possible, une utiliser une icône grisée
              NuiButtonImage slotButton = new NuiButtonImage(icon) { Id = $"spell_{id}", Tooltip = spellName, Height = 40, Width = 40 };
              enchantementSlotsRowChildren.Add(slotButton);
            }

            enchantementSlotsRowChildren.Add(new NuiSpacer());
            rootChildren.Add(enchantementSlotsRow);
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

            var armorEntry = Armor2da.armorTable[item.BaseACValue];
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Armure (effective) : {player.GetArmorProficiencyLevel(item.BaseACValue) * 10} %") { Tooltip = "Le pourcentage s'applique par rapport à la valeur maximum du bonus d'armure pour chaque type spécifique" } } });
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Bonus de dextérité maximal : {armorEntry.maxDex}") } });
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Pénalité d'armure : {armorEntry.ACPenalty}") } });
            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Echec des sorts profanes : {armorEntry.arcaneFailure} %") } });            
          }

          if (item.BaseItem.ItemType == BaseItemType.SmallShield || item.BaseItem.ItemType == BaseItemType.LargeShield || item.BaseItem.ItemType == BaseItemType.TowerShield)
          {
            /*int totalAC = item.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.AcBonus)
              .OrderByDescending(i => i.CostTableValue).FirstOrDefault().CostTableValue;*/

            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel($"Armure effective : {player.GetShieldProficiencyLevel(item.BaseItem.ItemType) * 10} %") { Tooltip = "Le pourcentage s'applique par rapport à la valeur maximum du bonus d'armure pour chaque type spécifique" } } });
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
            string ipName = $"{ip.Property.Name?.ToString()}";
            
            if (ip?.SubType.RowIndex > -1)
              ipName += $" : {NwGameTables.ItemPropertyTable.GetRow(ip.Property.RowIndex).SubTypeTable?.GetRow(ip.SubType.RowIndex).Name?.ToString()}";

            ipName += " " + ip.CostTableValue?.Name?.ToString();
            ipName += " " + ip.Param1TableValue?.Name?.ToString();
            ipName += ip.RemainingDuration != TimeSpan.Zero ? $" ({new TimeSpan(ip.RemainingDuration.Days, ip.RemainingDuration.Hours, ip.RemainingDuration.Minutes, ip.RemainingDuration.Seconds)})" : "";

            Color ipColor = ip.RemainingDuration != TimeSpan.Zero ? ColorConstants.Blue : ColorConstants.White;

            rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiLabel(ipName) { Height = 10, Width = 500, ForegroundColor = ipColor, Margin = 5 } } });
          }

          if (item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue)
          {
            List<NuiElement> durabilityRowChildren = new List<NuiElement>();
            NuiRow durabilityRow = new NuiRow() { Children = durabilityRowChildren };

            int durabilityState = item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value / item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value * 100;
            string durabilityText = "Flambant neuf"; ;
            Color durabilityColor = new Color(32, 255, 32);

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
              Color repairColor = ColorConstants.Orange;
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

              if(!Enum.TryParse(item.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value, out ResourceType resourceType))
              {
                Utils.LogMessageToDMs($"ITEM EXAMINE - Impossible de parser en ressource de craft : {item.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value}");
                return;
              }

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

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken =tempToken;
            nuiToken.OnNuiEvent += HandleItemExamineEvents;

            sequenceSaveButtonLabel.SetBindValue(player.oid, nuiToken.Token, "Continuer la séquence");
            itemName.SetBindValue(player.oid, nuiToken.Token, item.Name);
            itemDescription.SetBindValue(player.oid, nuiToken.Token, item.Description);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleItemExamineEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if(item == null)
          {
            CloseWindow();
            return;
          }

        if(modificationAllowed && item.Possessor != player.oid.ControlledCreature && !player.IsDm())
        {
          CloseWindow();
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
                item.Name = itemName.GetBindValue(player.oid, nuiToken.Token);
                return;

              case "modifyDescription":
                item.Description = itemDescription.GetBindValue(player.oid, nuiToken.Token);
                break;

              case "appearanceModifier":
                CloseWindow();
                OpenItemAppearanceModificationWindow();
                return;

              case "repair":
                player.HandleRepairItemChecks(item);
                CloseWindow();
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

                if (player.craftJob != null)
                {
                  player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
                  CloseWindow();
                  return;
                }

                player.craftJob = new CraftJob(player, item, JobType.BlueprintCopy);
                CloseWindow();

                if (!player.windows.TryAdd("activeCraftJob", new ActiveCraftJobWindow(player)))
                  ((ActiveCraftJobWindow)player.windows["activeCraftJob"]).CreateWindow();

                break;

              case "blueprintME":

                if (player.craftJob != null)
                {
                  player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
                  CloseWindow();
                  return;
                }

                player.craftJob = new CraftJob(player, item, JobType.BlueprintResearchMaterialEfficiency);
                CloseWindow();

                if (!player.windows.TryAdd("activeCraftJob", new ActiveCraftJobWindow(player)))
                  ((ActiveCraftJobWindow)player.windows["activeCraftJob"]).CreateWindow();

                return;

              case "blueprintTE":

                if (player.craftJob != null)
                  player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
                else
                {
                  player.craftJob = new CraftJob(player, item, JobType.BlueprintResearchTimeEfficiency);
                  CloseWindow();

                  if (!player.windows.TryAdd("activeCraftJob", new ActiveCraftJobWindow(player)))
                    ((ActiveCraftJobWindow)player.windows["activeCraftJob"]).CreateWindow();
                }

                CloseWindow();
                return;

              case "skillbook_learn":

                int learnableId = item.GetObjectVariable<LocalVariableInt>("_SKILL_ID").Value;
                player.learnableSkills.Add(learnableId, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[learnableId]));
                player.oid.SendServerMessage("Vous venez d'ajouter une nouvelle compétence à votre livre d'apprentissage !", ColorConstants.Rose);
                
                item.Destroy();
                CloseWindow();
                item.Destroy();

                return;

              case "hide":

                if (item.HiddenWhenEquipped == 0)
                  item.HiddenWhenEquipped = 1;
                else
                  player.craftJob = new(player, item, JobType.Renforcement);

                CloseWindow();
                return;

              case "recycle":
                if (player.craftJob != null)
                  player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
                else
                  player.craftJob = new(player, item, JobType.Recycling);

                CloseWindow();
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

                CloseWindow();
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
        private void OpenItemAppearanceModificationWindow()
        {
          switch (item.BaseItem.ItemType)
          {
            case BaseItemType.Armor:

              if (!player.windows.TryAdd("itemColorsModifier", new ArmorCustomizationWindow(player, item)))
                ((ArmorCustomizationWindow)player.windows["itemColorsModifier"]).CreateWindow(item);

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

              if (!player.windows.TryAdd("weaponAppearanceModifier", new WeaponAppearanceWindow(player, item)))
                ((WeaponAppearanceWindow)player.windows["weaponAppearanceModifier"]).CreateWindow(item);

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

              if (!player.windows.TryAdd("simpleItemAppearanceModifier", new SimpleItemAppearanceWindow(player, item)))
                ((SimpleItemAppearanceWindow)player.windows["simpleItemAppearanceModifier"]).CreateWindow(item);

              break;

            case BaseItemType.Helmet:

              if (!player.windows.TryAdd("helmetColorsModifier", new HelmetCustomizationWindow(player, item)))
                ((HelmetCustomizationWindow)player.windows["helmetColorsModifier"]).CreateWindow(item);

              break;

            case BaseItemType.Cloak:

              if (!player.windows.TryAdd("cloakColorsModifier", new CloakCustomizationWindow(player, item)))
                ((CloakCustomizationWindow)player.windows["cloakColorsModifier"]).CreateWindow(item);

              break;
          }
        }
        private string GetMateriaQualityIcon(int materiaQuality)
        {
          switch(materiaQuality)
          {
            case 1: return "ir_level1";
            case 2: return "ir_level2";
            case 3: return "ir_level3";
            case 4: return "ir_level4";
            case 5: return "ir_level5";
            case 6: return "ir_level6";
            case 7: return "ir_level789";
            case 8: 
            case 9:
              return "ir_cntrspell";
            default: return "ir_cantrips";
          }
        }
        private void LoadIPBindings()
        {
          List<string> ipNameList = new();
          List<Color> ipColorList = new();

          foreach (ItemProperty ip in item.ItemProperties)
          {
            string ipName = $"{ip.Property.Name?.ToString()}";

            if (ip?.SubType?.RowIndex > -1)
              ipName += $" : {NwGameTables.ItemPropertyTable.GetRow(ip.Property.RowIndex).SubTypeTable?.GetRow(ip.SubType.RowIndex).Name?.ToString()}";

            ipName += " " + ip.CostTableValue?.Name?.ToString();
            ipName += " " + ip.Param1TableValue?.Name?.ToString();
            ipName += ip.RemainingDuration > TimeSpan.Zero ? $" ({new TimeSpan(ip.RemainingDuration.Days, ip.RemainingDuration.Hours, ip.RemainingDuration.Minutes, ip.RemainingDuration.Seconds)})" : "";

            ipNameList.Add(ipName);
            ipColorList.Add(ip.DurationType == EffectDuration.Permanent ? ColorConstants.White : ColorConstants.Blue);
          }

          ipName.SetBindValues(player.oid, nuiToken.Token, ipNameList);
          ipColor.SetBindValues(player.oid, nuiToken.Token, ipColorList);
          listCount.SetBindValue(player.oid, nuiToken.Token, ipNameList.Count);
        }
        private void LoadStateBindings()
        {
          if (item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue)
          {
            int durabilityState = item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value / item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").Value * 100;
            string tooltip = "Etat : ";

            if(durabilityState >= 100)
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

            /*if (item.GetObjectVariable<LocalVariableInt>("_DURABILITY_NB_REPAIRS").HasValue) TODO : plutôt afficher cette info sur l'atelier
              tooltip += $"- Réparé {item.GetObjectVariable<LocalVariableInt>("_DURABILITY_NB_REPAIRS").Value} fois";*/

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

          if (item.Possessor != player.oid.ControlledCreature)
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

          if (player.learnableSkills.ContainsKey(skill) && player.learnableSkills[skill].totalPoints < 1)
          {
            player.oid.SendServerMessage("Il faut avoir étudié les techniques propre à ce métier avant de pouvoir se lancer dans un travail.", ColorConstants.Red);
            return false;
          }

          return true;
        }
      }
    }
  }
}
