using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkPassageSansTrace(NwCreature caster, OnUseFeat onFeat)
    {
      if (caster.GetFeatRemainingUses((Feat)CustomSkill.MonkPassageSansTrace) < 2)
      {
        caster.LoginPlayer?.SendServerMessage("Nécessite 2 charges de Ki", ColorConstants.Red);
        onFeat.PreventFeatUse = true;
        return;
      }
    }
  }
}
