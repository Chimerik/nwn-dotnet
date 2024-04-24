namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFrappeOcculte(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.OnCreatureAttack -= FighterUtils.OnAttackFrappeOcculte;
      player.oid.LoginCreature.OnCreatureAttack += FighterUtils.OnAttackFrappeOcculte;

      return true;
    }
  }
}
