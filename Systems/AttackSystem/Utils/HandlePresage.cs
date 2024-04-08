using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandlePresage(CNWSCreature creature)
    {
      if(creature.m_ScriptVars.GetInt(CreatureUtils.PresageVariableExo) > 0)
      {
        int presageRoll = creature.m_ScriptVars.GetInt(CreatureUtils.PresageVariableExo);
        creature.m_ScriptVars.DestroyInt(CreatureUtils.PresageVariableExo);

        SendNativeServerMessage("Présage".ColorString(StringUtils.gold), creature);
        LogUtils.LogMessage($"Présage - Jet forcé à : {presageRoll}", LogUtils.LogType.Combat);
        return presageRoll;
      }

      return 0;
    }
  }
}
