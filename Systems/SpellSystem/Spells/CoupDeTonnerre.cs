

using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void CoupDeTonnerre(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass casterClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      foreach (NwCreature target in oCaster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (target == oCaster)
          continue;

        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSonic));

        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass), 
          CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry));
      }

      oCaster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSoundBurst));
    }
  }
}
