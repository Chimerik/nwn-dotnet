using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetBarbarianRageBonusDamage(CNWSCreature creature, Anvil.API.Ability attackAbility, bool isMeleeAttack = true)
    {
      if (isMeleeAttack && attackAbility == Anvil.API.Ability.Strength)
      {
        var eff = creature.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.CompareNoCase(EffectSystem.barbarianRageEffectExoTag).ToBool());

        if (eff is not null)
        {
          int bonusDamage = eff.GetInteger(5);
          LogUtils.LogMessage($"Rage du Barbare : +{bonusDamage} dégâts", LogUtils.LogType.Combat);

          return bonusDamage;
        }
        else
        {
          eff = creature.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.CompareNoCase(EffectSystem.rageDuSanglierEffectExoTag).ToBool());

          if (eff is not null)
          {
            int bonusDamage = eff.GetInteger(5);
            LogUtils.LogMessage($"Rage du Sanglier : +{bonusDamage} dégâts", LogUtils.LogType.Combat);

            return bonusDamage;
          }
        }
      }

      return 0;  
    }
  }
}
