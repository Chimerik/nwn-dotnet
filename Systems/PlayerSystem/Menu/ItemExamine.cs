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
        private readonly List<NuiElement> rootChidren;
        private readonly NuiGroup sequenceRegisterGroup;
        private readonly NuiRow sequenceRegisterRow;
        private readonly NuiBind<string> sequenceSaveButtonLabel = new NuiBind<string>("sequenceSaveButtonLabel");
        private readonly NuiBind<string> itemName = new NuiBind<string>("itemName");
        private readonly NuiBind<string> itemDescription = new NuiBind<string>("itemDescription");
        private NwItem item { get; set; }

        public ItemExamineWindow(Player player, NwItem item) : base(player)
        {
          windowId = "itemExamine";

          rootChidren = new List<NuiElement>();
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
          bool modificationAllowed = (string.IsNullOrWhiteSpace(originalCrafterName) || originalCrafterName == player.oid.ControlledCreature.OriginalName)
            && (item.Possessor == player.oid.ControlledCreature || player.oid.IsDM);

          List<NuiElement> nameRowChildren = new List<NuiElement>();
          NuiRow nameRow = new NuiRow() { Children = nameRowChildren };

          nameRowChildren.Add(new NuiTextEdit("Nom de l'objet", itemName, 50, false) { Height = 30, Tooltip = item.Name });

          List<NuiElement> descriptionRowChildren = new List<NuiElement>();
          NuiRow descriptionRow = new NuiRow() { Children = descriptionRowChildren };
          descriptionRowChildren.Add(new NuiTextEdit("Description de l'objet", itemDescription, 3000, true) { Height = 200 });

          if (modificationAllowed)
          {
            nameRowChildren.Add(new NuiButton("Modifier") { Id = "modifyName", Height = 30 });
            descriptionRowChildren.Add(new NuiButton("Modifier") { Id = "modifyDescription", Height = 30 });
          }

          rootChidren.Add(nameRow);

          if (!string.IsNullOrWhiteSpace(originalCrafterName))
          {
            List<NuiElement> crafterRowChildren = new List<NuiElement>();
            NuiRow crafterRow = new NuiRow() { Children = crafterRowChildren };
            crafterRowChildren.Add(new NuiText(originalCrafterName) { Tooltip = $"Il est indiqué : 'Pour toute modification sur mesure, vous adresser à {originalCrafterName}'" });
            rootChidren.Add(crafterRow);
          }

          rootChidren.Add(descriptionRow);

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

          if(!string.IsNullOrEmpty(item.BaseItem.BaseItemStatsText))
          {
            List<NuiElement> baseStatsRowChildren = new List<NuiElement>();
            NuiRow baseStatsRow = new NuiRow() { Children = baseStatsRowChildren };
            descriptionRowChildren.Add(new NuiText(item.BaseItem.BaseItemStatsText) { Height = 200 });
            rootChidren.Add(baseStatsRow);

            if (ItemUtils.IsWeapon(item.BaseItem) || item.BaseItem.ItemType == BaseItemType.Gloves || item.BaseItem.ItemType == BaseItemType.Bracer)
            {
              int damage = item.BaseItem.DieToRoll * player.GetWeaponMasteryLevel(item.BaseItem.ItemType) / 10;
              if (damage < 1)
                damage = 1;

              descriptionRowChildren.Add(new NuiText("Votre potentiel de dégâts"));
              descriptionRowChildren.Add(new NuiText($"{item.BaseItem.NumDamageDice}d{damage}"));
              descriptionRowChildren.Add(new NuiText("Vos chances de critique"));
              descriptionRowChildren.Add(new NuiText($"{player.GetWeaponCritScienceLevel(item.BaseItem.ItemType)} %"));
              rootChidren.Add(baseStatsRow);
            }
          }

          foreach(ItemProperty ip in item.ItemProperties)
          {
            List<NuiElement> itemPropertyRowChildren = new List<NuiElement>();
            NuiRow itemPropertyRow = new NuiRow() { Children = itemPropertyRowChildren };

            string ipName = $"{ItemPropertyDefinition2da.ipDefinitionTable.GetIPDefinitionlDataEntry(ip.PropertyType).name} - " +
          $"{ItemPropertyDefinition2da.ipDefinitionTable.GetSubTypeName(ip.PropertyType, ip.SubType)} : {ip.CostTableValue}";
            
            ipName += ip.RemainingDuration != TimeSpan.Zero ? $"({new TimeSpan(ip.RemainingDuration.Days, ip.RemainingDuration.Hours, ip.RemainingDuration.Minutes, ip.RemainingDuration.Seconds).ToString()})" : "";

            itemPropertyRowChildren.Add(new NuiText(ipName) { Height = 30 });
            rootChidren.Add(itemPropertyRow);
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

            durabilityRowChildren.Add(new NuiText(durabilityText) { ForegroundColor = durabilityColor });

            if (item.GetObjectVariable<LocalVariableInt>("_DURABILITY_NB_REPAIRS").HasValue)
            {
              NuiColor repairColor = ColorConstants.Orange;
              durabilityRowChildren.Add(new NuiText($"Réparé {item.GetObjectVariable<LocalVariableInt>("_DURABILITY_NB_REPAIRS").Value} fois") { ForegroundColor = repairColor });
            }

            rootChidren.Add(durabilityRow);
          }

          switch(item.Tag)
          {
            case "blueprint":

              int baseItemType = item.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value;
              if (Craft.Collect.System.blueprintDictionnary.ContainsKey(baseItemType))
              {
                Craft.Blueprint blueprint = Craft.Collect.System.blueprintDictionnary[baseItemType];
                TimeSpan jobDuration = TimeSpan.FromSeconds(blueprint.GetBlueprintTimeCostForPlayer(player, item));

                List<NuiElement> blueprintRowChildren = new List<NuiElement>();
                NuiRow blueprintRow = new NuiRow() { Children = blueprintRowChildren };

                blueprintRowChildren.Add(new NuiText("Patron de création de l'objet artisanal"));
                blueprintRowChildren.Add(new NuiText(blueprint.name));
                rootChidren.Add(blueprintRow);

                blueprintRowChildren.Add(new NuiText("Recherche en rendement"));
                blueprintRowChildren.Add(new NuiText(item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value.ToString()));
                blueprintRowChildren.Add(new NuiText($"Coût initial en {blueprint.resourceType.ToDescription()}"));
                blueprintRowChildren.Add(new NuiText(blueprint.GetBlueprintMineralCostForPlayer(player, item).ToString()) { Tooltip = "Puis 10 % de moins par amélioration vers un matériau supérieur." });
                rootChidren.Add(blueprintRow);

                blueprintRowChildren.Add(new NuiText("Recherche en efficacité"));
                blueprintRowChildren.Add(new NuiText(item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value.ToString()));
                blueprintRowChildren.Add(new NuiText("Temps de fabrication et d'amélioration"));
                blueprintRowChildren.Add(new NuiText(new TimeSpan(jobDuration.Days, jobDuration.Hours, jobDuration.Minutes, jobDuration.Seconds).ToString()));
                rootChidren.Add(blueprintRow);

                int runs = item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_RUNS").Value;

                if (runs > 0)
                {
                  blueprintRowChildren.Add(new NuiText("Utilisations restantes"));
                  blueprintRowChildren.Add(new NuiText(runs.ToString()));
                  rootChidren.Add(blueprintRow);
                }
                else if(item.Possessor == player.oid.ControlledCreature) // TODO : rediriger vers le menu de copie de blueprint
                  blueprintRowChildren.Add(new NuiButton("Copier") { Id = "copy", Tooltip = "Lancer un travail de copie de ce patron original" });

                rootChidren.Add(blueprintRow);
              }
              else
              {
                player.oid.SendServerMessage("[ERREUR HRP] - Le patron utilisé n'est pas correctement initialisé. Le bug a été remonté au staff.");
                Utils.LogMessageToDMs($"Blueprint Invalid : {item.Name} - Base Item Type : {baseItemType} - Examined by : {player.oid.LoginCreature.Name}");
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
          }

          List<NuiElement> actionRowChildren = new List<NuiElement>();
          NuiRow actionRow = new NuiRow() { Children = actionRowChildren };

          if (modificationAllowed) // TODO : ajouter un métier permettant de modifier n'importe quelle tenue (ex : styliste)
            actionRowChildren.Add(new NuiButton("Modifier") { Id = "appearanceModifier", Tooltip = "Vers les modifications d'apparence" });

          if (item.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue)
            actionRowChildren.Add(new NuiButton("Réparer") { Id = "repair", Tooltip = "Vers les réparations" });

          rootChidren.Add(actionRow);
          
          // TODO : Si DM, bouton vers la modification d'objet spéciale ?

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 500, 300);

          window = new NuiWindow(rootGroup, item.Name)
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

          if(item == null || item.Possessor != player.oid.ControlledCreature)
          {
            player.oid.NuiDestroy(token);
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
                break;

              case "modifyDescription":
                item.Description = itemDescription.GetBindValue(player.oid, token);
                break;

              case "appearanceModifier":
                player.oid.NuiDestroy(token);
                OpenItemAppearanceModificationWindow();
                return;

              case "repair":
                player.oid.NuiDestroy(token);
                // TODO : menu de réparation
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
      }
    }
  }
}
