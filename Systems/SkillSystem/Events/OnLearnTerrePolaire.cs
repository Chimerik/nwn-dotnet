using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTerreAride(PlayerSystem.Player player, int customSkillId)
    {
      SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.HoldPerson, CustomClass.Druid);
      SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.RayOfFrost, CustomClass.Druid);
      SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.NappeDeBrouillard, CustomClass.Druid);

      return true;
    }
  }
}
