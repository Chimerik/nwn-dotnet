using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkTenebres(NwCreature caster, OnUseFeat onFeat)
    {
      if (caster.GetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkTenebres)) < 2)
      {
        caster.LoginPlayer?.SendServerMessage("Charges de ki insuffisantes", ColorConstants.Red);
        onFeat.PreventFeatUse = true;
        return;
      }

      FeatUtils.DecrementKi(caster);
    }
  }
}
