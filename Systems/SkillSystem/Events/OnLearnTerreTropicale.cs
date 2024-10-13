using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTerreTropicale(PlayerSystem.Player player, int customSkillId)
    {
      SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.AcidSplash, CustomClass.Druid);
      SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Web, CustomClass.Druid);
      SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.RayonEmpoisonne, CustomClass.Druid);
      
      return true;
    }
  }
}
