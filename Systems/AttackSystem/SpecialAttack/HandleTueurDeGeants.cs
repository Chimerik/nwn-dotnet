using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleTueurDeGeants(CNWSCreature attacker, CNWSCreature currentTarget, CNWSCombatRound combatRound, string attackerName, CNWSItem weapon, bool isRangedAttack)
    {
      uint targetID = attacker.m_ScriptVars.GetObject(CreatureUtils.TueurDeGeantsTargetVariableExo);

      if (targetID != 0x7F000000)
      {
        var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(targetID);

        if (target is not  null && target.m_idSelf != 0x7F000000) // OBJECT_INVALID
        {
          attacker.m_ScriptVars.SetInt(CreatureUtils.TueurDeGeantsCoolDownVariableExo, 1);
          attacker.m_ScriptVars.DestroyObject(CreatureUtils.TueurDeGeantsTargetVariableExo);

          combatRound.AddWhirlwindAttack(target.m_idSelf, 1);

          string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
          DelayMessage($"{attackerName.ColorString(ColorConstants.Cyan)} tueur de géant sur {targetName.ColorString(ColorConstants.Cyan)}", attacker);
        }
      }

      if (isRangedAttack || attacker.m_nCreatureSize < 4 || !currentTarget.m_pStats.HasFeat(CustomSkill.ChasseurTueurDeGeants).ToBool()
        || currentTarget.m_ScriptVars.GetInt(CreatureUtils.TueurDeGeantsCoolDownVariableExo).ToBool())
        return;

      currentTarget.m_ScriptVars.SetObject(CreatureUtils.TueurDeGeantsTargetVariableExo, currentTarget.m_idSelf);
    }
  }
}
