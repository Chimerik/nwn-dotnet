using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFrappeBrutaleBonusDamage(CNWSCreature creature, CNWSCreature target, Anvil.API.Ability attackAbility, bool isMeleeAttack = true)
    {
      int bonusDamage = 0;

      if (isMeleeAttack && attackAbility == Anvil.API.Ability.Strength 
        && creature.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.barbarianRageEffectExoTag).ToBool()))
      {
        if(creature.m_pStats.GetNumLevelsOfClass(CustomClass.Barbarian) > 16)
          bonusDamage = NwRandom.Roll(Utils.random, 20, 2);
        else
          bonusDamage = NwRandom.Roll(Utils.random, 10);

        LogUtils.LogMessage($"Frappe Brutale : +{bonusDamage} dégâts", LogUtils.LogType.Combat);
      }

      return bonusDamage;  
    }
  }
}
