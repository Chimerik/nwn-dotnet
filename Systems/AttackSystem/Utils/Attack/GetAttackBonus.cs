using System.Numerics;
using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    private static readonly CExoString chargerVariable = "_CHARGER_INITIAL_LOCATION".ToExoString();
    public static int GetAttackBonus(CNWSCreature attacker, CNWSCreature target, CNWSCombatAttackData attackData, CNWSItem weapon)
    {
      int attackBonus = attacker.m_pStats.GetAttackModifierVersus(target);

      if (attackData.m_bRangedAttack.ToBool() && attacker.m_pStats.HasFeat(CustomSkill.FighterCombatStyleArchery).ToBool())
      {
        attackBonus += 2;
        LogUtils.LogMessage($"Style de combat archerie : +2 BA", LogUtils.LogType.Combat);
      }

      var initiaLocation = attacker.m_pStats.m_pBaseCreature.m_ScriptVars.GetLocation(chargerVariable);
      
      if (initiaLocation.m_oArea != NWScript.OBJECT_INVALID && Vector3.Distance(initiaLocation.m_vPosition.ToManagedVector(), attacker.m_vPosition.ToManagedVector()) > 3)
      {
        attackBonus += 5;
        attacker.m_pStats.m_pBaseCreature.m_ScriptVars.SetInt("_CHARGER_ACTIVATED".ToExoString(), 1);
        LogUtils.LogMessage($"Chargeur : +5 BA", LogUtils.LogType.Combat);
      }

      if (weapon is not null)
      {
        if (IsCogneurLourd(attacker, weapon))
        {
          attackBonus -= 5;
          LogUtils.LogMessage($"Cogneur Lourd : -5 BA", LogUtils.LogType.Combat);
        }
        else if (IsTireurDelite(attacker, attackData, weapon))
        {
          attackBonus -= 5;
          LogUtils.LogMessage($"Tireur d'élite : -5 BA", LogUtils.LogType.Combat);
        }
      }

      return attackBonus;
    }
  }
}
