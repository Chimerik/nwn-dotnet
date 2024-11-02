using Anvil.API;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static int HandleAmeRadieuse(NwCreature caster, DamageType damageType)
    {
      int bonusDamage = 0;

      if (caster is not null && Utils.In(damageType, DamageType.Fire, DamageType.Divine))
      {
        bonusDamage = caster.GetAbilityModifier(Ability.Charisma) > 1 ? caster.GetAbilityModifier(Ability.Charisma) : 1;
        LogUtils.LogMessage($"Âme Radieuse : +{bonusDamage} dégâts", LogUtils.LogType.Combat);
      }

      return bonusDamage;
    }
  }
}
