using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> AffluxMental(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      List<NwGameObject> targets = new() { oTarget };

      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return targets;

      int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, casterClass.SpellCastingAbility);

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster,
        spell.GetSpellLevelForClass(casterClass.ClassType), CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry));

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDazedS));
      return targets;
    }
  }
}
