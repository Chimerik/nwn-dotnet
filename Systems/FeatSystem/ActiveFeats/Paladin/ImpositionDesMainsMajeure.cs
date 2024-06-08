using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ImpositionDesMainsMajeure(NwCreature caster, NwGameObject targetObject)
    {
      int healAmount = 4 * caster.GetClassInfo(ClassType.Paladin).Level;

      if (targetObject is not NwCreature targetCreature || Utils.In(targetCreature.Race.RacialType, RacialType.Construct, RacialType.Undead))
        healAmount = 0;

      targetObject.ApplyEffect(EffectDuration.Instant, Effect.Heal(healAmount));
      targetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingL));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Imposition des Mains - Majeure", StringUtils.gold, true);

      FeatUtils.DecrementLayOnHands(caster, 2);
    }
  }
}
