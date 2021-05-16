using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  class NoUseableItem
  {
    public NoUseableItem(NwCreature oTarget, bool apply = true)
    {
      if (apply)
        ApplyEffectToTarget(oTarget);
      else
        RemoveEffectFromTarget(oTarget);
    }
    private void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      oTarget.OnItemUse -= ItemSystem.OnItemUseBefore;
      oTarget.OnItemValidateUse -= NoUseableItemMalus;
      oTarget.OnItemValidateUse += NoUseableItemMalus;
    }
    private void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnItemUse -= ItemSystem.OnItemUseBefore;
      oTarget.OnItemUse += ItemSystem.OnItemUseBefore;
      oTarget.OnItemValidateUse -= NoUseableItemMalus;
    }
    private void NoUseableItemMalus(OnItemValidateUse onItemValidateUse)
    {
        onItemValidateUse.CanUse = false;
    }
  }
}
