using NLog;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
using NWN.Services;
using System.Linq;
using System.Threading.Tasks;

namespace NWN.Systems
{
  [ServiceBinding(typeof(Party))]
  class Party
  {
    public static void HandlePartyEvent(OnPartyEvent onPartyEvent)
    {
      switch (onPartyEvent.EventType)
      {
        case PartyEventType.AcceptInvitation:
        case PartyEventType.Kick:
        case PartyEventType.Leave:
          HandlePartyChange(onPartyEvent.Player);
          break;
      }
    }
    public static void HandlePartyChange(NwPlayer oPartyChanger)
    {
      API.Effect partyEffect = oPartyChanger.ActiveEffects.FirstOrDefault(e => e.Tag == "PartyEffect");
      if (partyEffect != null)
        oPartyChanger.RemoveEffect(partyEffect);

      oPartyChanger.ApplyEffect(EffectDuration.Permanent, GetPartySizeEffect(oPartyChanger.PartyMembers.Count<NwPlayer>(p => !p.IsDM)));
    }
    private static API.Effect GetPartySizeEffect(int iPartySize = 0)
    {
      API.Effect eParty = API.Effect.VisualEffect(VfxType.None);

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
