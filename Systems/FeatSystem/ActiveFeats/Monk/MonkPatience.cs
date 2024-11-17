using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkPatience(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.MonkPatience, NwTimeSpan.FromRounds(1));

      if(caster.GetClassInfo((ClassType)CustomClass.Monk).Level > 9)
        caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(NwRandom.Roll(Utils.random, CreatureUtils.GetUnarmedDamage(caster), 2)));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Patience", StringUtils.gold, true);
      FeatUtils.DecrementKi(caster);
    }
  }
}
