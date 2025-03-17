using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void NimbeSacree(NwCreature caster)
    {
      caster.SetFeatRemainingUses((Feat)CustomSkill.DevotionNimbeSacree, 0);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.NimbeSacree(caster), NwTimeSpan.FromRounds(100));
      UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(9);
      
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} Nimbe Sacrée", StringUtils.gold, true, true);
    }
  }
}
