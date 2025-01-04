using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ChampionAntique(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetChampionAntiqueEffect(caster), NwTimeSpan.FromRounds(10));
      UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(3);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} Champion Antique", StringUtils.gold, true, true);

      caster.SetFeatRemainingUses((Feat)CustomSkill.AnciensChampionAntique, 0);
    }
  }
}
