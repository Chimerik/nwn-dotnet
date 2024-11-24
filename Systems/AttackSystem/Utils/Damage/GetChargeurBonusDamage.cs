using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetChargeurBonusDamage(CNWSCreature creature)
    {
      int bonusDamage = 0;
      var initiaLocation = creature.m_pStats.m_pBaseCreature.m_ScriptVars.GetLocation(EffectSystem.chargerVariableExo);

      if (initiaLocation.m_oArea != NWScript.OBJECT_INVALID && Vector3.DistanceSquared(initiaLocation.m_vPosition.ToManagedVector(), creature.m_vPosition.ToManagedVector()) > 9)
      {
        bonusDamage = NwRandom.Roll(Utils.random, 8);

        foreach (var eff in creature.m_appliedEffects)
          if (eff.m_sCustomTag.CompareNoCase(EffectSystem.chargeurEffectExoTag).ToBool())
            creature.RemoveEffect(eff);

        //BroadcastNativeServerMessage($"Charge : Dégâts +1d8 = {bonusDamage}".ColorString(ColorConstants.Orange), creature);
        LogUtils.LogMessage($"Chargeur : Dégâts +1d8 = {bonusDamage}", LogUtils.LogType.Combat);
      }

      creature.m_pStats.m_pBaseCreature.m_ScriptVars.DestroyLocation(EffectSystem.chargerVariableExo);

      return bonusDamage;
    }
  }
}
