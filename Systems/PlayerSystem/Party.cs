using NWN.API;
using NWN.Core;
using NWN.Services;
using NWNX.API.Events;
using NWNX.Services;
using System.Linq;

namespace NWN.Systems
{
    [ServiceBinding(typeof(Party))]
    class Party
    {
        public Party(NWNXEventService nwnxEventService)
        {
            nwnxEventService.Subscribe<PartyEvents.OnLeaveBefore>(OnPartyLeaveBefore);
            nwnxEventService.Subscribe<PartyEvents.OnLeaveAfter>(OnPartyLeaveAfter);
            nwnxEventService.Subscribe<PartyEvents.OnKickBefore>(OnPartyKickBefore);
            nwnxEventService.Subscribe<PartyEvents.OnKickAfter>(OnPartyKickAfter);
            nwnxEventService.Subscribe<PartyEvents.OnAcceptInvitationAfter>(OnPartyJoinAfter);
        }
        private void OnPartyLeaveBefore(PartyEvents.OnLeaveBefore onPartyLeave)
        {
            API.Effect eParty = GetPartySizeEffect(onPartyLeave.Player.PartyMembers.Count<NwPlayer>() - 1);
                
            foreach(NwPlayer partyMember in onPartyLeave.Player.PartyMembers.Where<NwPlayer>(p => !p.IsPlayerDM))
            {
                partyMember.RemoveEffect(partyMember.ActiveEffects.Where(e => e.Tag == "PartyEffect").FirstOrDefault());
                partyMember.ApplyEffect(EffectDuration.Permanent, eParty);
            }
        }
        private void OnPartyLeaveAfter(PartyEvents.OnLeaveAfter onPartyLeave)
        {
            onPartyLeave.Player.RemoveEffect(onPartyLeave.Player.ActiveEffects.Where(e => e.Tag == "PartyEffect").FirstOrDefault());
        }
        private void OnPartyKickBefore(PartyEvents.OnKickBefore onPartyKicked)
        {
            NwPlayer player = ((uint)onPartyKicked.Kicked).ToNwObject<NwPlayer>();
            API.Effect eParty = GetPartySizeEffect(player.PartyMembers.Count<NwPlayer>() - 1);

            foreach (NwPlayer partyMember in player.PartyMembers.Where<NwPlayer>(p => !p.IsPlayerDM))
            {
                partyMember.RemoveEffect(partyMember.ActiveEffects.Where(e => e.Tag == "PartyEffect").FirstOrDefault());
                partyMember.ApplyEffect(EffectDuration.Permanent, eParty);
            }
        }
        private void OnPartyKickAfter(PartyEvents.OnKickAfter onPartyLeave)
        {
            onPartyLeave.Kicked.RemoveEffect(onPartyLeave.Kicked.ActiveEffects.Where(e => e.Tag == "PartyEffect").FirstOrDefault());
        }
        private void OnPartyJoinAfter(PartyEvents.OnAcceptInvitationAfter onPartyJoin)
        {
            API.Effect eParty = GetPartySizeEffect(onPartyJoin.AcceptedBy.PartyMembers.Count<NwPlayer>());

            foreach (NwPlayer partyMember in onPartyJoin.AcceptedBy.PartyMembers.Where<NwPlayer>(p => !p.IsPlayerDM))
            {
                partyMember.RemoveEffect(partyMember.ActiveEffects.Where(e => e.Tag == "PartyEffect").FirstOrDefault());
                partyMember.ApplyEffect(EffectDuration.Permanent, eParty);
            }
        }
        public static API.Effect GetPartySizeEffect(int iPartySize = 0)
        {
            API.Effect eParty = API.Effect.VisualEffect(API.Constants.VfxType.None);

            switch (iPartySize) // déterminer quel est l'effet de groupe à appliquer
            {
                case 2:
                case 7:
                    eParty = API.Effect.ACIncrease(1, API.Constants.ACBonus.Dodge);
                    break;
                case 3:
                case 6:
                    eParty = NWScript.EffectLinkEffects(NWScript.EffectACIncrease(1, NWScript.AC_DODGE_BONUS), NWScript.EffectAttackIncrease(1));
                    break;
                case 4:
                case 5:
                    eParty = NWScript.EffectLinkEffects(NWScript.EffectACIncrease(1, NWScript.AC_DODGE_BONUS), NWScript.EffectAttackIncrease(1));
                    eParty = NWScript.EffectLinkEffects(NWScript.EffectDamageIncrease(1, NWScript.DAMAGE_TYPE_BLUDGEONING), eParty);
                    break;
            }

            eParty.Tag = "PartyEffect";
            eParty.SubType = API.Constants.EffectSubType.Supernatural;
            return eParty;
        }
    }
}
