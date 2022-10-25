using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class JukeBoxWindow : PlayerWindow
      {
        NuiColumn rootColumn { get; }
        private readonly NuiBind<string> search = new ("search");
        private readonly NuiBind<string> currentMusic = new ("currentMusic");
        private readonly NuiBind<string> musicNames = new ("musicNames");
        private readonly NuiBind<int> listCount = new ("listCount");
        IEnumerable<AmbientMusicEntry> songList { get; set; }
        private NwCreature bard { get; set; }

        public JukeBoxWindow(Player player, NwCreature bard) : base(player)
        {
          windowId = "jukebox";

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell> { new NuiListTemplateCell(new NuiButton(musicNames) { Id = "select", Height = 35 }) };

          rootColumn = new NuiColumn() { Children = new List<NuiElement>()
          {
            new NuiRow() { Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiLabel(currentMusic), new NuiSpacer() } },
            new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 410 } } },
            new NuiList(rowTemplate, listCount) { RowHeight = 35 },
          } };

          CreateWindow(bard);
        }

        public void CreateWindow(NwCreature bard)
        {
          this.bard = bard;
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          window = new NuiWindow(rootColumn, $"{bard.Name}")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleJukeBoxEvents;

            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            if (bard.Gender == Gender.Female)
            {
              currentMusic.SetBindValue(player.oid, nuiToken.Token, "Ambiance du relais");
              songList = AmbientMusic2da.femaleAmbientMusicEntry;
            }
            else
            {
              currentMusic.SetBindValue(player.oid, nuiToken.Token, "Ambiance du dragon d'argent");
              songList = AmbientMusic2da.maleAmbientMusicEntry;
            }

            LoadList(songList);
          }
        }

        private void HandleJukeBoxEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "select":

                  AmbientMusicEntry selectedSong = songList.ElementAt(nuiEvent.ArrayIndex);
                  currentMusic.SetBindValue(player.oid, nuiToken.Token, selectedSong.name);

                  NwArea area = bard.Area;
                  area.StopBackgroundMusic();
                  area.MusicBackgroundDayTrack = selectedSong.RowIndex;
                  area.MusicBackgroundNightTrack = selectedSong.RowIndex;
                  area.PlayBackgroundMusic();

                  _ = bard.PlayAnimation(Animation.LoopingTalkLaughing, 3, true, TimeSpan.FromHours(24));
                  bard.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSoundBurstSilent));
                  bard.ApplyEffect(EffectDuration.Permanent, Effect.VisualEffect(VfxType.DurBardSong), TimeSpan.FromHours(24));

                  area.GetObjectVariable<LocalVariableString>("_LAST_REQUEST_BY").Value = player.oid.ControlledCreature.Name;

                  ChatSystem.chatService.SendMessage(ChatChannel.PlayerTalk, $"{player.oid.ControlledCreature.Name} vient de demander à jouer {selectedSong.name}", bard);

                  foreach(NwPlayer listener in NwModule.Instance.Players)
                    if(listener.ControlledCreature?.Area == area && Players.TryGetValue(listener.LoginCreature, out PlayerSystem.Player listeningPlayer) && listeningPlayer.TryGetOpenedWindow("jukeBoxCurrentSong", out PlayerWindow songWindow))
                    {
                      ((JukeBoxCurrentSongWindow)songWindow).song.SetBindValue(listener, songWindow.nuiToken.Token, $"Chanson en cours : {selectedSong.name}");
                      ((JukeBoxCurrentSongWindow)songWindow).askedBy.SetBindValue(listener, songWindow.nuiToken.Token, $"Demandée par : {player.oid.ControlledCreature.Name}");
                    } 

                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "search":

                  string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
                  var filteredList = songList;

                  if (!string.IsNullOrEmpty(currentSearch))
                    filteredList = filteredList.Where(m => m.name.ToLower().Contains(currentSearch));

                  LoadList(filteredList);

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
      }
    }
  }
}
