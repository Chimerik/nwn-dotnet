using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleFendre(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound combatRound, byte attackResult)
    {
      if (attackResult == 4)
      {
        if(attacker.m_ScriptVars.GetObject(CreatureUtils.FendreDamage1VariableExo) == currentTarget.m_idSelf)
            attacker.m_ScriptVars.DestroyObject(CreatureUtils.FendreDamage1VariableExo);
        else if (attacker.m_ScriptVars.GetObject(CreatureUtils.FendreDamage2VariableExo) == currentTarget.m_idSelf)
          attacker.m_ScriptVars.DestroyObject(CreatureUtils.FendreDamage2VariableExo);
      } 

      if (!attacker.m_ScriptVars.GetInt(CreatureUtils.FendreAttackVariableExo).ToBool())
        return;

      attacker.m_ScriptVars.DestroyInt(CreatureUtils.FendreAttackVariableExo);
      int i = 0;

      foreach(var gameObject in attacker.GetArea().m_aGameObjects)
      {
        var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(gameObject);

        if (target is null || target.m_idSelf == 0x7F000000 || target.m_idSelf == attacker.m_idSelf || target.m_idSelf == currentTarget.m_idSelf) // OBJECT_INVALID
          continue;

        string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}";

        /*ModuleSystem.Log.Info($"{targetName}\n" +
          $"PvPReputation : {attacker.GetPVPReputation(target.m_idSelf)}\n" +
          $"GetCreatureReputation : {attacker.GetCreatureReputation(target.m_idSelf, target.m_nOriginalFactionId)}");*/

        if ((attacker.GetCreatureReputation(target.m_idSelf, target.m_nOriginalFactionId) < 50
          || attacker.GetPVPReputation(target.m_idSelf) < 50)
          && Vector3.DistanceSquared(attacker.m_vPosition.ToManagedVector(), target.m_vPosition.ToManagedVector()) < 16)
        {
          combatRound.AddWhirlwindAttack(target.m_idSelf, 1);
          i++;

          switch(i)
          {
            case 1: attacker.m_ScriptVars.SetObject(CreatureUtils.FendreDamage1VariableExo, target.m_idSelf);break;
            case 2: attacker.m_ScriptVars.SetObject(CreatureUtils.FendreDamage2VariableExo, target.m_idSelf);break;
            default: return;
          }
        }
      } 
    }
    public static int HandleFendre(CNWSCreature attacker, CNWSCreature target, int damage)
    {
      int resultDamage = damage;
      var target1 = attacker.m_ScriptVars.GetObject(CreatureUtils.FendreDamage1VariableExo);
      var target2 = attacker.m_ScriptVars.GetObject(CreatureUtils.FendreDamage2VariableExo);

      if (target1 == target.m_idSelf)
        attacker.m_ScriptVars.DestroyObject(CreatureUtils.FendreDamage1VariableExo);
      else if (target2 == target.m_idSelf)
        attacker.m_ScriptVars.DestroyObject(CreatureUtils.FendreDamage2VariableExo);

      if (target1 != 0x7F000000 || target2 != 0x7F000000)
      {
        resultDamage /= 2;
        LogUtils.LogMessage($"Fendre : Dégâts ({damage}) divisés par 2 : {resultDamage}", LogUtils.LogType.Combat);
        return resultDamage;
      }

      return resultDamage;
    }
  }
}
