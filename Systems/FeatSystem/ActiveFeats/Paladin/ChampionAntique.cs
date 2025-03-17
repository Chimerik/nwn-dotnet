using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ChampionAntique(NwCreature caster)
    {
      caster.SetFeatRemainingUses((Feat)CustomSkill.AnciensChampionAntique, 0);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetChampionAntiqueEffect(caster), NwTimeSpan.FromRounds(10));
      UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(9);

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} Champion Antique", StringUtils.gold, true, true); 
    }
  }
}
