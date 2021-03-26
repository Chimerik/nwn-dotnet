using System;
using System.Collections.Generic;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
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
      player.setValue = Config.invalidInput;
      player.menu.Clear();

      int currentMusic = bard.Area.MusicBackgroundDayTrack;

      player.menu.titleLines = new List<string> {
        $"Bonjour ! ",
        $"Chanson actuelle : {NWScript.GetStringByStrRef(Int32.Parse(NWScript.Get2DAString("ambientmusic", "Description", currentMusic)))}",
        "Souhaitez-vous que je vous joue autre chose ?"
      };

      int[] musicArray = new int[] { 98, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178 };

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

      NwArea area = player.oid.Area;
      area.StopBackgroundMusic();
      area.MusicBackgroundDayTrack = music;
      area.MusicBackgroundNightTrack = music;
      area.PlayBackgroundMusic();


      bard.PlayAnimation(API.Constants.Animation.LoopingTalkLaughing, 3, true, TimeSpan.FromHours(24));
      bard.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(API.Constants.VfxType.FnfSoundBurstSilent));
      bard.ApplyEffect(EffectDuration.Permanent, API.Effect.VisualEffect(API.Constants.VfxType.DurBardSong), TimeSpan.FromHours(24));

      ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, $"{player.oid.Name} vient de demander à jouer {NWScript.GetStringByStrRef(Int32.Parse(NWScript.Get2DAString("ambientmusic", "Description", music)))}", bard);

      this.DrawWelcomePage(player, bard);
    }
  }
}
