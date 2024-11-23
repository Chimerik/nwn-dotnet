using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTerrePolaire(PlayerSystem.Player player, int customSkillId)
    {
      player.LearnAlwaysPreparedSpell((int)Spell.BurningHands, CustomClass.Druid);
      player.LearnAlwaysPreparedSpell((int)Spell.GhostlyVisage, CustomClass.Druid);
      player.LearnAlwaysPreparedSpell(CustomSpell.FireBolt, CustomClass.Druid);

      return true;
    }
  }
}
