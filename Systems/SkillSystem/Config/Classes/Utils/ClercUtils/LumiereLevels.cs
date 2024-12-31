using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleLumiereLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Lumière");
          player.oid.SetTextureOverride("clerc", "light_domain");

          player.LearnAlwaysPreparedSpell((int)Spell.Light, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.BurningHands, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.FaerieFire, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.Firebrand, CustomSkill.Clerc);
          player.LearnAlwaysPreparedSpell((int)Spell.SeeInvisibility, CustomSkill.Clerc);

          player.LearnClassSkill(CustomSkill.ClercIllumination);
          player.LearnClassSkill(CustomSkill.ClercRadianceDeLaube);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell((int)Spell.Fireball, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.LumiereDuJour, CustomClass.Clerc);

          break;

        case 7:

          player.LearnAlwaysPreparedSpell((int)Spell.WallOfFire, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.OeilMagique, CustomClass.Clerc);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.FlameStrike, CustomClass.Clerc);
          player.LearnAlwaysPreparedSpell(CustomSpell.Scrutation, CustomClass.Clerc);

          break;

        case 17: player.LearnClassSkill(CustomSkill.ClercHaloDeLumiere); break;
      }
    }
  }
}
