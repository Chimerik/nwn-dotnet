namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMagieDeGuerre(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.OnSpellAction -= FighterUtils.OnSpellCastMagieDeGuerre;
      player.oid.LoginCreature.OnSpellAction += FighterUtils.OnSpellCastMagieDeGuerre;

      player.oid.LoginCreature.OnCreatureAttack -= FighterUtils.OnAttackMagieDeGuerre;
      player.oid.LoginCreature.OnCreatureAttack += FighterUtils.OnAttackMagieDeGuerre;

      return true;
    }
  }
}
