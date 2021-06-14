using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  static class NoUseableItem
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      oTarget.OnItemUse -= ItemSystem.OnItemUseBefore;
      oTarget.OnItemValidateUse -= NoUseableItemMalus;
      oTarget.OnItemValidateUse += NoUseableItemMalus;
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnItemUse -= ItemSystem.OnItemUseBefore;
      oTarget.OnItemUse += ItemSystem.OnItemUseBefore;
      oTarget.OnItemValidateUse -= NoUseableItemMalus;
    }
    private static void NoUseableItemMalus(OnItemValidateUse onItemValidateUse)
    {
        onItemValidateUse.CanUse = false;
    }
  }
}
