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
      public class EditorItemWindow : PlayerWindow
      {
        private enum Tab
        {
          Base,
          Propriétés,
          Description,
          Variables
        }

        private NwItem targetItem;
        private readonly NuiGroup rootGroup = new() { Id = "rootGroup", Border = false, Padding = 0, Margin = 0 };
        private readonly NuiColumn layoutColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();

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

        private readonly NuiBind<int> listAcquiredFeatCount = new("listAcquiredFeatCount");
        private readonly NuiBind<string> availableFeatIcons = new("availableFeatIcons");
        private readonly NuiBind<string> acquiredFeatIcons = new("acquiredFeatIcons");
        private readonly NuiBind<string> availableFeatNames = new("availableFeatNames");
        private readonly NuiBind<string> acquiredFeatNames = new("acquiredFeatNames");
        private readonly NuiBind<string> availableFeatSearch = new("availableFeatSearch");
        private readonly NuiBind<string> acquiredFeatSearch = new("acquiredFeatSearch");

        private readonly NuiBind<string> itemDescription = new("itemDescription");
        private readonly NuiBind<string> itemComment = new("itemComment");

        private readonly List<NwFeat> availableFeats = new();
        private readonly List<NwFeat> acquiredFeats = new();
        private List<NwFeat> availableFeatSearcher = new();
        private List<NwFeat> acquiredFeatSearcher = new();

        public readonly List<NuiComboEntry> variableTypes = new()
        {
          new NuiComboEntry("bool", 0),
          new NuiComboEntry("int", 1),
          new NuiComboEntry("string", 2),
          new NuiComboEntry("float", 3),
          new NuiComboEntry("date", 4)
        };
        private readonly NuiBind<string> variableName = new("variableName");
        private readonly NuiBind<string> variableValue = new("variableValue");
        private readonly NuiBind<int> selectedVariableType = new("selectedVariableType");

        private readonly NuiBind<string> newVariableName = new("newVariableName");
        private readonly NuiBind<string> newVariableValue = new("newVariableValue");
        private readonly NuiBind<int> selectedNewVariableType = new("selectedNewVariableType");

        Tab currentTab;

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

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, 500);

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
            currentTab = Tab.Base;

            nuiToken.OnNuiEvent += HandleEditorItemEvents;

            LoadBaseBinding();

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
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
                  currentTab = Tab.Base;
                  LoadBaseLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadBaseBinding();
                  break;

                case "properties":
                  currentTab = Tab.Propriétés;
                  //LoadPortraitLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  //LoadPortraitBinding();
                  break;

                case "description":
                  currentTab = Tab.Description;
                  LoadDescriptionLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadDescriptionBinding();
                  break;

                case "variables":

                  currentTab = Tab.Variables;
                  LoadVariablesLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadVariablesBinding();

                  break;

                case "selectFeat":
                  NwFeat acquiredFeat = availableFeatSearcher[nuiEvent.ArrayIndex];

                  //targetItem.AddFeat(acquiredFeat);

                  if (!acquiredFeats.Contains(acquiredFeat))
                    acquiredFeats.Add(acquiredFeat);

                  if (!acquiredFeatSearcher.Contains(acquiredFeat))
                    acquiredFeatSearcher.Add(acquiredFeat);

                  availableFeats.Remove(acquiredFeat);
                  availableFeatSearcher.Remove(acquiredFeat);

                  var tempIcon = availableFeatIcons.GetBindValues(player.oid, nuiToken.Token);
                  tempIcon.RemoveAt(nuiEvent.ArrayIndex);
                  var tempName = availableFeatNames.GetBindValues(player.oid, nuiToken.Token);
                  tempName.RemoveAt(nuiEvent.ArrayIndex);

                  availableFeatIcons.SetBindValues(player.oid, nuiToken.Token, tempIcon);
                  availableFeatNames.SetBindValues(player.oid, nuiToken.Token, tempName);
                  listCount.SetBindValue(player.oid, nuiToken.Token, tempName.Count);

                  tempIcon = acquiredFeatIcons.GetBindValues(player.oid, nuiToken.Token);
                  tempIcon.Add(acquiredFeat.IconResRef);
                  tempName = acquiredFeatNames.GetBindValues(player.oid, nuiToken.Token);
                  tempName.Add(acquiredFeat.Name.ToString());

                  acquiredFeatIcons.SetBindValues(player.oid, nuiToken.Token, tempIcon);
                  acquiredFeatNames.SetBindValues(player.oid, nuiToken.Token, tempName);
                  listAcquiredFeatCount.SetBindValue(player.oid, nuiToken.Token, tempName.Count);

                  break;

                case "removeFeat":
                  NwFeat removedFeat = acquiredFeatSearcher[nuiEvent.ArrayIndex];

                  //targetItem.RemoveFeat(removedFeat);

                  if (!availableFeats.Contains(removedFeat))
                    availableFeats.Add(removedFeat);

                  if (!availableFeatSearcher.Contains(removedFeat))
                    availableFeatSearcher.Add(removedFeat);

                  acquiredFeats.Remove(removedFeat);
                  acquiredFeatSearcher.Remove(removedFeat);

                  var tempIconList = acquiredFeatIcons.GetBindValues(player.oid, nuiToken.Token);
                  tempIconList.RemoveAt(nuiEvent.ArrayIndex);
                  var tempNameList = acquiredFeatNames.GetBindValues(player.oid, nuiToken.Token);
                  tempNameList.RemoveAt(nuiEvent.ArrayIndex);

                  acquiredFeatIcons.SetBindValues(player.oid, nuiToken.Token, tempIconList);
                  acquiredFeatNames.SetBindValues(player.oid, nuiToken.Token, tempNameList);
                  listAcquiredFeatCount.SetBindValue(player.oid, nuiToken.Token, tempNameList.Count);

                  tempIconList = availableFeatIcons.GetBindValues(player.oid, nuiToken.Token);
                  tempIconList.Add(removedFeat.IconResRef);
                  tempNameList = availableFeatNames.GetBindValues(player.oid, nuiToken.Token);
                  tempNameList.Add(removedFeat.Name.ToString());

                  availableFeatIcons.SetBindValues(player.oid, nuiToken.Token, tempIconList);
                  availableFeatNames.SetBindValues(player.oid, nuiToken.Token, tempNameList);
                  listCount.SetBindValue(player.oid, nuiToken.Token, tempNameList.Count);

                  break;

                case "saveDescription":
                  targetItem.Description = itemDescription.GetBindValue(player.oid, nuiToken.Token);
                  targetItem.GetObjectVariable<LocalVariableString>("_COMMENT").Value = itemComment.GetBindValue(player.oid, nuiToken.Token);
                  player.oid.SendServerMessage($"La description et le commentaire de la créature {targetItem.Name.ColorString(ColorConstants.White)} ont bien été enregistrées.", new Color(32, 255, 32));
                  break;

                case "saveNewVariable":
                  ConvertLocalVariable(newVariableName.GetBindValue(player.oid, nuiToken.Token), newVariableValue.GetBindValue(player.oid, nuiToken.Token));
                  LoadVariablesBinding();
                  break;

                case "saveVariable":
                  ConvertLocalVariable(variableName.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex], variableValue.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex]);
                  break;

                case "deleteVariable":
                  targetItem.LocalVariables.ElementAt(nuiEvent.ArrayIndex).Delete();
                  LoadVariablesBinding();
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
              new NuiButton("Base") { Id = "base", Height = 35, Width = 60 },
              new NuiButton("Propriétés") { Id = "properties", Height = 35, Width = 60 },
              new NuiButton("Description") { Id = "description", Height = 35, Width = 60 },
              new NuiButton("Variables") { Id = "variables", Height = 35, Width = 60 },
              new NuiSpacer()
            }
          });
        }

        private void LoadBaseLayout()
        {
          rootChildren.Clear();

          LoadButtons();

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Nom") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Nom", name, 25, false) { Height = 35, Width = 200 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Tag") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Tag", tag, 30, false) { Height = 35, Width = 200 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Objet de base") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiCombo() { Height = 35, Width = 200, Entries = BaseItems2da.baseItemNameEntries, Selected = baseItemSelected },
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Coût") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Coût", cost, 30, false) { Height = 35, Width = 200 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Poids") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Poids", weight, 30, false) { Height = 35, Width = 200 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Charges") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Charges", charges, 30, false) { Height = 35, Width = 200 }
            }
          });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCheck("Inéchangeable", undroppableChecked) { Height = 35, Width = 35 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCheck("Identifié", identifiedChecked) { Height = 35, Width = 35 } } });
        }
        private void StopAllWatchBindings()
        {
          name.SetBindWatch(player.oid, nuiToken.Token, false);
          tag.SetBindWatch(player.oid, nuiToken.Token, false);

          availableFeatSearch.SetBindWatch(player.oid, nuiToken.Token, false);
          acquiredFeatSearch.SetBindWatch(player.oid, nuiToken.Token, false);
        }
        private void LoadBaseBinding()
        {
          StopAllWatchBindings();

          name.SetBindValue(player.oid, nuiToken.Token, targetItem.Name);
          name.SetBindWatch(player.oid, nuiToken.Token, true);
          tag.SetBindValue(player.oid, nuiToken.Token, targetItem.Tag);
          tag.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        private void LoadFeatsLayout()
        {
          rootChildren.Clear();
          LoadButtons();
          rowTemplate.Clear();

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(availableFeatIcons) { Id = "selectFeat", Tooltip = "Ajouter" }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(availableFeatNames) { Id = "availableFeatDescription", Tooltip = availableFeatNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true });

          List<NuiListTemplateCell> rowTemplateAcquiredFeats = new()
          {
            new NuiListTemplateCell(new NuiButtonImage(acquiredFeatIcons) { Id = "removeFeat", Tooltip = "Supprimer" }) { Width = 35 },
            new NuiListTemplateCell(new NuiLabel(acquiredFeatNames) { Id = "acquiredFeatDescription", Tooltip = acquiredFeatNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true }
          };

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          columnsChildren.Add(new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Dons disponibles", availableFeatSearch, 20, false) { Width = 190 } } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 190  } } }
            }
          });

          columnsChildren.Add(new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Dons acquis", acquiredFeatSearch, 20, false) { Width = 190 } } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplateAcquiredFeats, listAcquiredFeatCount) { RowHeight = 35, Width = 190 } } }
            }
          });
        }
        private void LoadFeatsBinding()
        {
          StopAllWatchBindings();

          availableFeats.Clear();
          acquiredFeats.Clear();

          availableFeatSearch.SetBindValue(player.oid, nuiToken.Token, "");
          availableFeatSearch.SetBindWatch(player.oid, nuiToken.Token, true);
          acquiredFeatSearch.SetBindValue(player.oid, nuiToken.Token, "");
          acquiredFeatSearch.SetBindWatch(player.oid, nuiToken.Token, true);

          List<string> availableIconsList = new();
          List<string> availableNamesList = new();
          List<string> acquiredIconsList = new();
          List<string> acquiredNamesList = new();

          /*foreach (Feat feat in (Feat[])Enum.GetValues(typeof(Feat)))
          {
            NwFeat baseFeat = NwFeat.FromFeatType(feat);

            if (targetItem.KnowsFeat(baseFeat))
            {
              acquiredIconsList.Add(baseFeat.IconResRef);
              acquiredNamesList.Add(baseFeat.Name.ToString().Replace("’", "'"));
              acquiredFeats.Add(baseFeat);
            }
            else
            {
              availableIconsList.Add(baseFeat.IconResRef);
              availableNamesList.Add(baseFeat.Name.ToString().Replace("’", "'"));
              availableFeats.Add(baseFeat);
            }
        }*/

          availableFeatIcons.SetBindValues(player.oid, nuiToken.Token, availableIconsList);
          availableFeatNames.SetBindValues(player.oid, nuiToken.Token, availableNamesList);
          acquiredFeatIcons.SetBindValues(player.oid, nuiToken.Token, acquiredIconsList);
          acquiredFeatNames.SetBindValues(player.oid, nuiToken.Token, acquiredNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, availableFeats.Count);
          listAcquiredFeatCount.SetBindValue(player.oid, nuiToken.Token, acquiredFeats.Count);

          availableFeatSearcher = availableFeats;
          acquiredFeatSearcher = acquiredFeats;
        }
        private void LoadDescriptionLayout()
        {
          rootChildren.Clear();
          LoadButtons();

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiTextEdit("Description", itemDescription, 999, true) { Height = 200, Width = 400 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiTextEdit("Commentaire", itemComment, 999, true) { Height = 200, Width = 400 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Sauvegarder") { Id = "saveDescription", Tooltip = "Enregistrer la description et le commentaire de cette créature", Height = 35, Width = 120 },
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
          rowTemplate.Add(new NuiListTemplateCell(new NuiCombo() { Entries = variableTypes, Selected = selectedVariableType, Width = 80 }));
          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("Valeur", variableValue, 20, false) { Tooltip = variableValue }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButton("Save") { Id = "saveVariable" }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButton("Delete") { Id = "deleteVariable" }) { Width = 35 });

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          columnsChildren.Add(new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiTextEdit("Nom", newVariableName, 20, false) { Tooltip = newVariableName, Width = 120 },
                new NuiCombo() { Entries = variableTypes, Selected = selectedNewVariableType, Width = 80 },
                new NuiTextEdit("Valeur", newVariableValue, 20, false) { Tooltip = newVariableValue, Width = 120 },
                new NuiButton("Save") { Id = "saveNewVariable", Width = 35 },
              }
            },
              new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 380  } } }
            }
          });
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
                variableValueList.Add(stringVar);
                selectedVariableTypeList.Add(2);
                break;
              case LocalVariableInt intVar:
                variableValueList.Add(intVar.ToString());
                selectedVariableTypeList.Add(1);
                break;
              case LocalVariableFloat floatVar:
                variableValueList.Add(floatVar.ToString());
                selectedVariableTypeList.Add(3);
                break;
              case LocalVariableBool boolVar:
                variableValueList.Add(boolVar.ToString());
                selectedVariableTypeList.Add(0);
                break;
              case DateTimeLocalVariable dateVar:
                variableValueList.Add(dateVar.ToString());
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
        private void ConvertLocalVariable(string localName, string localValue)
        {
          switch (selectedNewVariableType.GetBindValue(player.oid, nuiToken.Token))
          {
            case 0:
              targetItem.GetObjectVariable<LocalVariableBool>(localName).Value = Convert.ToBoolean(localValue);
              break;

            case 1:
              if (int.TryParse(localValue, out int parsedInt))
                targetItem.GetObjectVariable<LocalVariableInt>(localName).Value = parsedInt;
              else
                player.oid.SendServerMessage($"{localName.ColorString(ColorConstants.White)} : la valeur {localValue.ColorString(ColorConstants.White)} n'est pas un entier.", ColorConstants.Red);
              break;

            case 2:
              targetItem.GetObjectVariable<LocalVariableString>(localName).Value = localValue;
              break;

            case 3:
              if (float.TryParse(localValue, out float parsedFloat))
                targetItem.GetObjectVariable<LocalVariableFloat>(localName).Value = parsedFloat;
              else
                player.oid.SendServerMessage($"{localName.ColorString(ColorConstants.White)} : la valeur {localValue.ColorString(ColorConstants.White)} n'est pas un float.", ColorConstants.Red);
              break;

            case 4:
              if (DateTime.TryParse(localValue, out DateTime parsedDate))
                targetItem.GetObjectVariable<DateTimeLocalVariable>(localName).Value = parsedDate;
              else
                player.oid.SendServerMessage($"{localName.ColorString(ColorConstants.White)} : la valeur {localValue.ColorString(ColorConstants.White)} n'est pas une date.", ColorConstants.Red);
              break;
          }
        }
      }
    }
  }
}
