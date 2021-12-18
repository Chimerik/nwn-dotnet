using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    private static void ImprovedInvisibility(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType, false);

      int nDuration = nCasterLevel;
      Effect eImpact = Effect.VisualEffect(VfxType.ImpHeadMind);
      Effect eInvis = Effect.Invisibility(InvisibilityType.Improved);
      Effect eVis = Effect.VisualEffect(VfxType.DurInvisibility);
      Effect eDur = Effect.VisualEffect(VfxType.DurCessatePositive);
      Effect eCover = Effect.Concealment(50);
      Effect eLink = Effect.LinkEffects(eDur, eCover);
      eLink = Effect.LinkEffects(eLink, eVis);

      eInvis = Effect.LinkEffects(eInvis, Effect.AreaOfEffect((PersistentVfxType)193, null, scriptHandleFactory.CreateUniqueHandler(HandleInvisibiltyHeartBeat)));  // 193 = AoE 20 m

      if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
        nDuration = nDuration * 2; //Duration is +100%

      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, eLink, NwTimeSpan.FromRounds(nDuration));
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, eInvis, NwTimeSpan.FromRounds(nDuration));
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, eImpact);
    }
  }
}
