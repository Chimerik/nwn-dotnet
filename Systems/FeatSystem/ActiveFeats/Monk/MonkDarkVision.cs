using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkDarkVision(NwCreature caster, OnUseFeat onFeat)
    {
      if (caster.GetFeatRemainingUses((Feat)CustomSkill.MonkDarkVision) < 2)
      {
        caster.LoginPlayer?.SendServerMessage("Nécessite 2 charges de Ki", ColorConstants.Red);
        onFeat.PreventFeatUse = true;
        return;
      }

      caster.GetObjectVariable<LocalVariableInt>("_CAST_FROM_SHADOW_MONK_FEAT").Value = onFeat.Feat.Id;
    }
  }
}
