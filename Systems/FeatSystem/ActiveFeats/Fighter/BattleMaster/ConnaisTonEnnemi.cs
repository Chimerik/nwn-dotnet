using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ConnaisTonEnnemi(NwCreature caster, NwGameObject targetObject)
    {
      if (targetObject is not NwCreature target)
      {
        caster.LoginPlayer.SendServerMessage("La cible doit être une créature", ColorConstants.Red);
        return;
      }

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"*Observe {target.Name} (Maître de guerre)*", ColorConstants.Cyan, true);

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.WarMasterConnaisTonEnnemi);
    }
  }
}
