
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.Systems;
using System;
using NWN.API;

namespace NWN.System
{
    [ServiceBinding(typeof(StoreSystem))]
    class StoreSystem
    {
        [ScriptHandler("before_store_buy")]
        private void HandleBeforeStoreBuy(CallInfo callInfo)
        {
            if (PlayerSystem.Players.TryGetValue(callInfo.ObjectSelf, out PlayerSystem.Player player))
            {
                uint item = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM"));
                int price = Int32.Parse(EventsPlugin.GetEventData("PRICE"));
                int pocketGold = NWScript.GetGold(callInfo.ObjectSelf);

                if (pocketGold < price)
                {
                    if (pocketGold + player.bankGold < price)
                    {
                        CreaturePlugin.SetGold(callInfo.ObjectSelf, 0);
                        int bankGold = 0;

                        if (player.bankGold > 0)
                            bankGold = player.bankGold;

                        int debt = price - (pocketGold + bankGold);
                        player.bankGold -= debt;

                        ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, $"Très bien, je demanderai à la banque de vous faire un crédit sur {debt}. N'oubliez pas que les intérêts sont de 30 % par semaine.",
                        NWScript.GetLocalObject(NWScript.StringToObject(EventsPlugin.GetEventData("STORE")), "_STORE_NPC"), callInfo.ObjectSelf);
                    }
                    else
                    {
                        CreaturePlugin.SetGold(callInfo.ObjectSelf, 0);
                        player.bankGold -= price - pocketGold;

                        ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Très bien, je demanderai à la banque de prélever l'or sur votre compte.",
                        NWScript.GetLocalObject(NWScript.StringToObject(EventsPlugin.GetEventData("STORE")), "_STORE_NPC"), callInfo.ObjectSelf);
                    }
                }
                else
                    NWScript.TakeGoldFromCreature(price, player.oid, 1);

                EventsPlugin.SkipEvent();
                NWScript.CopyItem(item, player.oid, 1);

                string tag = NWScript.GetTag(NWScript.GetLocalObject(NWScript.StringToObject(EventsPlugin.GetEventData("STORE")), "_STORE_NPC"));

                switch (tag)
                {
                    case "blacksmith":
                    case "woodworker":
                    case "tanneur":
                    case "tribunal_hotesse":
                        break;
                    default:
                        NWScript.DestroyObject(item);
                        break;
                }
            }
        }
        [ScriptHandler("b_store_sell")]
        private void HandleBeforeStoreSell(CallInfo callInfo)
        {
            if (PlayerSystem.Players.TryGetValue(callInfo.ObjectSelf, out PlayerSystem.Player player))
            {
                EventsPlugin.SkipEvent();
                ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Navré, je n'achète rien. J'arrive déjà tout juste à m'acquiter de ma dette.",
                NWScript.GetLocalObject(NWScript.StringToObject(EventsPlugin.GetEventData("STORE")), "_STORE_NPC"), callInfo.ObjectSelf);
            }
        }
    }
}
