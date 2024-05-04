using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkTenebres(NwCreature caster, OnUseFeat onFeat)
    {
      if (caster.GetFeatRemainingUses((Feat)CustomSkill.MonkTenebres) < 2)
      {
        caster.LoginPlayer?.SendServerMessage("Nécessite 2 charges de ki", ColorConstants.Red);
        onFeat.PreventFeatUse = true;
        return;
      }

      caster.GetObjectVariable<LocalVariableInt>("_CAST_FROM_SHADOW_MONK_FEAT").Value = onFeat.Feat.Id;
    }
  }
}
