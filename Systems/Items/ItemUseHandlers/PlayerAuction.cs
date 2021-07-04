using System.Collections.Generic;
using System.Linq;
using NWN.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  class PlayerAuction
  {
    public PlayerAuction(NwCreature oPC, NwItem authorization)
    {
      if (!(Players.TryGetValue(oPC, out Player player)))
        return;

      int contractScienceLevel = 1;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.ContractScience))
        contractScienceLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ContractScience, player.learntCustomFeats[CustomFeats.ContractScience]);

      var result = SqLiteUtils.SelectQuery("playerAuctions",
        new List<string>() { { "count(*)" } },
        new List<string[]>() { new string[] { "characterId", player.characterId.ToString() } });

      if (result.Result != null && result.Result.GetInt(0) > contractScienceLevel)
      {
        player.oid.SendServerMessage($"Votre niveau de science du contrat actuel vous permet de gérer {contractScienceLevel.ToString().ColorString(ColorConstants.White)}, or vous en possédez déjà {result.Result.GetString(0).ColorString(ColorConstants.White)}", ColorConstants.Orange);
        return;
      }

      NwPlaceable plcShop = NwPlaceable.Create("player_shop_plc", oPC.Location, false, $"_PLAYER_AUCTION_PLC_{player.oid.CDKey}");
      NwStore shop = NwStore.Create("generic_shop_res", oPC.Location, false, $"_PLAYER_AUCTION_{player.oid.CDKey}");

      plcShop.GetLocalVariable<int>("_OWNER_ID").Value = player.characterId;
      plcShop.Name = "[ENCHERES]".ColorString(ColorConstants.Orange) + $" de {oPC.Name.ColorString(ColorConstants.Green)}";

      shop.GetLocalVariable<int>("_OWNER_ID").Value = player.characterId;
      shop.Name = $"Echoppe de {oPC.Name.ColorString(ColorConstants.Green)}";

      plcShop.OnUsed += PlaceableSystem.OnUsedPlayerOwnedAuction;

      player.oid.SendServerMessage($"Félicitations pour l'inauguration de votre {plcShop.Name.ColorString(ColorConstants.Green)} !");
      authorization.Destroy();
    }
  }
}
