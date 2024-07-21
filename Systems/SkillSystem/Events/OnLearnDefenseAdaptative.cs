using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDefenseAdaptative(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ChasseurDefenseAdaptative))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ChasseurDefenseAdaptative);

      player.oid.LoginCreature.OnDamaged -= RangerUtils.OnDamageDefenseAdaptative;
      player.oid.LoginCreature.OnDamaged += RangerUtils.OnDamageDefenseAdaptative;

      return true;
    }
  }
}
