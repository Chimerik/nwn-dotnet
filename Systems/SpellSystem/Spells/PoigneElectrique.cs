using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ElectricJolt(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, NwFeat feat = null)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, caster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);
      int nbDice = SpellUtils.GetSpellDamageDiceNumber(caster, spell);

      var castAbility = SpellUtils.GetSpellCastAbility(oCaster, casterClass, feat);

      foreach (var target in targets)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpLightningS));

        switch (SpellUtils.GetSpellAttackRoll(target, caster, spell, castAbility, 0))
        {
          case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(caster, spellEntry, nbDice); ; break;
          case TouchAttackResult.Hit: break;
          default: continue;
        }

        target.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(EffectSystem.noReactions(target), Effect.Icon(CustomEffectIcon.ElectricJolt)), NwTimeSpan.FromRounds(1));

        EffectUtils.RemoveTaggedEffect(target, EffectSystem.ReactionEffectTag);
        SpellUtils.DealSpellDamage(target, caster.CasterLevel, spellEntry, nbDice, caster, 0);
      }
    }
  }
}
