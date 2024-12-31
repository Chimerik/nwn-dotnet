using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FrappeGuidee(NwCreature caster, NwGameObject oTarget)
    {
      if (oTarget != caster && !CreatureUtils.HandleReactionUse(caster))
        return;

      oTarget.ApplyEffect(EffectDuration.Permanent, EffectSystem.FrappeGuidee);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Frappe Guidée - {oTarget.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadOdd));
      ClercUtils.ConsumeConduitDivin(caster);
    }
  }
}
