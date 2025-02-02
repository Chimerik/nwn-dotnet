using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleStabilisation(CNWSCreature creature, NwBaseItem weapon,  int roll, int dieToRoll)
    {
      if(creature.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.StabilisationEffectTag)
        && ItemUtils.IsCreatureWeaponExpert(creature, weapon) && Utils.In(weapon.ItemType, BaseItemType.HeavyCrossbow, BaseItemType.Longbow, BaseItemType.ThrowingAxe))
      {
        int secondRoll = NwRandom.Roll(Utils.random, dieToRoll, 1);
        LogUtils.LogMessage($"Stabilisation - Jet de dégâts entre : {roll} et {secondRoll}", LogUtils.LogType.Combat);
        roll = secondRoll > roll ? secondRoll : roll;
      }

      return roll;
    }
  }
}
