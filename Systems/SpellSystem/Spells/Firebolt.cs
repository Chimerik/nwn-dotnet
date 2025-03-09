using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FireBolt(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass, NwFeat feat)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);
      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);
      var castAbility = SpellUtils.GetSpellCastAbility(oCaster, castingClass, feat);

      foreach (var target in targets)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS));

        switch (SpellUtils.GetSpellAttackRoll(target, oCaster, spell, castAbility))
        {
          case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); ; break;
          case TouchAttackResult.Hit: break;
          default: continue;
        }

        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, nbDice, oCaster, spell.GetSpellLevelForClass(castingClass));
      }
    }
  }
}
