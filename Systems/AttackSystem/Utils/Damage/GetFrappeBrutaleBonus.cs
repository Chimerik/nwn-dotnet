using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFrappeBrutaleBonus(CNWSCreature creature, Anvil.API.Ability attackAbility, bool isCritical)
    {
      int roll = 0;

      if (attackAbility == Anvil.API.Ability.Strength)
      {
        if (creature.m_pStats.GetNumLevelsOfClass(CustomClass.Barbarian) > 16)
          roll = Utils.Roll(20, isCritical ? 4 : 2);
        else
          roll = Utils.Roll(10, isCritical ? 2 : 1);

        LogUtils.LogMessage($"Frappe Brutale : +{roll} dégâts", LogUtils.LogType.Combat);
      }

      return roll;  
    }
  }
}
