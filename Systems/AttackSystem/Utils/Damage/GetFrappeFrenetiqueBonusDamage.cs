using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFrappeFrenetiqueBonusDamage(CNWSCreature creature, Anvil.API.Ability attackAbility, bool isMeleeAttack = true)
    {
      if (isMeleeAttack && attackAbility == Anvil.API.Ability.Strength)
      {
        var eff = creature.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.CompareNoCase(EffectSystem.FrappeFrenetiqueEffectExoTag).ToBool());

        if (eff is not null)
        {
          int nbDices = eff.GetInteger(5);
          int bonusDamage = NwRandom.Roll(Utils.random, 6, nbDices);

          creature.RemoveEffect(eff);
          LogUtils.LogMessage($"Frappe Frénétique ({nbDices}d6) : +{bonusDamage} dégâts", LogUtils.LogType.Combat);
          
          return bonusDamage;
        }
      }

      return 0;  
    }
  }
}
