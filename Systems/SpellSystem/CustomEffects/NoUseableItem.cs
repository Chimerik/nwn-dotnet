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

      PlayerSystem.eventService.Unsubscribe<ItemEvents.OnItemUseBefore, NWNXEventFactory>(oTarget, ItemSystem.OnItemUseBefore);
      PlayerSystem.eventService.Subscribe<ItemEvents.OnItemUseBefore, NWNXEventFactory>(oTarget, NoUseableItemMalus)
        .Register<ItemEvents.OnItemUseBefore>();
    }
    private void RemoveEffectFromTarget(NwCreature oTarget)
    {
      PlayerSystem.eventService.Unsubscribe<ItemEvents.OnItemUseBefore, NWNXEventFactory>(oTarget, NoUseableItemMalus);
      PlayerSystem.eventService.Subscribe<ItemEvents.OnItemUseBefore, NWNXEventFactory>(oTarget, ItemSystem.OnItemUseBefore)
        .Register<ItemEvents.OnItemUseBefore>();
    }
    private void NoUseableItemMalus(ItemEvents.OnItemUseBefore onItemUse)
    {
      onItemUse.Skip = true;
      ((NwPlayer)onItemUse.Creature).SendServerMessage("L'interdiction d'utilisation d'objets est en vigueur.", Color.RED);
    }
  }
}
