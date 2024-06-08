using Anvil.API;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Friends(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target || target.IsReactionTypeHostile(caster))
        return new List<NwGameObject>();

      if (target.IsImmuneTo(ImmunityType.Charm) || target.IsImmuneTo(ImmunityType.MindSpells))
      {
        caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.White)} est immunisé", ColorConstants.Orange);
        return new List<NwGameObject>();
      }

      SpellUtils.SignalEventSpellCast(target, caster, spell.SpellType);
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDazedS));
      target.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingNegative)), NwTimeSpan.FromRounds(spellEntry.duration));

      return new List<NwGameObject> { target };
    }
  }
}
