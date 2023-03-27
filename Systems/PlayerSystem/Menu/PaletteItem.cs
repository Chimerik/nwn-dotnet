using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using Newtonsoft.Json;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class PaletteItemWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();
        private readonly NuiBind<string> newItemName = new("newItemName");
        private readonly NuiBind<string> itemName = new("itemName");
        private readonly NuiBind<string> creatorName = new("creatorName");
        private readonly NuiBind<string> comment = new("comment");
        private readonly NuiBind<string> lastModified = new("lastModified");
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> search = new("search");
        private readonly NuiBind<bool> isCreatorOrAdmin = new("isCreatorOrAdmin");
        private readonly NuiBind<bool> isModelLoaded = new("isModelLoaded");

        private readonly NuiBind<List<NuiComboEntry>> creators = new("creators");
        private readonly NuiBind<int> selectedCreator = new("selectedCreator");

        private NwItem selectionTarget;
        private int currentArrayindex = -1;
        private bool AuthorizeSave { get; set; }
        private int nbDebounce { get; set; }

        private IEnumerable<PaletteEntry> currentList;

        public PaletteItemWindow(Player player) : base(player)
        {
          windowId = "paletteItem";
          AuthorizeSave = false;
          nbDebounce = 0;

          rootColumn.Children = rootChildren;
          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("Nom créature", itemName, 50, false) { Tooltip = itemName }) { Width = 140 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(creatorName) { Tooltip = comment, VerticalAlign = NuiVAlign.Middle }) { Width = 60 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(lastModified) { Tooltip = lastModified, VerticalAlign = NuiVAlign.Middle }));
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_action") { Id = "copy", Tooltip = "Sélectionner le modèle pour cette entrée", Enabled = isCreatorOrAdmin }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_attack") { Id = "spawn", Tooltip = "Créer cet objet" }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_empytqs") { Id = "save", Tooltip = "Valider les modifications", Enabled = isCreatorOrAdmin }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_ban") { Id = "delete", Tooltip = "Supprimer cette entrée", Enabled = isCreatorOrAdmin }) { Width = 35 });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiTextEdit("Nom palette", newItemName, 50, false) { Tooltip = newItemName, Height = 35 },
            new NuiButtonImage("ir_action") { Id = "selectNewItem", Tooltip = "Sélectionner l'objet à sauvegarder", Width = 35, Height = 35 },
            new NuiButtonImage("ir_animalemp") { Id = "create", Tooltip = "Ajouter l'objet à la palette", Enabled = isModelLoaded, Width = 35, Height = 35 }
          }
          });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiCombo() { Entries = creators, Selected = selectedCreator }, new NuiSpacer() } });
          rootChildren.Add(new NuiRow() { Height = 300, Width = 540, Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 600, 540);

          window = new NuiWindow(rootColumn, "Palette des objets")
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
            nuiToken.OnNuiEvent += HandlePaletteItemEvents;

            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);

            creators.SetBindValue(player.oid, nuiToken.Token, Utils.itemPaletteCreatorsList);

            selectedCreator.SetBindValue(player.oid, nuiToken.Token, 0);
            selectedCreator.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            currentList = Utils.itemPaletteList;
            LoadItemList(currentList);
          }
        }
        private void HandlePaletteItemEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "selectNewItem":

                  currentArrayindex = -1;
                  player.oid.SendServerMessage("Quel objet souhaitez-vous prendre pour modèle ?", ColorConstants.Orange);
                  player.oid.EnterTargetMode(SelectItem, ObjectTypes.Item, MouseCursor.Action);

                  break;

                case "create":

                  HandleInsertNewItem(newItemName.GetBindValue(player.oid, nuiToken.Token), selectionTarget.Serialize().ToBase64EncodedString(), player.oid.PlayerName, selectionTarget.GetObjectVariable<LocalVariableString>("_COMMENT").Value);

                  newItemName.SetBindValue(player.oid, nuiToken.Token, "");
                  selectionTarget = null;
                  isModelLoaded.SetBindValue(player.oid, nuiToken.Token, false);

                  LoadItemList(currentList);

                  break;

                case "copy":

                  currentArrayindex = nuiEvent.ArrayIndex;
                  player.oid.SendServerMessage("Quel objet souhaitez-vous prendre pour modèle ?", ColorConstants.Orange);
                  player.oid.EnterTargetMode(SelectItem, ObjectTypes.Item, MouseCursor.Action);

                  break;

                case "spawn":

                  currentArrayindex = nuiEvent.ArrayIndex;
                  player.oid.SendServerMessage("Où souhaitez-vous créer cet objet ?", ColorConstants.Orange);
                  player.oid.EnterTargetMode(CreateItem, ObjectTypes.All, MouseCursor.Create);

                  break;

                case "save":

                  PaletteEntry entry = currentList.ElementAt(nuiEvent.ArrayIndex);
                  entry.name = itemName.GetBindValues(player.oid, nuiToken.Token).ElementAt(nuiEvent.ArrayIndex);
                  PaletteSave();

                  player.oid.SendServerMessage($"L'objet {entry.name.ColorString(ColorConstants.White)} a bien été sauvegardé dans la palette.", new Color(32, 255, 32));

                  break;

                case "delete":

                  PaletteEntry deletedEntry = currentList.ElementAt(nuiEvent.ArrayIndex);
                  Utils.itemPaletteList.Remove(deletedEntry);

                  PaletteSave();
                  LoadItemList(currentList);

                  player.oid.SendServerMessage($"L'objet {deletedEntry.name.ColorString(ColorConstants.White)} a bien été supprimé de la palette.", new Color(32, 255, 32));

                  break;
              }
              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "search":
                case "selectedCreator":

                  string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
                  int creatorId = selectedCreator.GetBindValue(player.oid, nuiToken.Token);

                  if (creatorId < 1)
                    currentList = string.IsNullOrEmpty(currentSearch) ? Utils.itemPaletteList : Utils.itemPaletteList.Where(c => c.name.ToLower().Contains(currentSearch));
                  else
                  {
                    string selectedCreatorName = Utils.itemPaletteCreatorsList.FirstOrDefault(c => c.Value == creatorId).Label;
                    currentList = string.IsNullOrEmpty(currentSearch) ? Utils.itemPaletteList.Where(c => c.creator == selectedCreatorName) : Utils.itemPaletteList.Where(c => c.name.ToLower().Contains(currentSearch) && c.creator == selectedCreatorName);
                  }

                  LoadItemList(currentList);

                  break;
              }

              break;
          }
        }
        private void LoadItemList(IEnumerable<PaletteEntry> filteredList)
        {
          List<string> itemNameList = new();
          List<string> creatorNameList = new();
          List<string> commentList = new();
          List<string> lastModifiedList = new();
          List<bool> enabledList = new();

          foreach (PaletteEntry entry in filteredList)
          {
            itemNameList.Add(entry.name);
            creatorNameList.Add(entry.creator);
            commentList.Add(entry.comment);
            lastModifiedList.Add(entry.lastModified);

            if (player.oid.PlayerName == entry.creator || player.oid.PlayerName == "Chim")
              enabledList.Add(true);
            else
              enabledList.Add(false);
          }

          itemName.SetBindValues(player.oid, nuiToken.Token, itemNameList);
          creatorName.SetBindValues(player.oid, nuiToken.Token, creatorNameList);
          comment.SetBindValues(player.oid, nuiToken.Token, commentList);
          lastModified.SetBindValues(player.oid, nuiToken.Token, lastModifiedList);
          isCreatorOrAdmin.SetBindValues(player.oid, nuiToken.Token, enabledList);
          listCount.SetBindValue(player.oid, nuiToken.Token, filteredList.Count());
        }
        private void SelectItem(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || selection.TargetObject is not NwItem target)
            return;

          if (currentArrayindex > -1)
          {
            PaletteEntry entry = currentList.ElementAt(currentArrayindex);
            entry.serializedObject = target.Serialize().ToBase64EncodedString();
            player.oid.SendServerMessage($"L'objet {target.Name.ColorString(ColorConstants.White)} sera désormais utilisée comme modèle pour l'entrée de la palette {entry.name.ColorString(ColorConstants.White)}.", new Color(32, 255, 32));
            selectionTarget = null;
          }
          else
          {
            selectionTarget = target;
            isModelLoaded.SetBindValue(player.oid, nuiToken.Token, true);
          }
        }

        private void CreateItem(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled)
            return;

          NwItem item = NwItem.Deserialize(currentList.ElementAt(currentArrayindex).serializedObject.ToByteArray());

          if (selection.TargetObject is NwGameObject targetInventory)
          {
            item.Clone(targetInventory);
            item.Destroy();
          }
          else
            item.Location = Location.Create(player.oid.ControlledCreature.Area, selection.TargetPosition, player.oid.ControlledCreature.Rotation);
        }

        private void HandleInsertNewItem(string itemName, string serializedItem, string playerName, string comment)
        {
          if (!Utils.itemPaletteList.Any(c => c.creator == playerName))
          {
            Utils.itemPaletteList.Add(new PaletteEntry(itemName, playerName, serializedItem, DateTime.Now.ToString(), comment));

            Utils.itemPaletteCreatorsList.Clear();
            Utils.itemPaletteCreatorsList.Add(new NuiComboEntry("Tous", 0));
            int index = 1;

            foreach (var entry in Utils.itemPaletteList.DistinctBy(c => c.creator).OrderBy(c => c.creator))
            {
              Utils.itemPaletteCreatorsList.Add(new NuiComboEntry(entry.creator, index));
              index++;
            }

            creators.SetBindValue(player.oid, nuiToken.Token, Utils.itemPaletteCreatorsList);
          }
          else
            Utils.itemPaletteList.Add(new PaletteEntry(itemName, playerName, serializedItem, DateTime.Now.ToString(), comment));

          player.oid.SendServerMessage("Votre objet a bien été ajoutée à la palette.", new Color(32, 255, 32));
          PaletteSave();

          //selectedCreator.SetBindValue(player.oid, nuiToken.Token, Utils.itemPaletteCreatorsList.FirstOrDefault(c => c.Label == playerName).Value);
          //search.SetBindValue(player.oid, nuiToken.Token, itemName); 
        }
        private void PaletteSave()
        {
          DateTime elapsed = DateTime.Now;

          if (!AuthorizeSave)
          {
            if (nbDebounce > 0)
            {
              nbDebounce += 1;
              return;
            }
            else
            {
              nbDebounce = 1;
              LogUtils.LogMessage($"Character {player.characterId} : scheduling item palette save in 10s", LogUtils.LogType.ModuleAdministration);
              DebouncePaletteSave(nbDebounce);
              return;
            }
          }
          else
            HandlePaletteSave();

          LogUtils.LogMessage($"Character {player.characterId} item palette saved in : {(DateTime.Now - elapsed).TotalSeconds} s", LogUtils.LogType.ModuleAdministration);
        }

        private async void DebouncePaletteSave(int initialNbDebounce)
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task awaitDebounce = NwTask.WaitUntil(() => nbDebounce != initialNbDebounce, tokenSource.Token);
          Task awaitSaveAuthorized = NwTask.Delay(TimeSpan.FromSeconds(10), tokenSource.Token);

          await NwTask.WhenAny(awaitDebounce, awaitSaveAuthorized);
          tokenSource.Cancel();

          if (awaitDebounce.IsCompletedSuccessfully)
          {
            nbDebounce += 1;
            DebouncePaletteSave(initialNbDebounce + 1);
            return;
          }

          if (awaitSaveAuthorized.IsCompletedSuccessfully)
          {
            nbDebounce = 0;
            AuthorizeSave = true;
            LogUtils.LogMessage($"Character {player.characterId} : debounce done after {nbDebounce} triggers, item palette save authorized", LogUtils.LogType.ModuleAdministration);
            PaletteSave();
          }
        }
        private async void HandlePaletteSave()
        {
          Task<string> serializeItemPalette = Task.Run(() => JsonConvert.SerializeObject(Utils.itemPaletteList.OrderBy(c => c.name).ThenByDescending(c => DateTime.TryParse(c.lastModified, out DateTime lastModified)).ToList()));
          await serializeItemPalette;

          SqLiteUtils.UpdateQuery("modulePalette",
          new List<string[]>() { new string[] { "items", serializeItemPalette.Result } },
          new List<string[]>() { new string[] { "rowid", "1" } });

          nbDebounce = 0;
          AuthorizeSave = false;
        }
      }
    }
  }
}
