using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using NLog.LayoutRenderers.Wrappers;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class AreaMusicEditorWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiBind<string> search = new("search");
        private readonly NuiBind<string> currentMusic = new("currentMusic");
        private readonly NuiBind<string> musicNames = new("musicNames");
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<int> musicTypeSelected = new("musicTypeSelected");
        private readonly List<NuiComboEntry> musicTypeCombo = new()
        {
          new("Ambiance jour", 0),
          new("Ambiance nuit", 1),
          new("Combat", 2)
        };
        IEnumerable<AmbientMusicEntry> songList { get; set; }
        private NwArea area { get; set; }
        private int saveScheduled { get; set; }

        public AreaMusicEditorWindow(Player player, NwArea area) : base(player)
        {
          windowId = "areaMusicEditor";
          saveScheduled = 0;
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>  { new NuiListTemplateCell(new NuiButton(musicNames) { Id = "select", Tooltip = musicNames, Height = 35, Width = 410 }) };

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel(currentMusic) { Tooltip = currentMusic, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle, Width = 375,  Height = 35 },
            new NuiButtonImage("ir_empytqs") { Id = "persist", Tooltip = "Rendre persistante la sélection musicale de cette zone", Width = 35, Height = 35 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCombo() { Entries = musicTypeCombo, Selected = musicTypeSelected, Width = 410, Height = 35 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 410, Height = 35 } } });
          rootChildren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 35 });

          CreateWindow(area);
        }

        public void CreateWindow(NwArea area)
        {
          this.area = area;
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          window = new NuiWindow(rootColumn, $"Choix de musique - {area.Name}")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleAreaMusicEditorEvents;

            currentMusic.SetBindValue(player.oid, nuiToken.Token, AmbientMusic2da.ambientMusicTable.GetRow(area.MusicBackgroundDayTrack).name);
            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);
            musicTypeSelected.SetBindValue(player.oid, nuiToken.Token, 0);
            musicTypeSelected.SetBindWatch(player.oid, nuiToken.Token, true);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            songList = AmbientMusic2da.ambientMusicTable.ToList();
            LoadList(songList);
          }
        }

        private void HandleAreaMusicEditorEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "select":

                  AmbientMusicEntry selectedSong = songList.ElementAt(nuiEvent.ArrayIndex);
                  currentMusic.SetBindValue(player.oid, nuiToken.Token, selectedSong.name);
                  area.StopBackgroundMusic();
                  area.StopBattleMusic();

                  switch (musicTypeSelected.GetBindValue(player.oid, nuiToken.Token))
                  {
                    case 0: 
                      area.MusicBackgroundDayTrack = selectedSong.RowIndex;
                      area.PlayBackgroundMusic();
                      break;
                    case 1: area.MusicBackgroundNightTrack = selectedSong.RowIndex;
                      area.PlayBackgroundMusic();
                      break;
                    case 2: area.MusicBattleTrack = selectedSong.RowIndex;
                      area.PlayBattleMusic();
                      break;
                  }

                  player.oid.SendServerMessage($"Vous venez de sélectionner : {selectedSong.name}", ColorConstants.Orange);

                  break;

                case "persist":

                  if (saveScheduled > 0)
                    saveScheduled += 1;
                  else
                  {
                    saveScheduled = 1;
                    Utils.LogMessageToDMs($"{player.oid.PlayerName} : scheduling {area.Name} musics save in 10s");
                    DebounceSave(area, 1, player.oid.PlayerName);
                  }

                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "search":

                  string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
                  var filteredList = string.IsNullOrEmpty(currentSearch) ? songList : songList.Where(m => m.name.ToLower().Contains(currentSearch));
                  LoadList(filteredList);

                  break;

                case "musicTypeSelected":
                  
                  switch (musicTypeSelected.GetBindValue(player.oid, nuiToken.Token))
                  {
                    case 0: currentMusic.SetBindValue(player.oid, nuiToken.Token, AmbientMusic2da.ambientMusicTable.GetRow(area.MusicBackgroundDayTrack).name); break;
                    case 1: currentMusic.SetBindValue(player.oid, nuiToken.Token, AmbientMusic2da.ambientMusicTable.GetRow(area.MusicBackgroundNightTrack).name); break;
                    case 2: currentMusic.SetBindValue(player.oid, nuiToken.Token, AmbientMusic2da.ambientMusicTable.GetRow(area.MusicBattleTrack).name); break;
                  }

                  break;
              }
              break;
          }
        }
        private void LoadList(IEnumerable<AmbientMusicEntry> songList)
        {
          List<string> nameList = new();

          foreach (AmbientMusicEntry song in songList)
            nameList.Add(song.name);

          musicNames.SetBindValues(player.oid, nuiToken.Token, nameList);
          listCount.SetBindValue(player.oid, nuiToken.Token, nameList.Count);
        }
        public async void DebounceSave(NwArea areaToSave, int nbDebounces, string playerName)
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task awaitDebounce = NwTask.WaitUntil(() => saveScheduled != nbDebounces, tokenSource.Token);
          Task awaitSaveAuthorized = NwTask.Delay(TimeSpan.FromSeconds(10), tokenSource.Token);

          await NwTask.WhenAny(awaitDebounce, awaitSaveAuthorized);
          tokenSource.Cancel();

          if (areaToSave == null || !areaToSave.IsValid)
            return;

          if (awaitDebounce.IsCompletedSuccessfully)
          {
            DebounceSave(areaToSave, nbDebounces + 1, playerName);
            return;
          }

          if (awaitSaveAuthorized.IsCompletedSuccessfully)
          {
            saveScheduled = 0;
            Utils.LogMessageToDMs($"{playerName} : {area.Name} saving musics");
            HandleSave(areaToSave);
          }
        }
        public async void HandleSave(NwArea areaToSave)
        {
          await SqLiteUtils.InsertQueryAsync("areaMusics",
          new List<string[]>() {
            new string[] { "areaTag", areaToSave.Tag },
            new string[] { "backgroundDay", areaToSave.MusicBackgroundDayTrack.ToString() },
            new string[] { "backgroundNight", areaToSave.MusicBackgroundNightTrack.ToString() },
            new string[] { "battle", areaToSave.MusicBattleTrack.ToString() } },
          new List<string>() { "areaTag" },
          new List<string[]>() { new string[] { "backgroundDay" }, new string[] { "backgroundNight" }, new string[] { "battle" } });
        }
      }
    }
  }
}
