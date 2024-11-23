using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTerreTropicale(PlayerSystem.Player player, int customSkillId)
    {
      player.LearnAlwaysPreparedSpell((int)Spell.AcidSplash, CustomClass.Druid);
      player.LearnAlwaysPreparedSpell((int)Spell.Web, CustomClass.Druid);
      player.LearnAlwaysPreparedSpell(CustomSpell.RayonEmpoisonne, CustomClass.Druid);
      
      return true;
    }
  }
}
