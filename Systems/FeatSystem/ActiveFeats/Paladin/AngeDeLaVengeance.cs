using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void AngeDeLaVengeance(NwCreature caster)
    {
      caster.SetFeatRemainingUses((Feat)CustomSkill.AngeDeLaVengeance, 0);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.AngeDeLaVengeance(caster), NwTimeSpan.FromRounds(100));
      UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(9);

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Ange de la Vengeance", StringUtils.gold, true, true);
    }
  }
}
