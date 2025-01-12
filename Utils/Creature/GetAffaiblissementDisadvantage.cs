﻿using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetAffaiblissementDisadvantage(CNWSCreature creature, CGameEffect eff)
    {
      LogUtils.LogMessage("Désavantage - Affecté par Affaiblissement", LogUtils.LogType.Combat);
      EffectUtils.DelayEffectRemoval(creature, eff);
      return true;
    }
  }
}
