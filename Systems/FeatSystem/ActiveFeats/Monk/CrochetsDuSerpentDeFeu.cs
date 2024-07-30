using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void CrochetsDuSerpentDeFeu(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.CrochetsDuSerpentDeFeu(caster), NwTimeSpan.FromRounds(10));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Crochets du Serpent de Feu", StringUtils.gold, true, true);
      FeatUtils.DecrementKi(caster);
    }
  }
}
