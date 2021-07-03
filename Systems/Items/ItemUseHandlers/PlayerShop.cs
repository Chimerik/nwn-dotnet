using System.Collections.Generic;
using System.Linq;
using NWN.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class PlayerShop
  {
    public PlayerShop(NwCreature oPC, NwItem authorization)
    {
      if (!(Players.TryGetValue(oPC, out Player player)))
        return;

      int MagnatLevel = 1;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.Magnat))
        MagnatLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Magnat, player.learntCustomFeats[CustomFeats.Magnat]);

      var result = SqLiteUtils.SelectQuery("playerShops",
          new List<string>() { { "count(*)" } },
          new List<string[]>() { new string[] { "characterId", player.characterId.ToString() } });

      if (result != null && result.FirstOrDefault().GetInt(0) > MagnatLevel)
      {
        player.oid.SendServerMessage($"Votre niveau de magnat actuel vous permet de gérer {MagnatLevel.ToString().ColorString(ColorConstants.White)}, or vous en possédez déjà {result.FirstOrDefault().GetString(0).ColorString(ColorConstants.White)}", ColorConstants.Orange);
        return;
      }

      NwPlaceable plcShop = NwPlaceable.Create("player_shop_plc", oPC.Location, false, $"_PLAYER_SHOP_PLC_{player.oid.CDKey}");
      NwStore shop = NwStore.Create("generic_shop_res", oPC.Location, false, $"_PLAYER_SHOP_{player.oid.CDKey}");

      plcShop.GetLocalVariable<int>("_OWNER_ID").Value = player.characterId;
      plcShop.Name = $"Echoppe de {oPC.Name.ColorString(ColorConstants.Green)}";

      shop.GetLocalVariable<int>("_OWNER_ID").Value = player.characterId;
      shop.Name = $"Echoppe de {oPC.Name.ColorString(ColorConstants.Green)}";

      plcShop.OnUsed += PlaceableSystem.OnUsedPlayerOwnedShop;

      player.oid.SendServerMessage($"Félicitations pour l'inauguration de votre boutique {plcShop.Name.ColorString(ColorConstants.Green)} !");
      authorization.Destroy();
    }
  }
}
