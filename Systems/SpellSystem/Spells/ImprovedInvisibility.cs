using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ImprovedInvisibility(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      int nCasterLevel = oCaster.CasterLevel;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType, false);

      int nDuration = nCasterLevel;
      Effect eImpact = Effect.VisualEffect(VfxType.ImpHeadMind);
      Effect eInvis = Effect.Invisibility(InvisibilityType.Improved);
      Effect eVis = Effect.VisualEffect(VfxType.DurInvisibility);
      Effect eDur = Effect.VisualEffect(VfxType.DurCessatePositive);
      Effect eCover = Effect.Concealment(50);
      Effect eLink = Effect.LinkEffects(eDur, eCover);
      eLink = Effect.LinkEffects(eLink, eVis);

      //eInvis = Effect.LinkEffects(eInvis, Effect.AreaOfEffect((PersistentVfxType)193, null, scriptHandleFactory.CreateUniqueHandler(HandleInvisibiltyHeartBeat)));  // 193 = AoE 20 m

      /*if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
        nDuration = nDuration * 2; //Duration is +100%
      */
      oTarget.ApplyEffect(EffectDuration.Temporary, eLink, NwTimeSpan.FromRounds(nDuration));
      oTarget.ApplyEffect(EffectDuration.Temporary, eInvis, NwTimeSpan.FromRounds(nDuration));
      oTarget.ApplyEffect(EffectDuration.Instant, eImpact);
    }
  }
}
