using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void DelivranceDesMaledictions(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRemoveCondition));
        EffectUtils.RemoveTaggedEffect(target, EffectSystem.MaledictionAttaqueEffectTag, EffectSystem.MaledictionConstitutionEffectTag,
          EffectSystem.MaledictionDegatsEffectTag, EffectSystem.MaledictionDexteriteEffectTag, EffectSystem.MaledictionEffroiEffectTag, 
          EffectSystem.MaledictionForceEffectTag, EffectSystem.MaledictionIntelligenceEffectTag, EffectSystem.MaledictionSagesseEffectTag,
          EffectSystem.MaledictionCharismeEffectTag, EffectSystem.MaleficeTag);
      }
    }
  }
}
