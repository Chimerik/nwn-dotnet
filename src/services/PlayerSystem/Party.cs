using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using System.Linq;

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
      Effect partyEffect = oPartyChanger.LoginCreature.ActiveEffects.FirstOrDefault(e => e.Tag == "PartyEffect");
      if (partyEffect != null)
        oPartyChanger.LoginCreature.RemoveEffect(partyEffect);

      oPartyChanger.LoginCreature.ApplyEffect(EffectDuration.Permanent, GetPartySizeEffect(oPartyChanger.PartyMembers.Count(p => !p.IsDM)));
    }
    private static Effect GetPartySizeEffect(int iPartySize = 0)
    {
      Effect eParty = Effect.VisualEffect(VfxType.None);

      switch (iPartySize) // déterminer quel est l'effet de groupe à appliquer
      {
        case 2:
        case 7:
          eParty = Effect.ACIncrease(1, ACBonus.Dodge);
          break;
        case 3:
        case 6:
          eParty = Effect.LinkEffects(Effect.ACIncrease(1, ACBonus.Dodge), Effect.AttackIncrease(1));
          break;
        case 4:
        case 5:
          eParty = Effect.LinkEffects(Effect.ACIncrease(1, ACBonus.Dodge), Effect.AttackIncrease(1));
          eParty = Effect.LinkEffects(Effect.DamageIncrease(1, DamageType.Bludgeoning), eParty);
          break;
      }

      eParty.Tag = "PartyEffect";
      eParty.SubType = EffectSubType.Supernatural;
      return eParty;
    }
  }
}
