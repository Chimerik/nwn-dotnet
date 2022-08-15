using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Threading;

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
      public class EditorPlaceableWindow : PlayerWindow
      {
        private NwPlaceable targetPlaceable;
        private readonly NuiGroup rootGroup = new() { Id = "rootGroup", Border = false, Padding = 0, Margin = 0 };
        private readonly NuiColumn layoutColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();
        private readonly NuiBind<int> listCount = new("listCount");

        private readonly NuiBind<string> name = new("name");
        private readonly NuiBind<string> tag = new("tag");

        private readonly NuiBind<string> apparenceSearch = new("apparenceSearch");
        private readonly NuiBind<List<NuiComboEntry>> apparence = new("apparence");
        private readonly NuiBind<int> apparenceSelected = new("apparenceSelected");

        private readonly NuiBind<bool> plotChecked = new("plotChecked");
        private readonly NuiBind<bool> statVisible = new("statVisible");
        private readonly NuiBind<bool> useableChecked = new("useableChecked");
        private readonly NuiBind<bool> hasInventoryChecked = new("hasInventoryChecked");
        private readonly NuiBind<bool> inventoryVisible = new("inventoryVisible");
        private readonly NuiBind<bool> persistantChecked = new("persistantChecked");
        private readonly NuiBind<bool> updateVisibility = new("updateVisibility");

        private readonly NuiBind<string> hardness = new("hardness");
        private readonly NuiBind<string> hitPoints = new("hitPoints");
        private readonly NuiBind<string> scale = new("scale");
        private readonly NuiBind<string> orientation = new("orientation");
        private readonly NuiBind<string> xPosition = new("xPosition");
        private readonly NuiBind<string> yPosition = new("yPosition");
        private readonly NuiBind<string> zPosition = new("zPosition");
        private readonly NuiBind<string> xRotation = new("xRotation");
        private readonly NuiBind<string> yRotation = new("yRotation");
        private readonly NuiBind<string> zRotation = new("zRotation");
        private readonly NuiBind<string> xTranslation = new("xTranslation");
        private readonly NuiBind<string> yTranslation = new("yTranslation");
        private readonly NuiBind<string> zTranslation = new("zTranslation");

        private readonly NuiBind<string> itemDescription = new("itemDescription");
        private readonly NuiBind<string> itemComment = new("itemComment");

        private readonly NuiBind<string> variableName = new("variableName");
        private readonly NuiBind<string> variableValue = new("variableValue");
        private readonly NuiBind<int> selectedVariableType = new("selectedVariableType");

        private readonly NuiBind<string> newVariableName = new("newVariableName");
        private readonly NuiBind<string> newVariableValue = new("newVariableValue");
        private readonly NuiBind<int> selectedNewVariableType = new("selectedNewVariableType");

        public EditorPlaceableWindow(Player player, NwPlaceable targetPlaceable) : base(player)
        {
          windowId = "editorPlaceable";

          rootGroup.Layout = layoutColumn;
          layoutColumn.Children = rootChildren;

          CreateWindow(targetPlaceable);
        }
        public void CreateWindow(NwPlaceable targetPlaceable)
        {
          this.targetPlaceable = targetPlaceable;

          LoadBaseLayout();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, 500);

          window = new NuiWindow(rootGroup, $"Modification de {targetPlaceable.Name}")
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

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleEditorItemEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (targetPlaceable == null)
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

                case "saveDescription":
                  targetPlaceable.Description = itemDescription.GetBindValue(player.oid, nuiToken.Token);
                  targetPlaceable.GetObjectVariable<LocalVariableString>("_COMMENT").Value = itemComment.GetBindValue(player.oid, nuiToken.Token);
                  player.oid.SendServerMessage($"La description et le commentaire de l'objet {targetPlaceable.Name.ColorString(ColorConstants.White)} ont bien été enregistrées.", new Color(32, 255, 32));
                  break;

                case "saveNewVariable":
                  Utils.ConvertLocalVariable(newVariableName.GetBindValue(player.oid, nuiToken.Token), newVariableValue.GetBindValue(player.oid, nuiToken.Token), selectedNewVariableType.GetBindValue(player.oid, nuiToken.Token), targetPlaceable, player.oid);
                  LoadVariablesBinding();
                  break;

                case "saveVariable":
                  Utils.ConvertLocalVariable(variableName.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex], variableValue.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex], selectedVariableType.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex], targetPlaceable, player.oid);
                  LoadVariablesBinding();
                  break;

                case "deleteVariable":
                  targetPlaceable.LocalVariables.ElementAt(nuiEvent.ArrayIndex).Delete();
                  LoadVariablesBinding();
                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "name":
                  targetPlaceable.Name = name.GetBindValue(player.oid, nuiToken.Token);
                  break;

                case "tag":
                  targetPlaceable.Tag = tag.GetBindValue(player.oid, nuiToken.Token);
                  break;

                case "apparenceSearch":
                  string aSearch = apparenceSearch.GetBindValue(player.oid, nuiToken.Token).ToLower();
                  apparence.SetBindValue(player.oid, nuiToken.Token, string.IsNullOrEmpty(aSearch) ? Utils.placeableEntries : Utils.placeableEntries.Where(v => v.Label.ToLower().Contains(aSearch)).ToList());
                  break;

                case "apparenceSelected":
                  targetPlaceable.Appearance = NwGameTables.PlaceableTable.GetRow(apparenceSelected.GetBindValue(player.oid, nuiToken.Token));
                  NwPlaceable newPlaceable = targetPlaceable.Clone(targetPlaceable.Location);
                  targetPlaceable.Destroy();
                  targetPlaceable = newPlaceable;
                  break;

                case "orientation":

                  if (float.TryParse(orientation.GetBindValue(player.oid, nuiToken.Token), out float newOrientation))
                  {
                    newOrientation = newOrientation < 0 ? 0 : newOrientation;
                    newOrientation = newOrientation > 360 ? 360 : newOrientation;
                    targetPlaceable.Rotation = newOrientation;
                  }

                  break;

                case "xPosition":

                  if (float.TryParse(xPosition.GetBindValue(player.oid, nuiToken.Token), out float newXPosition))
                    targetPlaceable.Position = new Vector3(newXPosition, targetPlaceable.Position.Y, targetPlaceable.Position.Z);

                  break;

                case "yPosition":

                  if (float.TryParse(yPosition.GetBindValue(player.oid, nuiToken.Token), out float newYPosition))
                    targetPlaceable.Position = new Vector3(targetPlaceable.Position.X, newYPosition, targetPlaceable.Position.Z);

                  break;

                case "zPosition":

                  if (float.TryParse(zPosition.GetBindValue(player.oid, nuiToken.Token), out float newZPosition))
                  {
                    newZPosition = newZPosition < -10 ? -10 : newZPosition;
                    newZPosition = newZPosition > 100 ? 100 : newZPosition;

                    targetPlaceable.Position = new Vector3(targetPlaceable.Position.X, targetPlaceable.Position.Y, newZPosition);
                  }

                  break;

                case "scale":

                  if (float.TryParse(scale.GetBindValue(player.oid, nuiToken.Token), out float newSize))
                  {                
                    if(newSize == 0 && !targetPlaceable.Useable && targetPlaceable.VisualTransform.Translation == Vector3.Zero && targetPlaceable.VisualTransform.Rotation == Vector3.Zero)
                    {
                      targetPlaceable.IsStatic = true;
                      targetPlaceable.VisibilityOverride = VisibilityMode.Default;
                    }
                    else
                    {
                      targetPlaceable.IsStatic = false;
                      targetPlaceable.VisibilityOverride = VisibilityMode.AlwaysVisible;
                    }

                    targetPlaceable.VisualTransform.Scale = newSize > 100 ? 100 : newSize;
                  }

                  break;

                case "xRotation":
                 
                  if (float.TryParse(xRotation.GetBindValue(player.oid, nuiToken.Token), out float newXRotation))
                  {
                    newXRotation = newXRotation < 0 ? 0 : newXRotation;
                    newXRotation = newXRotation > 360 ? 360 : newXRotation;

                    if (newXRotation == 0 && !targetPlaceable.Useable && targetPlaceable.VisualTransform.Translation == Vector3.Zero && targetPlaceable.VisualTransform.Rotation == Vector3.Zero)
                    {
                      targetPlaceable.IsStatic = true;
                      targetPlaceable.VisibilityOverride = VisibilityMode.Default;
                    }
                    else
                    {
                      targetPlaceable.IsStatic = false;
                      targetPlaceable.VisibilityOverride = VisibilityMode.AlwaysVisible;
                    }

                    targetPlaceable.VisualTransform.Rotation = new Vector3(newXRotation, targetPlaceable.VisualTransform.Rotation.Y, targetPlaceable.VisualTransform.Rotation.Z);
                  }

                  break;

                case "yRotation":

                  if (float.TryParse(yRotation.GetBindValue(player.oid, nuiToken.Token), out float newYRotation))
                  {
                    newYRotation = newYRotation < 0 ? 0 : newYRotation;
                    newYRotation = newYRotation > 360 ? 360 : newYRotation;

                    if (newYRotation == 0 && !targetPlaceable.Useable && targetPlaceable.VisualTransform.Translation == Vector3.Zero && targetPlaceable.VisualTransform.Rotation == Vector3.Zero)
                    {
                      targetPlaceable.IsStatic = true;
                      targetPlaceable.VisibilityOverride = VisibilityMode.Default;
                    }
                    else
                    {
                      targetPlaceable.IsStatic = false;
                      targetPlaceable.VisibilityOverride = VisibilityMode.AlwaysVisible;
                    }

                    targetPlaceable.VisualTransform.Rotation = new Vector3(targetPlaceable.VisualTransform.Rotation.X, newYRotation, targetPlaceable.VisualTransform.Rotation.Z);
                  }

                  break;

                case "zRotation":

                  if (float.TryParse(zRotation.GetBindValue(player.oid, nuiToken.Token), out float newZRotation))
                  {
                    newZRotation = newZRotation < 0 ? 0 : newZRotation;
                    newZRotation = newZRotation > 360 ? 360 : newZRotation;

                    if (newZRotation == 0 && !targetPlaceable.Useable && targetPlaceable.VisualTransform.Translation == Vector3.Zero && targetPlaceable.VisualTransform.Rotation == Vector3.Zero)
                    {
                      targetPlaceable.IsStatic = true;
                      targetPlaceable.VisibilityOverride = VisibilityMode.Default;
                    }
                    else
                    {
                      targetPlaceable.IsStatic = false;
                      targetPlaceable.VisibilityOverride = VisibilityMode.AlwaysVisible;
                    }

                    targetPlaceable.VisualTransform.Rotation = new Vector3(targetPlaceable.VisualTransform.Rotation.X, targetPlaceable.VisualTransform.Rotation.Y, newZRotation);
                  }

                  break;

                case "xTranslation":

                  if (float.TryParse(xTranslation.GetBindValue(player.oid, nuiToken.Token), out float newXTranslation))
                  {
                    newXTranslation = newXTranslation < -50 ? -50 : newXTranslation;
                    newXTranslation = newXTranslation > 50 ? 50 : newXTranslation;

                    if (newXTranslation == 0 && !targetPlaceable.Useable && targetPlaceable.VisualTransform.Translation == Vector3.Zero && targetPlaceable.VisualTransform.Rotation == Vector3.Zero)
                    {
                      targetPlaceable.IsStatic = true;
                      targetPlaceable.VisibilityOverride = VisibilityMode.Default;
                    }
                    else
                    {
                      targetPlaceable.IsStatic = false;
                      targetPlaceable.VisibilityOverride = VisibilityMode.AlwaysVisible;
                    }

                    targetPlaceable.VisualTransform.Translation = new Vector3(newXTranslation, targetPlaceable.VisualTransform.Translation.Y, targetPlaceable.VisualTransform.Translation.Z);
                  }

                  break;

                case "yTranslation":

                  if (float.TryParse(yTranslation.GetBindValue(player.oid, nuiToken.Token), out float newYTranslation))
                  {
                    newYTranslation = newYTranslation < -60 ? -60 : newYTranslation;
                    newYTranslation = newYTranslation > 60 ? 60 : newYTranslation;

                    if (newYTranslation == 0 && !targetPlaceable.Useable && targetPlaceable.VisualTransform.Translation == Vector3.Zero && targetPlaceable.VisualTransform.Rotation == Vector3.Zero)
                    {
                      targetPlaceable.IsStatic = true;
                      targetPlaceable.VisibilityOverride = VisibilityMode.Default;
                    }
                    else
                    {
                      targetPlaceable.IsStatic = false;
                      targetPlaceable.VisibilityOverride = VisibilityMode.AlwaysVisible;
                    }

                    targetPlaceable.VisualTransform.Translation = new Vector3(targetPlaceable.VisualTransform.Translation.X, newYTranslation, targetPlaceable.VisualTransform.Translation.Z);
                  }

                  break;

                case "zTranslation":

                  if (float.TryParse(zTranslation.GetBindValue(player.oid, nuiToken.Token), out float newZTranslation))
                  {
                    newZTranslation = newZTranslation < -10 ? -10 : newZTranslation;
                    newZTranslation = newZTranslation > 100 ? 100 : newZTranslation;

                    if (newZTranslation == 0 && !targetPlaceable.Useable && targetPlaceable.VisualTransform.Translation == Vector3.Zero && targetPlaceable.VisualTransform.Rotation == Vector3.Zero)
                    {
                      targetPlaceable.IsStatic = true;
                      targetPlaceable.VisibilityOverride = VisibilityMode.Default;
                    }
                    else
                    {
                      targetPlaceable.IsStatic = false;
                      targetPlaceable.VisibilityOverride = VisibilityMode.AlwaysVisible;
                    }

                    targetPlaceable.VisualTransform.Translation = new Vector3(targetPlaceable.VisualTransform.Translation.X, targetPlaceable.VisualTransform.Translation.Y, newZTranslation);
                  }

                  break;

                case "hardness":

                  if (int.TryParse(hardness.GetBindValue(player.oid, nuiToken.Token), out int newHardness))
                  {
                    newHardness = newHardness < 0 ? 0 : newHardness;
                    newHardness = newHardness > 250 ? 250 : newHardness;
                    targetPlaceable.Hardness = newHardness;
                  }

                  break;

                case "hitPoints":

                  if (int.TryParse(hitPoints.GetBindValue(player.oid, nuiToken.Token), out int newHP))
                  {
                    newHP = newHP < 0 ? 0 : newHP;
                    newHP = newHP > 10000 ? 10000 : newHP;
                    targetPlaceable.HP = newHP;
                  }

                  break;

                case "plotChecked":
                  targetPlaceable.PlotFlag = plotChecked.GetBindValue(player.oid, nuiToken.Token);
                  statVisible.SetBindValue(player.oid, nuiToken.Token, !targetPlaceable.PlotFlag);
                  break;

                case "useableChecked":

                  targetPlaceable.Useable = useableChecked.GetBindValue(player.oid, nuiToken.Token);
                  inventoryVisible.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.Useable);

                  if (!targetPlaceable.Useable)
                  {
                    targetPlaceable.HasInventory = false;
                    hasInventoryChecked.SetBindWatch(player.oid, nuiToken.Token, false);
                    hasInventoryChecked.SetBindValue(player.oid, nuiToken.Token, false);
                    hasInventoryChecked.SetBindWatch(player.oid, nuiToken.Token, true);

                    if (targetPlaceable.VisualTransform.Scale == 1 && targetPlaceable.VisualTransform.Translation == Vector3.Zero && targetPlaceable.VisualTransform.Rotation == Vector3.Zero)
                    {
                      targetPlaceable.IsStatic = true;
                      targetPlaceable.VisibilityOverride = VisibilityMode.Default;
                    } 
                  }
                  else
                  {
                    targetPlaceable.IsStatic = false;

                    if (targetPlaceable.VisualTransform.Scale != 1 || targetPlaceable.VisualTransform.Translation != Vector3.Zero || targetPlaceable.VisualTransform.Rotation != Vector3.Zero)
                      targetPlaceable.VisibilityOverride = VisibilityMode.AlwaysVisible;
                  }

                  break;

                case "hasInventoryChecked":
                  targetPlaceable.HasInventory = hasInventoryChecked.GetBindValue(player.oid, nuiToken.Token);
                  break;

                case "persistantChecked":

                  bool persistance = persistantChecked.GetBindValue(player.oid, nuiToken.Token);

                  if (player.QueryAuthorized())
                  {
                    if (persistance)
                    {
                      if (targetPlaceable.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").HasNothing)
                      {
                        SqLiteUtils.InsertQuery("placeableSpawn",
                          new List<string[]>() { new string[] { "areaTag", targetPlaceable.Area.Tag }, new string[] { "position", targetPlaceable.Position.ToString() }, new string[] { "facing", targetPlaceable.Rotation.ToString() }, new string[] { "serializedPlaceable", targetPlaceable.Serialize().ToBase64EncodedString() } });

                        var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT last_insert_rowid()");
                        query.Execute();

                        targetPlaceable.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value = query.Result.GetInt(0);
                        player.oid.SendServerMessage($"{targetPlaceable.Name.ColorString(ColorConstants.White)} est désormais un placeable persistant.", ColorConstants.Orange);
                      }
                    }
                    else
                    {
                      if (targetPlaceable.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").HasValue)
                      {
                        string spawnId = targetPlaceable.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value.ToString();

                        SqLiteUtils.DeletionQuery("placeableSpawn",
                          new Dictionary<string, string>() { { "rowid", spawnId } });

                        targetPlaceable.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Delete();
                        player.oid.SendServerMessage($"{targetPlaceable.Name.ColorString(ColorConstants.White)} n'est plus un placeable persistant. Il disparaitra au prochain reboot.", ColorConstants.Orange);
                      }
                    }
                  }
                  else
                  {
                    persistantChecked.SetBindWatch(player.oid, nuiToken.Token, false);
                    persistantChecked.SetBindValue(player.oid, nuiToken.Token, !persistance);
                    persistantChecked.SetBindWatch(player.oid, nuiToken.Token, true);
                  }

                  updateVisibility.SetBindValue(player.oid, nuiToken.Token, persistantChecked.GetBindValue(player.oid, nuiToken.Token));

                  break;

                case "updatePersistantPlc":
                  if (targetPlaceable.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").HasValue && player.QueryAuthorized())
                  {
                    SqLiteUtils.UpdateQuery("placeableSpawn",
                    new List<string[]>() { new string[] { "areaTag", targetPlaceable.Area.Tag }, new string[] { "position", targetPlaceable.Position.ToString() }, new string[] { "facing", targetPlaceable.Rotation.ToString() }, new string[] { "serializedPlaceable", targetPlaceable.Serialize().ToBase64EncodedString() } },
                    new List<string[]>() { new string[] { "rowid", targetPlaceable.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value.ToString() } });

                    player.oid.SendServerMessage($"{targetPlaceable.Name.ColorString(ColorConstants.White)} les données de persistance ont bien été mises à jour.", ColorConstants.Orange);
                  }
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
              new NuiButton("Description") { Id = "description", Height = 35, Width = 90 },
              new NuiButton("Variables") { Id = "variables", Height = 35, Width = 90 },
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

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiLabel("Tag") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
            new NuiTextEdit("Tag", tag, 30, false) { Height = 35, Width = 200 }
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiLabel("Apparence") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
            new NuiCombo() { Height = 35, Width = 200, Entries = apparence, Selected = apparenceSelected },
            new NuiTextEdit("Recherche", apparenceSearch, 20, false) { Height = 35, Width = 100 }
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiCheck("Indestructible", plotChecked) { Height = 35, Width = 120 },
            new NuiLabel("Résistance") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Visible = statVisible, Tooltip = "Doit être compris entre 0 et 250" },
            new NuiTextEdit("Résistance", hardness, 3, false) { Height = 35, Width = 35, Visible = statVisible },
            new NuiLabel("HP") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Visible = statVisible, Tooltip = "Doit être compris entre 0 et 10000" },
            new NuiTextEdit("HP", hitPoints, 5, false) { Height = 35, Width = 35, Visible = statVisible },
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiCheck("Utilisable", useableChecked) { Height = 35, Width = 90 },
            new NuiCheck("Inventaire", hasInventoryChecked) { Height = 35, Width = 90, Visible = inventoryVisible },
            new NuiCheck("Persistant", persistantChecked) { Height = 35, Width = 90 },
            new NuiButtonImage("ir_empytqs") { Id = "updatePersistantPlc", Height = 35, Width = 35, Visible = updateVisibility, Tooltip = "Mettre à jour le placeable persistant. Utile si des modifications ont été faites depuis la dernière sauvegarde." },
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiLabel("Orientation") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Tooltip = "Doit être compris entre 0 et 360.0" },
            new NuiTextEdit("Orientation", orientation, 5, false) { Height = 35, Width = 100 },
            new NuiLabel("Taille") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Tooltip = "Doit être compris entre 0.01 et 99.99" },
            new NuiTextEdit("Taille", scale, 5, false) { Height = 35, Width = 100 }
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiLabel("Position") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
            new NuiTextEdit("X", xPosition, 5, false) { Height = 35, Width = 100, Tooltip = "Doit être compris entre 0 et 50.0" },
            new NuiTextEdit("Y", yPosition, 5, false) { Height = 35, Width = 100, Tooltip = "Doit être compris entre 0 et 60.0" },
            new NuiTextEdit("Z", zPosition, 5, false) { Height = 35, Width = 100, Tooltip = "Doit être compris entre -10 et 100" }
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiLabel("Rotation") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Tooltip = "Doit être compris entre 0 et 360.0" },
            new NuiTextEdit("X", xRotation, 5, false) { Height = 35, Width = 100 },
            new NuiTextEdit("Y", yRotation, 5, false) { Height = 35, Width = 100 },
            new NuiTextEdit("Z", zRotation, 5, false) { Height = 35, Width = 100 }
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiLabel("Translation") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
            new NuiTextEdit("X", xTranslation, 5, false) { Height = 35, Width = 100, Tooltip = "Doit être compris entre -50.00 et 50.00" },
            new NuiTextEdit("Y", yTranslation, 5, false) { Height = 35, Width = 100, Tooltip = "Doit être compris entre -60.00 et 60.00" },
            new NuiTextEdit("Z", zTranslation, 5, false) { Height = 35, Width = 100, Tooltip = "Doit être compris entre -10.00 et 100" }
          } });
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

          apparence.SetBindValue(player.oid, nuiToken.Token, Utils.placeableEntries);
          apparenceSelected.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.Appearance.RowIndex);
          apparenceSelected.SetBindWatch(player.oid, nuiToken.Token, true);
          apparenceSearch.SetBindWatch(player.oid, nuiToken.Token, true);

          plotChecked.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.PlotFlag);
          plotChecked.SetBindWatch(player.oid, nuiToken.Token, true);
          persistantChecked.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").HasValue);
          persistantChecked.SetBindWatch(player.oid, nuiToken.Token, true);
          updateVisibility.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").HasValue);
          statVisible.SetBindValue(player.oid, nuiToken.Token, !targetPlaceable.PlotFlag);
          hardness.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.Hardness.ToString());
          hardness.SetBindWatch(player.oid, nuiToken.Token, true);
          hitPoints.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.HP.ToString());
          hitPoints.SetBindWatch(player.oid, nuiToken.Token, true);

          useableChecked.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.Useable);
          useableChecked.SetBindWatch(player.oid, nuiToken.Token, true);
          inventoryVisible.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.Useable);
          inventoryVisible.SetBindWatch(player.oid, nuiToken.Token, true);
          hasInventoryChecked.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.HasInventory);
          hasInventoryChecked.SetBindWatch(player.oid, nuiToken.Token, true);

          orientation.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.Rotation.ToString());
          xPosition.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.Position.X.ToString());
          yPosition.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.Position.Y.ToString());
          zPosition.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.Position.Z.ToString());

          scale.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.VisualTransform.Scale.ToString());
          scale.SetBindWatch(player.oid, nuiToken.Token, true);

          xRotation.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.VisualTransform.Rotation.X.ToString());
          yRotation.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.VisualTransform.Rotation.Y.ToString());
          zRotation.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.VisualTransform.Rotation.Z.ToString());

          xTranslation.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.VisualTransform.Translation.X.ToString());
          yTranslation.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.VisualTransform.Translation.Y.ToString());
          zTranslation.SetBindValue(player.oid, nuiToken.Token, targetPlaceable.VisualTransform.Translation.Z.ToString());

          orientation.SetBindWatch(player.oid, nuiToken.Token, true);
          xPosition.SetBindWatch(player.oid, nuiToken.Token, true);
          yPosition.SetBindWatch(player.oid, nuiToken.Token, true);
          zPosition.SetBindWatch(player.oid, nuiToken.Token, true);
          xRotation.SetBindWatch(player.oid, nuiToken.Token, true);
          yRotation.SetBindWatch(player.oid, nuiToken.Token, true);
          zRotation.SetBindWatch(player.oid, nuiToken.Token, true);
          xTranslation.SetBindWatch(player.oid, nuiToken.Token, true);
          yTranslation.SetBindWatch(player.oid, nuiToken.Token, true);
          zTranslation.SetBindWatch(player.oid, nuiToken.Token, true);
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
