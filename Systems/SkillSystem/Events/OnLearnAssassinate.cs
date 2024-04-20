using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAssassinate(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AssassinAssassinate))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.AssassinAssassinate);

      player.oid.OnCombatStatusChange -= RogueUtils.OnCombatAssassinate;
      player.oid.OnCombatStatusChange += RogueUtils.OnCombatAssassinate;

      return true;
    }
  }
}
