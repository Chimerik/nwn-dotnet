using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Chanceux(NwCreature caster, NwGameObject oTarget)
    {
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHolyAid));

      if (caster == oTarget)
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.ChanceuxAvantage);
      else
        NWScript.AssignCommand(caster, () => oTarget.ApplyEffect(EffectDuration.Permanent, EffectSystem.ChanceuxDesavantage));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name} - Chanceux", StringUtils.gold, true, true);
      FeatUtils.DecrementFeatUses(caster, CustomSkill.Chanceux);
    }
  }
}
