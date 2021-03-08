using NWN.API;
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

      NwPlaceable plcShop = NwPlaceable.Create("player_shop_plc", oPC.Location, false, $"_PLAYER_SHOP_PLC_{oPC.CDKey}");
      NwStore shop = NwStore.Create("generic_shop_res", oPC.Location, false, $"_PLAYER_SHOP_{oPC.CDKey}");

      plcShop.GetLocalVariable<int>("_OWNER_ID").Value = player.characterId;
      plcShop.Name = $"Echoppe de {oPC.Name.ColorString(Color.GREEN)}";

      shop.GetLocalVariable<int>("_OWNER_ID").Value = player.characterId;
      shop.Name = $"Echoppe de {oPC.Name.ColorString(Color.GREEN)}";

      plcShop.OnUsed += PlaceableSystem.OnUsedPlayerOwnedShop;

      player.oid.SendServerMessage($"Félicitations pour l'inauguration de votre boutique {plcShop.Name.ColorString(Color.GREEN)} !");
      authorization.Destroy();
    }
  }
}
