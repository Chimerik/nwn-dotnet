﻿using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Druide
  {
    public static void HandleCerclePelagiqueLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(6).SetPlayerOverride(player.oid, "Cercle Pélagique");
          player.oid.SetTextureOverride("druide", "druide_mer");

          player.LearnAlwaysPreparedSpell((int)Spell.RayOfFrost, CustomClass.Druid);
          player.LearnAlwaysPreparedSpell(CustomSpell.NappeDeBrouillard, CustomClass.Druid);
          player.LearnAlwaysPreparedSpell(CustomSpell.Fracassement, CustomClass.Druid);
          player.LearnAlwaysPreparedSpell((int)Spell.GustOfWind, CustomClass.Druid);
          player.LearnAlwaysPreparedSpell((int)Spell.Balagarnsironhorn, CustomClass.Druid);

          player.LearnClassSkill(CustomSkill.DruideFureurDesFlots);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell((int)Spell.LightningBolt, CustomClass.Druid);
          player.LearnAlwaysPreparedSpell(CustomSpell.RespirationAquatique, CustomClass.Druid);

          break;

        case 7:

          player.LearnAlwaysPreparedSpell((int)Spell.IceStorm, CustomClass.Druid);
          player.LearnAlwaysPreparedSpell(CustomSpell.ControleDeLeau, CustomClass.Druid);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.HoldMonster, CustomClass.Druid);
          player.LearnAlwaysPreparedSpell((int)Spell.SummonCreatureVii, CustomClass.Druid);

          break;
      }
    }
  }
}
