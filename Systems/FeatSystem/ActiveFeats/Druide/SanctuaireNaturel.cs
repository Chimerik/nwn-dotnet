using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SanctuaireNaturel(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.SantuaireNaturelAura(caster), NwTimeSpan.FromRounds(10));
      UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(3);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfNaturesBalance));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Sanctuaire Naturel", StringUtils.gold, true, true);
      
      DruideUtils.DecrementFormeSauvage(caster);
    }
  }
}
