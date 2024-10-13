using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTerrePolaire(PlayerSystem.Player player, int customSkillId)
    {
      SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.BurningHands, CustomClass.Druid);
      SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.GhostlyVisage, CustomClass.Druid);
      SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.FireBolt, CustomClass.Druid);

      return true;
    }
  }
}
