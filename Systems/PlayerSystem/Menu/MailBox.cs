using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using Color = Anvil.API.Color;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class MailBox : PlayerWindow
      {
        private readonly NuiGroup rootGroup = new() { Id = "rootGroup", Border = false, Padding = 0, Margin = 0 };
        private readonly NuiColumn layoutColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();
        private readonly NuiBind<int> listCount = new("listCount");

        private readonly NuiBind<string> title = new("title");
        private readonly NuiBind<string> content = new("content");
        private readonly NuiBind<string> expeditorName = new("expeditorName");
        private readonly NuiBind<string> receivedDate = new("receiveDate");
        private readonly NuiBind<string> unread = new("unread");

        public MailBox(Player player) : base(player)
        {
          windowId = "mailBox";

          rootGroup.Layout = layoutColumn;
          layoutColumn.Children = rootChildren;

          CreateWindow();
        }
        public void CreateWindow()
        {
          LoadInboxLayout();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, 500);

          window = new NuiWindow(rootGroup, "Boîte aux lettres")
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

            LoadDescriptionBinding();

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleEditorItemEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
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

                case "description":
                  LoadDescriptionLayout();
                  rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
                  LoadDescriptionBinding();
                  break;

                case "variables":
                  LoadVariablesLayout();
                  rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
                  LoadVariablesBinding();

                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "name":

                  break;

                case "tag":

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
              new NuiButton("Réception") { Id = "receive", Height = 35, Width = 90 },
              new NuiButton("Envoi") { Id = "send", Height = 35, Width = 90 },
              new NuiSpacer()
            }
          });
        }

        private void LoadInboxLayout()
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
            new NuiLabel("Apparence") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
            new NuiCombo() { Height = 35, Width = 200, Entries = apparence, Selected = apparenceSelected },
            new NuiTextEdit("Recherche", apparenceSearch, 20, false) { Height = 35, Width = 100 }
          }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiCheck("Indestructible", plotChecked) { Height = 35, Width = 120 },
            new NuiLabel("Résistance") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Visible = statVisible, Tooltip = "Doit être compris entre 0 et 250" },
            new NuiTextEdit("Résistance", hardness, 3, false) { Height = 35, Width = 35, Visible = statVisible },
            new NuiLabel("HP") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Visible = statVisible, Tooltip = "Doit être compris entre 0 et 10000" },
            new NuiTextEdit("HP", hitPoints, 5, false) { Height = 35, Width = 35, Visible = statVisible },
          }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiCheck("Utilisable", useableChecked) { Height = 35, Width = 90 },
            new NuiCheck("Inventaire", hasInventoryChecked) { Height = 35, Width = 90, Visible = inventoryVisible },
            new NuiCheck("Persistant", persistantChecked) { Height = 35, Width = 90 },
            new NuiButtonImage("ir_empytqs") { Id = "updatePersistantPlc", Height = 35, Width = 35, Visible = updateVisibility, Tooltip = "Mettre à jour le placeable persistant. Utile si des modifications ont été faites depuis la dernière sauvegarde." },
          }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiLabel("Orientation") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Tooltip = "Doit être compris entre 0 et 360.0" },
            new NuiTextEdit("Orientation", orientation, 5, false) { Height = 35, Width = 100 },
            new NuiLabel("Taille") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Tooltip = "Doit être compris entre 0.01 et 99.99" },
            new NuiTextEdit("Taille", scale, 5, false) { Height = 35, Width = 100 }
          }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiLabel("Position") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
            new NuiTextEdit("X", xPosition, 5, false) { Height = 35, Width = 100, Tooltip = "Doit être compris entre 0 et 50.0" },
            new NuiTextEdit("Y", yPosition, 5, false) { Height = 35, Width = 100, Tooltip = "Doit être compris entre 0 et 60.0" },
            new NuiTextEdit("Z", zPosition, 5, false) { Height = 35, Width = 100, Tooltip = "Doit être compris entre -10 et 100" }
          }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiLabel("Rotation") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Tooltip = "Doit être compris entre 0 et 360.0" },
            new NuiTextEdit("X", xRotation, 5, false) { Height = 35, Width = 100 },
            new NuiTextEdit("Y", yRotation, 5, false) { Height = 35, Width = 100 },
            new NuiTextEdit("Z", zRotation, 5, false) { Height = 35, Width = 100 }
          }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiLabel("Translation") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
            new NuiTextEdit("X", xTranslation, 5, false) { Height = 35, Width = 100, Tooltip = "Doit être compris entre -50.00 et 50.00" },
            new NuiTextEdit("Y", yTranslation, 5, false) { Height = 35, Width = 100, Tooltip = "Doit être compris entre -60.00 et 60.00" },
            new NuiTextEdit("Z", zTranslation, 5, false) { Height = 35, Width = 100, Tooltip = "Doit être compris entre -10.00 et 100" }
          }
          });
        }
        private void StopAllWatchBindings()
        {
          name.SetBindWatch(player.oid, nuiToken.Token, false);
          tag.SetBindWatch(player.oid, nuiToken.Token, false);
          apparenceSelected.SetBindWatch(player.oid, nuiToken.Token, false);
          apparenceSearch.SetBindWatch(player.oid, nuiToken.Token, false);
          plotChecked.SetBindWatch(player.oid, nuiToken.Token, false);
          hardness.SetBindWatch(player.oid, nuiToken.Token, false);
          hitPoints.SetBindWatch(player.oid, nuiToken.Token, false);
          useableChecked.SetBindWatch(player.oid, nuiToken.Token, false);
          hasInventoryChecked.SetBindWatch(player.oid, nuiToken.Token, false);
          scale.SetBindWatch(player.oid, nuiToken.Token, false);
          xRotation.SetBindWatch(player.oid, nuiToken.Token, false);
          yRotation.SetBindWatch(player.oid, nuiToken.Token, false);
          zRotation.SetBindWatch(player.oid, nuiToken.Token, false);
          xTranslation.SetBindWatch(player.oid, nuiToken.Token, false);
          yTranslation.SetBindWatch(player.oid, nuiToken.Token, false);
          zTranslation.SetBindWatch(player.oid, nuiToken.Token, false);
          orientation.SetBindWatch(player.oid, nuiToken.Token, false);
          xPosition.SetBindWatch(player.oid, nuiToken.Token, false);
          yPosition.SetBindWatch(player.oid, nuiToken.Token, false);
          zPosition.SetBindWatch(player.oid, nuiToken.Token, false);
          persistantChecked.SetBindWatch(player.oid, nuiToken.Token, false);
        }
        private void LoadBaseBinding()
        {
          StopAllWatchBindings();

          name.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.Name);
          name.SetBindWatch(player.oid, nuiToken.Token, true);
          tag.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.Tag);
          tag.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        private void LoadDescriptionLayout()
        {
          rootChildren.Clear();
          LoadButtons();

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiTextEdit("Description", itemDescription, 999, true) { Height = 170, Width = 390 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiTextEdit("Commentaire", itemComment, 999, true) { Height = 170, Width = 390 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButtonImage("ir_empytqs") { Id = "saveDescription", Tooltip = "Enregistrer la description et le commentaire de cette créature", Height = 35, Width = 35 },
              new NuiSpacer()
            }
          });
        }

        private void LoadDescriptionBinding()
        {
          StopAllWatchBindings();

          itemDescription.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.Description);
          itemComment.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.GetObjectVariable<LocalVariableString>("_COMMENT").Value);
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
                new NuiTextEdit("Nom", newVariableName, 20, false) { Tooltip = newVariableName, Width = 120 },
                new NuiCombo() { Entries = Utils.variableTypes, Selected = selectedNewVariableType, Width = 80 },
                new NuiTextEdit("Valeur", newVariableValue, 20, false) { Tooltip = newVariableValue, Width = 120 },
                new NuiButtonImage("ir_empytqs") { Id = "saveNewVariable", Height = 35, Width = 35 },
              }
            },
              new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 380 } } }
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

          foreach (var variable in targetPlaceable.LocalVariables)
          {
            switch (variable)
            {
              case LocalVariableString stringVar:

                if (stringVar.Name == "ITEM_KEY")
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
