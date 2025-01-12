using System.Collections.Generic;
using System.Numerics;
using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetChargeurBonus(CNWSCreature creature, CGameEffect eff, List<string> noStack, bool isCritical)
    {
      int roll = 0;

      if (!noStack.Contains(EffectSystem.ChargeurEffectTag))
      {
        var initiaLocation = creature.m_pStats.m_pBaseCreature.m_ScriptVars.GetLocation(EffectSystem.chargerVariableExo);

        if (initiaLocation.m_oArea != NWScript.OBJECT_INVALID && Vector3.DistanceSquared(initiaLocation.m_vPosition.ToManagedVector(), creature.m_vPosition.ToManagedVector()) > 9)
        {
          roll = Utils.Roll(8, isCritical ? 2 : 1);
          //BroadcastNativeServerMessage($"Charge : Dégâts +1d8 = {bonusDamage}".ColorString(ColorConstants.Orange), creature);
          creature.RemoveEffect(eff);
          LogUtils.LogMessage($"Chargeur : Dégâts +1d8 = {roll}", LogUtils.LogType.Combat);
        }
      }

      noStack.Add(EffectSystem.ChargeurEffectTag);
      creature.m_pStats.m_pBaseCreature.m_ScriptVars.DestroyLocation(EffectSystem.chargerVariableExo);

      return roll;
    }
  }
}
