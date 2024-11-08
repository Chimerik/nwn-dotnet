using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetAbilityModifier(CNWSCreature creature, Anvil.API.Ability ability)
    {
      byte mod = creature.m_pStats.GetAbilityMod((byte)ability);
      int bonus = mod > 122 ? mod - 255 : mod;

      LogUtils.LogMessage($"Adding {ability} modifier (+{bonus})", LogUtils.LogType.Combat);

      return bonus;
    }
  }
}
