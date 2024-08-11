using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Injonction(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

      if(CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
      {
        target.ApplyEffect(EffectDuration.Temporary, Effect.Knockdown(), SpellUtils.GetSpellDuration(oCaster, spellEntry));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpKnock));
      }
    }
  }
}
