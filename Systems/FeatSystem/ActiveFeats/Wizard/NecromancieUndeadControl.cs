using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void NecromancieUndeadControl(NwCreature caster, NwGameObject targetObject)
    {
      if (targetObject is not NwCreature target || target.Race.RacialType != RacialType.Undead || caster == target)
      {
        caster.LoginPlayer?.SendServerMessage($"Veuillez choisir une cible valide", ColorConstants.Red);
        return;
      }
    }
  }
}
