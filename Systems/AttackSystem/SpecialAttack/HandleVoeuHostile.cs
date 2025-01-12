using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Core;
using NWN.Native.API;
using ClassType = NWN.Native.API.ClassType;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleVoeuHostile(CNWSCreature attacker, CNWSCombatRound round, CNWSCombatAttackData data, string attackerName)
    {
      foreach (var eff in attacker.m_appliedEffects.Where(e => e.m_sCustomTag.ToString() == EffectSystem.VoeuDHostiliteEffectTag))
      {
        var creator = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(eff.m_oidCreator);

        if (creator is null || creator.m_ScriptVars.GetInt(CreatureUtils.VoeuHostileVariableExo) > 0
          || creator.m_pStats.GetNumLevelsOfClass((byte)ClassType.Paladin) < 15
          || Vector3.DistanceSquared(creator.m_vPosition.ToManagedVector(), attacker.m_vPosition.ToManagedVector()) > 9)
          continue;

        creator.m_ScriptVars.SetObject(CreatureUtils.VoeuHostileVariableExo, attacker.m_idSelf);
        creator.m_ScriptVars.SetInt(CreatureUtils.VoeuHostileVariableExo, 1);
      }

      if (!data.m_bRangedAttack.ToBool() && attacker.m_ScriptVars.GetObject(CreatureUtils.VoeuHostileVariableExo) != NWScript.OBJECT_INVALID)
      {
        var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.m_ScriptVars.GetObject(CreatureUtils.VoeuHostileVariableExo));
        attacker.m_ScriptVars.DestroyObject(CreatureUtils.VoeuHostileVariableExo);

        if (target is null)
          return;

        string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
        BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} abat son hostilité sur {targetName}", attacker);

        round.AddWhirlwindAttack(target.m_idSelf, 1);
        LogUtils.LogMessage($"Attaque supplémentaire - Voeu d'hostilité", LogUtils.LogType.Combat);
      }

    }
  }
}
