using System;
using System.Collections.Generic;
using NWN.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Jukebox
  {
    public Jukebox(Player player, NwCreature bard)
    {
      this.DrawWelcomePage(player, bard);
    }
    private void DrawWelcomePage(Player player, NwCreature bard)
    {
      player.menu.Clear();

      int currentMusic = bard.Area.MusicBackgroundDayTrack;
      
      player.menu.titleLines = new List<string> {
        $"Bonjour ! ",
        $"Chanson actuelle : {NWScript.GetStringByStrRef(Int32.Parse(NWScript.Get2DAString("ambientmusic", "Description", currentMusic)))}",
        "Souhaitez-vous que je vous joue autre chose ?"
      };

      int[] musicArray = new int[] { 96, 97, 98, 121, 151, 152, 153, 162, 163, 164, 165, 166, 167, 168, 170, 171, 172, 173, 174, 175, 176, 177, 178 };

      foreach (int music in musicArray)
      {
        if (currentMusic != music)
          player.menu.choices.Add(($"{NWScript.GetStringByStrRef(Int32.Parse(NWScript.Get2DAString("ambientmusic", "Description", music)))}", () => HandleChangeBackgroundMusic(player, music, bard)));
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleChangeBackgroundMusic(Player player, int music, NwCreature bard)
    {
      player.menu.Clear();

      NwArea area = bard.Area;
      area.StopBackgroundMusic();
      area.MusicBackgroundDayTrack = music;
      area.MusicBackgroundNightTrack = music;
      area.PlayBackgroundMusic();


      bard.PlayAnimation(API.Constants.Animation.LoopingTalkLaughing, 3, true, TimeSpan.FromHours(24));
      bard.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(API.Constants.VfxType.FnfSoundBurstSilent));
      bard.ApplyEffect(EffectDuration.Permanent, API.Effect.VisualEffect(API.Constants.VfxType.DurBardSong), TimeSpan.FromHours(24));

      ChatSystem.chatService.SendMessage(Services.ChatChannel.PlayerTalk, $"{player.oid.ControlledCreature.Name} vient de demander à jouer {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("ambientmusic", "Description", music)))}", bard);
      this.DrawWelcomePage(player, bard);
    }
  }
}
