using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Observation(NwCreature caster, NwGameObject targetObject)
    {
      if (targetObject is not NwCreature target)
      {
        caster.LoginPlayer.SendServerMessage("La cible doit être une créature", ColorConstants.Red);
        return;
      }

      FeatUtils.ClearPreviousManoeuvre(caster);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"*Observe {target.Name} (Maître de guerre)*", ColorConstants.Cyan, true);
    }
  }
}
