using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;

using NWN.Core.NWNX;

using static Anvil.API.Events.ModuleEvents;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class PlaceableManagerWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();
        private readonly NuiBind<string> placeableName = new("placeableName");
        private readonly NuiBind<string> persistState = new("persistState");
        private readonly NuiBind<string> persistTooltip = new("persistTooltip");
        private readonly NuiBind<bool> persistEnabled = new("persistEnabled");

        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> search = new("search");

        private List<NwPlaceable> areaList = new();
        private IEnumerable<NwPlaceable> currentList;

        public PlaceableManagerWindow(Player player) : base(player)
        {
          windowId = "placeableManager";

          rootColumn.Children = rootChildren;
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(placeableName) { Tooltip = placeableName, VerticalAlign = NuiVAlign.Middle }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_action") { Id = "examine", Tooltip = "Ouvre le menu d'édition" }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ief_darkvis") { Id = "display", Tooltip = "Appliquer un effet visuel pour identifier l'objet" }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_more_sp_ab") { Id = "teleport", Tooltip = "Se téléporter sur l'objet" }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(persistState) { Id = "persist", Tooltip = persistTooltip, Enabled = persistEnabled }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ief_arcanefail") { Id = "remove", Tooltip = "Supprimer" }) { Width = 35 });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) } });
          rootChildren.Add(new NuiRow() { Height = 460, Width = 540, Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 600, 540);

          window = new NuiWindow(rootColumn, $"{player.oid.ControlledCreature.Area.Name} - Placeables")
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
            nuiToken.OnNuiEvent += HandlePalettePlaceableEvents;

            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            areaList = player.oid.ControlledCreature.Area.FindObjectsOfTypeInArea<NwPlaceable>().OrderBy(p => p.DistanceSquared(player.oid.ControlledCreature)).ToList();
            currentList = areaList;
            LoadItemList(currentList);
          }
        }
        private void HandlePalettePlaceableEvents(OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "examine":

                  if (!player.windows.ContainsKey("editorPlaceable")) player.windows.Add("editorPlaceable", new EditorPlaceableWindow(player, currentList.ElementAt(nuiEvent.ArrayIndex)));
                  else ((EditorPlaceableWindow)player.windows["editorPlaceable"]).CreateWindow(currentList.ElementAt(nuiEvent.ArrayIndex));

                  break;

                case "display":

                  NwPlaceable plc = currentList.ElementAt(nuiEvent.ArrayIndex);
                  PlayerPlugin.ApplyLoopingVisualEffectToObject(player.oid.ControlledCreature, plc, 524);
                  RemoveLoopingEffect(plc);

                  break;

                case "teleport": player.oid.ControlledCreature.Location = currentList.ElementAt(nuiEvent.ArrayIndex).Location; break;

                case "remove":

                  NwPlaceable removedPlc = currentList.ElementAt(nuiEvent.ArrayIndex);
                  areaList.Remove(removedPlc);
                  removedPlc.Destroy();
                  LoadItemList(currentList);

                  break;

                case "persist":

                  if (player.QueryAuthorized())
                  {
                    NwPlaceable targetPlaceable = currentList.ElementAt(nuiEvent.ArrayIndex);

                    if (targetPlaceable.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").HasNothing)
                    {
                      SqLiteUtils.InsertQuery("placeableSpawn",
                          new List<string[]>() { new string[] { "areaTag", targetPlaceable.Area.Tag }, new string[] { "position", targetPlaceable.Position.ToString() }, new string[] { "facing", targetPlaceable.Rotation.ToString() }, new string[] { "serializedPlaceable", targetPlaceable.Serialize().ToBase64EncodedString() } });

                      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT last_insert_rowid()");
                      query.Execute();

                      targetPlaceable.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value = query.Result.GetInt(0);
                      player.oid.SendServerMessage($"{targetPlaceable.Name.ColorString(ColorConstants.White)} est désormais un placeable persistant.", ColorConstants.Orange);

                      List<string> persistStateList = persistState.GetBindValues(player.oid, nuiToken.Token);
                      List<string> persistTooltipList = persistTooltip.GetBindValues(player.oid, nuiToken.Token);

                      persistStateList[nuiEvent.ArrayIndex] = "ir_ban";
                      persistTooltipList[nuiEvent.ArrayIndex] = "Désactiver la persistance";

                      persistState.SetBindValues(player.oid, nuiToken.Token, persistStateList);
                      persistTooltip.SetBindValues(player.oid, nuiToken.Token, persistTooltipList);
                    }
                    else
                    {
                      string spawnId = targetPlaceable.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value.ToString();

                      SqLiteUtils.DeletionQuery("placeableSpawn",
                        new Dictionary<string, string>() { { "rowid", spawnId } });

                      targetPlaceable.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Delete();
                      player.oid.SendServerMessage($"{targetPlaceable.Name.ColorString(ColorConstants.White)} n'est plus un placeable persistant. Il disparaitra au prochain reboot.", ColorConstants.Orange);

                      List<string> persistStateList = persistState.GetBindValues(player.oid, nuiToken.Token);
                      List<string> persistTooltipList = persistTooltip.GetBindValues(player.oid, nuiToken.Token);

                      persistStateList[nuiEvent.ArrayIndex] = "ir_empytqs";
                      persistTooltipList[nuiEvent.ArrayIndex] = "Activer la persistance";

                      persistState.SetBindValues(player.oid, nuiToken.Token, persistStateList);
                      persistTooltip.SetBindValues(player.oid, nuiToken.Token, persistTooltipList);
                    }
                  }

                  break;
              }
              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "search":

                  string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
                  currentList = string.IsNullOrEmpty(currentSearch) ? areaList : areaList.Where(c => c.Name.ToLower().Contains(currentSearch));

                  LoadItemList(currentList);

                  break;
              }

              break;
          }
        }
        private void LoadItemList(IEnumerable<NwPlaceable> filteredList)
        {
          List<string> placeableNameList = new();
          List<string> persistStateList = new();
          List<string> persistTooltipList = new();
          List<bool> persistEnabledList = new();

          foreach (NwPlaceable entry in filteredList)
          {
            placeableNameList.Add(entry.Name);
            persistEnabledList.Add(entry.GetObjectVariable<LocalVariableBool>("_EDITOR_PLACEABLE").HasNothing);

            if (entry.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").HasNothing)
            {
              persistStateList.Add("ir_empytqs");
              persistTooltipList.Add("Activer la persistance");
            }
            else
            {
              persistStateList.Add("ir_ban");
              persistTooltipList.Add("Désactiver la persistance");
            }
          }

          placeableName.SetBindValues(player.oid, nuiToken.Token, placeableNameList);
          persistState.SetBindValues(player.oid, nuiToken.Token, persistStateList);
          persistTooltip.SetBindValues(player.oid, nuiToken.Token, persistTooltipList);
          persistEnabled.SetBindValues(player.oid, nuiToken.Token, persistEnabledList);
          listCount.SetBindValue(player.oid, nuiToken.Token, filteredList.Count());
        }
        private async void RemoveLoopingEffect(NwPlaceable plc)
        {
          await NwTask.Delay(TimeSpan.FromSeconds(10));
          PlayerPlugin.ApplyLoopingVisualEffectToObject(player.oid.ControlledCreature, plc, 524);
        }
      }
    }
  }
}
