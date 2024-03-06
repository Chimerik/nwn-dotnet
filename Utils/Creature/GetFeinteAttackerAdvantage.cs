using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetFeinteAttackerAdvantage(CNWSCreature attacker)
    {
      if(attacker.m_ScriptVars.GetInt(ManoeuvreTypeVariableExo) == CustomSkill.WarMasterFeinte)
      {
        if (attacker.m_ScriptVars.GetInt(BonusActionVariableExo) > 0)
        {
          attacker.m_ScriptVars.SetInt(BonusActionVariableExo, attacker.m_ScriptVars.GetInt(BonusActionVariableExo) - 1);

          NativeUtils.BroadcastNativeServerMessage("Feinte".ColorString(StringUtils.gold), attacker);

          LogUtils.LogMessage("Avantage - Feinte", LogUtils.LogType.Combat);
          return true;
        }
        else
        {
          NativeUtils.SendNativeServerMessage("Feinte - Avantage annulé - Aucune action bonus disponible".ColorString(ColorConstants.Orange), attacker);
          LogUtils.LogMessage("Feinte - Aucune action bonus disponible", LogUtils.LogType.Combat);
        }
      }

      return false;
    }
  }
}
