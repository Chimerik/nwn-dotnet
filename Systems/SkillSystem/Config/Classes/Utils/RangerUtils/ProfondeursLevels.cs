using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Ranger
  {
    public static void HandleProfondeursLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(14).SetPlayerOverride(player.oid, "Conclave des Profondeurs");
          player.oid.SetTextureOverride("ranger", "profondeurs");

          player.LearnClassSkill(CustomSkill.TraqueurRedoutable);
          player.LearnClassSkill(CustomSkill.TraqueurLinceulDombre);
          player.LearnClassSkill(CustomSkill.ProfondeursFrappeRedoutable);
          player.LearnAlwaysPreparedSpell(CustomSpell.Deguisement, CustomClass.Ranger);

          break;

        case 5:  player.LearnAlwaysPreparedSpell(CustomSpell.CordeEnchantee, CustomClass.Ranger); break;
        case 7: player.LearnClassSkill(CustomSkill.WisdomSavesProficiency); break;
        case 9: player.LearnAlwaysPreparedSpell((int)Spell.GlyphOfWarding, CustomClass.Ranger); break;
        case 11: player.LearnClassSkill(CustomSkill.TraqueurRafale);  break;
        case 13: player.LearnAlwaysPreparedSpell((int)Spell.ImprovedInvisibility, CustomClass.Ranger); break;
        case 15: player.LearnClassSkill(CustomSkill.TraqueurEsquive); break;
        case 17: player.LearnAlwaysPreparedSpell(CustomSpell.ApparencesTrompeuses, CustomClass.Ranger); break;
      }
    }
  }
}
