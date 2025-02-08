using System.Collections.Generic;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetPreparationBonusDamage(CNWSCreature attacker, bool isCritical, List<string> noStack)
    {
      noStack.Add(EffectSystem.PreparationEffectTag);

      if (isCritical)
        return 0;

      int bonusDamage = GetAbilityModifier(attacker, Anvil.API.Ability.Strength);
      LogUtils.LogMessage($"Préparation - Ajout du bonus de force (+{bonusDamage})", LogUtils.LogType.Combat);

      return bonusDamage;
    }
  }
}
