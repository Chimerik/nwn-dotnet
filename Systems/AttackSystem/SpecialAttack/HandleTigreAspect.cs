using System.Numerics;
using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleTigreAspect(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound combatRound, string attackerName)
    {
      if (attacker.m_ScriptVars.GetInt(CreatureUtils.AspectTigreVariableExo).ToBool())
      {
        currentTarget.m_ScriptVars.SetInt(CreatureUtils.ApplyBleedVariableExo, 1);

        var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.GetNearestEnemy(2, currentTarget.m_idSelf, 0, 1));

        if (target is null || target.m_idSelf == 0x7F000000) // OBJECT_INVALID
          return;

        attacker.m_ScriptVars.DestroyInt(CreatureUtils.AspectTigreVariableExo);
        attacker.m_ScriptVars.SetInt(CreatureUtils.AspectTigreMalusVariableExo, 1);
        target.m_ScriptVars.SetInt(CreatureUtils.ApplyBleedVariableExo, 1);

        string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
        BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} rage du tigre {targetName}", attacker);
        combatRound.AddWhirlwindAttack(target.m_idSelf, 1);
        LogUtils.LogMessage($"Attaque supplémentaire - Rage du tigre", LogUtils.LogType.Combat);
      }

      if (attacker.m_ScriptVars.GetInt(CreatureUtils.AspectTigreMalusVariableExo).ToBool())
      {
        int tigerMalus = attacker.m_ScriptVars.GetInt(CreatureUtils.AspectTigreMalusVariableExo);

        if (tigerMalus > 2)
          attacker.m_ScriptVars.DestroyInt(CreatureUtils.AspectTigreMalusVariableExo);
        else
          attacker.m_ScriptVars.SetInt(CreatureUtils.AspectTigreMalusVariableExo, tigerMalus + 1);
      }
    }
  }
}
