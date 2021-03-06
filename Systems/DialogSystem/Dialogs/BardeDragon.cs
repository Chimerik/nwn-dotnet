﻿using System;
using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class BardeDragon
  {
    public BardeDragon(Player player, NwCreature bard)
    {
      this.DrawWelcomePage(player, bard);
    }
    private void DrawWelcomePage(Player player, NwCreature bard)
    {
      player.menu.Clear();

      int currentMusic = bard.Area.MusicBackgroundDayTrack;

      player.menu.titleLines = new List<string> {
        $"Bonjour ! ",
        $"Chanson actuelle : {AmbientMusic2da.ambientMusicTable.GetName(currentMusic)}",
        "Souhaitez-vous que je vous joue autre chose ?"
      };

      int[] musicArray = new int[] { 98, 133, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179 };

      foreach (int music in musicArray)
      {
        if (currentMusic != music)
          player.menu.choices.Add(($"{AmbientMusic2da.ambientMusicTable.GetName(currentMusic)}", () => HandleChangeBackgroundMusic(player, music, bard)));
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private async void HandleChangeBackgroundMusic(Player player, int music, NwCreature bard)
    {
      player.menu.Clear();

      NwArea area = bard.Area;
      area.StopBackgroundMusic();
      area.MusicBackgroundDayTrack = music;
      area.MusicBackgroundNightTrack = music;
      area.PlayBackgroundMusic();


      await bard.PlayAnimation(Animation.LoopingTalkLaughing, 3, true, TimeSpan.FromHours(24));
      bard.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSoundBurstSilent));
      bard.ApplyEffect(EffectDuration.Permanent, Effect.VisualEffect(VfxType.DurBardSong), TimeSpan.FromHours(24));

      ChatSystem.chatService.SendMessage(ChatChannel.PlayerTalk, $"{player.oid.ControlledCreature.Name} vient de demander à jouer {AmbientMusic2da.ambientMusicTable.GetName(music)}", bard);

      this.DrawWelcomePage(player, bard);
    }
  }
}
