using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWNX.API.Events;
using NWNX.Services;

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
      oTarget.OnItemUse -= NoUseableItemMalus;
      oTarget.OnItemUse += NoUseableItemMalus;
    }
    private void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnItemUse -= ItemSystem.OnItemUseBefore;
      oTarget.OnItemUse += ItemSystem.OnItemUseBefore;
      oTarget.OnItemUse -= NoUseableItemMalus;
    }
    private void NoUseableItemMalus(OnItemUse onItemUse)
    {
      onItemUse.PreventUseItem = true;
      ((NwPlayer)onItemUse.UsedBy).SendServerMessage("L'interdiction d'utilisation d'objets est en vigueur.", Color.RED);
    }
  }
}
