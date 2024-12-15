using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetHebetementACMalus(CNWSCreature creature)
    {
      if(creature.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.HebetementEffectExoTag).ToBool()))
      {
        int dexBonus = GetAbilityModifier(creature, Anvil.API.Ability.Dexterity);

        if (dexBonus > 0)
        {
          LogUtils.LogMessage($"Hébétement : -{dexBonus} CA", LogUtils.LogType.Combat);
          return dexBonus;
        }
      }

      return 0;
    }
  }
}
