
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.Systems;
using System;
using NWN.API;
using System.Linq;
using System.Threading.Tasks;

namespace NWN.System
{
  [ServiceBinding(typeof(StoreSystem))]
  class StoreSystem
  {
    [ScriptHandler("before_store_buy")]
    private void HandleBeforeStoreBuy(CallInfo callInfo)
    {
      if (!PlayerSystem.Players.TryGetValue(callInfo.ObjectSelf, out PlayerSystem.Player player))
        return;

      NwStore store = NWScript.StringToObject(EventsPlugin.GetEventData("STORE")).ToNwObject<NwStore>();
      NwItem item = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM")).ToNwObject<NwItem>();

      if (store.Tag.StartsWith("_PLAYER_SHOP_"))
      {
        if(store.GetLocalVariable<int>("_OWNER_ID").Value != player.characterId)
          return;
        else
        {
          EventsPlugin.SkipEvent();
          item.Clone(player.oid, null, true);
          item.Destroy();

          Task awaitItemDestruction = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));
            PlayerOwnedShop.SaveShop(player, store);
          });
          
          return;
        }
      }

      if (store.Tag.StartsWith("_PLAYER_AUCTION_"))
      {
        EventsPlugin.SkipEvent();
        player.oid.SendServerMessage("Veuillez attendre la fin de l'enchère avant de pouvoir obtenir ce bien.", Color.ROSE);
        return;
      }

      int price = Int32.Parse(EventsPlugin.GetEventData("PRICE"));
      int pocketGold = (int)player.oid.Gold;
      
      if (pocketGold < price)
      {
        if (pocketGold + player.bankGold < price)
        {
          player.oid.Gold = 0;
          int bankGold = 0;

          if (player.bankGold > 0)
            bankGold = player.bankGold;

          int debt = price - (pocketGold + bankGold);
          player.bankGold -= debt;

          ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, $"Très bien, je demanderai à la banque de vous faire un crédit sur {debt}. N'oubliez pas que les intérêts sont de 30 % par semaine.",
          NWScript.GetLocalObject(store, "_STORE_NPC"), callInfo.ObjectSelf);
        }
        else
        {
          player.oid.Gold = 0;
          player.bankGold -= price - pocketGold;

          ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Très bien, je demanderai à la banque de prélever l'or sur votre compte.",
          NWScript.GetLocalObject(store, "_STORE_NPC"), callInfo.ObjectSelf);
        }
      }
      else
        NWScript.TakeGoldFromCreature(price, player.oid, 1);

      EventsPlugin.SkipEvent();
      NWScript.CopyItem(item, player.oid, 1);

      switch (store.Tag)
      {
        case "blacksmith_shop":
        case "woodworker_shop":
        case "tannery_shop":
        case "magic_shop":
          break;
        default:
          NWScript.DestroyObject(item);
          break;
      }
    }
    [ScriptHandler("b_store_sell")]
    private void HandleBeforeStoreSell(CallInfo callInfo)
    {
      if (PlayerSystem.Players.TryGetValue(callInfo.ObjectSelf, out PlayerSystem.Player player))
      {
        EventsPlugin.SkipEvent();
        if (!NWScript.StringToObject(EventsPlugin.GetEventData("STORE")).ToNwObject<NwStore>().Tag.StartsWith("_PLAYER_SHOP_"))
        {
          ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Navré, je n'achète rien. J'arrive déjà tout juste à m'acquiter de ma dette.",
          NWScript.GetLocalObject(NWScript.StringToObject(EventsPlugin.GetEventData("STORE")), "_STORE_NPC"), callInfo.ObjectSelf);
        }
        else
        {
          player.oid.SendServerMessage("Impossible de vendre dans ce type d'échoppe.", Color.ORANGE);
        }
      }
    }
    [ScriptHandler("after_store_buy")]
    private void HandleAfterStoreBuy(CallInfo callInfo)
    {
      if (!PlayerSystem.Players.TryGetValue(callInfo.ObjectSelf, out PlayerSystem.Player buyer))
        return;

      if (!Int32.TryParse(EventsPlugin.GetEventData("RESULT"), out int result) || result == 0)
        return;

      NwStore store = NWScript.StringToObject(EventsPlugin.GetEventData("STORE")).ToNwObject<NwStore>();
      if (!store.Tag.StartsWith("_PLAYER_SHOP_"))
        return;

      int price = Int32.Parse(EventsPlugin.GetEventData("PRICE")) * 95 / 100;
      int ownerId = store.GetLocalVariable<int>("_OWNER_ID").Value;
      int shopId = store.GetLocalVariable<int>("_SHOP_ID").Value;
      NwItem item = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM")).ToNwObject<NwItem>();

      NwPlayer oSeller = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p, "characterId") == ownerId);
      if (oSeller != null)
      {
        if (PlayerSystem.Players.TryGetValue(oSeller, out PlayerSystem.Player seller))
          seller.bankGold += price;

        oSeller.SendServerMessage($"La vente de votre {item.Name.ColorString(Color.ORANGE)} à {buyer.oid.Name.ColorString(Color.PINK)} vient de vous rapporter {price.ToString().ColorString(Color.GREEN)}");
      }
      else
      {
        // TODO : envoyer un courrier à la prochaine connexion du seller pour le prévenir de sa vente.

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters SET bankGold = bankGold + @bankGold where rowid = @characterId");
        NWScript.SqlBindInt(query, "@characterId", ownerId);
        NWScript.SqlBindInt(query, "@bankGold", price);
        NWScript.SqlStep(query);
      }

      PlayerOwnedShop.SaveShop(buyer, store);
    }
  }
}
