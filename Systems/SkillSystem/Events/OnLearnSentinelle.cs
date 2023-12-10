using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnSentinelle(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Sentinelle)))
        player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.Sentinelle));

      player.oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackSentinelle;
      player.oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackSentinelle;

      return true;
    }
  }
}
