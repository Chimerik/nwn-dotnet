using Anvil.API;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Friends(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, NwFeat feat)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);  

      if (oCaster is NwCreature caster)
      {
        int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, SpellUtils.GetSpellCastAbility(oCaster, casterClass, feat));

        foreach (var target in targets)
        {
          if (target is not NwCreature targetCreature || caster.IsReactionTypeHostile(targetCreature)
            || EffectSystem.IsCharmeImmune(caster, targetCreature))
          {
            caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.White)} est immunisé", ColorConstants.Orange);
            continue;
          }

          if (CreatureUtils.GetSavingThrow(oCaster, targetCreature, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
          {
            target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDazedS));
            target.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingNegative)), SpellUtils.GetSpellDuration(oCaster, spellEntry));
          }
        }
      }
      
      return targets;
    }
  }
}
