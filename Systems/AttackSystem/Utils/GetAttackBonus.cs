using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    private static readonly CExoString chargerVariable = "_CHARGER_INITIAL_LOCATION".ToExoString();
    public static int GetAttackBonus(CNWSCreature attacker, CNWSCreature target, CNWSCombatAttackData attackData)
    {
      int attackBonus = attacker.m_pStats.GetAttackModifierVersus(target);

      if (attackData.m_bRangedAttack > 1 && PlayerSystem.Players.TryGetValue(attacker.m_idSelf, out PlayerSystem.Player player)
        && player.learnableSkills.TryGetValue(CustomSkill.FighterCombatStyleArchery, out LearnableSkill archery)
        && archery.currentLevel > 0)
        attackBonus += 2;

      var initiaLocation = attacker.m_pStats.m_pBaseCreature.m_ScriptVars.GetLocation(chargerVariable);

      if (initiaLocation is not null && Vector3.Distance(initiaLocation.m_vPosition.ToManagedVector(), attacker.m_vPosition.ToManagedVector()) > 3)
      {
        attackBonus += 5;
        attacker.m_pStats.m_pBaseCreature.m_ScriptVars.SetInt("_CHARGER_ACTIVATED".ToExoString(), 1);
      }

      if (IsCogneurLourd(attacker, attackData))
      {
        attackBonus -= 5;
        LogUtils.LogMessage($"Cogneur Lourd : -5 BA", LogUtils.LogType.Combat);
      }
      else if (IsTireurDelite(attacker, attackData))
      {
        attackBonus -= 5;
        LogUtils.LogMessage($"Tireur d'élite : -5 BA", LogUtils.LogType.Combat);
      }

      return attackBonus;
    }
  }
}
