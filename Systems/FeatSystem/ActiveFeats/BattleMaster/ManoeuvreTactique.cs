using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ManoeuvreTactique(NwCreature caster, NwGameObject targetObject)
    {
      if(targetObject is not NwCreature target)
      {
        caster.ControllingPlayer?.SendServerMessage("Vous devez cibler une créature", ColorConstants.Red);
        return;
      }

      if(target == caster)
      {
        caster.ControllingPlayer?.SendServerMessage("Cette manoeuvre ne permet pas de vous cibler vous même", ColorConstants.Red);
        return;
      }

      target.ApplyEffect(EffectDuration.Temporary, EffectSystem.manoeuvreTactique, NwTimeSpan.FromRounds(1));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"Manoeuvre Tactique ({target.Name})", StringUtils.gold);
      FeatUtils.DecrementManoeuvre(caster);
    }
  }
}
