using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class Jukebox
  {
    public Jukebox(Player player, uint bard)
    {
      this.DrawWelcomePage(player, bard);
    }
    private void DrawWelcomePage(Player player, uint bard)
    {
      player.setValue = 0;
      player.menu.Clear();

      int currentMusic = NWScript.MusicBackgroundGetDayTrack(NWScript.GetArea(player.oid));
      
      player.menu.titleLines = new List<string> {
        $"Bonjour ! ",
        $"Chanson actuelle : {NWScript.GetStringByStrRef(Int32.Parse(NWScript.Get2DAString("ambientmusic", "Description", currentMusic)))}",
        "Souhaitez-vous que je vous joue autre chose ?"
      };

      int[] musicArray = new int[] { 96, 97, 98, 121 };

      foreach (int music in musicArray)
      {
        if (currentMusic != music)
          player.menu.choices.Add(($"{NWScript.GetStringByStrRef(Int32.Parse(NWScript.Get2DAString("ambientmusic", "Description", music)))}", () => HandleChangeBackgroundMusic(player, music, bard)));
      }

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleChangeBackgroundMusic(Player player, int music, uint bard)
    {
      player.menu.Clear();

      uint area = NWScript.GetArea(player.oid);
      NWScript.MusicBackgroundStop(area);
      NWScript.MusicBackgroundChangeDay(area, music);
      NWScript.MusicBackgroundChangeNight(area, music);
      NWScript.MusicBackgroundPlay(area);

      NWScript.AssignCommand(bard, () => NWScript.PlayAnimation(NWScript.ANIMATION_LOOPING_TALK_LAUGHING, 3, 99999.0f));
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_FNF_SOUND_BURST_SILENT), bard);
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, NWScript.EffectVisualEffect(NWScript.VFX_DUR_BARD_SONG), bard);

      ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, $"{NWScript.GetName(player.oid)} vient de demander à jouer {NWScript.GetStringByStrRef(Int32.Parse(NWScript.Get2DAString("ambientmusic", "Description", music)))}", bard);

      this.DrawWelcomePage(player, bard);
    }
  }
}
