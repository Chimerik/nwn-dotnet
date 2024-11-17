using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetAttaquesEtudieesAdvantage(CGameEffect eff, CNWSCreature attacker)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.AttaquesEtudieesEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Attaque Etudiees", LogUtils.LogType.Combat);
        DelayEffRemoval(attacker);
        return true;
      }
      else
        return false;
    }
    private static async void DelayEffRemoval(CNWSCreature attacker)
    {
      await NwTask.NextFrame();
      EffectUtils.RemoveTaggedEffect(attacker, EffectSystem.AttaquesEtudieesEffectExoTag);
    }
  }
}
