using NWN.Native.API;
using RacialType = Anvil.API.RacialType;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetProtectionContreLeMalEtLeBienDisadvantage(CNWSCreature attacker)
    {
      if (!Utils.In((RacialType)attacker.m_pStats.m_nRace, RacialType.Fey, RacialType.Aberration, CustomRacialType.Fielon, CustomRacialType.Celeste, RacialType.Elemental, RacialType.Undead))
        return false;

      LogUtils.LogMessage("Désavantage - Protection contre le Mal et le Bien", LogUtils.LogType.Combat);
      return true;
    }
  }
}
