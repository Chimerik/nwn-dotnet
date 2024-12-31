using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void HaloDeLumiere(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.HaloDeLumiereAura, NwTimeSpan.FromRounds(10));
      UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(18);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Halo de Lumière", StringUtils.gold, true, true);

      caster.SetFeatRemainingUses((Feat)CustomSkill.ClercHaloDeLumiere, 0);
    }
  }
}
