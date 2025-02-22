using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMagieDeGuerre(PlayerSystem.Player player, int customSkillId)
    {
      if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EldritchKnightMagieDeGuerre))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.EldritchKnightMagieDeGuerre);

      player.oid.LoginCreature.OnSpellAction -= FighterUtils.OnSpellCastMagieDeGuerre;
      player.oid.LoginCreature.OnSpellAction += FighterUtils.OnSpellCastMagieDeGuerre;

      player.oid.LoginCreature.OnCreatureAttack -= FighterUtils.OnAttackMagieDeGuerre;
      player.oid.LoginCreature.OnCreatureAttack += FighterUtils.OnAttackMagieDeGuerre;

      return true;
    }
  }
}
