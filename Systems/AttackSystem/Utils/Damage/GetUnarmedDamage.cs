using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetUnarmedDamage(CNWSCreature creature)
    {
      int unarmedDieToRoll = CreatureUtils.GetUnarmedDamage(creature.m_pStats);
      int damage = NwRandom.Roll(Utils.random, unarmedDieToRoll);
      damage += GetDegatsVaillantsBonus(creature)
        + GetFaveurDuMalinBonusDamage(creature);

      LogUtils.LogMessage($"Mains nues - 1d{unarmedDieToRoll} => {damage}", LogUtils.LogType.Combat);

      return damage;
    }
  }
}
