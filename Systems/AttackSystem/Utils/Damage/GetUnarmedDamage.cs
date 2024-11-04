using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetUnarmedDamage(CNWSCreature creature, CNWSCreature target, Anvil.API.Ability attackAbility)
    {
      int unarmedDieToRoll = CreatureUtils.GetUnarmedDamage(creature.m_pStats);
      int damage = NwRandom.Roll(Utils.random, unarmedDieToRoll);
      damage += GetDegatsVaillantsBonus(creature)
        + GetBarbarianRageBonusDamage(creature, attackAbility)
        + GetFrappeBrutaleBonusDamage(creature, target, attackAbility)
        + GetFaveurDuMalinBonusDamage(creature);

      LogUtils.LogMessage($"Mains nues - 1d{unarmedDieToRoll} => {damage}", LogUtils.LogType.Combat);

      return damage;
    }
  }
}
