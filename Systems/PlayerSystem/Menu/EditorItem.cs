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
      public class EditorItemWindow : PlayerWindow
      {
        private NwItem targetItem;
        private readonly NuiGroup rootGroup = new() { Id = "rootGroup", Border = false, Padding = 0, Margin = 0 };
        private readonly NuiColumn layoutColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();

        private readonly NuiBind<bool> visibilityDM = new("visibilityDM");

        private readonly NuiBind<string> name = new("name");
        private readonly NuiBind<string> tag = new("tag");
        private readonly NuiBind<string> cost = new("cost");
        private readonly NuiBind<string> weight = new("weight");
        private readonly NuiBind<string> charges = new("charges");
        private readonly NuiBind<string> size = new("size");
        private readonly NuiBind<int> baseItemSelected = new("baseItemSelected");
        private readonly NuiBind<bool> undroppableChecked = new("undroppableChecked");
        private readonly NuiBind<bool> identifiedChecked = new("identifiedChecked");

        private readonly NuiBind<int> listCount = new("listCount");

        private readonly NuiBind<int> listAcquiredIPCount = new("listAcquiredFeatCount");
        private readonly NuiBind<string> availableIP = new("availableFeatNames");
        private readonly NuiBind<string> acquiredIP = new("acquiredFeatNames");
        private readonly NuiBind<bool> ipDetailVisibility = new("ipDetailVisibility");
        private readonly NuiBind<string> selectedIP = new("selectedIP");
        private readonly NuiBind<int> subTypeSelected = new("subTypeSelected");
        private readonly NuiBind<int> costValueSelected = new("costValueSelected");
        private readonly NuiBind<int> paramValueSelected = new("paramValueSelected");
        private readonly NuiBind<List<NuiComboEntry>> subTypeBind = new("subTypeBind");
        private readonly NuiBind<List<NuiComboEntry>> costValueBind = new("costValueBind");
        private readonly NuiBind<List<NuiComboEntry>> paramValueBind = new("paramValueBind");
        private readonly List<NuiComboEntry> subTypeEntries = new();
        private readonly List<NuiComboEntry> costValueEntries = new();
        private readonly List<NuiComboEntry> paramValueEntries = new();
        private readonly NuiBind<string> addTooltip = new("addTooltip");

        private readonly NuiBind<string> itemDescription = new("itemDescription");
        private readonly NuiBind<string> itemComment = new("itemComment");

        private readonly NuiBind<string> variableName = new("variableName");
        private readonly NuiBind<string> variableValue = new("variableValue");
        private readonly NuiBind<int> selectedVariableType = new("selectedVariableType");

        private readonly NuiBind<string> newVariableName = new("newVariableName");
        private readonly NuiBind<string> newVariableValue = new("newVariableValue");
        private readonly NuiBind<int> selectedNewVariableType = new("selectedNewVariableType");

        private readonly NuiBind<bool> modificationAllowed = new("modificationAllowed");

        private readonly List<ItemPropertyTableEntry> availableIPList = new();
        private readonly List<ItemProperty> acquiredIPList = new();

        private ItemProperty lastClickedIP;

        public EditorItemWindow(Player player, NwItem targetItem) : base(player)
        {
          windowId = "editorItem";

          rootGroup.Layout = layoutColumn;
          layoutColumn.Children = rootChildren;

          CreateWindow(targetItem);
        }
        public void CreateWindow(NwItem targetItem)
        {
          this.targetItem = targetItem;
          LoadBaseLayout();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 820, 500);

          window = new NuiWindow(rootGroup, $"Modification de {targetItem.Name}")
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

            nuiToken.OnNuiEvent += HandleEditorItemEvents;

            LoadBaseBinding();

            visibilityDM.SetBindValue(player.oid, nuiToken.Token, player.oid.IsDM || player.oid.PlayerName == "Chim");
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            availableIPList.Clear();

            foreach (var entry in NwGameTables.ItemPropertyTable)
              if (entry.ItemMap.IsItemPropertyValidForItem(targetItem.BaseItem) && entry.Name is not null)
                availableIPList.Add(entry);
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleEditorItemEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (targetItem == null)
          {
            player.oid.SendServerMessage("L'objet édité n'est plus valide.", ColorConstants.Red);
            CloseWindow();
            return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "base":
                  LoadBaseLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadBaseBinding();
                  break;

                case "properties":
                  LoadItemPropertyLayout();
                  rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
                  LoadItemPropertyBinding();
                  break;

                case "description":
                  LoadDescriptionLayout();
                  rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
                  LoadDescriptionBinding();
                  break;

                case "variables":
                  LoadVariablesLayout();
                  rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
                  selectedNewVariableType.SetBindValue(player.oid, nuiToken.Token, 0);
                  LoadVariablesBinding();

                  break;

                case "addIP":
                  ItemPropertyTableEntry ipEntry = availableIPList[nuiEvent.ArrayIndex];
                  /*LogUtils.LogMessage($"ipEntry : {ipEntry}", LogUtils.LogType.Combat);
                  LogUtils.LogMessage($"ipEntry?.SubTypeTable?.GetRow(0) ?? null : {ipEntry?.SubTypeTable?.GetRow(0) ?? null}", LogUtils.LogType.Combat);
                  LogUtils.LogMessage($"ipEntry?.CostTable?.GetRow(1) ?? null : {ipEntry?.CostTable?.GetRow(0)}", LogUtils.LogType.Combat);
                  LogUtils.LogMessage($"ipEntry?.Param1Table?.GetRow(0) ?? null : {ipEntry?.Param1Table?.GetRow(0) ?? null}", LogUtils.LogType.Combat);*/
                  var ip = ItemProperty.Custom(ipEntry, ipEntry?.SubTypeTable?.GetRow(0) ?? null, ipEntry?.CostTable.Count > 0 ? ipEntry?.CostTable?.GetRow(1) : null, ipEntry?.Param1Table?.GetRow(0) ?? null);
                  targetItem.AddItemProperty(ip, EffectDuration.Permanent);
                  LoadItemPropertyBinding();
                  break;

                case "removeIP":
                  targetItem.RemoveItemProperty(acquiredIPList[nuiEvent.ArrayIndex]);
                  LoadItemPropertyBinding();
                  break;

                case "openIP":

                  lastClickedIP = acquiredIPList[nuiEvent.ArrayIndex];
                  ipDetailVisibility.SetBindValue(player.oid, nuiToken.Token, true);
                  selectedIP.SetBindValue(player.oid, nuiToken.Token, lastClickedIP.Property.Name?.ToString());

                  subTypeEntries.Clear();
                  costValueEntries.Clear();
                  paramValueEntries.Clear();

                  if (NwGameTables.ItemPropertyTable.GetRow(lastClickedIP.Property.RowIndex).SubTypeTable != null)
                    foreach (var entry in NwGameTables.ItemPropertyTable.GetRow(lastClickedIP.Property.RowIndex).SubTypeTable)
                      if(entry.Name.HasValue)
                        subTypeEntries.Add(new NuiComboEntry(entry.Name?.ToString(), entry.RowIndex));

                  if (lastClickedIP.CostTable != null)
                    foreach (var entry in lastClickedIP.CostTable)
                      if (entry.RowIndex > 0)
                        if (entry.Name.HasValue)
                          costValueEntries.Add(new NuiComboEntry(entry.Name?.ToString(), entry.RowIndex));

                  if (lastClickedIP.Param1Table != null)
                    foreach (var entry in lastClickedIP.Param1Table)
                      if (entry.Name.HasValue)
                        paramValueEntries.Add(new NuiComboEntry(entry.Name?.ToString(), entry.RowIndex));

                  subTypeBind.SetBindValue(player.oid, nuiToken.Token, subTypeEntries);
                  costValueBind.SetBindValue(player.oid, nuiToken.Token, costValueEntries);
                  paramValueBind.SetBindValue(player.oid, nuiToken.Token, paramValueEntries);

                  subTypeSelected.SetBindValue(player.oid, nuiToken.Token, lastClickedIP?.SubType?.RowIndex ?? -1);
                  costValueSelected.SetBindValue(player.oid, nuiToken.Token, lastClickedIP?.CostTableValue?.RowIndex ?? -1);
                  paramValueSelected.SetBindValue(player.oid, nuiToken.Token, lastClickedIP?.Param1TableValue?.RowIndex ?? -1);

                  break;

                case "setIPDetail":
                  targetItem.RemoveItemProperty(lastClickedIP);
                  targetItem.AddItemProperty(ItemProperty.Custom(lastClickedIP.Property.RowIndex, subTypeSelected.GetBindValue(player.oid, nuiToken.Token), costValueSelected.GetBindValue(player.oid, nuiToken.Token), paramValueSelected.GetBindValue(player.oid, nuiToken.Token)), EffectDuration.Permanent);
                  LoadItemPropertyBinding();
                  ipDetailVisibility.SetBindValue(player.oid, nuiToken.Token, true);
                  break;

                case "saveDescription":
                  targetItem.Description = itemDescription.GetBindValue(player.oid, nuiToken.Token);
                  targetItem.GetObjectVariable<LocalVariableString>("_COMMENT").Value = itemComment.GetBindValue(player.oid, nuiToken.Token);
                  player.oid.SendServerMessage($"La description et le commentaire de l'objet {targetItem.Name.ColorString(ColorConstants.White)} ont bien été enregistrées.", new Color(32, 255, 32));
                  break;

                case "saveNewVariable":
                  Utils.ConvertLocalVariable(newVariableName.GetBindValue(player.oid, nuiToken.Token), newVariableValue.GetBindValue(player.oid, nuiToken.Token), selectedNewVariableType.GetBindValue(player.oid, nuiToken.Token), targetItem, player.oid);
                  LoadVariablesBinding();
                  break;

                case "saveVariable":
                  Utils.ConvertLocalVariable(variableName.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex], variableValue.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex], selectedVariableType.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex], targetItem, player.oid);
                  LoadVariablesBinding();
                  break;

                case "deleteVariable":
                  targetItem.LocalVariables.ElementAt(nuiEvent.ArrayIndex).Delete();
                  LoadVariablesBinding();
                  break;

                case "appearance":
                  if (modificationAllowed.GetBindValue(player.oid, nuiToken.Token))
                    ItemUtils.OpenItemCustomizationWindow(targetItem, player);
                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "name":
                  targetItem.Name = name.GetBindValue(player.oid, nuiToken.Token);
                  break;

                case "tag":
                  targetItem.Tag = tag.GetBindValue(player.oid, nuiToken.Token);
                  break;

                case "size":

                  if (float.TryParse(size.GetBindValue(player.oid, nuiToken.Token), out float newSize))
                  {
                    targetItem.VisualTransform.Scale = newSize > 100 ? 100 : newSize;
                    targetItem.GetObjectVariable<LocalVariableFloat>("_ITEM_SCALE").Value = newSize;
                  }

                  break;

                case "baseItemSelected":
                  targetItem.BaseItem = NwBaseItem.FromItemId(baseItemSelected.GetBindValue(player.oid, nuiToken.Token));
                  NwItem newItem = targetItem.Clone(player.oid.ControlledCreature);
                  targetItem.Destroy();
                  targetItem = newItem;
                  break;

                case "cost":

                  if (int.TryParse(cost.GetBindValue(player.oid, nuiToken.Token), out int newCost))
                    targetItem.GetObjectVariable<LocalVariableInt>("_ITEM_COST").Value = newCost;

                  break;

                case "weight":

                  if (decimal.TryParse(weight.GetBindValue(player.oid, nuiToken.Token), out decimal newWeight))
                    targetItem.Weight = newWeight;

                  break;

                case "charges":

                  if (int.TryParse(charges.GetBindValue(player.oid, nuiToken.Token), out int newCharges))
                    targetItem.ItemCharges = newCharges;

                  break;

                case "undroppableChecked":
                  targetItem.CursedFlag = undroppableChecked.GetBindValue(player.oid, nuiToken.Token);
                  break;

                case "identifiedChecked":
                  targetItem.Identified = identifiedChecked.GetBindValue(player.oid, nuiToken.Token);
                  break;
              }


              break;
          }
        }

        private void LoadButtons()
        {
          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Base") { Id = "base", Height = 35, Width = 90 },
              new NuiButton("Propriétés") { Id = "properties", Height = 35, Width = 90, Enabled = visibilityDM },
              new NuiButton("Description") { Id = "description", Height = 35, Width = 90 },
              new NuiButton("Variables") { Id = "variables", Height = 35, Width = 90, Enabled = visibilityDM },
              new NuiSpacer()
            }
          });
        }

        private void LoadBaseLayout()
        {
          rootChildren.Clear();

          LoadButtons();

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiLabel("Nom") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
            new NuiTextEdit("Nom", name, 25, false) { Height = 35, Width = 200 }
          } });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Tag") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Visible = visibilityDM },
              new NuiTextEdit("Tag", tag, 30, false) { Height = 35, Width = 200, Visible = visibilityDM }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Type") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiCombo() { Height = 35, Width = 200, Entries = BaseItems2da.baseItemNameEntries, Selected = baseItemSelected,  Enabled = visibilityDM },
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Taille") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Taille", size, 4, false) { Height = 35, Width = 200, Enabled = visibilityDM }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Coût") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Coût", cost, 7, false) { Height = 35, Width = 200, Enabled = visibilityDM }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Poids") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Poids", weight, 4, false) { Height = 35, Width = 200, Enabled = visibilityDM }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Charges") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Charges", charges, 30, false) { Height = 35, Width = 200, Enabled = visibilityDM }
            }
          });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiCheck("Inéchangeable", undroppableChecked) { Height = 35, Width = 120, Enabled = visibilityDM },
            new NuiCheck("Identifié", identifiedChecked) { Height = 35, Width = 120, Enabled = visibilityDM },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("Apparence") { Id = "appearance", Height = 35, Width = 120, Enabled = modificationAllowed },
            new NuiSpacer()
          } });
        }
        private void StopAllWatchBindings()
        {
          name.SetBindWatch(player.oid, nuiToken.Token, false);
          tag.SetBindWatch(player.oid, nuiToken.Token, false);
          baseItemSelected.SetBindWatch(player.oid, nuiToken.Token, false);
          size.SetBindWatch(player.oid, nuiToken.Token, false);
          cost.SetBindWatch(player.oid, nuiToken.Token, false);
          weight.SetBindWatch(player.oid, nuiToken.Token, false);
          charges.SetBindWatch(player.oid, nuiToken.Token, false);
          undroppableChecked.SetBindWatch(player.oid, nuiToken.Token, false);
          identifiedChecked.SetBindWatch(player.oid, nuiToken.Token, false);
          ipDetailVisibility.SetBindValue(player.oid, nuiToken.Token, false);
        }
        private void LoadBaseBinding()
        {
          StopAllWatchBindings();

          name.SetBindValue(player.oid, nuiToken.Token, targetItem.Name);
          name.SetBindWatch(player.oid, nuiToken.Token, true);
          tag.SetBindValue(player.oid, nuiToken.Token, targetItem.Tag);
          tag.SetBindWatch(player.oid, nuiToken.Token, true);
          baseItemSelected.SetBindValue(player.oid, nuiToken.Token, (int)targetItem.BaseItem.Id);
          baseItemSelected.SetBindWatch(player.oid, nuiToken.Token, true);
          size.SetBindValue(player.oid, nuiToken.Token, targetItem.VisualTransform.Scale.ToString());
          size.SetBindWatch(player.oid, nuiToken.Token, true);
          cost.SetBindValue(player.oid, nuiToken.Token, targetItem.GetObjectVariable<LocalVariableInt>("_ITEM_COST").HasValue ? targetItem.GetObjectVariable<LocalVariableInt>("_ITEM_COST").Value.ToString() : targetItem.BaseGoldValue.ToString());
          cost.SetBindWatch(player.oid, nuiToken.Token, true);
          weight.SetBindValue(player.oid, nuiToken.Token, targetItem.Weight.ToString());
          weight.SetBindWatch(player.oid, nuiToken.Token, true);
          charges.SetBindValue(player.oid, nuiToken.Token, targetItem.ItemCharges.ToString());
          charges.SetBindWatch(player.oid, nuiToken.Token, true);
          undroppableChecked.SetBindValue(player.oid, nuiToken.Token, targetItem.Droppable);
          identifiedChecked.SetBindValue(player.oid, nuiToken.Token, targetItem.Identified);
          undroppableChecked.SetBindWatch(player.oid, nuiToken.Token, true);
          identifiedChecked.SetBindWatch(player.oid, nuiToken.Token, true);
          string originalCrafterName = targetItem.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value;
          modificationAllowed.SetBindValue(player.oid, nuiToken.Token, (string.IsNullOrWhiteSpace(originalCrafterName) || originalCrafterName == player.oid.ControlledCreature.OriginalName)
            && (targetItem.Possessor == player.oid.ControlledCreature || player.IsDm()));
        }
        private void LoadItemPropertyLayout()
        {
          rootChildren.Clear();
          LoadButtons();
          rowTemplate.Clear();

          rootChildren.Add(new NuiRow()
          {
            Visible = ipDetailVisibility,
            Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel(selectedIP) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = 35, Tooltip = selectedIP  },
            new NuiCombo() { Height = 35, Width = 100, Entries = subTypeBind, Selected = subTypeSelected },
            new NuiCombo() { Height = 35, Width = 100, Entries = costValueBind, Selected = costValueSelected },
            new NuiCombo() { Height = 35, Width = 100, Entries = paramValueBind, Selected = paramValueSelected },
            new NuiButtonImage("ir_empytqs") { Id = "setIPDetail", Height = 35, Width = 35, Tooltip = "Valider la modification" },
            new NuiSpacer()
          }
          });

          rowTemplate.Add(new NuiListTemplateCell(new NuiButton(availableIP) { Id = "addIP", Tooltip = addTooltip }) { VariableSize = true });

          List<NuiListTemplateCell> rowTemplateAcquiredIP = new()
          {
            new NuiListTemplateCell(new NuiButton(acquiredIP) { Id = "openIP", Tooltip = acquiredIP }) { VariableSize = true },
            new NuiListTemplateCell(new NuiButtonImage("ir_ban") { Id = "removeIP", Tooltip = "Supprimer" }) { Width = 35 }
          };

          rootChildren.Add(new NuiRow()
          {
            Height = 380,
            Children = new List<NuiElement>()
          {
            new NuiColumn() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 390  } } },
            new NuiColumn() { Children = new List<NuiElement>() { new NuiList(rowTemplateAcquiredIP, listAcquiredIPCount) { RowHeight = 35, Width = 390 } } }
          }
          });
        }
        private void LoadItemPropertyBinding()
        {
          List<string> acquiredIPNames = new();
          List<string> availableIPNames = new();
          List<string> addTooltipList = new();

          StopAllWatchBindings();
          acquiredIPList.Clear();

          foreach (var ip in targetItem.ItemProperties)
          {
            if (ip.DurationType == EffectDuration.Permanent)
            {
              acquiredIPList.Add(ip);

              string ipName = $"{ip.Property.Name?.ToString()}";

              if (ip?.SubType?.RowIndex > -1)
                ipName += $" : {NwGameTables.ItemPropertyTable.GetRow(ip.Property.RowIndex).SubTypeTable?.GetRow(ip.SubType.RowIndex).Name?.ToString()}";

              ipName += " " + ip.CostTableValue?.Name?.ToString();
              ipName += " " + ip.Param1TableValue?.Name?.ToString();

              acquiredIPNames.Add(ipName);
            }
          }

          foreach (var ip in availableIPList)
          {
            availableIPNames.Add(ip.Name.Value.ToString());
            addTooltipList.Add($"Ajouter (id {ip.RowIndex})");
          }

          availableIP.SetBindValues(player.oid, nuiToken.Token, availableIPNames);
          addTooltip.SetBindValues(player.oid, nuiToken.Token, addTooltipList);
          acquiredIP.SetBindValues(player.oid, nuiToken.Token, acquiredIPNames);
          listCount.SetBindValue(player.oid, nuiToken.Token, availableIPNames.Count);
          listAcquiredIPCount.SetBindValue(player.oid, nuiToken.Token, acquiredIPNames.Count);
        }
        private void LoadDescriptionLayout()
        {
          rootChildren.Clear();
          LoadButtons();

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiTextEdit("Description", itemDescription, 999, true) { Height = 170, Width = 780 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiTextEdit("Commentaire", itemComment, 999, true) { Height = 170, Width = 780 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButtonImage("ir_empytqs") { Id = "saveDescription", Tooltip = "Enregistrer la description et le commentaire de cet objet", Height = 35, Width = 35 },
              new NuiSpacer()
            }
          });
        }

        private void LoadDescriptionBinding()
        {
          StopAllWatchBindings();

          itemDescription.SetBindValue(player.oid, nuiToken.Token, targetItem.Description);
          itemComment.SetBindValue(player.oid, nuiToken.Token, targetItem.GetObjectVariable<LocalVariableString>("_COMMENT").Value);
        }
        private void LoadVariablesLayout()
        {
          rootChildren.Clear();
          LoadButtons();
          rowTemplate.Clear();

          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("Nom", variableName, 20, false) { Tooltip = variableName }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiCombo() { Entries = Utils.variableTypes, Selected = selectedVariableType, Width = 60 }));
          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("Valeur", variableValue, 20, false) { Tooltip = variableValue }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_empytqs") { Id = "saveVariable", Tooltip = "Sauvegarder" }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_ban") { Id = "deleteVariable", Tooltip = "Supprimer" }) { Width = 35 });

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          columnsChildren.Add(new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiTextEdit("Nom", newVariableName, 20, false) { Tooltip = newVariableName, Width = 120 },
                new NuiCombo() { Entries = Utils.variableTypes, Selected = selectedNewVariableType, Width = 80 },
                new NuiTextEdit("Valeur", newVariableValue, 20, false) { Tooltip = newVariableValue, Width = 120 },
                new NuiButtonImage("ir_empytqs") { Id = "saveNewVariable", Height = 35, Width = 35 },
                new NuiSpacer()
              }
            },
              new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 780  } } }
            }
          }); ;
        }
        private void LoadVariablesBinding()
        {
          StopAllWatchBindings();

          List<string> variableNameList = new();
          List<int> selectedVariableTypeList = new();
          List<string> variableValueList = new();
          int count = 0;

          foreach (var variable in targetItem.LocalVariables)
          {
            switch (variable)
            {
              case LocalVariableString stringVar:

                if (stringVar.Name == "DM_ITEM_CREATED_BY" && player.oid.PlayerName != "Chim")
                  continue;

                variableValueList.Add(stringVar.Value);
                selectedVariableTypeList.Add(2);
                break;
              case LocalVariableInt intVar:
                variableValueList.Add(intVar.Value.ToString());
                selectedVariableTypeList.Add(1);
                break;
              case LocalVariableFloat floatVar:
                variableValueList.Add(floatVar.Value.ToString());
                selectedVariableTypeList.Add(3);
                break;
              case DateTimeLocalVariable dateVar:
                variableValueList.Add(dateVar.Value.ToString());
                selectedVariableTypeList.Add(4);
                break;

              default:
                continue;
            }

            variableNameList.Add(variable.Name);
            count++;
          }

          variableName.SetBindValues(player.oid, nuiToken.Token, variableNameList);
          selectedVariableType.SetBindValues(player.oid, nuiToken.Token, selectedVariableTypeList);
          variableValue.SetBindValues(player.oid, nuiToken.Token, variableValueList);
          listCount.SetBindValue(player.oid, nuiToken.Token, count);
        }
      }
    }
  }
}
