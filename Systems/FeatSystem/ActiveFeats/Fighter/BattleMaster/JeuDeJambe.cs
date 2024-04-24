using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void JeuDeJambe(NwCreature caster)
    {
      FeatUtils.ClearPreviousManoeuvre(caster);

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.jeuDeJambe, NwTimeSpan.FromRounds(1));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Jeu de Jambe", StringUtils.gold, true);
      FeatUtils.DecrementManoeuvre(caster);
    }
  }
}
