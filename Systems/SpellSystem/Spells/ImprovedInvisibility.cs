using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ImprovedInvisibility(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      int nCasterLevel = oCaster.CasterLevel;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType, false);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      int nDuration = nCasterLevel;
      Effect eImpact = Effect.VisualEffect(VfxType.ImpHeadMind);
      Effect eInvis = Effect.Invisibility(InvisibilityType.Improved);
      Effect eVis = Effect.VisualEffect(VfxType.DurInvisibility);
      Effect eDur = Effect.VisualEffect(VfxType.DurCessatePositive);
      Effect eCover = Effect.Concealment(50);
      Effect eLink = Effect.LinkEffects(eDur, eCover);
      eLink = Effect.LinkEffects(eLink, eVis);

      //eInvis = Effect.LinkEffects(eInvis, Effect.AreaOfEffect((PersistentVfxType)193, null, scriptHandleFactory.CreateUniqueHandler(HandleInvisibiltyHeartBeat)));  // 193 = AoE 20 m

      foreach (var target in targets)
      {
        target.ApplyEffect(EffectDuration.Temporary, eLink, SpellUtils.GetSpellDuration(oCaster, spellEntry));
        target.ApplyEffect(EffectDuration.Temporary, eInvis, SpellUtils.GetSpellDuration(oCaster, spellEntry));
        target.ApplyEffect(EffectDuration.Instant, eImpact);
      }
    }
  }
}
