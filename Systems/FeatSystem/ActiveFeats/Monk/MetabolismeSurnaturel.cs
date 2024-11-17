using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MetabolismeSurnaturel(NwCreature caster)
    {
      if (caster.IsInCombat)
      {
        caster.LoginPlayer?.SendServerMessage("Inutilisable en combat", ColorConstants.Red);
        return;
      }

      MonkUtils.RestoreKi(caster);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.MonkMetabolismeSurnaturel);
    }
  }
}
