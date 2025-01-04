using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTerrePolaire(PlayerSystem.Player player, int customSkillId)
    {
      player.LearnAlwaysPreparedSpell((int)Spell.HoldPerson, CustomClass.Druid);
      player.LearnAlwaysPreparedSpell((int)Spell.RayOfFrost, CustomClass.Druid);
      player.LearnAlwaysPreparedSpell(CustomSpell.NappeDeBrouillard, CustomClass.Druid);

      return true;
    }
  }
}
