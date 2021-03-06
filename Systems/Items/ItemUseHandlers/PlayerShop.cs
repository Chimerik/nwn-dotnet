using System;
using System.Linq;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class PlayerShop
  {
    public PlayerShop(NwPlayer oPC, NwItem authorization)
    {
      if (!(Players.TryGetValue(oPC, out Player player)))
        return;

      // TODO ajouter check nombre de shop possibles en fonction du niveau de compétence du joueur

      NwPlaceable plcShop = NwPlaceable.Create("player_shop_plc", oPC.Location, false, $"player_shop_plc_{oPC.CDKey}");
      VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, plcShop, VisibilityPlugin.NWNX_VISIBILITY_DM_ONLY);
      VisibilityPlugin.SetVisibilityOverride(oPC, plcShop, VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);

      plcShop.OnUsed += PlaceableSystem.HandlePlaceableUsed;
    }
  }
}
