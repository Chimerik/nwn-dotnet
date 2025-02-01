using System.Linq;
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
        var bonusAction = attacker.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.BonusActionEffectTag);

        if (bonusAction is not null)
        {
          attacker.RemoveEffect(bonusAction);

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
