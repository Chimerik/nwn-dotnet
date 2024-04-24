using Anvil.API;
using Anvil.API.Events;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Friends(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target || target.IsReactionTypeHostile(caster))
        return;

      if (target.IsImmuneTo(ImmunityType.Charm) || target.IsImmuneTo(ImmunityType.MindSpells))
      {
        caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.White)} est immunisé", ColorConstants.Orange);
        return;
      }

      SpellUtils.SignalEventSpellCast(target, caster, spell.SpellType);
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDazedS));
      target.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingNegative), Effect.VisualEffect(VfxType.DurCessateNegative)), NwTimeSpan.FromRounds(spellEntry.duration));

      EffectSystem.ApplyConcentrationEffect(caster, spell.Id, new List<NwGameObject> { target }, spellEntry.duration);
    }
  }
}
