using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkSlowFall(NwCreature caster)
    {
      if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value < 1)
      {
        caster.LoginPlayer?.SendServerMessage("Aucune réaction disponible", ColorConstants.Red);
        return;
      }

      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value -= 1;
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseWind));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Chute contrôlée", StringUtils.gold, true);
    }
  }
}
