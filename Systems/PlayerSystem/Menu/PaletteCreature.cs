﻿using System;
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
      public class PaletteCreatureWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();
        private readonly NuiBind<string> newCreatureName = new("newCreatureName");
        private readonly NuiBind<string> creatureName = new("creatureName");
        private readonly NuiBind<string> creatorName = new("creatorName");
        private readonly NuiBind<string> comment = new("comment");
        private readonly NuiBind<string> lastModified = new("lastModified");
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> search = new("search");
        private readonly NuiBind<bool> permanentSpawn = new("permanentSpawn");
        private readonly NuiBind<bool> isCreatorOrAdmin = new("isCreatorOrAdmin");
        private readonly NuiBind<bool> isModelLoaded = new("isModelLoaded");
        private readonly NuiBind<int> selectedCreatureType = new("selectedCreatureType");
        private readonly NuiBind<bool> creatureTypeEnabler = new("creatureTypeEnabler");

        private readonly NuiBind<List<NuiComboEntry>> creators = new("creators");
        private readonly NuiBind<int> selectedCreator = new("selectedCreator");

        private NwCreature selectionTarget;
        private int currentArrayindex = -1;
        private bool AuthorizeSave { get; set; }
        private int nbDebounce { get; set; }

        private IEnumerable<PaletteCreatureEntry> currentList;

        public PaletteCreatureWindow(Player player) : base(player)
        {
          windowId = "paletteCreature";
          AuthorizeSave = false;
          nbDebounce = 0;

          rootColumn.Children = rootChildren;
          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("Nom créature", creatureName, 50, false) { Tooltip = creatureName, Height = 35 }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(creatorName) { Tooltip = comment, Height = 35, VerticalAlign = NuiVAlign.Middle }));
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(lastModified) { Tooltip = lastModified, Height = 35, VerticalAlign = NuiVAlign.Middle }));
          rowTemplate.Add(new NuiListTemplateCell(new NuiButton("Select") { Id = "copy", Tooltip = "Sélectionner le modèle pour cette entrée", Enabled = isCreatorOrAdmin, Height = 35, Width = 35 }));
          rowTemplate.Add(new NuiListTemplateCell(new NuiButton("Spawn") { Id = "spawn", Tooltip = "Faire apparaître cette créature", Height = 35, Width = 35 }));
          rowTemplate.Add(new NuiListTemplateCell(new NuiButton("Save") { Id = "save", Tooltip = "Valider les modifications", Enabled = isCreatorOrAdmin, Height = 35, Width = 35 }));
          rowTemplate.Add(new NuiListTemplateCell(new NuiButton("Supprimer") { Id = "delete", Tooltip = "Supprimer cette entrée", Enabled = isCreatorOrAdmin, Height = 35, Width = 35 }));

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiTextEdit("Nom palette", newCreatureName, 50, false) { Tooltip = newCreatureName, Height = 35 },
            new NuiButton("Sélection") { Id = "selectNewCreature", Tooltip = "Sélectionner la créature à sauvegarder", Width = 80 },
            new NuiButton("Ajouter") { Id = "create", Tooltip = "Ajouter la créature à la palette", Enabled = isModelLoaded, Width = 80 }
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCombo() { Entries = creators, Selected = selectedCreator, Width = 370 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 370 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCheck("Spawn Permanent", permanentSpawn) { Tooltip = "Si cette option est cochée, la créature sera intégrée au système de spawn et persistera après reboot" } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiOptions() { Selection = selectedCreatureType, Direction = NuiDirection.Horizontal, Options = { "mob", "pnj fixe", "neutral" }, Tooltip = "mob = monstre hostile. PNJ fixe = immobile. Neutral = créature neutre qui se balade aléatoirement", Enabled = creatureTypeEnabler } } });
          rootChildren.Add(new NuiRow() { Height = 385, Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, 500);

          window = new NuiWindow(rootColumn, "Palette des créatures")
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
            nuiToken.OnNuiEvent += HandlePaletteCreatureEvents;

            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);

            permanentSpawn.SetBindValue(player.oid, nuiToken.Token, false);
            permanentSpawn.SetBindWatch(player.oid, nuiToken.Token, true);
            creators.SetBindValue(player.oid, nuiToken.Token, Utils.creaturePaletteCreatorsList);

            selectedCreator.SetBindValue(player.oid, nuiToken.Token, 0);
            selectedCreator.SetBindWatch(player.oid, nuiToken.Token, true);

            selectedCreatureType.SetBindValue(player.oid, nuiToken.Token, 0);
            creatureTypeEnabler.SetBindValue(player.oid, nuiToken.Token, false);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);


            currentList = Utils.creaturePaletteList;
            LoadCreatureList(currentList);
          }
        }
        private void HandlePaletteCreatureEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "selectNewCreature":

                  currentArrayindex = -1;
                  player.oid.SendServerMessage("Quelle créature souhaitez-vous prendre pour modèle ?", ColorConstants.Orange);
                  player.oid.EnterTargetMode(SelectCreature, ObjectTypes.Creature, MouseCursor.Action);

                  break;

                case "create":

                  HandleInsertNewCreature(newCreatureName.GetBindValue(player.oid, nuiToken.Token), selectionTarget.Serialize().ToBase64EncodedString(), player.oid.PlayerName, selectionTarget.GetObjectVariable<LocalVariableString>("_COMMENT").Value);

                  newCreatureName.SetBindValue(player.oid, nuiToken.Token, "");
                  selectionTarget = null;
                  isModelLoaded.SetBindValue(player.oid, nuiToken.Token, false);

                  break;

                case "copy":

                  currentArrayindex = nuiEvent.ArrayIndex;
                  player.oid.SendServerMessage("Quelle créature souhaitez-vous prendre pour modèle ?", ColorConstants.Orange);
                  player.oid.EnterTargetMode(SelectCreature, ObjectTypes.Creature, MouseCursor.Action);

                  break;

                case "spawn":

                  currentArrayindex = nuiEvent.ArrayIndex;
                  player.oid.SendServerMessage("Veuillez sélectionner un emplacement de spawn.", ColorConstants.Orange);
                  player.oid.EnterTargetMode(SpawnCreature, ObjectTypes.All, MouseCursor.Create);

                  break;

                case "save":

                  PaletteCreatureEntry entry = currentList.ElementAt(nuiEvent.ArrayIndex);
                  entry.name = creatureName.GetBindValues(player.oid, nuiToken.Token).ElementAt(nuiEvent.ArrayIndex);
                  PaletteSave();

                  player.oid.SendServerMessage($"La créature {entry.name.ColorString(ColorConstants.White)} a bien été sauvegardée dans la palette.", new Color(32, 255, 32));

                  break;

                case "delete":

                  PaletteCreatureEntry deletedEntry = currentList.ElementAt(nuiEvent.ArrayIndex);
                  Utils.creaturePaletteList.Remove(deletedEntry);

                  PaletteSave();
                  LoadCreatureList(currentList);

                  player.oid.SendServerMessage($"La créature {deletedEntry.name.ColorString(ColorConstants.White)} a bien été supprimée de la palette.", new Color(32, 255, 32));

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
                    currentList = string.IsNullOrEmpty(currentSearch) ? Utils.creaturePaletteList : Utils.creaturePaletteList.Where(c => c.name.ToLower().Contains(currentSearch));
                  else
                  {
                    string selectedCreatorName = creators.GetBindValue(player.oid, nuiToken.Token).FirstOrDefault(c => c.Value == creatorId).Label;
                    currentList = string.IsNullOrEmpty(currentSearch) ? Utils.creaturePaletteList.Where(c => c.creator == selectedCreatorName) : Utils.creaturePaletteList.Where(c => c.name.ToLower().Contains(currentSearch) && c.creator == selectedCreatorName);
                  }

                  LoadCreatureList(currentList);

                  break;

                case "permanentSpawn":
                  creatureTypeEnabler.SetBindValue(player.oid, nuiToken.Token, permanentSpawn.GetBindValue(player.oid, nuiToken.Token));
                  break;
              }

              break;
          }
        }
        private void LoadCreatureList(IEnumerable<PaletteCreatureEntry> filteredList)
        {
          List<string> creatureNameList = new List<string>();
          List<string> creatorNameList = new List<string>();
          List<string> commentList = new List<string>();
          List<string> lastModifiedList = new List<string>();
          List<bool> enabledList = new List<bool>();

          foreach (PaletteCreatureEntry entry in filteredList)
          {
            creatureNameList.Add(entry.name);
            creatorNameList.Add(entry.creator);
            commentList.Add(entry.comment);
            lastModifiedList.Add(entry.lastModified);

            if (player.oid.PlayerName == entry.creator || player.oid.PlayerName == "Chim")
              enabledList.Add(true);
            else
              enabledList.Add(false);
          }

          creatureName.SetBindValues(player.oid, nuiToken.Token, creatureNameList);
          creatorName.SetBindValues(player.oid, nuiToken.Token, creatorNameList);
          comment.SetBindValues(player.oid, nuiToken.Token, commentList);
          lastModified.SetBindValues(player.oid, nuiToken.Token, lastModifiedList);
          isCreatorOrAdmin.SetBindValues(player.oid, nuiToken.Token, enabledList);
          listCount.SetBindValue(player.oid, nuiToken.Token, filteredList.Count());
        }
        private void SelectCreature(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || selection.TargetObject is not NwCreature target)
            return;

          if (currentArrayindex > -1)
          {
            PaletteCreatureEntry entry = currentList.ElementAt(currentArrayindex);
            entry.serializedCreature = target.Serialize().ToBase64EncodedString();
            player.oid.SendServerMessage($"La créature {target.Name.ColorString(ColorConstants.White)} sera désormais utilisée comme modèle pour l'entrée de la palette {entry.name.ColorString(ColorConstants.White)}.", new Color(32, 255, 32));
            selectionTarget = null;
          }
          else
          {
            selectionTarget = target;
            isModelLoaded.SetBindValue(player.oid, nuiToken.Token, true);
          }
        }

        private void SpawnCreature(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled)
            return;

          Location spawnLocation = Location.Create(player.oid.ControlledCreature.Area, selection.TargetPosition, player.oid.ControlledCreature.Rotation);
          NwCreature creature = NwCreature.Deserialize(currentList.ElementAt(currentArrayindex).serializedCreature.ToByteArray());

          if (!permanentSpawn.GetBindValue(player.oid, nuiToken.Token))
          {
            creature.Location = spawnLocation;
            creature.OnPerception += CreatureUtils.OnMobPerception;
          }
          else
          {
            switch (selectedCreatureType.GetBindValue(player.oid, nuiToken.Token))
            {
              case 1:
                creature.Tag = "npc";
                break;
              case 2:
                creature.Tag = "walker";
                break;
            }

            NwWaypoint spawnPoint = NwWaypoint.Create("creature_spawn", spawnLocation);
            creature.GetObjectVariable<LocalVariableString>("_SPAWNED_BY").Value = player.oid.PlayerName;
            spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value = creature.Serialize().ToBase64EncodedString();
          }
        }

        private void HandleInsertNewCreature(string creatureName, string serializedCreature, string playerName, string comment)
        {
          if(!Utils.creaturePaletteList.Any(c => c.creator == playerName))
          {
            Utils.creaturePaletteList.Add(new PaletteCreatureEntry(creatureName, playerName, serializedCreature, DateTime.Now.ToString(), comment));

            Utils.creaturePaletteCreatorsList.Clear();
            Utils.creaturePaletteCreatorsList.Add(new NuiComboEntry("Tous", 0));
            int index = 1;

            foreach (var entry in Utils.creaturePaletteList.DistinctBy(c => c.creator).OrderBy(c => c.creator))
            {
              Utils.creaturePaletteCreatorsList.Add(new NuiComboEntry(entry.creator, index));
              index++;
            }

            creators.SetBindValue(player.oid, nuiToken.Token, Utils.creaturePaletteCreatorsList);
          }
          else
            Utils.creaturePaletteList.Add(new PaletteCreatureEntry(creatureName, playerName, serializedCreature, DateTime.Now.ToString(), comment));

          selectedCreator.SetBindValue(player.oid, nuiToken.Token, Utils.creaturePaletteCreatorsList.FirstOrDefault(c => c.Label == playerName).Value);
          search.SetBindValue(player.oid, nuiToken.Token, creatureName);

          player.oid.SendServerMessage("Votre créature a bien été ajoutée à la palette.", new Color(32, 255, 32));

          PaletteSave();
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
              Log.Info($"Character {player.characterId} : scheduling creature palette save in 10s");
              DebouncePaletteSave(nbDebounce);
              return;
            }
          }
          else
            HandlePaletteSave();

          Log.Info($"Character {player.characterId} creature palette saved in : {(DateTime.Now - elapsed).TotalSeconds} s");
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
            Log.Info($"Character {player.characterId} : debounce done after {nbDebounce} triggers, creature palette save authorized");
            PaletteSave();
          }
        }
        private async void HandlePaletteSave()
        {
          Task<string> serializeCreaturePalette = Task.Run(() => JsonConvert.SerializeObject(Utils.creaturePaletteList.OrderBy(c => c.name).ThenByDescending(c => DateTime.TryParse(c.lastModified, out DateTime lastModified)).ToList()));
          await serializeCreaturePalette;

          SqLiteUtils.UpdateQuery("modulePalette",
          new List<string[]>() { new string[] { "creatures", serializeCreaturePalette.Result } },
          new List<string[]>() { new string[] { "rowid", "1" } });
          
          nbDebounce = 0;
          AuthorizeSave = false;
        }
      }
    }
  }
}