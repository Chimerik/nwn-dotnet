using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetUnarmedDamage(CNWSCreature creature, CNWSCreature target, Anvil.API.Ability attackAbility, bool isCritical)
    {
      int unarmedDieToRoll = CreatureUtils.GetUnarmedDamage(creature);
      int damage = NwRandom.Roll(Utils.random, unarmedDieToRoll);
      damage += GetDamageEffects(creature, target, attackAbility, true, isCritical)
        + GetBagarreurDeTaverneBonusDamage(creature, isCritical)
        + GetAnimalCompanionBonusDamage(creature, isCritical)
        - GetMaitreArmureLourdeDamageReduction(target, isCritical)
        - GetParadeDamageReduction(target, isCritical);

      LogUtils.LogMessage($"Mains nues - 1d{unarmedDieToRoll} => {damage}", LogUtils.LogType.Combat);

      return damage;
    }
  }
}
