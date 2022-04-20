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

          rootColumn = new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Height = 35, Children = new List<NuiElement>() { new NuiLabel(currentMusic) } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 410 } } },
              new NuiList(rowTemplate, listCount) { RowHeight = 35 },
            }
          };

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

          player.oid.OnNuiEvent -= HandleJukeBoxEvents;
          player.oid.OnNuiEvent += HandleJukeBoxEvents;
          player.oid.OnServerSendArea -= OnAreaChangeCloseWindow;
          player.oid.OnServerSendArea += OnAreaChangeCloseWindow;

          token = player.oid.CreateNuiWindow(window, windowId);

          search.SetBindValue(player.oid, token, "");
          search.SetBindWatch(player.oid, token, true);
          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          if (bard.Gender == Gender.Female)
          {
            currentMusic.SetBindValue(player.oid, token, "Ambiance du relais");
            songList = AmbientMusic2da.femaleAmbientMusicEntry;
          }
          else
          {
            currentMusic.SetBindValue(player.oid, token, "Ambiance du dragon d'argent");
            songList = AmbientMusic2da.maleAmbientMusicEntry;
          }

          LoadList(songList);
        }

        private void HandleJukeBoxEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "select":

                  AmbientMusicEntry selectedSong = songList.ElementAt(nuiEvent.ArrayIndex);
                  currentMusic.SetBindValue(player.oid, token, selectedSong.name);

                  NwArea area = bard.Area;
                  area.StopBackgroundMusic();
                  area.MusicBackgroundDayTrack = selectedSong.RowIndex;
                  area.MusicBackgroundNightTrack = selectedSong.RowIndex;
                  area.PlayBackgroundMusic();

                  _ = bard.PlayAnimation(Animation.LoopingTalkLaughing, 3, true, TimeSpan.FromHours(24));
                  bard.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSoundBurstSilent));
                  bard.ApplyEffect(EffectDuration.Permanent, Effect.VisualEffect(VfxType.DurBardSong), TimeSpan.FromHours(24));

                  ChatSystem.chatService.SendMessage(ChatChannel.PlayerTalk, $"{player.oid.ControlledCreature.Name} vient de demander à jouer {selectedSong.name}", bard);

                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "search":

                  string currentSearch = search.GetBindValue(player.oid, token).ToLower();
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
          List<string> nameList = new List<string>();

          foreach (AmbientMusicEntry song in songList) 
            nameList.Add(song.name);

          musicNames.SetBindValues(player.oid, token, nameList);
          listCount.SetBindValue(player.oid, token, nameList.Count);
        }
      }
    }
  }
}
