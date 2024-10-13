using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTerreTempere(PlayerSystem.Player player, int customSkillId)
    {
      SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.ElectricJolt, CustomClass.Druid);
      SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Sleep, CustomClass.Druid);
      SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.FouleeBrumeuse, CustomClass.Druid);

      return true;
    }
  }
}
