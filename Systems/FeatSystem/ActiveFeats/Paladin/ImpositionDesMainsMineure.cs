using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ImpositionDesMainsMineure(NwCreature caster, NwGameObject targetObject)
    {
      int healAmount = 2 * caster.GetClassInfo(ClassType.Paladin).Level;

      if (targetObject is not NwCreature targetCreature || Utils.In(targetCreature.Race.RacialType, RacialType.Construct, RacialType.Undead))
        healAmount = 0;

      targetObject.ApplyEffect(EffectDuration.Instant, Effect.Heal(healAmount));
      targetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingM));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Imposition des Mains - Mineure", StringUtils.gold, true);

      FeatUtils.DecrementLayOnHands(caster);
    }
  }
}
