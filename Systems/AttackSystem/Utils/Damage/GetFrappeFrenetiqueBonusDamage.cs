using System.Collections.Generic;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFrappeFrenetiqueBonus(CNWSCreature creature, CGameEffect eff, Anvil.API.Ability attackAbility, bool isCritical, List<string> noStack)
    {
      if (attackAbility == Anvil.API.Ability.Strength)
      {
        int barbarianLevel = creature.m_pStats.GetNumLevelsOfClass(CustomClass.Barbarian);
        int nbDices = barbarianLevel > 15 ? 4 : barbarianLevel > 8 ? 3 : 2; ;
        int roll = Utils.Roll(6, isCritical ? nbDices * 2 : nbDices);

        creature.RemoveEffect(eff);
        LogUtils.LogMessage($"Frappe Frénétique ({nbDices}d6) : +{roll} dégâts", LogUtils.LogType.Combat);
        return roll;
      }

      noStack.Add(EffectSystem.FrappeFrenetiqueEffectTag);

      return 0;  
    }
  }
}
