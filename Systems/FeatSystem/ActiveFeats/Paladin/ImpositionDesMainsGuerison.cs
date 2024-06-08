using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ImpositionDesMainsGuerison(NwCreature caster, NwGameObject targetObject)
    {
      if (targetObject is not NwCreature targetCreature || Utils.In(targetCreature.Race.RacialType, RacialType.Construct, RacialType.Undead))
        return;

      EffectUtils.RemoveEffectType(targetObject, EffectType.Disease, EffectType.Poison);
      targetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingG));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Imposition des Mains - Guérison", StringUtils.gold, true);

      FeatUtils.DecrementLayOnHands(caster, 2);
    }
  }
}
