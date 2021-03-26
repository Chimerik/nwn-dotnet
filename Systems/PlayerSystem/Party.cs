using NLog;
using NWN.API;
using NWN.API.Constants;
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
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public Party(NWNXEventService nwnxEventService)
    {

    }
    public static void OnPartyLeaveBefore(PartyEvents.OnLeaveBefore onPartyLeave)
    {
      API.Effect eParty = GetPartySizeEffect(onPartyLeave.Player.PartyMembers.Count<NwPlayer>() - 1);

      foreach (NwPlayer partyMember in onPartyLeave.Player.PartyMembers.Where<NwPlayer>(p => !p.IsPlayerDM))
      {
        Log.Info($"Changing buff before party leave for {partyMember.Name}");

        partyMember.RemoveEffect(partyMember.ActiveEffects.Where(e => e.Tag == "PartyEffect").FirstOrDefault());
        partyMember.ApplyEffect(EffectDuration.Permanent, eParty);
      }
    }
    public static void OnPartyLeaveAfter(PartyEvents.OnLeaveAfter onPartyLeave)
    {
      Log.Info($"Removing buff after party leave for {onPartyLeave.Player.Name}");
      onPartyLeave.Player.RemoveEffect(onPartyLeave.Player.ActiveEffects.Where(e => e.Tag == "PartyEffect").FirstOrDefault());
    }
    public static void OnPartyKickBefore(PartyEvents.OnKickBefore onPartyKicked)
    {
      NwPlayer player = ((uint)onPartyKicked.Kicked).ToNwObject<NwPlayer>();
      API.Effect eParty = GetPartySizeEffect(player.PartyMembers.Count<NwPlayer>() - 1);

      foreach (NwPlayer partyMember in player.PartyMembers.Where<NwPlayer>(p => !p.IsPlayerDM))
      {
        Log.Info($"Changing buff before party kicked for {partyMember.Name}");

        partyMember.RemoveEffect(partyMember.ActiveEffects.Where(e => e.Tag == "PartyEffect").FirstOrDefault());
        partyMember.ApplyEffect(EffectDuration.Permanent, eParty);
      }
    }
    public static void OnPartyKickAfter(PartyEvents.OnKickAfter onPartyLeave)
    {
      Log.Info($"Removing buff after party leave for {onPartyLeave.Kicked.Name}");
      onPartyLeave.Kicked.RemoveEffect(onPartyLeave.Kicked.ActiveEffects.Where(e => e.Tag == "PartyEffect").FirstOrDefault());
    }
    public static void OnPartyJoinAfter(PartyEvents.OnAcceptInvitationAfter onPartyJoin)
    {
      API.Effect eParty = GetPartySizeEffect(onPartyJoin.AcceptedBy.PartyMembers.Count<NwPlayer>());

      foreach (NwPlayer partyMember in onPartyJoin.AcceptedBy.PartyMembers.Where<NwPlayer>(p => !p.IsPlayerDM))
      {
        Log.Info($"Changing buff after party joined for {partyMember.Name}");

        partyMember.RemoveEffect(partyMember.ActiveEffects.Where(e => e.Tag == "PartyEffect").FirstOrDefault());
        partyMember.ApplyEffect(EffectDuration.Permanent, eParty);
      }
    }
    public static API.Effect GetPartySizeEffect(int iPartySize = 0)
    {
      Log.Info($"Party Size : {iPartySize}");

      API.Effect eParty = API.Effect.VisualEffect(API.Constants.VfxType.None);

      switch (iPartySize) // déterminer quel est l'effet de groupe à appliquer
      {
        case 2:
        case 7:
          eParty = API.Effect.ACIncrease(1, ACBonus.Dodge);
          break;
        case 3:
        case 6:
          eParty = NWScript.EffectLinkEffects(API.Effect.ACIncrease(1, ACBonus.Dodge), API.Effect.AttackIncrease(1));
          break;
        case 4:
        case 5:
          eParty = NWScript.EffectLinkEffects(API.Effect.ACIncrease(1, ACBonus.Dodge), API.Effect.AttackIncrease(1));
          eParty = NWScript.EffectLinkEffects(API.Effect.DamageIncrease(1, DamageType.Bludgeoning), eParty);
          break;
      }

      eParty.Tag = "PartyEffect";
      eParty.SubType = EffectSubType.Supernatural;
      return eParty;
    }
  }
}
