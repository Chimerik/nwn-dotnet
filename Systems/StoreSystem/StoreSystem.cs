using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.Systems;
using System;
using NWN.API;
using System.Linq;
using System.Threading.Tasks;
using NWN.API.Events;

namespace NWN.System
{
  [ServiceBinding(typeof(StoreSystem))]
  public class StoreSystem
  {
    public static void HandleBeforeStoreBuy(OnStoreRequestBuy onStoreRequestBuy)
    {
      if (!PlayerSystem.Players.TryGetValue(onStoreRequestBuy.Creature, out PlayerSystem.Player player))
        return;

      if (onStoreRequestBuy.Store.Tag.StartsWith("_PLAYER_SHOP_"))
      {
        if (onStoreRequestBuy.Store.GetLocalVariable<int>("_OWNER_ID").Value != player.characterId)
        {
          if (onStoreRequestBuy.Result.Value)
          {
            int price = onStoreRequestBuy.Price * 95 / 100;
            int ownerId = onStoreRequestBuy.Store.GetLocalVariable<int>("_OWNER_ID").Value;
            int shopId = onStoreRequestBuy.Store.GetLocalVariable<int>("_SHOP_ID").Value;

            NwPlayer oSeller = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p, "characterId") == ownerId);
            if (oSeller != null)
            {
              if (PlayerSystem.Players.TryGetValue(oSeller, out PlayerSystem.Player seller))
                seller.bankGold += price;

              oSeller.SendServerMessage($"La vente de votre {onStoreRequestBuy.Item.Name.ColorString(Color.ORANGE)} à {player.oid.Name.ColorString(Color.PINK)} vient de vous rapporter {price.ToString().ColorString(Color.GREEN)}");
            }
            else
            {
              // TODO : envoyer un courrier à la prochaine connexion du seller pour le prévenir de sa vente.

              var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters SET bankGold = bankGold + @bankGold where rowid = @characterId");
              NWScript.SqlBindInt(query, "@characterId", ownerId);
              NWScript.SqlBindInt(query, "@bankGold", price);
              NWScript.SqlStep(query);
            }

            PlayerOwnedShop.SaveShop(player, onStoreRequestBuy.Store);
          }
          return;
        }
        else
        {
          onStoreRequestBuy.PreventBuy = true;
          onStoreRequestBuy.Item.Clone(player.oid);
          onStoreRequestBuy.Item.Destroy();

          Task awaitItemDestruction = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));
            PlayerOwnedShop.SaveShop(player, onStoreRequestBuy.Store);
          });

          return;
        }
      }

      if (onStoreRequestBuy.Store.Tag.StartsWith("_PLAYER_STORAGE_"))
      {
        onStoreRequestBuy.PreventBuy = true;
        onStoreRequestBuy.Item.Clone(player.oid);
        onStoreRequestBuy.Item.Destroy();
        return;
      }

      if (onStoreRequestBuy.Store.Tag.StartsWith("_PLAYER_AUCTION_"))
      {
        onStoreRequestBuy.PreventBuy = true;
        player.oid.SendServerMessage("Veuillez attendre la fin de l'enchère avant de pouvoir obtenir ce bien.", Color.ROSE);
        return;
      }

      int pocketGold = (int)player.oid.Gold;

      if (pocketGold < onStoreRequestBuy.Price)
      {
        if (pocketGold + player.bankGold < onStoreRequestBuy.Price)
        {
          player.oid.Gold = 0;
          int bankGold = 0;

          if (player.bankGold > 0)
            bankGold = player.bankGold;

          int debt = onStoreRequestBuy.Price - (pocketGold + bankGold);
          player.bankGold -= debt;

          ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, $"Très bien, je demanderai à la banque de vous faire un crédit sur {debt}. N'oubliez pas que les intérêts sont de 30 % par semaine.",
          onStoreRequestBuy.Store.GetLocalVariable<NwObject>("_STORE_NPC").Value, onStoreRequestBuy.Creature);
        }
        else
        {
          player.oid.Gold = 0;
          player.bankGold -= onStoreRequestBuy.Price - pocketGold;

          ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Très bien, je demanderai à la banque de prélever l'or sur votre compte.",
          onStoreRequestBuy.Store.GetLocalVariable<NwObject>("_STORE_NPC").Value, onStoreRequestBuy.Creature);
        }
      }
      else
        player.oid.TakeGold(onStoreRequestBuy.Price);

      onStoreRequestBuy.PreventBuy = true;

      onStoreRequestBuy.Item.Clone(player.oid);

      switch (onStoreRequestBuy.Store.Tag)
      {
        case "blacksmith_shop":
        case "woodworker_shop":
        case "tannery_shop":
        case "magic_shop":
          break;
        default:
          onStoreRequestBuy.Item.Destroy();
          break;
      }
    }
    public static void HandleBeforeStoreSell(OnStoreRequestSell onStoreRequestSell)
    {
      if (PlayerSystem.Players.TryGetValue(onStoreRequestSell.Creature, out PlayerSystem.Player player))
      {
        onStoreRequestSell.PreventSell = true;

        if (onStoreRequestSell.Store.Tag.StartsWith("_PLAYER_STORAGE_"))
        {
          onStoreRequestSell.Item.Clone(onStoreRequestSell.Store);
          onStoreRequestSell.Item.Destroy();
          return;
        }

        if (!onStoreRequestSell.Store.Tag.StartsWith("_PLAYER_SHOP_"))
        {
          
          ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Navré, je n'achète rien. J'arrive déjà tout juste à m'acquiter de ma dette.",
          onStoreRequestSell.Store.GetLocalVariable<NwObject>("_STORE_NPC").Value, onStoreRequestSell.Creature);
        }
        else
        {
          player.oid.SendServerMessage("Impossible de vendre dans ce type d'échoppe.", Color.ORANGE);
        }
      }
    }
  }
}
