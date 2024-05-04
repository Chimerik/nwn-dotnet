using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnBerserkerRepresailles(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BersekerRepresailles))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.BersekerRepresailles);

      player.oid.LoginCreature.OnDamaged -= BarbarianUtils.OnDamagedBerserkerRepresailles;
      player.oid.LoginCreature.OnDamaged += BarbarianUtils.OnDamagedBerserkerRepresailles;

      return true;
    }
  }
}
