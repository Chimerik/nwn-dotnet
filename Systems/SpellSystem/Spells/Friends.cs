using Anvil.API;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Friends(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      if (oCaster is NwCreature caster)
      {
        foreach (var target in targets)
        {
          if (target is not NwCreature targetCreature || caster.IsReactionTypeHostile(targetCreature) 
            || EffectSystem.IsCharmeImmune(caster, targetCreature))
          {
            caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.White)} est immunisé", ColorConstants.Orange);
            return new List<NwGameObject>();
          }

          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDazedS));
          target.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingNegative)), SpellUtils.GetSpellDuration(oCaster, spellEntry));
        }
      }
      
      return targets;
    }
  }
}
