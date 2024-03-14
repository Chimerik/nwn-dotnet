using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkPlenitude(NwCreature caster)
    {
      var monk = caster.GetClassInfo(NwClass.FromClassType(ClassType.Monk));

      if (monk is null || monk.Level < 1)
        return;

      caster.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.Heal(monk.Level * 3), Effect.VisualEffect(VfxType.ImpHealingM)));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Plénitude", StringUtils.gold, true);
    }
  }
}
