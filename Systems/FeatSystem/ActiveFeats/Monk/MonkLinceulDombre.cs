using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkLinceulDombre(NwCreature caster, OnUseFeat onFeat)
    {
      if(!NwModule.Instance.IsNight && !caster.Location.Area.IsInterior)
      {
        caster.LoginPlayer?.SendServerMessage("Il faut attendre la nuit, ou bien être en intérieur pour utiliser cette capacité", ColorConstants.Red);
        onFeat.PreventFeatUse = true;
        return;
      }

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Linceul d'ombre", StringUtils.gold, true, true);
    }
  }
}
