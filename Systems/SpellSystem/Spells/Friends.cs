using Anvil.API;
using Anvil.API.Events;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Friends(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature caster || onSpellCast.TargetObject is not NwCreature target || target.IsReactionTypeHostile(caster))
        return;

      if (target.IsImmuneTo(ImmunityType.Charm) || target.IsImmuneTo(ImmunityType.MindSpells))
      {
        caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.White)} est immunisé", ColorConstants.Orange);
        return;
      }

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDazedS));
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingNegative), Effect.VisualEffect(VfxType.DurCessateNegative)), NwTimeSpan.FromRounds(spellEntry.duration));

      EffectSystem.ApplyConcentrationEffect(caster, onSpellCast.Spell.Id, new List<NwGameObject> { target }, spellEntry.duration);
    }
  }
}
