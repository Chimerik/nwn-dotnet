using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetRepliqueDupliciteAdvantage(CGameEffect eff, CNWSCreature attacker)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.RepliqueInvoqueeEffectExoTag).ToBool())
      {
        var replique = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(eff.m_oidCreator);

        if (replique is null || replique.m_idSelf == 0x7F000000)
          return false;

        var repliqueCaster = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(replique.m_oidMaster);

        if (repliqueCaster is null || repliqueCaster.m_idSelf == 0x7F000000)
          return false;

        if (repliqueCaster.m_pStats.GetNumLevelsOfClass(CustomClass.Clerc) > 16 || replique.m_oidMaster == attacker.m_idSelf)
        {
          if (Vector3.DistanceSquared(attacker.m_vPosition.ToManagedVector(), replique.m_vPosition.ToManagedVector()) < 1600)
          {
            LogUtils.LogMessage("Avantage - Réplique de Duplicité", LogUtils.LogType.Combat);
            return true;
          }
        }
      }

      return false;
    }
  }
}
