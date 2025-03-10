using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetUnarmedDamage(CNWSCreature creature, CNWSCreature target, Anvil.API.Ability attackAbility, bool isCritical)
    {
      int unarmedDieToRoll = CreatureUtils.GetUnarmedDamage(creature);
      int damage = Utils.Roll(unarmedDieToRoll);

      if(damage < 2 && creature.m_pStats.HasFeat(CustomSkill.AgresseurSauvage).ToBool())
      {
        int reroll = Utils.Roll(unarmedDieToRoll);

        if (reroll > damage)
        {
          LogUtils.LogMessage($"Bagarreur de Taverne reroll {damage} vs {reroll} = {reroll}", LogUtils.LogType.Combat);
          damage = reroll;
        }
      }

      damage += GetDamageEffects(creature, target, attackAbility, true, isCritical)
        + GetAnimalCompanionBonusDamage(creature, isCritical)
        - GetMaitreArmureLourdeDamageReduction(target, isCritical)
        - GetParadeDamageReduction(target, isCritical);

      LogUtils.LogMessage($"Mains nues - 1d{unarmedDieToRoll} => {damage}", LogUtils.LogType.Combat);

      return damage;
    }
  }
}
