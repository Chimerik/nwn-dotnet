using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnClercElectrocution(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercElectrocution))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ClercElectrocution);

      player.oid.LoginCreature.OnCreatureDamage -= ClercUtils.OnDamageElectrocution;
      player.oid.LoginCreature.OnCreatureDamage += ClercUtils.OnDamageElectrocution;

      return true;
    }
  }
}
