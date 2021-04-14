using NLog;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Services;
using System.Linq;
using System.Threading.Tasks;

namespace NWN.Systems
{
  [ServiceBinding(typeof(Party))]
  class Party
  {
    public static void HandlePartyChange(NwGameObject oPartyChanger)
    {
      NwPlayer oPC = null;

      if (oPartyChanger is NwPlayer)
        oPC = (NwPlayer)oPartyChanger;
      else
        oPC = (NwPlayer)((NwCreature)oPartyChanger).Master;

      API.Effect partyEffect = oPC.ActiveEffects.FirstOrDefault(e => e.Tag == "PartyEffect");
      if (partyEffect != null)
        oPC.RemoveEffect(partyEffect);

      oPC.ApplyEffect(EffectDuration.Permanent, GetPartySizeEffect(oPC.PartyMembers.Count<NwPlayer>(p => !p.IsDM)));

      Task waitForPartyChange = NwTask.Run(async () =>
      {
        await NwTask.WaitUntilValueChanged(() => oPC.PartyMembers.Count<NwPlayer>(p => !p.IsDM));
        Party.HandlePartyChange(oPC);
      });
    }
    public static API.Effect GetPartySizeEffect(int iPartySize = 0)
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
