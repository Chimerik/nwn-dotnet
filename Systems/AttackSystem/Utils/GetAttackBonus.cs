using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetAttackBonus(CNWSCreature attacker, CNWSCreature target, CNWSCombatAttackData attackData)
    {
      int attackBonus = attacker.m_pStats.GetAttackModifierVersus(target);

      if (attackData.m_bRangedAttack > 1 && PlayerSystem.Players.TryGetValue(attacker.m_idSelf, out PlayerSystem.Player player)
        && player.learnableSkills.TryGetValue(CustomSkill.FighterCombatStyleArchery, out LearnableSkill archery)
        && archery.currentLevel > 0)
        attackBonus += 2;

      return attackBonus;
    }
  }
}
