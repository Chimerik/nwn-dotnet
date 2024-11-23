using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTerreTempere(PlayerSystem.Player player, int customSkillId)
    {
      player.LearnAlwaysPreparedSpell((int)Spell.ElectricJolt, CustomClass.Druid);
      player.LearnAlwaysPreparedSpell((int)Spell.Sleep, CustomClass.Druid);
      player.LearnAlwaysPreparedSpell(CustomSpell.FouleeBrumeuse, CustomClass.Druid);

      return true;
    }
  }
}
