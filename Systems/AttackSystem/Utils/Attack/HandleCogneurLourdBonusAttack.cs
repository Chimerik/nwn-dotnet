using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleCogneurLourdBonusAttack(CNWSCreature attacker, CNWSObject target, CNWSCombatRound combatRound, CNWSCombatAttackData attackData, int damageDealt, string attackerName)
    {
      if (attackData.m_nAttackResult != 3 && target.m_nCurrentHitPoints > damageDealt)
        return;

      if (attacker.m_pStats.HasFeat(CustomSkill.CogneurLourd).ToBool() && combatRound.GetCurrentAttackWeapon() is not null
      && !attackData.m_bRangedAttack.ToBool())
      {
        var bonusAction = attacker.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.BonusActionEffectTag);

        if (bonusAction is null)
          return;

        CNWSObject bonusTarget = target;

        if (target.m_nCurrentHitPoints - damageDealt < 1)
        {
          bonusTarget = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.GetNearestEnemy(3, target.m_idSelf, 1, 1));

          if (bonusTarget is null || bonusTarget.m_idSelf == 0x7F000000) // OBJECT_INVALID
            return;

          combatRound.AddWhirlwindAttack(bonusTarget.m_idSelf, 1);
        }
        else
          combatRound.AddCleaveAttack(bonusTarget.m_idSelf);

        string targetName = $"{bonusTarget.GetFirstName().GetSimple(0)} {bonusTarget.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
        BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} cogneur lourd contre {targetName}", attacker);

        attacker.RemoveEffect(bonusAction);
        LogUtils.LogMessage($"Attaque supplémentaire - Cogneur Lourd", LogUtils.LogType.Combat);
      }
    }
  }
}
