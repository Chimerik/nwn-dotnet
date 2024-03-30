using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTueurDeMage(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.TueurDeMage))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.TueurDeMage);

      player.oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackTueurDeMage;
      player.oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackTueurDeMage;

      return true;
    }
  }
}
